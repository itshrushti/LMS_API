using LMS_Project_APIs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;

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

        [HttpPost("StartTrainingDirect")]
        public async Task<IActionResult> StartTrainingDirect([FromBody] TrainingStartModel request)
        {
            if (request == null || request.StudentId == null || request.TrainingId == null)
            {
                return BadRequest("Invalid data provided.");
            }

            try
            {
                // Check if training requires approval
                var approvalRequired = await _context.Tbl_Training
                    .Where(t => t.training_id == request.TrainingId)
                    .Select(t => t.requires_approval)
                    .FirstOrDefaultAsync();

                if (approvalRequired == false)
                {
                    await _context.Database.ExecuteSqlRawAsync("EXEC start_Training @studentid = {0}, @trainingid = {1}",
                        request.StudentId, request.TrainingId);

                    return Ok("Training started successfully.");
                }
                else
                {
                    return BadRequest("This training requires approval before starting.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error starting training: " + ex.Message);
            }
        }

        [HttpPost("RequestTrainingApproval")]
        public async Task<IActionResult> RequestTrainingApproval([FromBody] TrainingStartModel request)
        {
            if (request == null || request.StudentId == null || request.TrainingId == null)
            {
                return BadRequest("Invalid data provided.");
            }

            try
            {
                // Check if training requires approval
                var approvalRequired = await _context.Tbl_Training
                    .Where(t => t.training_id == request.TrainingId)
                    .Select(t => t.requires_approval)
                    .FirstOrDefaultAsync();

                if (approvalRequired == true)
                {
                    await _context.Database.ExecuteSqlRawAsync("EXEC training_Approval @student_id = {0}, @training_id = {1}, @action = {2}",
                        request.StudentId, request.TrainingId, "TRAINING");

                    return Ok("Training request submitted. Waiting for approval.");
                }
                else
                {
                    return BadRequest("This training does not require approval.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error processing approval request: " + ex.Message);
            }
        }



        [HttpPost("Approval")]
        public async Task<IActionResult> Approval([FromBody] TrainingStartModel request)
        {
            if (request == null || request.StudentId == null || request.TrainingId == null)
            {
                return BadRequest("Invalid data provided.");
            }

            //var studentparam = new SqlParameter("@studentid", request.StudentId ?? (object)DBNull.Value);
            //var trainingparam = new SqlParameter("@trainingid", request.TrainingId ?? (object)DBNull.Value);
            //var actionParam = new SqlParameter("@action", "ACCEPT");

            //var studentIdParam = new SqlParameter("@studentid", studentparam);
            //var trainingIdParam = new SqlParameter("@trainingid", trainingparam);

            // Call stored procedure
            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC training_Approval @student_id = {0}, @training_id = {1}, @action = {2}", request.StudentId, request.TrainingId, "ACCEPT");

                return Ok(new { message = "Training request approved successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("Deny")]
        public async Task<IActionResult> Deny([FromBody] TrainingStartModel request)
        {
            if (request == null || request.StudentId == null || request.TrainingId == null)
            {
                return BadRequest("Invalid data provided.");
            }

            //var studentparam = new SqlParameter("@studentid", request.StudentId ?? (object)DBNull.Value);
            //var trainingparam = new SqlParameter("@trainingid", request.TrainingId ?? (object)DBNull.Value);
            //var actionParam = new SqlParameter("@action", "DENY");

            //var studentIdParam = new SqlParameter("@studentid", studentparam);
            //var trainingIdParam = new SqlParameter("@trainingid", trainingparam);
            try
            {
                // Call stored procedure
                await _context.Database.ExecuteSqlRawAsync("EXEC training_Approval @student_id = {0}, @training_id = {1}, @action = {2}", request.StudentId, request.TrainingId, "DENY");

                return Ok(new { message = "Training request denied successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
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

            //var email = HttpContext.Session.GetString("Email"); 
            

            var user = _context.DisplayStudents
        .FromSqlRaw("EXEC display_Student")
        .AsEnumerable()
        .FirstOrDefault(s => s.Student_Id == request.StudentId);

            if (user == null)
            {
                return NotFound();
            }

            var normalizedEmail = user.Email?.Trim().ToLower();

            var training = await _context.Tbl_Training.FirstOrDefaultAsync(t => t.training_id == request.TrainingId);
            if (training == null)
            {
                return NotFound("Training not found.");
            }



            await SendCompletedEmail(user.Firstname, user.Lastname, normalizedEmail, training);

            // Call stored procedure
            await _context.Database.ExecuteSqlRawAsync("EXEC complete_Training @studentid = {0}, @trainingid = {1}", request.StudentId, request.TrainingId);

            return Ok("Training completed successfully.");
        }


        private async Task SendCompletedEmail(string firstname, string lastname, string receiverEmail, tbl_Training training)
        {


            try
            {
                string subject = "🎓 Training Completed Successfully!";

                string body = $@"
<html>
<head>
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background-color: #eef2f7;
            padding: 40px;
        }}
        .container {{
            background-color: #ffffff;
            padding: 40px;
            border-radius: 12px;
            max-width: 650px;
            margin: auto;
            box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1);
        }}
        h2 {{
            color: #2c3e50;
        }}
        p {{
            color: #4a4a4a;
            font-size: 15px;
            line-height: 1.6;
        }}
        .highlight {{
            font-weight: bold;
            color: #2c7be5;
        }}
        .footer {{
            margin-top: 40px;
            font-size: 0.9em;
            color: #999;
            text-align: center;
        }}
    
    </style>
</head>
<body>
    <div class='container'>
        <h2>Hello {firstname} {lastname},</h2>
        <p>🎉 <strong>Congratulations!</strong> You've successfully completed the training: <span class='highlight'>{training.training_name}</span></p>
        <p>This training covered essential topics to help you enhance your skills and grow professionally.</p>
        <p>We encourage you to explore more trainings to continue your learning journey.</p>
        <p>Keep up the amazing work!</p>
        <p>Best regards,</p>
        <p><strong>LMS Team</strong></p>
        <div class='footer'>
            This is an automated message from the LMS System. Please do not reply.
        </div>
    </div>
</body>
</html>
";

                var senderEmail = new MailAddress("shrukirti7377@gmail.com", "LMS System");
                var receiver = new MailAddress(receiverEmail, firstname);
                var password = "qbpeomyducbtrcaa"; // App-specific password

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(senderEmail.Address, password)
                };

                using (var message = new MailMessage(senderEmail, receiver)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                {
                    await smtp.SendMailAsync(message);
                }
            }
            catch (Exception ex)
            {
                string logPath = @"Not Found";
                if (!Directory.Exists(Path.GetDirectoryName(logPath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(logPath));
                await System.IO.File.AppendAllTextAsync(logPath, $"{DateTime.Now}: {ex.ToString()}\n");
            }
        }


        [HttpPost("Requestagain")]
        public async Task<IActionResult> ReRequest([FromBody] TrainingStartModel request)
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
            await _context.Database.ExecuteSqlRawAsync("EXEC sp_RequestTrainingAgain @student_id = {0}, @training_id = {1}", request.StudentId, request.TrainingId );

            return Ok("Training request approved successfully.");
        }



        [HttpGet("GetTrainingDocument/{fileName}")]
        public IActionResult GetTrainingDocument(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return BadRequest("Invalid file name.");
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found.");
            }

            var contentType = "application/pdf"; // Assuming PDFs
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            // Ensure inline display in browser
            return File(fileStream, contentType, fileName, enableRangeProcessing: true);
        }
    }
}
