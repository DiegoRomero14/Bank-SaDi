using System;
using System.Collections.Generic;

namespace Bank_SaDi.Models;

public partial class ChangesHistory
{
    public int ChangesId { get; set; }

    public int UserId { get; set; }

    public DateTime ChangeDate { get; set; }

    public string ChangeType { get; set; } = null!;

    public string TableAffected { get; set; } = null!;

    public int? RecordId { get; set; }

    public string Description { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
