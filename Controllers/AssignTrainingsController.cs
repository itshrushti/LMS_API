using System.Data;
using LMS_Project_APIs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace LMS_Project_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignTrainingsController : ControllerBase
    {
        private readonly LearningManagementSystemContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;



        public AssignTrainingsController(LearningManagementSystemContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }


        [HttpPost("AssignTrainings")]
        [AdminAuthorize]
        public async Task<IActionResult> AssignTrainings(AssignTrainings tblassign)
        {
            var studentId = _httpContextAccessor.HttpContext.Session.GetInt32("StudentId");

            if (tblassign == null)
            {
                return BadRequest("Invalid student ID or training IDs.");
            }
            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC AssignTrainings @p0, @p1",
                    //tblassign.StudentId,
                    studentId,
                    tblassign.TrainingIds ?? (object)DBNull.Value
                );
                return Ok("Trainings assigned successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred", Error = ex.Message });
            }
        }
    }
}

