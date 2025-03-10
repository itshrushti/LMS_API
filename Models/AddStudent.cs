namespace LMS_Project_APIs.Models
{
    public class AddStudent
    {
        public int Student_Id { get; set; }

        public string? Student_No { get; set; }

        public string? Firstname { get; set; }

        public string? Middlename { get; set; }

        public string? Lastname { get; set; }

        public string? Username { get; set; }

        public string? Password { get; set; }

        public string? Email { get; set; }

        public int? Role_Id { get; set; }

        public string? Profile_Image { get; set; }

        public DateOnly? Archive_Date { get; set; }

        public string? Phone_No { get; set; }

        public string? Address { get; set; }

        public string? City { get; set; }

        public string? Postal_Code { get; set; }

        public string? State { get; set; }

        public string? Country { get; set; }       
          
    }
}
