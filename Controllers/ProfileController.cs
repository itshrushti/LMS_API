using LMS_Project_APIs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Mono.TextTemplating;
using System.Diagnostics.Metrics;
using System.Net;

namespace LMS_Project_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly LearningManagementSystemContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png" };

        public ProfileController(LearningManagementSystemContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("GetStudentProfile")]

        public ActionResult GetStudentProfile([FromQuery] int studentId) 
        {
            if (studentId <= 0)
            {
                return BadRequest(new { Message = "Invalid Student ID." });
            }

            var student = _context.EditStudentProfiles
                .FromSqlRaw("EXEC getEditProfile @p0", studentId)
                .AsEnumerable()
                .FirstOrDefault();

            if (student == null)
            {
                return NotFound(new { Message = "Student not found." });
            }

            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            return Ok(new
            {
                studentId = student.Student_Id,
                firstname = student.firstname,
                lastname = student.lastname,
                Email = student.Email,
                Phoneno = student.Phone_No,
                address = student.Address,
                city = student.City,
                postalcode = student.Postal_Code,
                state = student.State,
                country = student.Country
            });
        }

        [HttpGet("GetProfileImage")]

        public ActionResult GetProfileImage([FromQuery] int studentId)
        {
            if (studentId <= 0)
            {
                return BadRequest(new { Message = "Invalid Student ID." });
            }

            var student = _context.EditProfileImage
                .FromSqlRaw("EXEC getProfileImage @p0", studentId)
                .AsEnumerable()
                .FirstOrDefault();

            if (student == null)
            {
                return NotFound(new { Message = "Student not found." });
            }

            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            return Ok(new
            {
                ProfileImage = $"{baseUrl}/ProfileImages/{student.Profile_Image_Name}",

            });
        }


        [HttpPost("EditStudentProfileImage")]
        public async Task<IActionResult> EditStudentProfileImage(IFormFile profileImage, [FromForm] int studentId)
        {
            if (studentId <= 0)
            {
                return BadRequest(new { Message = "Invalid student ID." });
            }

            // Fetch existing student record
            var student = await _context.TblStudents
     .FromSqlRaw("SELECT Student_Id, Profile_Image FROM tbl_Student WHERE Student_Id = @p0", studentId)
     .Select(s => new { s.Student_Id, s.Profile_Image })
     .FirstOrDefaultAsync();


            if (student == null)
            {
                return NotFound(new { Message = "Student not found." });
            }

            string defaultImage = "profile_image.jpg";
            string existingProfileImage = student.Profile_Image ?? defaultImage;
            string filename = existingProfileImage;

            if (profileImage != null && profileImage.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ProfileImages");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Delete old image if it's not the default one
                if (!string.IsNullOrEmpty(existingProfileImage) && existingProfileImage != defaultImage)
                {
                    string oldFilePath = Path.Combine(uploadsFolder, existingProfileImage);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Generate a unique filename
                filename = profileImage.FileName;
                string filePath = Path.Combine(uploadsFolder, filename);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profileImage.CopyToAsync(stream);
                }
            }

            try
            {
                await _context.Database.ExecuteSqlRawAsync(
       "EXEC edit_profilePhoto @p0, @p1",
       new SqlParameter("@p0", (object)filename ?? DBNull.Value),
       new SqlParameter("@p1", studentId)
   );

                return Ok(new { Message = "Profile image updated successfully.", FileName = filename });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred.", Error = ex.Message });
            }
        }

        [HttpPost("EditStudentProfile")]
        public async Task<IActionResult> EditStudentProfile([FromBody] EditStudentProfile stud)
        {
            var studentExists = await _context.TblStudents
                .FromSqlRaw("SELECT COUNT(*) as StudentCount FROM tbl_Student WHERE Student_Id = @p0", stud.Student_Id)
                .Select(s => s.Student_Id)
                .CountAsync();

            if (studentExists == 0)
            {
                return BadRequest(new { Message = "Student not found in tbl_Student." });
            }

            try
            {
                // Update the student profile information
                await _context.Database.ExecuteSqlRawAsync("EXEC edit_studentProfile @p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7",
                    stud.Student_Id,
                    stud.Email,
                    stud.Phone_No,
                    stud.Address,
                    stud.City,
                    stud.Postal_Code,
                    stud.State,
                    stud.Country
                );

                return Ok(new { Message = "Student Profile Updated." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred: ", Error = ex.Message });
            }
        }



        [HttpPost("SetCompanyLogo")]
        public async Task<IActionResult> SetCompanyLogo(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "Image file is required." });
            }

            var allowedExtensions = new List<string> { ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest(new { message = "Only JPG, JPEG, and PNG formats are allowed." });
            }

            try
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/CompanyLogo");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder); 
                }

                var existingLogo = await _context.Logo
            .FromSqlRaw("SELECT company_image FROM tbl_CompanyLogo")
            .Select(s => s.company_image)
            .FirstOrDefaultAsync();

                if (!string.IsNullOrEmpty(existingLogo))
                {
                    string oldFilePath = Path.Combine(uploadsFolder, existingLogo);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                string filename = Path.GetFileName(file.FileName);
                string filePath = Path.Combine(uploadsFolder, filename);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC setCompanyLogo @p0",
                    new SqlParameter("@p0", filename) 
                );

                return Ok(new { message = "Company logo updated successfully."});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }

        [HttpGet("DisplayCompanyLogo")]
       
        public ActionResult DisplayCompanyLogo()
        {
            var logo = _context.Logo.FromSqlRaw("EXEC display_CompanyLogo")
                .AsEnumerable()
                .FirstOrDefault();
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            return Ok(new { companylogo = $"{baseUrl}/CompanyLogo/{logo.company_image}" 
            });
        }
    }
}

