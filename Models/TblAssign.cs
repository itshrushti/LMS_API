using System;
using System.Collections.Generic;

namespace LMS_Project_APIs.Models;

public partial class TblAssign
{
    public int AssignId { get; set; }

    public int? StudentId { get; set; }

    public int? TrainingId { get; set; }

    public string? TrainingName { get; set; }

    public DateTime? AssignDate { get; set; }

    public int? StatusId { get; set; }

    public virtual TblStatus? Status { get; set; }

    public virtual TblStudent? Student { get; set; }

    public virtual TblTraining? Training { get; set; }
}
