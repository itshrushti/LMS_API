using LMS_Project_APIs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace LMS_Project_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignStudentsController : ControllerBase
    {
        private readonly LearningManagementSystemContext _context;
        private readonly IHttpContextAccessor _httpcontextAccessor;

        public AssignStudentsController(LearningManagementSystemContext context, IHttpContextAccessor httpcontextAccessor)
        {
            _context = context;
            _httpcontextAccessor = httpcontextAccessor;
        }

        [HttpPost("AssignStudents")]
        public async Task<IActionResult> AssignStudents(TblAssignStudents tblassign)
        {
            //var TrainingId = _httpcontextAccessor.HttpContext.Session.GetInt32("TrainingId");

            if (tblassign == null)
            {
                return BadRequest("Invalid student ID or training IDs.");
            }

            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC AssignStudents @p0, @p1",
                     new SqlParameter("@p0", tblassign.TrainingId),
                    new SqlParameter("@p1", string.Join(",", tblassign.StudentIds))
                //tblassign.TrainingId,
                //trainingid,
                //tblassign.StudentIds ?? (object)DBNull.Value
                );
                return Ok(new { message = "Training Assigned to Student successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred", Error = ex.Message });
            }
        }

        //for display assign student in edit training
        [HttpGet("GetStudentIds/{TrainingId}")]
        public async Task<IActionResult> GetStudentIds(int TrainingId)
        {
            var studentIds = await _context.Database
                .SqlQueryRaw<int>("EXEC GetAssignStudents @p0", TrainingId)
                .ToListAsync();

            if (!studentIds.Any())
                return NotFound(new { message = "No assigned student found" });

            return Ok(studentIds); // ✅ Returns JSON array `[1,2,3]`
        }
         
        //For display training name and status and student name
        [HttpGet("getDisplayStatus")]
        public IActionResult getDisplayStatus()
        {
            var displayStatuses = _context.TblDisplayStatuses
                            .FromSqlRaw("EXEC display_Status")
                            .AsEnumerable()
                            .ToList();
            return Ok(displayStatuses);
        }

        [HttpGet("searchDisplayStatus")]
        public async Task<ActionResult> searchDisplayStatus(string SearchValue)
        {
            if (string.IsNullOrWhiteSpace(SearchValue))
            {
                return BadRequest("Search value cannot be empty!");
            }

            var results = await _context.TblDisplayStatuses
                                        .FromSqlRaw("EXEC display_Status @p0", SearchValue)
                                        .ToListAsync();
            if (results == null || results.Count == 0)
            {
                return NotFound("No matching training records found.");
            }
            return Ok(results);
        }
    }
}
