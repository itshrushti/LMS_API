using LMS_Project_APIs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

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

        [HttpPost("EditStudentProfile")]
        public async Task<IActionResult> EditStudentProfile(EditStudentProfile stud)
        {
            var studentId = _httpContextAccessor.HttpContext.Session.GetInt32("StudentId");
            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC edit_studentProfile @p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8",

                    studentId,
                    stud.Email,
                    stud.Profile_Image,
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
                return StatusCode(500, new { Message = "An error : ", Error = ex.Message });
            }
        }

        [HttpPost("SetCompanyLogo")]
        [AdminAuthorize]
        public async Task<IActionResult> SetCompanyLogo(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "Image file is required." });
            }

            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!AllowedExtensions.Contains(fileExtension))
            {
                return BadRequest(new { message = "Only JPG, JPEG, and PNG formats are allowed." });
            }

            try
            {
                // Convert image to Base64 string
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                string base64String = Convert.ToBase64String(memoryStream.ToArray());

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC setCompanyLogo @p0",
                    new SqlParameter("@p0", base64String)
                );

                return Ok(new { message = "Company logo updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }
    }
}

