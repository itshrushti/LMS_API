﻿using LMS_Project_APIs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace LMS_Project_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManageEnrollmentController : ControllerBase
    {
        private readonly LearningManagementSystemContext _context;

        public ManageEnrollmentController(LearningManagementSystemContext context)
        {
            _context = context;
        }

        [HttpPost("StartTraining")]
        public async Task<IActionResult> StartTraining([FromBody] TrainingStartModel request)
        {

            if (request == null || request.StudentId == null || request.TrainingId == null)
            {
                return BadRequest("Invalid data provided.");
            }

            var approvalRequired = await _context.Tbl_Training
                  .Where(t => t.training_id == request.TrainingId)
                  .Select(t => t.requires_approval)
                  .FirstOrDefaultAsync();


            var studentparam = new SqlParameter("@studentid", request.StudentId ?? (object)DBNull.Value);
            var trainingparam = new SqlParameter("@trainingid", request.TrainingId ?? (object)DBNull.Value);
            var actionParam = new SqlParameter("@action", "TRAINING");

            var studentIdParam = new SqlParameter("@studentid", studentparam);
            var trainingIdParam = new SqlParameter("@trainingid", trainingparam);

            if (approvalRequired == true)
            {
                // Call the training_Approval stored procedure for approval-required trainings
                await _context.Database.ExecuteSqlRawAsync("EXEC training_Approval @student_id = {0}, @training_id = {1}, @action = {2}", request.StudentId, request.TrainingId, "TRAINING");
                return Ok("Training request submitted. Waiting for approval.");
            }
            else
            {
                // Directly start training without approval
                await _context.Database.ExecuteSqlRawAsync("EXEC start_Training @studentid = {0}, @trainingid = {1}", request.StudentId, request.TrainingId);
                return Ok("Training started successfully.");

            }

    

        }

        [HttpPost("Approval")]
        public async Task<IActionResult> Approval([FromBody] TrainingStartModel request)
        {
            if (request == null || request.StudentId == null || request.TrainingId == null)
            {
                return BadRequest("Invalid data provided.");
            }

            var studentparam = new SqlParameter("@studentid", request.StudentId ?? (object)DBNull.Value);
            var trainingparam = new SqlParameter("@trainingid", request.TrainingId ?? (object)DBNull.Value);
            var actionParam = new SqlParameter("@action", "ACCEPT");

            var studentIdParam = new SqlParameter("@studentid", studentparam);
            var trainingIdParam = new SqlParameter("@trainingid", trainingparam);

            // Call stored procedure
            await _context.Database.ExecuteSqlRawAsync("EXEC training_Approval @student_id = {0}, @training_id = {1}, @action = {2}", request.StudentId, request.TrainingId, "ACCEPT");

            return Ok("Training request approved successfully.");
        }

        [HttpPost("Deny")]
        public async Task<IActionResult> Deny([FromBody] TrainingStartModel request)
        {
            if (request == null || request.StudentId == null || request.TrainingId == null)
            {
                return BadRequest("Invalid data provided.");
            }

            var studentparam = new SqlParameter("@studentid", request.StudentId ?? (object)DBNull.Value);
            var trainingparam = new SqlParameter("@trainingid", request.TrainingId ?? (object)DBNull.Value);
            var actionParam = new SqlParameter("@action", "DENY");

            var studentIdParam = new SqlParameter("@studentid", studentparam);
            var trainingIdParam = new SqlParameter("@trainingid", trainingparam);

            // Call stored procedure
            await _context.Database.ExecuteSqlRawAsync("EXEC training_Approval @student_id = {0}, @training_id = {1}, @action = {2}", request.StudentId, request.TrainingId, "DENY");

            return Ok("Training request denied successfully.");
        }

        [HttpPost("CompletedTraining")]
        public async Task<IActionResult> CompletedTraining([FromBody] TrainingStartModel request)
        {
            if (request == null || request.StudentId == null || request.TrainingId == null)
            {
                return BadRequest("Invalid data provided.");
            }

            var studentparam = new SqlParameter("@studentid", request.StudentId ?? (object)DBNull.Value);
            var trainingparam = new SqlParameter("@trainingid", request.TrainingId ?? (object)DBNull.Value);

            var studentIdParam = new SqlParameter("@studentid", studentparam);
            var trainingIdParam = new SqlParameter("@trainingid", trainingparam);

            // Call stored procedure
            await _context.Database.ExecuteSqlRawAsync("EXEC complete_Training @studentid = {0}, @trainingid = {1}", request.StudentId, request.TrainingId);

            return Ok("Training completed successfully.");
        }   

    }
}
