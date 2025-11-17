using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bank_SaDi.Models;
using System.Threading.Tasks;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;

namespace Bank_SaDi.Controllers
{
    public class ProfileController : Controller
    {
        private readonly BankSaDiDbContext _context;

        public ProfileController(BankSaDiDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Profile()
        {
            // Obtener el UserID desde la sesión
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                // Si no hay un usuario en sesión, redirigir a la página de inicio de sesión
                return RedirectToAction("Index", "Home");
            }

            // Buscar al usuario en la base de datos por su UserID
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                // Si el usuario no se encuentra, redirigir o mostrar un mensaje de error
                return RedirectToAction("Index", "Home");
            }

            // Enviar el modelo de usuario a la vista
            return View(user);
        }

        #region UPDATE DATOS
        [HttpPost]
        public async Task<IActionResult> UpdateData(string fieldToUpdate, string newValue)
        {
            // Obtener el UserID desde la sesión
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Buscar el usuario en la base de datos
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }



            if (fieldToUpdate == "Password")
            {
                // Hash de la nueva contraseña usando PasswordHasher
                user.PasswordHash = HashPassword(newValue);


                // Guardar los cambios
                await _context.SaveChangesAsync();
                return RedirectToAction("LogOut", "Profile");
            }



            // Usar reflexión para actualizar el campo específico
            var property = typeof(User).GetProperty(fieldToUpdate);
            if (property != null && property.CanWrite)
            {
                property.SetValue(user, newValue);
            }

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return RedirectToAction("Profile"); // Redirigir de nuevo a la página de perfil
        }

        // Método auxiliar para hashear la contraseña
        private byte[] HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
        #endregion

        #region LOG OUT 
        public IActionResult LogOut()
        {
            HttpContext.Session.Clear(); // Elimina todos los datos de la sesión
            return RedirectToAction("Index", "Home"); // Redirige al login o inicio
        }
        #endregion


    }


}
