using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS_Project_APIs.Models;

public partial class TblTraining
{
    [Key]
    public int TrainingId { get; set; }

    public string? TrainingName { get; set; }

    public string? TrainingCode { get; set; }

    public int? Trainingtype_Id { get; set; }

    //public string? trainingtype_name { get; set; }

    [NotMapped]
    public IFormFile? DocumentFile { get; set; }

    public string? ExternalLinkUrl { get; set; }

    public string? TrainingHours { get; set; }

    public bool? RequiresApproval { get; set; }

    public DateOnly? ArchiveDate { get; set; } 

    public string? Summary { get; set; }

    public bool? CourseCatalog { get; set; }

    public DateOnly? CstartDate { get; set; } 

    public DateOnly? CendDate { get; set; }

    [NotMapped]
    public IFormFile? ThumbnailImage { get; set; }

    //public DateTime? CreateDate { get; set; }

    //public DateTime? UpdateDate { get; set; }

    //public virtual ICollection<TblApproval> TblApprovals { get; set; } = new List<TblApproval>();

    //public virtual ICollection<TblAssign> TblAssigns { get; set; } = new List<TblAssign>();

    //public virtual ICollection<TblTrainingTranscript> TblTrainingTranscripts { get; set; } = new List<TblTrainingTranscript>();

    //public virtual TblTrainingType? Trainingtype { get; set; }
}
