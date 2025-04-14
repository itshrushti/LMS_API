using Azure.Core;
using LMS_Project_APIs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace LMS_Project_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisplayDataController : ControllerBase
    {
        private readonly LearningManagementSystemContext _context;
        private readonly IWebHostEnvironment _env;

        public DisplayDataController(LearningManagementSystemContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet("DisplayCourseCatalog/{studentId}")]
        public async Task<IActionResult> GetCourseCatalog(int studentId)
        {
            try
            {
                var studentIdParam = new SqlParameter("@studentid", studentId);

                var courseCatalog = await _context.CourseCatalogs
                    .FromSqlRaw("EXEC display_CourseCatalog @studentid", studentIdParam)
                    .ToListAsync();

                return Ok(courseCatalog);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching the course catalog.");
            }
        }

        [HttpGet("DisplayIDP/{studentId}")]
        public async Task<IActionResult> GetIDP(int studentId)
        {
            try
            {
                var studentIdParam = new SqlParameter("@studentid", studentId);

                var courseIDP = await _context.DisplayIDPs
                    .FromSqlRaw("EXEC display_IDP @studentid", studentIdParam)
                    .ToListAsync();

                return Ok(courseIDP);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching the course catalog.");
            }
        }

        [HttpGet("DisplayTrainingTrascript/{studentId}")]
        public async Task<IActionResult> GetTrainingTrascript(int studentId)
        {

            var studentIdParam = new SqlParameter("@studentid", studentId);

            var transcriptdata = await _context.TrainingTrascriptDatas
                .FromSqlRaw("EXEC sp_DisplayTranscriptData @studentid", studentIdParam)
                .ToListAsync();

            return Ok(transcriptdata);


        }

        [HttpGet("DisplayEnrollment/{studentId}")]
        public async Task<IActionResult> GetEnrollmentData(int studentId)
        {
            try
            {
                var studentIdParam = new SqlParameter("@studentid", studentId);

                var enrolldata = await _context.DisplayEnrollments
                    .FromSqlRaw("EXEC DisplayEnrollment @studentid", studentIdParam)
                    .ToListAsync();

                return Ok(enrolldata);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching the course catalog.");
            }

        }

        [HttpGet("DisplayPending")]
        public async Task<IActionResult> DisplayPendingApproval()
        {
            try
            {

                var approvaldata = await _context.PendingApprovals
                    .FromSqlRaw("EXEC display_pending_Approval")
                    .ToListAsync();

                return Ok(approvaldata);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching the course catalog.");
            }

        }

        [HttpGet("searchPendingApproval")]
        //[AdminAuthorize]
        public async Task<ActionResult> searchPendingApproval(string searchValue)
        {
            if (string.IsNullOrWhiteSpace(searchValue))
            {
                return BadRequest("Search value cannot be empty!");
            }

            var results = await _context.PendingApprovals
                                        .FromSqlRaw("EXEC display_pending_Approval @p0", searchValue)
                                        .ToListAsync();
            if (results == null || results.Count == 0)
            {
                return NotFound("No matching pending approval records found.");
            }
            return Ok(results);
        }

        [HttpGet("GetInProgress/{studentId}")]
        public async Task<IActionResult> GetInProgress(int studentId)
        {
            
                var studentIdParam = new SqlParameter("@studentid", studentId);

                var courseCatalog = await _context.DisplayIDPs
                    .FromSqlRaw("EXEC display_InProgress_Trainings @studentid", studentIdParam)
                    .ToListAsync();

                return Ok(courseCatalog);
             
        }


        [HttpGet("GetNotStarted/{studentId}")]
        public async Task<IActionResult> GetNotStarted(int studentId)
        {
            try
            {
                var studentIdParam = new SqlParameter("@studentid", studentId);

                var courseIDP = await _context.DisplayIDPs
                    .FromSqlRaw("EXEC display_NotStartedTrainings @studentid", studentIdParam)
                    .ToListAsync();

                return Ok(courseIDP);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching the data.");
            }
        }


        [HttpGet("GetTranscriptByID/{transcriptid}")]
        public async Task<IActionResult> GetTranscriptID(int transcriptid)
        {

            var transcriptIdparam = new SqlParameter("@transcriptid", transcriptid);

            var transcriptdata = await _context.TrainingTrascriptDatas
                .FromSqlRaw("EXEC sp_GetTranscriptByID @transcriptid", transcriptIdparam)
                .ToListAsync();

            return Ok(transcriptdata);


        }


        [HttpGet("GetTrainingDataByID/{trainingid}/{studentid}")]
        public async Task<IActionResult> GetTrainingByID(int trainingid, int studentid)
        {
            var trainingidParam = new SqlParameter("@trainingid", trainingid);
            var studentidParam = new SqlParameter("@studentid", studentid);

            var trainingdata = await _context.TrainingDataByIDs
                .FromSqlRaw("EXEC GetTrainingByID @trainingid, @studentid", trainingidParam, studentidParam)
                .ToListAsync();

            return Ok(trainingdata);
        }

        //get training thumbnail image 
        [HttpGet("GetThumbnail")]
        public IActionResult GetThumbnail(string fileName, string type)
        {
            var uploadsPath = Path.Combine(_env.WebRootPath, "uploads");
            var defaultsPath = Path.Combine(_env.WebRootPath, "uploads");

            string filePath = string.IsNullOrEmpty(fileName)
                ? ""
                : Path.Combine(uploadsPath, fileName);
             
            if (!System.IO.File.Exists(filePath))
            {
                var defaultFile = type?.ToLower() switch
                {
                    "document" => "Doc_image.jpg",
                    "external link" => "EL_image.jpg",
                    _ => "default_training.jpg"  
                };

                filePath = Path.Combine(defaultsPath, defaultFile);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("Image not found.");
                }
            }

            var mimeType = GetMimeType(filePath); 
            return PhysicalFile(filePath, mimeType);
        }

        private string GetMimeType(string filePath)
        {
            var ext = Path.GetExtension(filePath).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };
        }

    }
}
