using System;
using System.Collections.Generic;

namespace LMS_Project_APIs.Models;

public partial class TblRole
{
    public int RoleId { get; set; }

    public string? RoleName { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public virtual ICollection<TblStudent> TblStudents { get; set; } = new List<TblStudent>();
}
