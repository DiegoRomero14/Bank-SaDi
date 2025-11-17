using Bank_SaDi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;

namespace Bank_SaDi.Controllers
{
    public class HomeController : Controller
    {
        private readonly BankSaDiDbContext _context;

        public HomeController(BankSaDiDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        #region Log In
        [HttpPost]
        public async Task<IActionResult> Login(string identifier, string password)
        {
            // Verifica si identifier es null o vacío
            if (string.IsNullOrWhiteSpace(identifier) || string.IsNullOrWhiteSpace(password))
            {
                ViewData["IPasswordError"] = "Fill in all fields";
                return View("Index");
            }

            // Determina si el identifier es un correo electrónico
            bool isEmail = identifier.Contains("@") && identifier.Contains(".");

            // Busca al usuario usando el Email o el NationalID
            var user = isEmail
                ? await _context.Users.FirstOrDefaultAsync(u => u.Email == identifier)
                : await _context.Users.FirstOrDefaultAsync(u => u.NationalId == identifier);

            // Verifica si el usuario no fue encontrado o si la contraseña no es correcta
            if (user == null || !VerifyPassword(user.PasswordHash, password))
            {
                ViewData["IPasswordError"] = "Incorrect password or username";
                return View("Index");
            }

            // Si el usuario existe y la contraseña es correcta
            if (user.UserType == "Client")
            {
                // Guardar el UserID y UserType en la sesión
                HttpContext.Session.SetInt32("UserID", user.UserId); // Almacenar el ID del usuario
                HttpContext.Session.SetString("UserRole", user.UserType); // Almacenar el rol del usuario

                // Redirigir a la página principal de cliente (Menu)
                return RedirectToAction("Menu", "Menu");
            }
            else if (user.UserType == "Admin")
            {
                // Guardar el UserID y UserType en la sesión
                HttpContext.Session.SetInt32("UserID", user.UserId);
                HttpContext.Session.SetString("UserRole", user.UserType);

                // Redirigir a una página de administración o registro, según tu lógica
                return RedirectToAction("CRUD", "CRUD");
            }

            // En caso de que el tipo de usuario no sea reconocido, redirige al login
            return RedirectToAction("Index");
        }

        private bool VerifyPassword(byte[] storedHash, string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var passwordBytes = Encoding.UTF8.GetBytes(password);
                var hashedPassword = sha256.ComputeHash(passwordBytes);
                return hashedPassword.SequenceEqual(storedHash);
            }
        }
        #endregion

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
