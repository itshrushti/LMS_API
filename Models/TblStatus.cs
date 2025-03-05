using System;
using System.Collections.Generic;

namespace LMS_Project_APIs.Models;

public partial class TblStatus
{
    public int StatusId { get; set; }

    public string? StatusName { get; set; }

    public virtual ICollection<TblAssign> TblAssigns { get; set; } = new List<TblAssign>();

    public virtual ICollection<TblTrainingTranscript> TblTrainingTranscripts { get; set; } = new List<TblTrainingTranscript>();
}
