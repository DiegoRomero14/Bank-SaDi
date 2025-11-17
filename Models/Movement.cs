using System;
using System.Collections.Generic;

namespace Bank_SaDi.Models;

public partial class Movement
{
    public int IdTransaction { get; set; }

    public int? OriginAccount { get; set; }

    public int? DestinyAccount { get; set; }

    public int TransactionTId { get; set; }

    public decimal Amount { get; set; }

    public DateTime TransactionDate { get; set; }

    public virtual Account DestinyAccountNavigation { get; set; } = null!;

    public virtual Account? OriginAccountNavigation { get; set; }

    public virtual TypeOfMovement TransactionT { get; set; } = null!;

    // Propiedad de ayuda para el tipo de movimiento
    public string TransactionTypeName
    {
        get
        {
            return TransactionTId switch
            {
                1 => "Deposit",
                2 => "Withdraw",
                3 => "Transfer Sent",
                _ => "Unknown" // Valor por defecto si no coincide con ninguno
            };
        }
    }
}
