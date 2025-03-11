namespace LMS_Project_APIs.Models
{
    public class TblCountStudentDashboard
    {
        public string? trainingtype_name {  get; set; }

        public int? CompletedTrainingCount { get; set; }

        public int? AssignedTrainingCount { get; set; }

        public int? EnrollTrainingCount { get;  set; }
    }
}
