namespace LMS_Project_APIs.Models
{
    public class ResetPassword
    {
        public int Student_Id { get; set; }
        public string? Current_Password { get; set; }
        public string? New_Password { get; set; }
        public string? Confirm_Password { get; set; }


    }
}
