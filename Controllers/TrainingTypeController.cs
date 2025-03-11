using LMS_Project_APIs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;

namespace LMS_Project_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingTypeController : ControllerBase
    {
        private readonly LearningManagementSystemContext _context;

        public TrainingTypeController(LearningManagementSystemContext context)
        {
            _context = context;
        }

        [HttpGet("getTrainingType")]
        public async Task<ActionResult> getTrainingType()
        {
            var trainingtypes = await _context.SearchTrainingTypes
                                       .FromSqlRaw("EXEC display_TrainingType")
                                       .ToListAsync();
            return Ok(trainingtypes);
        }

        [HttpPost("addTrainingType")]
        public async Task<ActionResult> addTrainingType([FromBody] TblTrainingType tblTrainingType)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC add_edit_TrainingType @p0, @p1, @p2",
                                                        tblTrainingType.Trainingtype_Id == 0 ? null : tblTrainingType.Trainingtype_Id,
                                                        tblTrainingType.Trainingtype_Name,
                                                        tblTrainingType.Description);
                return Ok(new {Message = "Training Type Added/Updated."});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new {Message = "An error:", Error = ex.Message});
            }
        }

        [HttpDelete("deleteTrainingType/{trainingtypeid}")]
        public async Task<ActionResult> deleteTrainingType(int trainingtypeid)
        {
            if (trainingtypeid == 0)
            {
                return BadRequest(new { Message = "Invalid Training Type ID." });
            }

            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC delete_TrainingType @p0", trainingtypeid);
                return Ok(new { Message = "Training type and related Training deleted" });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error:", Error = ex.Message });
            }
        }

        [HttpGet("searchTrainingType")]
        public async Task<ActionResult> searchTrainingType(string searchValue)
        {
            if (string.IsNullOrEmpty(searchValue))
            {
                return BadRequest("search value can not be empty!");
            }

            var results = await _context.SearchTrainingTypes
                                        .FromSqlRaw("EXEC search_TrainingType @p0", searchValue)
                                        .ToListAsync();
            if (results == null || results.Count == 0)
            {
                return NotFound("No matching training records found.");
            }
            return Ok(results);
        }

    }
}
