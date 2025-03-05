using System;
using System.Collections.Generic;

namespace LMS_Project_APIs.Models;

public partial class TblCompanyLogo
{
    public int CompanylogoId { get; set; }

    public string? CompanyImage { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }
}
