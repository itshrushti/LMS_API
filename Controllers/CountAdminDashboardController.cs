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

        public CountAdminDashboardController(LearningManagementSystemContext context)
        {
            _context = context;
        }

        [HttpGet("getCountAdminDashboard/{studentId}")]
        public IActionResult getCountAdminDashboard(int studentId)
        {
            if (studentId == 0)
            {
                return BadRequest(new { Message = "Invalid Student Type ID." });
            }

            try
            {
                var result =  _context.TblCountAdminDashboards
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
        //[HttpGet("getCountAdminDashboard/{studentId}")]
        //public async Task<IActionResult> getCountAdminDashboard(int studentId)
        //{
        //    var result = await _context.TblCount_Admin_Dashboards
        //                                .FromSqlRaw("EXEC admin_Dashboard @p0", studentId)
        //                                .AsNoTracking()
        //                                .ToListAsync();
        //    return Ok(result);
        //}
    }
}
