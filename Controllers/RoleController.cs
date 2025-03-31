using LMS_Project_APIs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS_Project_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly LearningManagementSystemContext _context;

        public RoleController(LearningManagementSystemContext context)
        {
            _context = context;
        }

        [HttpGet("GetRoles")]

        public async Task<IActionResult> GetRoles()
        {
            try
            {
                var roles = await _context.Roles.FromSqlRaw("EXEC display_Role").ToListAsync();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error : ", Error = ex.Message });
            }
        }

        [HttpPost("AddEditRole")]
        public async Task<IActionResult> AddEditRole(Role role)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC add_edit_Role @p0, @p1",

                    role.Role_Id == 0 ? null : role.Role_Id,
                    role.Role_Name
                  );
                return Ok(new { Message = "Role Added/Updated." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error : ", Error = ex.Message });
            }
        }

        [HttpDelete("DeleteRole")]
        public async Task<IActionResult> DeleteRole(int roleid)
        {
           
            try
            {
             
                await _context.Database.ExecuteSqlRawAsync("EXEC delete_Role @p0", roleid);
                return Ok(new { Message = "Students deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "an error : ", Error = ex.Message });
            }
        }

        [HttpGet("searchRole")]
        public async Task<IActionResult> SearchStudent(string searchValue)
        {
            if (string.IsNullOrEmpty(searchValue))
            {
                return BadRequest(new { Message = "Search Value is required." });
            }
            try
            {
                var stud = await _context.Roles.FromSqlRaw("EXEC search_Role @p0", searchValue).ToListAsync();
                return Ok(stud);
            }

            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error : ", Error = ex.Message });
            }
        }
    }
}
