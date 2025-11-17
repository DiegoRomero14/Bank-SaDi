using Bank_SaDi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace Bank_SaDi.Controllers
{
    public class SignInController : Controller
    {
        private readonly BankSaDiDbContext _context;

        public SignInController(BankSaDiDbContext context)
        {
            _context = context;
        }

        public IActionResult SignIn()
        {
            return View();
        }

        #region METHOD REGISTER
        [HttpPost]
        public IActionResult Register(string firstname, string lastname, string nationalID, string email, string password, string confirmPassword)
        {
            #region verifications
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.NationalId == nationalID))
                {
                    ViewData["NationalIDError"] = "This ID is already in use";
                    return View("SignIn");
                }

                if (_context.Users.Any(u => u.Email == email))
                {
                    ViewData["EmailError"] = "The mail is already in use";
                    return View("SignIn");
                }

                if (password.Length < 8 || !password.Any(char.IsUpper) || !password.Any(char.IsLower))
                {
                    ViewData["PasswordError"] = "minimum 8 characters, one uppercase and one lowercase";
                    return View("SignIn");
                }

                if (password != confirmPassword)
                {
                    ViewData["ConfirmPasswordError"] = "Passwords do not match";
                    return View("SignIn");
                }
            }
            #endregion

            // Generar hash de la contraseña
            byte[] passwordHash;
            using (var sha256 = SHA256.Create())
            {
                passwordHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

            // Crear nuevo usuario
            var user = new User
            {
                FirstName = char.ToUpper(firstname[0]) + firstname.Substring(1).ToLower(),
                LastName = char.ToUpper(lastname[0]) + lastname.Substring(1).ToLower(),
                NationalId = nationalID,
                Email = email,
                PasswordHash = passwordHash,
                UserType = "Client", // Asigna un tipo de usuario predeterminado
                CreatedAt = DateTime.Now
            };

            // Guardar en la base de datos
            _context.Users.Add(user);
            _context.SaveChanges();

            // Redirigir al inicio después del registro
            return RedirectToAction("Index", "Home");
        }
        #endregion
    }
}
