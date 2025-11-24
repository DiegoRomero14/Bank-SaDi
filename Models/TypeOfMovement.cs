using System;
using System.Collections.Generic;

namespace Bank_SaDi.Models;

public partial class TypeOfMovement
{
    public int TransactionTId { get; set; }

    public string TypeTransId { get; set; } = null!;

    public virtual ICollection<Movement> Movements { get; set; } = new List<Movement>();
}
