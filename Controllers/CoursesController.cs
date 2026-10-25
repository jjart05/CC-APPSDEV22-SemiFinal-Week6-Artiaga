using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentRosterDbApi.Data;
using StudentRosterDbApi.DTOs;
using StudentRosterDbApi.Models;

namespace StudentRosterDbApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CoursesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CoursesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses()
        {
            var courses = await _context.Courses
                .Select(c => new CourseDto { Id = c.Id, Title = c.Title })
                .ToListAsync();

            return Ok(courses);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CourseDto>> CreateCourse([FromBody] CourseCreateDto dto)
        {
            var course = new Course { Title = dto.Title };
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            var result = new CourseDto { Id = course.Id, Title = course.Title };
            return CreatedAtAction(nameof(GetCourses), new { id = course.Id }, result);
        }
    }
}
