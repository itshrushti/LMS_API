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

        public DisplayDataController(LearningManagementSystemContext context)
        {
            _context = context;
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
            try
            {
                var studentIdParam = new SqlParameter("@studentid", studentId);

                var transcriptdata = await _context.TrainingTrascriptDatas
                    .FromSqlRaw("EXEC sp_DisplayTranscriptData @studentid", studentIdParam)
                    .ToListAsync();

                return Ok(transcriptdata);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching the course catalog.");
            }

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

        [HttpGet("GetInProgress/{studentId}")]
        public async Task<IActionResult> GetInProgress(int studentId)
        {
            try
            {
                var studentIdParam = new SqlParameter("@studentid", studentId);

                var courseCatalog = await _context.CourseCatalogs
                    .FromSqlRaw("EXEC display_InProgress_Trainings @studentid", studentIdParam)
                    .ToListAsync();

                return Ok(courseCatalog);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching the course catalog.");
            }
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


    }
}
