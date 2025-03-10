using LMS_Project_APIs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS_Project_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly LearningManagementSystemContext _context;

        public ProfileController(LearningManagementSystemContext context)
        {
            _context = context;
        }

        [HttpPost("EditStudentProfile")]
        public async Task<IActionResult> EditStudentProfile(EditStudentProfile stud)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC edit_studentProfile @p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8",

                    stud.Student_Id,
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


    }
}
