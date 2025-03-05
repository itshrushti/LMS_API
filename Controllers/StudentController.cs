using LMS_Project_APIs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS_Project_APIs.Controllers
{
    public class StudentController : Controller
    {
        private readonly LearningManagementSystemContext _context;

        public StudentController(LearningManagementSystemContext context)
        {
            _context = context;
        }

        [HttpGet("GetStudents")]
        public async Task<ActionResult<IEnumerable<TblStudent>>> GetStudents()
        {
            var students = await _context.TblStudents
                .Include(s => s.Role)
                .Select(s => new TblStudent
                {
                    StudentId=s.StudentId,
                    StudentNo=s.StudentNo,
                    Firstname=s.Firstname,
                    Middlename=s.Middlename,
                    Lastname=s.Lastname,
                    Username=s.Username,
                    Password = s.Password,
                    Email =s.Email,
                    RoleId=s.RoleId,
                    Role_name=s.Role.RoleName,
                    ArchiveDate=s.ArchiveDate,
                    PhoneNo=s.PhoneNo,
                    Address=s.Address,
                    City=s.City,
                    PostalCode=s.PostalCode,
                    State=s.State,
                    Country=s.Country,
                    CreateDate=s.CreateDate,
                    UpdateDate=s.UpdateDate

                }).ToListAsync();

            return Ok(students);
        }
    }
}
