﻿using System;
using System.Collections.Generic;

namespace LMS_Project_APIs.Models;

public partial class TblTrainingType
{
    public int Trainingtype_Id { get; set; }

    public string? Trainingtype_Name { get; set; }

    public string? Description { get; set; }

    //public DateTime? CreateDate { get; set; }

    //public DateTime? UpdateDate { get; set; }

    //public virtual ICollection<TblApproval> TblApprovals { get; set; } = new List<TblApproval>();

    //public virtual ICollection<TblTraining> TblTrainings { get; set; } = new List<TblTraining>();
}
