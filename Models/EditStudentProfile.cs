using System.ComponentModel.DataAnnotations.Schema;

namespace LMS_Project_APIs.Models
{
    public class EditStudentProfile
    {
        public int Student_Id {get; set;}
        public string? Email { get; set; }
        public string? firstname { get; set; }
        public string? lastname { get; set; }

        [NotMapped]
        public IFormFile? Profile_Image { get; set;}
        public string? Profile_Image_Name { get; set; } 

        public string? Phone_No {get; set;}
        public string? Address {get; set;}
        public string? City {get; set;}
        public string? Postal_Code { get; set; }
        public string? State {get; set;}
        public string? Country {get; set;}

    }
}
