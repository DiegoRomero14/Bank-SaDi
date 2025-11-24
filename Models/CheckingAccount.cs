using System;
using System.Collections.Generic;

namespace Bank_SaDi.Models;

public partial class CheckingAccount
{
    public int AccountNumber { get; set; }

    public decimal OverDraftLimit { get; set; }

    public virtual Account AccountNumberNavigation { get; set; } = null!;
}
