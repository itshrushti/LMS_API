namespace LMS_Project_APIs.Models
{
    public class TblCountAdminDashboard
    {

        public int? total_admins { get; set; }

        public int? total_students { get; set; }

        public int? total_trainings { get; set; }

        public int? pending_approvals { get; set; }

        public string? login_username { get; set; }

        public string? login_image { get; set; }
    
    }
}
