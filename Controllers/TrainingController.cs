﻿using LMS_Project_APIs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.IdentityModel.Tokens;

namespace LMS_Project_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingController : ControllerBase
    {
        private readonly LearningManagementSystemContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TrainingController(LearningManagementSystemContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
          
        }

        [HttpGet("getTraining")]
        public IActionResult getTraining()
        {
            var trainings = _context.SearchTrainings
                            .FromSqlRaw("EXEC display_Training")
                            .AsEnumerable()
                            .ToList();
            return Ok(trainings);
        }


        [HttpPost("addTraining")]
        [AdminAuthorize]
        public async Task<IActionResult> AddTraining(TblTraining training)
        {
            string docPath = null;
            string imagePath = null;
            if (training.ThumbnailImage != null && training.ThumbnailImage.Length > 0)
            {
                string[] allowedExtensions = [".jpg", ".jpeg", ".png"];
                string fileExtension = Path.GetExtension(training.ThumbnailImage.FileName).ToLower();

                if (allowedExtensions.Contains(fileExtension))
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    imagePath = Path.GetFileName(training.ThumbnailImage.FileName);
                    string filePath = Path.Combine(uploadsFolder, imagePath);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await training.ThumbnailImage.CopyToAsync(stream);
                    }
                }
                else
                {
                    return BadRequest(new { Message = "Invalid Thumbnail Image format. Allowed formats: JPG, JPEG, PNG." });
                }
            }

            if (training.DocumentFile != null && training.DocumentFile.Length > 0)
            {

                string[] allowedExtensions = [".pdf"];
                string fileExtension = Path.GetExtension(training.DocumentFile.FileName).ToLower();

                if (allowedExtensions.Contains(fileExtension))
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    docPath = Path.GetFileName(training.DocumentFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, docPath);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await training.DocumentFile.CopyToAsync(stream);
                    }
                }
                else
                {
                    return BadRequest(new { Message = "Invalid Document File format. Allowed formats: PDF." });
                }
            }
            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC add_edit_training @p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12, @p13",
                    null, // Since it's a new training, TrainingId is null
                    training.TrainingName,
                    training.TrainingCode,
                    training.TrainingtypeId,
                    docPath,
                    training.ExternalLinkUrl,
                    training.TrainingHours,
                    training.RequiresApproval,
                    training.ArchiveDate,
                    training.Summary,
                    training.CourseCatalog,
                    training.CstartDate,
                    training.CendDate,
                    imagePath);

                return Ok(new { Message = "Training Added Successfully", ImagePath = imagePath, DocumentPath = docPath });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred", Error = ex.Message });
            }
        }


        [HttpPut("updateTraining")]
        [AdminAuthorize]
        public async Task<IActionResult> UpdateTraining(TblTraining training)
        {
            var trainingId = training.TrainingId;
            var trainingExist = await _context.TblTraining
                                            .FromSqlRaw("SELECT COUNT(*)AS TrainingCount FROM tbl_training WHERE training_id = @p0", trainingId)
                                            .Select(x => x.TrainingId)
                                            .CountAsync();

            if(trainingExist == 0)
            {
                return BadRequest(new { Message = "Training not found in tbl_training." });
            }

            //thumbnail Image update
            string imagePath = null;
            var existingTimage = await _context.TblUpdateTraining
                                                .FromSqlRaw("SELECT thumbnail_image FROM tbl_training WHERE training_id = @p0", trainingId)
                                                .Select(x => x.thumbnail_image)
                                                .FirstOrDefaultAsync();

            if (training.ThumbnailImage != null && training.ThumbnailImage.Length > 0)
            {
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
                string fileExtension = Path.GetExtension(training.ThumbnailImage.FileName).ToLower();

                if (allowedExtensions.Contains(fileExtension))
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    if (!string.IsNullOrEmpty(existingTimage))
                    {
                        string oldFilePath = Path.Combine(uploadsFolder, existingTimage);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }
                    imagePath = Path.GetFileName(training.ThumbnailImage.FileName);
                    string filePath = Path.Combine(uploadsFolder, imagePath);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await training.ThumbnailImage.CopyToAsync(stream);
                    }
                }
                else
                {
                    return BadRequest(new { Message = "Invalid Thumbnail Image format. Allowed formats: JPG, JPEG, PNG." });
                }
            }

            //Document update
            string docPath = null;
            var existingDocfile = await _context.TblUpdateTraining
                                                .FromSqlRaw("SELECT document_file FROM tbl_training WHERE training_id = @p0", trainingId)
                                                .Select(x => x.document_file)
                                                .FirstOrDefaultAsync();

            if (training.DocumentFile != null && training.DocumentFile.Length > 0)
            {

                string[] allowedExtensions = { ".pdf" };
                string fileExtension = Path.GetExtension(training.DocumentFile.FileName).ToLower();

                if (allowedExtensions.Contains(fileExtension))
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    if (!string.IsNullOrEmpty(existingDocfile))
                    {
                        string oldFilePath = Path.Combine(uploadsFolder, existingDocfile);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }
                    docPath = Path.GetFileName(training.DocumentFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, docPath);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await training.DocumentFile.CopyToAsync(stream);
                    }
                }
                else
                {
                    return BadRequest(new { Message = "Invalid Document File format. Allowed formats: PDF." });
                }
            }
            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC add_edit_training @p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12, @p13",
                    training.TrainingId,
                    training.TrainingName,
                    training.TrainingCode,
                    training.TrainingtypeId,
                    docPath,
                    training.ExternalLinkUrl,
                    training.TrainingHours,
                    training.RequiresApproval,
                    training.ArchiveDate,
                    training.Summary,
                    training.CourseCatalog,
                    training.CstartDate,
                    training.CendDate,
                    imagePath
                    );
                
                return Ok(new { Message = "Training details updated successfully.", ImagePath = imagePath, DocumentPath = docPath });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        [HttpDelete("deleteTraining")]
        [AdminAuthorize]
        public async Task<IActionResult> DeleteTraining([FromBody] List<int> trainingIds)
        {
            try
            {
                string idStr = string.Join(",", trainingIds);

                // Fetch file paths from database before deletion
                var fileRecords = await _context.TblUpdateTraining
                    .FromSqlRaw("SELECT thumbnail_image, document_file FROM tbl_training WHERE training_id IN ({0})", idStr)
                    .Select(x => new { x.thumbnail_image, x.document_file })
                    .ToListAsync();

                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                foreach (var fileRecord in fileRecords)
                {
                    var filePaths = new List<string> { fileRecord.thumbnail_image, fileRecord.document_file };

                    foreach (var filePath in filePaths)
                    {
                        if (!string.IsNullOrEmpty(filePath))
                        {
                            string fullFilePath = Path.Combine(uploadsFolder, filePath);
                            if (System.IO.File.Exists(fullFilePath))
                            {
                                System.IO.File.Delete(fullFilePath);
                            }
                        }
                    }
                }

                // Call stored procedure to delete training records
                await _context.Database.ExecuteSqlRawAsync("EXEC delete_Training @p0", idStr);

                return Ok(new { Message = "Trainings and their associated files deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred", Error = ex.Message });
            }
        }


        [HttpGet("searchTraining")]
        [AdminAuthorize]
        public async Task<ActionResult> searchTraining(string searchValue)
        {
            if (string.IsNullOrWhiteSpace(searchValue))
            {
                return BadRequest("Search value cannot be empty!");
            }

            var results = await _context.SearchTrainings
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
