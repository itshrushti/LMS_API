using LMS_Project_APIs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS_Project_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignStudentsController : ControllerBase
    {
        private readonly LearningManagementSystemContext _context;

        public AssignStudentsController(LearningManagementSystemContext context)
        {
            _context = context;
        }

        [HttpPost("AssignStudents")]
        public async Task<IActionResult> AssignTrainings(TblAssignStudents tblassign)
        {
            if (tblassign == null || tblassign.TrainingId <= 0)
            {
                return BadRequest("Invalid student ID or training IDs.");
            }

            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC AssignStudents @p0, @p1",
                    tblassign.TrainingId,
                    tblassign.StudentIds ?? (object)DBNull.Value
                );
                return Ok("Training Assigned to Student successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred", Error = ex.Message });
            }
        }


    }
}
