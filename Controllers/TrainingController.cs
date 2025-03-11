using LMS_Project_APIs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;

namespace LMS_Project_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingController : ControllerBase
    {
        private readonly LearningManagementSystemContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TrainingController(LearningManagementSystemContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("getTraining")]
        public IActionResult getTraining()
        {
            var trainings =  _context.SearchTrainings
                            .FromSqlRaw("EXEC display_Training")
                            .AsEnumerable()
                            .ToList();
            return Ok(trainings);
        }

        [HttpPost("addTraining")]
        public async Task<ActionResult> addTraining([FromBody] TblTraining tblTraining, IFormFile thumbnailImage, IFormFile documentfile)
        {
            try
            {
                string filePath = tblTraining.ThumbnailImage; //keep existing file if no new upload

                //handle file upload
                if(thumbnailImage != null && thumbnailImage.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    if(!Directory.Exists(uploadsFolder)) 
                        Directory.CreateDirectory(uploadsFolder);

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(thumbnailImage.FileName);
                    filePath = Path.Combine("uploads", fileName);

                    using (var stream = new FileStream(Path.Combine(_webHostEnvironment.WebRootPath, filePath), FileMode.Create))
                    {
                        await thumbnailImage.CopyToAsync(stream);
                    }
                }

                await _context.Database.ExecuteSqlRawAsync("EXEC add_edit_training @p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12, @p13",
                                                          
                                                               tblTraining.TrainingId == 0 ? null : tblTraining.TrainingId,
                                                               tblTraining.TrainingName,
                                                               tblTraining.TrainingCode,
                                                               tblTraining.TrainingtypeId,
                                                               tblTraining.DocumentFile,
                                                               tblTraining.ExternalLinkUrl,
                                                               tblTraining.TrainingHours,
                                                               tblTraining.RequiresApproval,
                                                               tblTraining.ArchiveDate,
                                                               tblTraining.Summary,
                                                               tblTraining.CourseCatalog,
                                                               tblTraining.CstartDate,
                                                               tblTraining.CendDate,
                                                               filePath
                                                               //tblTraining.ThumbnailImage
                                                           );
                 return Ok(new {Message = "Training Added/Updated", FilePath = filePath});
            }
            catch(Exception ex) 
            {
                return StatusCode(500, new {Message = "An error:", Error = ex.Message});
            }
        }

        [HttpDelete]
        [Route("deleteTraining")]
        public async Task<ActionResult<IEnumerable<TblTraining>>> deleteTraining([FromBody] List<int> trainingIds)
        {
            if (trainingIds == null || trainingIds.Count == 0)
            {
                return BadRequest(new { Message = "No trainingIds provide for deletion." });
            }

            try
            {
                //get image path before deletion
                var imagePaths = await _context.TblTrainings
                                                .Where(t=> trainingIds.Contains(t.TrainingId))
                                                .Select(t => t.ThumbnailImage)
                                                .ToListAsync();

                //delete redords from db
                string multrainingId = string.Join(",", trainingIds);
                await _context.Database.ExecuteSqlRawAsync("EXEC delete_Training @p0", multrainingId);

                //delete image from folder 
                foreach(var imagePath in  imagePaths)
                {
                    if(!string.IsNullOrEmpty(imagePath))
                    {
                        string fullPath = Path.Combine(_webHostEnvironment.WebRootPath, imagePath);
                        if(System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }
                }
                return Ok(new { Message = "Training deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "an error : ", Error = ex.Message });
            }
        }

        [HttpGet("searchTraining")]
        public async Task<ActionResult> searchTraining(string searchValue)
        {
            if(string.IsNullOrWhiteSpace(searchValue))
            {
                return BadRequest("search value can not be empty!");
            }

            var results =  await _context.SearchTrainings
                                        .FromSqlRaw("EXEC search_Training @p0", searchValue)
                                        .ToListAsync();
            if (results == null || results.Count == 0)
            {
                return NotFound("No matching training records found.");
            }
            return Ok(results);
        }
    }
}
