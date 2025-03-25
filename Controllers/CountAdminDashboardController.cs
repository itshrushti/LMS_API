using LMS_Project_APIs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS_Project_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountAdminDashboardController : ControllerBase
    {
        private readonly LearningManagementSystemContext _context;
        private readonly IHttpContextAccessor _httpcontextAccessor;

        public CountAdminDashboardController(LearningManagementSystemContext context, IHttpContextAccessor httpcontextAccessor)
        {
            _context = context;
            _httpcontextAccessor = httpcontextAccessor;
        }

        //[HttpGet("getCountAdminDashboard")]
        //[AdminAuthorize] // Ensure proper authorization
        //public IActionResult GetCountAdminDashboard([FromQuery] int? studentId)
        //{
        //    // Retrieve StudentId from session if not provided via query
        //    var sessionStudentId = _httpcontextAccessor.HttpContext.Session.GetInt32("StudentId");

        //    // Prefer query parameter if provided, fallback to session-based value
        //    var effectiveStudentId = studentId ?? sessionStudentId;

        //    if (!effectiveStudentId.HasValue || effectiveStudentId.Value == 0)
        //    {
        //        return BadRequest(new { Message = "Student ID not found in session or query parameter." });
        //    }

        //    try
        //    {
        //        // Use SQL query to fetch dashboard data
        //        var result = _context.TblCountAdminDashboards
        //            .FromSqlRaw("EXEC admin_Dashboard @p0", effectiveStudentId.Value)
        //            .AsNoTracking()
        //            .ToList();

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { Message = "An error occurred while fetching dashboard data.", Error = ex.Message });
        //    }
        //}
        //[HttpGet("getCountAdminDashboard")]
        //[AdminAuthorize]
        //public IActionResult getCountAdminDashboard()
        //{
        //    var studentid = _httpcontextAccessor.HttpContext.Session.GetInt32("StudentId");

        //    if (studentid == 0)
        //    {
        //        return BadRequest(new { Message = "Student ID not found in session." });
        //    }

        //    try
        //    {
        //        var result =  _context.TblCountAdminDashboards
        //                            .FromSqlRaw("EXEC admin_Dashboard @p0", studentid)
        //                            .AsEnumerable()
        //                            .ToList();
        //        return Ok(result);

        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { Message = "An error:", Error = ex.Message });
        //    }
        //}

        [HttpGet("getCountAdminDashboard/{studentId}")]
        public IActionResult getCountAdminDashboard(int studentId)
        {
            if (studentId == 0)
            {
                return BadRequest(new { Message = "Invalid student type Id." });
            }

            try
            {
                var result = _context.TblCountAdminDashboards
                                        .FromSqlRaw("EXEC admin_Dashboard @p0", studentId)
                                        .AsEnumerable()
                                        .ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error:", Error = ex.Message });
            }
        }
    }
}
