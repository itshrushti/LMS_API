﻿using LMS_Project_APIs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;

namespace LMS_Project_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountStudentDashboardController : ControllerBase
    {
        private readonly LearningManagementSystemContext _context;

        public CountStudentDashboardController(LearningManagementSystemContext context)
        {
            _context = context;
        }

        [HttpGet("getCountStudentDashboard/{studentId}")]
        public IActionResult getCountStudentDashboard(int studentId)
        {
            if (studentId == 0)
            {
                return BadRequest(new {Message = "Invalid student type Id."});
            }

            try
            {
                var result = _context.TblCountStudentDashboards
                                        .FromSqlRaw("EXEC student_Dashboard @p0", studentId)
                                        .AsEnumerable()
                                        .ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new {Message = "An error:", Error = ex.Message});
            }
        }
    }
}
