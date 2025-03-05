using System;
using System.Collections.Generic;

namespace LMS_Project_APIs.Models;

public partial class TblTrainingTranscript
{
    public int TranscriptId { get; set; }

    public int? TrainingId { get; set; }

    public string TrainingName { get; set; } = null!;

    public int? StudentId { get; set; }

    public int? StatusId { get; set; }

    public DateOnly? EnrollDate { get; set; }

    public DateOnly? CompletionDate { get; set; }

    public DateOnly? CreateDate { get; set; }

    public virtual TblStatus? Status { get; set; }

    public virtual TblStudent? Student { get; set; }

    public virtual TblTraining? Training { get; set; }
}
