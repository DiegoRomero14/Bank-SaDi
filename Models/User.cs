using System;
using System.Collections.Generic;

namespace Bank_SaDi.Models;

public partial class User
{
    public int UserId { get; set; }

    public string NationalId { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public byte[] PasswordHash { get; set; } = null!;

    public string UserType { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<ChangesHistory> ChangesHistories { get; set; } = new List<ChangesHistory>();
}
