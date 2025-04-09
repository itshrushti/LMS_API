using LMS_Project_APIs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS_Project_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly LearningManagementSystemContext _context;

        public ConfigurationController(LearningManagementSystemContext context)
        {
            _context = context;
        }

        [HttpGet("getConfiguration")]
        //[AdminAuthorize]
        public async Task<IActionResult> getConfiguration()
        {
            var result = await _context.DisplayConfigurations
                                        .FromSqlRaw("EXEC display_configuration")
                                        .ToListAsync();
            return Ok(result);
        }

        [HttpPost("updateConfiguration")]
        //[AdminAuthorize]
        public async Task<ActionResult> updateConfiguration([FromBody] TblConfiguration tblConfig)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC add_edit_configuration @p0, @p1, @p2",
                                                        tblConfig.config_id == 0 ? null : tblConfig.config_id,
                                                        tblConfig.config_key,
                                                        tblConfig.config_value);
                return Ok(new { Message = "Configuration Added/Updated." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error:", Error = ex.Message });
            }
        }

    }
}
