using System.ComponentModel.DataAnnotations.Schema;

namespace LMS_Project_APIs.Models
{
    public class EditProfileImage
    {
        public int Student_Id { get; set; }

        [NotMapped]
        public IFormFile? Profile_Image { get; set; }
        public string? Profile_Image_Name { get; set; }
    }
}
