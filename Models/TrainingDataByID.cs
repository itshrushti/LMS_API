namespace LMS_Project_APIs.Models
{
    public class TrainingDataByID
    {
        public int training_id { get; set; }
        public string? training_name { get; set; }
        public string? training_code { get; set; }
        public int trainingtype_id { get; set; }
        public string? trainingtype_name { get; set; }
        public string? document_file { get; set; }
        public string? external_link_URL { get; set; }
        public string? training_hours { get; set; }
        public bool requires_approval { get; set; }
        public DateOnly archive_date { get; set; }
        public string? summary { get; set; } 
        public string? thumbnail_image { get; set; } 

    }
}
 
 