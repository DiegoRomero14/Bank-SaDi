using Bank_SaDi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Bank_SaDi.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Bank_SaDi.Controllers
{
    public class Bank_OperationsController : Controller
    {
        private readonly BankSaDiDbContext _context;

        public Bank_OperationsController(BankSaDiDbContext context)
        {
            _context = context;
        }

        #region VIEW
        public IActionResult Bank_Operations(int accountNumber)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            // Guarda el número de cuenta en la sesión para usarlo en otros métodos
            HttpContext.Session.SetInt32("AccountNumber", accountNumber);


            // Busca la cuenta con el número proporcionado
            var account = _context.Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);

            if (account == null || user == null)
            {
                // Maneja el caso en que la cuenta no existe (opcional) o el usuario 
                return NotFound();
            }

            if (!account.IsActive) // Asegúrate de que esta propiedad exista en tu modelo
            {
                return RedirectToAction("Accounts", "Accounts");
            }


            // Obtener los movimientos de la cuenta, incluyendo el tipo de movimiento
            var movements = _context.Movements
                .Include(m => m.OriginAccountNavigation)
                .Include(m => m.DestinyAccountNavigation)
                .Include(m => m.TransactionT) // Asegúrate de incluir el tipo de movimiento
                .Where(m => m.OriginAccount == accountNumber || m.DestinyAccount == accountNumber)
                .ToList();

            var viewModel = new ATMViewModel
            {
                Account = account,
                User = user,
                Movements = movements, // Asignar los movimientos al ViewModel
                AccountNumberL = accountNumber
            };

            return View("Bank_Operations", viewModel); // Pasa el modelo de cuenta a la vista
        }
        #endregion

        #region METHOD DEPOSIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deposit(decimal InputDeposit)
        {
            var accountNumber = HttpContext.Session.GetInt32("AccountNumber");

            if (accountNumber == null)
            {
                return Json(new { success = false, message = "Account number not found.", redirectUrl = Url.Action("Bank_Operations", "Bank_Operations", new { accountNumber }) });
            }

            var account = await _context.Accounts
                .Include(a => a.MovementDestinyAccountNavigations)
                .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

            if (account == null)
            {
                return Json(new { success = false, message = "Account not found.", redirectUrl = Url.Action("Bank_Operations", "Bank_Operations", new { accountNumber }) });
            }

            if (InputDeposit <= 0)
            {
                return Json(new { success = false, message = "The amount must be greater than 0.", redirectUrl = Url.Action("Bank_Operations", "Bank_Operations", new { accountNumber }) });
            }

            var movement = new Movement
            {
                DestinyAccount = accountNumber,
                TransactionTId = 1,
                Amount = InputDeposit,
                TransactionDate = DateTime.Now
            };

            account.Balance += InputDeposit;
            account.MovementDestinyAccountNavigations.Add(movement);

            await _context.SaveChangesAsync();

            // Redirigir a la vista de Bank Operations después de realizar el depósito
            return Json(new { success = true, message = "Deposit made successfully.", redirectUrl = Url.Action("Bank_Operations", "Bank_Operations", new { accountNumber }) });
        }
        #endregion

        #region METHOD WITHDRAW 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Withdraw(decimal InputWithdraw)
        {
            var accountNumber = HttpContext.Session.GetInt32("AccountNumber");

            if (accountNumber == null)
            {
                return Json(new { success = false, message = "Account number not found.", redirectUrl = Url.Action("Bank_Operations", "Bank_Operations", new { accountNumber }) });
            }

            var account = await _context.Accounts
                .Include(a => a.MovementDestinyAccountNavigations)
                .Include(a => a.CheckingAccount) // Incluir la relación con CheckingAccount
                .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

            if (account == null)
            {
                return Json(new { success = false, message = "Account number not found.", redirectUrl = Url.Action("Bank_Operations", "Bank_Operations", new { accountNumber }) });
            }

            if (InputWithdraw <= 0)
            {
                return Json(new { success = false, message = "The amount must be greater than 0.", redirectUrl = Url.Action("Bank_Operations", "Bank_Operations", new { accountNumber }) });
            }

            // Verificar si la cuenta es de tipo Checking Account
            if (account.TypeId == 2) // Suponiendo que 2 es el ID para cuentas corrientes
            {
                // Obtener el límite de sobregiro
                decimal overdraftLimit = account.CheckingAccount.OverDraftLimit;

                // Calcular el saldo disponible considerando el sobregiro
                decimal availableBalance = account.Balance + overdraftLimit;

                if (InputWithdraw > availableBalance)
                {
                    return Json(new { success = false, message = "Insufficient funds to make the withdrawal, even with the allowed overdraft.", redirectUrl = Url.Action("Bank_Operations", "Bank_Operations", new { accountNumber }) });
                }
            }
            else // Para cuentas de ahorro
            {
                if (InputWithdraw > account.Balance)
                {
                    return Json(new { success = false, message = "Insufficient funds to make the withdrawal.", redirectUrl = Url.Action("Bank_Operations", "Bank_Operations", new { accountNumber }) });
                }
            }

            var movement = new Movement
            {
                DestinyAccount = accountNumber,
                TransactionTId = 2, // Supongamos que 2 es el ID para retiros
                Amount = InputWithdraw,
                TransactionDate = DateTime.Now
            };

            // Restar el monto del saldo
            account.Balance -= InputWithdraw;

            // Agregar el movimiento a la cuenta
            account.MovementDestinyAccountNavigations.Add(movement);

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Withdrawal completed successfully.", redirectUrl = Url.Action("Bank_Operations", "Bank_Operations", new { accountNumber }) });
        }
        #endregion

        #region METHOD INTERES
        [HttpGet]
        public IActionResult CalculateInterest()
        {
            var accountNumber = HttpContext.Session.GetInt32("AccountNumber");

            if (accountNumber == null)
            {
                return Json(new { success = false, message = "Número de cuenta no encontrado." });
            }

            // Obtener la cuenta
            var account = _context.Accounts
                .Include(a => a.SavingAccount) // Incluir la relación con SavingAccount
                .FirstOrDefault(a => a.AccountNumber == accountNumber);

            if (account == null)
            {
                return Json(new { success = false, message = "Cuenta no encontrada." });
            }

            // Verificar si es una cuenta de ahorros
            if (account.TypeId != 1) // Suponiendo que 1 es el ID para cuentas de ahorro
            {
                return Json(new { success = false, message = "La cuenta no es una cuenta de ahorros." });
            }

            // Obtener la tasa de interés desde la tabla Saving_Account
            decimal interestRate = account.SavingAccount.InterestRate;

            // Calcula el interés
            decimal interest = account.Balance * (interestRate / 100); // Convertir a porcentaje

            // Devuelve el interés calculado
            return Json(new { success = true, interest });
        }
        #endregion

        #region METHOD TRANSFER 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Transfer(string InputTransferNumAccount, decimal InputTransferAmount)
        {
            var accountNumber = HttpContext.Session.GetInt32("AccountNumber");

            if (accountNumber == null)
            {
                return Json(new { success = false, message = "Account number not found.", redirectUrl = Url.Action("Bank_Operations", "Bank_Operations", new { accountNumber }) });
            }

            var originAccount = await _context.Accounts
                .Include(a => a.CheckingAccount) // Incluir la relación con CheckingAccount
                .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

            if (originAccount == null)
            {
                return Json(new { success = false, message = "Account number not found.", redirectUrl = Url.Action("Bank_Operations", "Bank_Operations", new { accountNumber }) });
            }

            // Verificar que la cuenta de origen es una cuenta corriente
            if (originAccount.TypeId != 2) // Suponiendo que 2 es el ID para cuentas corrientes
            {
                return Json(new { success = false, message = "Only current accounts can make transfers.", redirectUrl = Url.Action("Bank_Operations", "Bank_Operations", new { accountNumber }) });
            }

            if (InputTransferAmount <= 0)
            {
                return Json(new { success = false, message = "The amount must be greater than 0.", redirectUrl = Url.Action("Bank_Operations", "Bank_Operations", new { accountNumber }) });
            }

            // Obtener la cuenta de destino
            var destinyAccount = await _context.Accounts
                .FirstOrDefaultAsync(a => a.AccountNumber.ToString() == InputTransferNumAccount);

            if (destinyAccount == null)
            {
                return Json(new { success = false, message = "Destination account not found.", redirectUrl = Url.Action("Bank_Operations", "Bank_Operations", new { accountNumber }) });
            }

            // Verificar si el monto a transferir excede el saldo de la cuenta de origen
            decimal availableBalance = originAccount.Balance + (originAccount.CheckingAccount?.OverDraftLimit ?? 0);

            if (InputTransferAmount > availableBalance)
            {
                return Json(new { success = false, message = "Insufficient funds to make the transfer.", redirectUrl = Url.Action("Bank_Operations", "Bank_Operations", new { accountNumber }) });
            }

            // Realizar la transferencia
            originAccount.Balance -= InputTransferAmount; // Restar del saldo de la cuenta de origen
            destinyAccount.Balance += InputTransferAmount; // Sumar al saldo de la cuenta de destino

            // Crear el movimiento para la cuenta de origen
            var movement = new Movement
            {
                OriginAccount = accountNumber,
                DestinyAccount = destinyAccount.AccountNumber,
                TransactionTId = 3, // ID para transferencias
                Amount = InputTransferAmount, // Monto positivo para el movimiento de salida
                TransactionDate = DateTime.Now
            };



            await _context.Movements.AddAsync(movement);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Transfer completed successfully.", redirectUrl = Url.Action("Bank_Operations", "Bank_Operations", new { accountNumber }) });
        }
        #endregion

        #region METHOD MOVEMENTS
        public IActionResult GetMovements(int accountNumber)
        {
            var movements = _context.Movements
                .Include(m => m.OriginAccountNavigation)
                .Include(m => m.DestinyAccountNavigation)
                .Include(m => m.TransactionT)
                .Where(m => m.OriginAccount == accountNumber || m.DestinyAccount == accountNumber)
                .ToList();

            return PartialView("_MovementsPartial", movements); // Devuelve una vista parcial si necesitas cargar solo los movimientos
        }
        #endregion
    }
}

