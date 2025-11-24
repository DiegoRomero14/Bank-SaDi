using Bank_SaDi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Bank_SaDi.Controllers
{
    public class AccountsController : Controller
    {
        private readonly BankSaDiDbContext _context;

        public AccountsController(BankSaDiDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Accounts()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _context.Users
                .Include(u => u.Accounts) // Esto incluye las cuentas relacionadas con el usuario
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(user);
        }


        [HttpPost]
        public async Task<IActionResult> AddAccount(string accountName, int accountType)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var newAccountNumber = GenerarNumeroDeCuenta();

            var user = await _context.Users.Include(u => u.Accounts).FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null) return NotFound();

            var newAccount = new Account
            {
                AccountNumber = newAccountNumber, // Generar número de cuenta único
                TypeId = accountType,
                UserId = userId.Value,
                Balance = 0,
                NameAccount = accountName,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            // Asignar valores específicos dependiendo del tipo de cuenta
            if (accountType == 1) // Si es una cuenta de ahorro (SavingAccount)
            {
                var savingAccount = new SavingAccount
                {
                    AccountNumber = newAccountNumber,
                    InterestRate = 10.0m // 10% de interés
                };
                newAccount.SavingAccount = savingAccount; // Asocia la cuenta de ahorros con la cuenta general
            }
            else if (accountType == 2) // Si es una cuenta corriente (CheckingAccount)
            {
                var checkingAccount = new CheckingAccount
                {
                    AccountNumber = newAccountNumber,
                    OverDraftLimit = 50000.0m // Límite de sobregiro de 50,000
                };
                newAccount.CheckingAccount = checkingAccount; // Asocia la cuenta corriente con la cuenta general
            }

            user.Accounts.Add(newAccount);
            await _context.SaveChangesAsync();

            return RedirectToAction("Accounts");
        }

        private int GenerarNumeroDeCuenta()
        {
            var random = new Random();
            return random.Next(1000, 10000);
        }


    }
}


