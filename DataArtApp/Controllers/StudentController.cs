using DataArtApp.DTOs;
using DataArtApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DataArtApp.Controllers
{
    [Route("api/student")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateStudent([FromBody] StudentDTO studentDTO)
        {
            if (studentDTO == null)
            {
                return BadRequest("Student data is null.");
            }

            var response = await _studentService.CreateStudent(studentDTO);
            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return StatusCode(500, response.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentById(uint id)
        {
            try
            {
                var student = await _studentService.GetStudentById(id);
                if (student != null)
                {
                    return Ok(student);
                }
                else
                {
                    return NotFound();
                }
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetStudentByName(string name)
        {
            try
            {
                var student = await _studentService.GetStudentByName(name);
                if (student != null)
                {
                    return Ok(student);
                }
                else
                {
                    return NotFound();
                }
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public IActionResult GetAllStudents()
        {
            try
            {
                var students = _studentService.GetAllStudents();
                return Ok(students);
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("by-teacher")]
        public IActionResult GetStudentsByTeacherName([FromQuery] string name)
        {
            try
            {
                var students = _studentService.GetStudentsByTeacherName(name);
                return Ok(students);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(uint id, [FromBody] StudentDTO studentDTO)
        {
            if (studentDTO == null)
            {
                return BadRequest("Student data is null.");
            }

            var response = await _studentService.UpdateStudent(id, studentDTO);
            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return StatusCode(500, response.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(uint id)
        {
            var response = await _studentService.DeleteStudent(id);
            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return StatusCode(500, response.Message);
            }
        }
    }
}
