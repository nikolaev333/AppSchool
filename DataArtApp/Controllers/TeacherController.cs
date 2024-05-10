using DataArtApp.DTOs;
using DataArtApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DataArtApp.Controllers
{
    [Route("api/teacher")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private readonly ITeacherService _teacherService;

        public TeacherController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeacher([FromBody] TeacherDTO teacherDTO)
        {
            if (teacherDTO == null)
            {
                return BadRequest("Teacher data is null.");
            }

            var response = await _teacherService.CreateTeacher(teacherDTO);
            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return StatusCode(500, response.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTeachers()
        {
            try
            {
                var teachers = _teacherService.GetAllTeachers();
                return Ok(teachers);
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("/{id}")]
        public async Task<IActionResult> GetTeacherById(uint id)
        {
            try
            {
                var teacher = await _teacherService.GetTeacherById(id);
                if (teacher != null)
                {
                    return Ok(teacher);
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

        [HttpGet("/name/{name}")]
        public async Task<IActionResult> GetTeacherByName(string name)
        {
            try
            {
                var teacher = await _teacherService.GetTeacherByName(name);
                if (teacher != null)
                {
                    return Ok(teacher);
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
      

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeacher(uint id, [FromBody] TeacherDTO teacherDTO)
        {
            if (teacherDTO == null)
            {
                return BadRequest("Teacher data is null.");
            }

            var response = await _teacherService.UpdateTeacher(id, teacherDTO);
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
        public async Task<IActionResult> DeleteTeacher(uint id)
        {
            var response = await _teacherService.DeleteTeacher(id);
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