using Bank_SaDi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Bank_SaDi.Controllers
{
    public class MenuController : Controller
    {
        private readonly BankSaDiDbContext _context;

        public MenuController(BankSaDiDbContext context)
        {
            _context = context;
        }

        // Acción para la vista del menú
        public async Task<IActionResult> Menu()
        {
            // Obtener el UserID desde la sesión
            int? userId = HttpContext.Session.GetInt32("UserID");
            var userRole = HttpContext.Session.GetString("UserRole");
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
    }
}
