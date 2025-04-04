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

        //public int? Trainingtype_Id { get; set; } // Original FK field

        [NotMapped]
        public string? TrainingTypeName { get; set; } // ✅ Training type name (Fetched from `tbl_TrainingType`)

        // ✅ 1. Store the file name from DB
        //public string? DocumentFileName { get; set; } // This comes from the database

        [NotMapped]
        public IFormFile? DocumentFile { get; set; }

        public string? ExternalLinkUrl { get; set; }

        public string? TrainingHours { get; set; }

        public bool? RequiresApproval { get; set; }

        public DateTime? ArchiveDate { get; set; }

        public string? Summary { get; set; }

        public bool? CourseCatalog { get; set; }

        public DateTime? CstartDate { get; set; }

        public DateTime? CendDate { get; set; }

        [NotMapped]
        public IFormFile? ThumbnailImage { get; set; }
    }

}

