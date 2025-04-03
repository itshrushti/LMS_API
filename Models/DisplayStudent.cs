namespace LMS_Project_APIs.Models
{
    public class DisplayStudent
    {
        public int Student_Id { get; set; }

        public string? Student_No { get; set; }

        public string? Firstname { get; set; }

        //public string? Middlename { get; set; }

        public string? Lastname { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string? Email { get; set; }

        public string? Role_name { get; set; }

        public DateOnly? Archive_Date { get; set; }

      

    }
}
