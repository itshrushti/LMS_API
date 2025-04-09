using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LMS_Project_APIs.Models
{
    public class DisplayTrainingbyId
    {
        [Key]
        public int TrainingId { get; set; }

        public string? TrainingName { get; set; }

        public string? TrainingCode { get; set; }

        public int? Trainingtype_Id { get; set; } // Original FK field

        public string? TrainingTypeName { get; set; } // ✅ Training type name (Fetched from `tbl_TrainingType`)

        // ✅ 1. Store the file name from DB
        //public string? DocumentFileName { get; set; } // This comes from the database
        public string? DocumentFile { get; set; } // mapped to SQL result

        [NotMapped]
        public IFormFile? DocumentFileUpload { get; set; } // used for upload only

        public string? ExternalLinkUrl { get; set; }

        public string? TrainingHours { get; set; }

        public bool? RequiresApproval { get; set; }

        public DateTime? ArchiveDate { get; set; }

        public string? Summary { get; set; }

        public bool? CourseCatalog { get; set; }

        public DateTime? CstartDate { get; set; }

        public DateTime? CendDate { get; set; }

        public string? ThumbnailImage { get; set; } // mapped to SQL result

        [NotMapped]
        public IFormFile? ThumbnailImageUpload { get; set; }

    }

}

