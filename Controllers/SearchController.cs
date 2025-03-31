using LMS_Project_APIs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace LMS_Project_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly LearningManagementSystemContext _context;

        public SearchController(LearningManagementSystemContext context)
        {
            _context = context;
        }

        [HttpGet("SearchIDP")]
        public async Task<IActionResult> SearchIDP(string searchValue,int studentID)
        {
            try
            {
                var searchParam = new SqlParameter("@searchvalue", searchValue ?? (object)DBNull.Value);
                var studentid = new SqlParameter("@studentId", studentID);

                var searchResults = await _context.IDPSearchings
                    .FromSqlRaw("EXEC Search_IDP @searchvalue,@studentId", searchParam, studentid)
                    .ToListAsync();

                return Ok(searchResults);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching the course catalog.");
            }

        }


        [HttpGet("SearchCC")]
        public async Task<IActionResult> SearchCourseCatalog(int studentID,string searchValue)
        {
            try
            {
                var studentid = new SqlParameter("@studentid", studentID);
                var searchParam = new SqlParameter("@searchValue", searchValue ?? (object)DBNull.Value);

                var searchResults = await _context.IDPSearchings
                    .FromSqlRaw("EXEC search_CourseCatalog @studentid,@searchValue", searchParam, studentid)
                    .ToListAsync();

                return Ok(searchResults);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching the course catalog.");
            }

        }

        [HttpGet("SearchTranscript")]
        public async Task<IActionResult> SearchTranscript(string searchValue, int studentID)
        {
            try
            {
                var searchParam = new SqlParameter("@searchvalue", searchValue ?? (object)DBNull.Value);
                var studentid = new SqlParameter("@studentId", studentID);

                var searchResults = await _context.TranscriptSearchings
                    .FromSqlRaw("EXEC Search_TrainingTranscript @searchvalue,@studentId", searchParam, studentid)
                    .ToListAsync();

                return Ok(searchResults);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching the course catalog.");
            }

        }

        [HttpGet("SearchEnroll")]
        public async Task<IActionResult> SearchEnrollment(string searchValue, int studentID)
        {
            try
            {
                var searchParam = new SqlParameter("@searchvalue", searchValue ?? (object)DBNull.Value);
                var studentid = new SqlParameter("@studentId", studentID);

                var searchResults = await _context.DisplayEnrollments
                    .FromSqlRaw("EXEC Search_Enrollment @searchvalue,@studentId", searchParam, studentid)
                    .ToListAsync();

                return Ok(searchResults);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching the course catalog.");
            }

        }
    }
}
