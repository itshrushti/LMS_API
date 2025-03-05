using System;
using System.Collections.Generic;

namespace LMS_Project_APIs.Models;

public partial class TblApproval
{
    public int ApprovalId { get; set; }

    public int? TrainingId { get; set; }

    public string? TrainingName { get; set; }

    public int? StudentId { get; set; }

    public string? StudentName { get; set; }

    public int? TrainingtypeId { get; set; }

    public string? TrainingtypeName { get; set; }

    public bool? RequireApproval { get; set; }

    public DateTime? DecisionDate { get; set; }

    public virtual TblStudent? Student { get; set; }

    public virtual TblTraining? Training { get; set; }

    public virtual TblTrainingType? Trainingtype { get; set; }
}
