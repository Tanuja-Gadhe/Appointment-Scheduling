using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class TblRole
{
    public Guid RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual ICollection<TblUser> TblUsers { get; set; } = new List<TblUser>();

    public TblRole()
    {
        RoleId = Guid.NewGuid();
    }
}
