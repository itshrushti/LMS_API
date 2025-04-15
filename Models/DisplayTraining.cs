namespace LMS_Project_APIs.Models
{
    public class DisplayTraining
    {
        public int? training_id { get; set; }
        public string? training_name { get; set; }

        public string? training_code { get; set; }

        public string? trainingtype_name { get; set; }

        public DateOnly? archive_date { get; set; }

        public string? summary {  get; set; }

        public string? thumbnail_image {  get; set; }

        public DateTime? create_date { get; set; }
    }
}
