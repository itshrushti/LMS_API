using LMS_Project_APIs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;

namespace LMS_Project_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly LearningManagementSystemContext _context;

        public StudentController(LearningManagementSystemContext context)
        {
            _context = context;
        }

        [HttpGet("GetStudents")]
        public async Task<IActionResult> GetStudents()
        {
            try
            {
                var students = await _context.DisplayStudent.FromSqlRaw("EXEC display_Student").ToListAsync();
                return Ok(students);
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { Message = "An error : ", Error = ex.Message });
            }
        }


        [HttpPost("AddStudent")]
        public async Task<IActionResult> AddStudent(AddStudent tbstud)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC add_edit_Student @p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12,@p13, @p14, @p15, @p16",

                    tbstud.Student_Id == 0 ? null : tbstud.Student_Id,
                    tbstud.Student_No,
                    tbstud.Firstname,
                    tbstud.Middlename,
                    tbstud.Lastname,
                    tbstud.Username,
                    tbstud.Password,
                    tbstud.Email,
                    tbstud.Role_Id,
                    tbstud.Profile_Image,
                    tbstud.Archive_Date,
                    tbstud.Phone_No,
                    tbstud.Address,
                    tbstud.City,
                    tbstud.Postal_Code,
                    tbstud.State,
                    tbstud.Country
                  );
                return Ok(new { Message = "Student Added/Updated." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error : ", Error = ex.Message });
            }
        }


        [HttpDelete("DeleteStudents")]
        public async Task<IActionResult> DeleteStudents(List<int> studentIds)
        {
            if(studentIds == null || studentIds.Count == 0)
            {
                return BadRequest(new { Message = "No studentIds provide for deletion." });
            }

            try
            {
                string studentIdStr = string.Join(",", studentIds);
                await _context.Database.ExecuteSqlRawAsync("EXEC delete_Student @p0", studentIdStr);
                return Ok(new { Message = "Students deleted successfully." });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { Message = "an error : ", Error = ex.Message });
            }
        }

        [HttpGet("searchStudent")]
        public async Task<IActionResult> SearchStudent(string searchValue)
        {
            if(string.IsNullOrEmpty(searchValue))
            {
                return BadRequest(new { Message = "Search Value is required." });
            }
            try
            {
                var stud = await _context.DisplayStudent.FromSqlRaw("EXEC search_Student @p0", searchValue).ToListAsync();
                return Ok(stud);
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { Message = "An error : ", Error = ex.Message });
            }
        }
    }
}
