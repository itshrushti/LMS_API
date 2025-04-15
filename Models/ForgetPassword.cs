namespace LMS_Project_APIs.Models
{
    public class ForgetPassword
    {
        public string Email { get; set; }
        //public string UsernameAndEmail { get; set; }
        public string New_Password { get; set; }
        public string Confirm_Password { get; set; }
    }
}
