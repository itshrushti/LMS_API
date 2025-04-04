namespace LMS_Project_APIs.Models
{
    public class PendingApproval
    {
        public int student_id {  get; set; }
        public int training_id { get; set; }
        public string? student_name { get; set; }
        public string? training_name { get; set; }
        public string? trainingtype_name { get; set; }
        public DateTime? decision_date { get; set; }
    }
}
 