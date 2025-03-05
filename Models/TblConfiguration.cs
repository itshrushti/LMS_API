using System;
using System.Collections.Generic;

namespace LMS_Project_APIs.Models;

public partial class TblConfiguration
{
    public int ConfigId { get; set; }

    public string? ConfigKey { get; set; }

    public bool? ConfigValue { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }
}
