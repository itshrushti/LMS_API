﻿using LMS_Project_APIs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
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

                return Unauthorized(new { message = "Your Account is archived." , archive_date = student.Archive_Date });


            //var httpContext = _httpContextAccessor.HttpContext;
            HttpContext.Session.SetInt32("StudentId", student.Student_Id);
            HttpContext.Session.SetString("firstname", student.Firstname);
            HttpContext.Session.SetString("lastname", student.Lastname);
            HttpContext.Session.SetString("UserRole", student.Role_name);
            HttpContext.Session.SetString("LoginTime", DateTime.UtcNow.ToString());


            return Ok(new
            {
                Message = "Login Successful",
                Role = student.Role_name,
                StudentId = student.Student_Id,
                firstname = student.Firstname,
                lastname = student.Lastname,
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


        [HttpPost("CheckUserExists")]
        public IActionResult CheckUserExists([FromBody] string emailOrUsername)
        {
            if (string.IsNullOrEmpty(emailOrUsername))
            {
                return BadRequest(new { message = "Username or Email is required." });
            }

            var students = _context.DisplayStudents
                .FromSqlRaw("EXEC display_Student")
                .AsEnumerable()
                .ToList();

            var userExists = students.Any(s => s.Username == emailOrUsername || s.Email == emailOrUsername);

            if (!userExists)
            {
                return BadRequest(new { message = "User not found." });
            }

            return Ok(new { message = "User exists." });
        }


        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPassword model)
        {
            if (string.IsNullOrEmpty(model.UsernameAndEmail) ||
                string.IsNullOrEmpty(model.New_Password) ||
                string.IsNullOrEmpty(model.Confirm_Password))
            {
                return BadRequest(new { message = "All fields are required." });
            }

            if (model.New_Password != model.Confirm_Password)
            {
                return BadRequest(new { message = "New password and confirm password do not match." });
            }

            var result = await _context.Database.ExecuteSqlRawAsync(
                "EXEC forgetPassword @p0, @p1, @p2",
                new SqlParameter("@p0", model.UsernameAndEmail),
                new SqlParameter("@p1", model.New_Password),
                new SqlParameter("@p2", model.Confirm_Password)
            );

            return Ok(new { message = "Password has been reset successfully." });
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

        
    }
}

