using LMS_Project_APIs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using System.Data;

namespace LMS_Project_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly LearningManagementSystemContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StudentController(LearningManagementSystemContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("GetStudents")]
        public async Task<IActionResult> GetStudents()
        {
            try
            {
                var students = await _context.DisplayStudents.FromSqlRaw("EXEC display_Student").ToListAsync();
                return Ok(students);
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { Message = "An error : ", Error = ex.Message });
            }
        }
        [HttpGet("GetStudentDetails/{studentId}")]
        public ActionResult GetStudentDetails(int studentId)
        {
         
            var student = _context.AddStudents.FromSqlRaw("EXEC GetStudentDetails @p0", studentId)
                .AsEnumerable()
                .FirstOrDefault();

            if (student == null)
            {
                return NotFound(new { Message = "Student not found" });
            }

            return Ok(student);
        }

        [HttpPost("AddEditStudent")]
        public async Task<IActionResult> AddEditStudent(AddStudent tbstud)
        {
            try
            {
                var studentIdParam = new SqlParameter("@NewStudentId", SqlDbType.Int) { Direction = ParameterDirection.Output };

                await _context.Database.ExecuteSqlRawAsync("EXEC add_edit_student @p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12, @p13, @p14, @p15, @p16, @NewStudentId OUTPUT",
                    new SqlParameter("@p0", tbstud.Student_Id == 0 ? (object)DBNull.Value : tbstud.Student_Id),
                    new SqlParameter("@p1", tbstud.Student_No ?? (object)DBNull.Value),
                    new SqlParameter("@p2", tbstud.Firstname ?? (object)DBNull.Value),
                    new SqlParameter("@p3", tbstud.Middlename ?? (object)DBNull.Value),
                    new SqlParameter("@p4", tbstud.Lastname ?? (object)DBNull.Value),
                    new SqlParameter("@p5", tbstud.Username ?? (object)DBNull.Value),
                    new SqlParameter("@p6", tbstud.Password ?? (object)DBNull.Value),
                    new SqlParameter("@p7", tbstud.Email ?? (object)DBNull.Value),
                    new SqlParameter("@p8", tbstud.Role_Id ?? (object)DBNull.Value),
                    new SqlParameter("@p9", tbstud.Profile_Image ?? (object)DBNull.Value),
                    new SqlParameter("@p10", tbstud.Archive_Date ?? (object)DBNull.Value),  
                    new SqlParameter("@p11", tbstud.Phone_No ?? (object)DBNull.Value),
                    new SqlParameter("@p12", tbstud.Address ?? (object)DBNull.Value),
                    new SqlParameter("@p13", tbstud.City ?? (object)DBNull.Value),
                    new SqlParameter("@p14", tbstud.Postal_Code ?? (object)DBNull.Value),
                    new SqlParameter("@p15", tbstud.State ?? (object)DBNull.Value),
                    new SqlParameter("@p16", tbstud.Country ?? (object)DBNull.Value),
                    studentIdParam
                );


                // If editing, ensure the existing Student_Id is returned
                int studentId = studentIdParam.Value != DBNull.Value ? (int)studentIdParam.Value : tbstud.Student_Id;
                if (studentId == 0)
                {
                    return StatusCode(500, new { Message = "Failed to retrieve student ID. Please try again." });
                }

                _httpContextAccessor.HttpContext.Session.SetInt32("StudentId", studentId);
                return Ok(new { Message = "Student Added/Updated.", StudentId = studentId });
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
                var stud = await _context.DisplayStudents.FromSqlRaw("EXEC search_Student @p0", searchValue).ToListAsync();
                return Ok(stud);
            }
          
            catch(Exception ex)
            {
                return StatusCode(500, new { Message = "An error : ", Error = ex.Message });
            }
        }

    }
}
