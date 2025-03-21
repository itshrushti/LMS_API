﻿using LMS_Project_APIs.Models;
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

        [HttpGet("GetStudentProfile")]
public ActionResult GetStudentProfile()
{
    var studentId = _httpContextAccessor.HttpContext.Session.GetInt32("StudentId");

    if (studentId == null)
    {
        return BadRequest(new { Message = "Student ID not found in session." });
    }

            var student =  _context.EditStudentProfiles.FromSqlRaw("EXEC getEditProfile @p0",studentId)
                .AsEnumerable()
                .FirstOrDefault();
        
    if (student == null)
    {
        return NotFound(new { Message = "Student not found." });
    }

    return Ok(student);
}

        [HttpPost("EditStudentProfile")]
        public async Task<IActionResult> EditStudentProfile([FromForm] EditStudentProfile stud)
        {
            var studentId = _httpContextAccessor.HttpContext.Session.GetInt32("StudentId");

            var studentExists = await _context.TblStudents
      .FromSqlRaw("SELECT COUNT(*) as StudentCount FROM tbl_Student WHERE Student_Id = @p0", studentId)
      .Select(s => s.Student_Id)
      .CountAsync();

            if (studentExists == 0)
            {
                return BadRequest(new { Message = "Student not found in tbl_Student." });
            }


            string filename = null;
            string defaultImage = "profile_image.jpg";

            var existingProfileImage = await _context.TblStudents
     .FromSqlRaw("SELECT Profile_Image FROM tbl_Student WHERE Student_Id = @p0", studentId)
     .Select(s => s.Profile_Image)
     .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(existingProfileImage))
            {
                existingProfileImage = defaultImage; 
            }


            if (stud.Profile_Image != null && stud.Profile_Image.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ProfileImages");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                if (!string.IsNullOrEmpty(existingProfileImage) && existingProfileImage != defaultImage)
                {
                    string oldFilePath = Path.Combine(uploadsFolder, existingProfileImage);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                filename = Path.GetFileName(stud.Profile_Image.FileName);
                string filePath = Path.Combine(uploadsFolder, filename);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await stud.Profile_Image.CopyToAsync(stream);
                }
            }
            else
            {
                filename = existingProfileImage;
            }
            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC edit_studentProfile @p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8",

                    studentId,
                    stud.Email,
                    filename,
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
            return Ok(logo);
        }

    }
}

