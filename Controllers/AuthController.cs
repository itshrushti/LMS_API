using LMS_Project_APIs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace LMS_Project_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly LearningManagementSystemContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthController(LearningManagementSystemContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("Login")]
        public ActionResult Login(LoginUser loginRequest)
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.logintext) || string.IsNullOrEmpty(loginRequest.password))
                return BadRequest(new { message = "Enter username/email & password." });

            var student = _context.DisplayStudents
                .FromSqlRaw("EXEC student_Login @p0, @p1",
                    new SqlParameter("@p0", loginRequest.logintext),
                    new SqlParameter("@p1", loginRequest.password))
                .AsEnumerable()
                .FirstOrDefault();

            if (student == null)
                return Unauthorized(new { message = "Invalid username or password." });

            if (student.Archive_Date.HasValue && student.Archive_Date.Value < DateOnly.FromDateTime(DateTime.Now))

                return Unauthorized(new { message = "Your Account is archived.", archive_date = student.Archive_Date });


            //var httpContext = _httpContextAccessor.HttpContext;
            HttpContext.Session.SetInt32("StudentId", student.Student_Id);
            HttpContext.Session.SetString("firstname", student.Firstname);
            HttpContext.Session.SetString("lastname", student.Lastname);
            HttpContext.Session.SetString("UserRole", student.Role_name);
            HttpContext.Session.SetString("Email", student.Email);
            HttpContext.Session.SetString("LoginTime", DateTime.UtcNow.ToString());


            return Ok(new
            {
                Message = "Login Successful",
                Role = student.Role_name,
                StudentId = student.Student_Id,
                firstname = student.Firstname,
                lastname = student.Lastname,
                email=student.Email,
                archivedate = student.Archive_Date
            });
        }

        [HttpPost("checkPassword")]
        public async Task<IActionResult> CheckPassword([FromBody] CheckPasswordRequest request)
        {
            var user = await _context.CheckPasswordRequest
                .FromSqlRaw("SELECT s.*,r.role_name FROM Tbl_Student s JOIN tbl_Role r on r.role_id=s.role_id WHERE Student_Id = {0}", request.student_Id)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            if (request.Password == user.Password)
            {
                return Ok(new { message = "Password is correct" });
            }
            else
            {
                return Unauthorized(new { message = "Incorrect password" });
            }
        }


        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(Models.ResetPassword request)
        {


            if (request == null || string.IsNullOrEmpty(request.Current_Password) || string.IsNullOrEmpty(request.New_Password) || string.IsNullOrEmpty(request.Confirm_Password))
                return BadRequest(new { message = "All fields are required." });

            var result = await _context.Database.ExecuteSqlRawAsync("EXEC resetPassword @p0, @p1, @p2, @p3",
                new SqlParameter("@p0", request.Student_Id),
                new SqlParameter("@p1", request.Current_Password),
                new SqlParameter("@p2", request.New_Password),
                new SqlParameter("@p3", request.Confirm_Password));

            if (result > 0)
                return Ok(new { message = "Password reset successfully." });

            return BadRequest(new { message = "Password reset failed." });
        }



        //only for testing

        [HttpGet("getstudentid")]
        public IActionResult GetStudentId()
        {
            var studentId = _httpContextAccessor.HttpContext.Session.GetInt32("StudentId");
            var firstname = _httpContextAccessor.HttpContext.Session.GetString("firstname");
            var lastname = _httpContextAccessor.HttpContext.Session.GetString("lastname");

            if (studentId == null)
                return Unauthorized(new { message = "No student is logged in." });

            return Ok(new { student_id = studentId, Firstname = firstname, Lastname = lastname });
        }

        [HttpPost("RequestPasswordReset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] ForgotPasswordRequest model)
        {
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                return BadRequest();
            }

            var normalizedEmail = model.Email.Trim().ToLower();

            var user = _context.DisplayStudents
                .FromSqlRaw("EXEC display_Student")
                .AsEnumerable()
                .FirstOrDefault(s => s.Email?.Trim().ToLower() == normalizedEmail);

            if (user == null)
            {
                return NotFound();
            }

            var resetLink = $"http://localhost:4200/ChangePassword?email={Uri.EscapeDataString(normalizedEmail)}";

            // Call the async version
            await SendResetPasswordEmail(user.Username, normalizedEmail, resetLink);

            return NoContent();
        }

        private async Task SendResetPasswordEmail(string username, string receiverEmail, string resetLink)
        {
            try
            {
                string subject = "Reset Your Password - LMS App";
                string body = $@"
    Hello {username},

    We have received a request to reset the password associated with your account on the Learning Management System (LMS).

    To proceed with resetting your password, please click the secure link below:

    {resetLink}

    If you did not initiate this request, no further action is required, and you may safely disregard this message.

    If you require any assistance, please do not hesitate to contact our support team.

    Thank You,  
    LMS Support Team
";
                var senderEmail = new MailAddress("shrukirti7377@gmail.com", "LMS System");
                var receiver = new MailAddress(receiverEmail, username);
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
                    Body = body
                })
                {
                    await smtp.SendMailAsync(message);
                }
            }
            catch (Exception ex)
            {
                string logPath = @"C:\ErrorLogs\email_log.txt";
                if (!Directory.Exists(Path.GetDirectoryName(logPath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(logPath));
                await System.IO.File.AppendAllTextAsync(logPath, $"{DateTime.Now}: {ex.ToString()}\n");
            }
        }
    

     [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ForgetPassword model)
        {
            if (string.IsNullOrEmpty(model.Email) ||
                string.IsNullOrEmpty(model.New_Password) ||
                string.IsNullOrEmpty(model.Confirm_Password))
            {
                return BadRequest(new { message = "All fields are required." });
            }

            if (model.New_Password != model.Confirm_Password)
            {
                return BadRequest(new { message = "Passwords do not match." });
            }

            var user = _context.DisplayStudents
                .FromSqlRaw("EXEC display_Student")
                .AsEnumerable()
                .FirstOrDefault(s => s.Email == model.Email);

            if (user == null)
            {
                return BadRequest(new { message = "Invalid email." });
            }

            // Update password via your stored proc
            var result = await _context.Database.ExecuteSqlRawAsync(
                "EXEC forgetPassword @p0, @p1, @p2",
                new SqlParameter("@p0", model.Email),
                new SqlParameter("@p1", model.New_Password),
                new SqlParameter("@p2", model.Confirm_Password)
            );

            return Ok(new { message = "Password has been reset successfully." });
        }
    }
    }

