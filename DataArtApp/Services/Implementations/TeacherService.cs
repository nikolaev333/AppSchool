using AutoMapper;
using DataArtApp.Data;
using DataArtApp.DTOs;
using DataArtApp.Entities;
using DataArtApp.Exceptions;
using DataArtApp.Responses;
using DataArtApp.Services.Interfaces;
using DataArtApp.Validations;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace DataArtApp.Services.Implementations
{
    public class TeacherService : ITeacherService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public TeacherService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<GeneralResponse> CreateTeacher(TeacherDTO teacherDTO)
        {

            var errors = ValidationService.ValidatePersonDTO(teacherDTO);

            if (errors != null && errors.Any())
            {
                var errorMessage = new StringBuilder();
                foreach (var error in errors)
                {
                    errorMessage.Append(error);
                }
                errorMessage.Length -= 2;
                return new GeneralResponse(false, errorMessage.ToString());
            }

            var teacher = _mapper.Map<Teacher>(teacherDTO);
            AddStudents(teacherDTO, teacher);

            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "Teacher successfully created.");
        }

        public async Task<Teacher> GetTeacherById(uint id)
        {
            var teacher = await _context.Teachers.FindAsync(id);

            if (teacher == null)
            {
                throw new EntityNotFoundException($"Teacher with ID: {id} does not exist.");
            }

            return teacher;
        }

        public async Task<Teacher> GetTeacherByName(string name)
        {
            var teacher = await _context.Teachers
                                        .Where(t => t.Name == name)
                                        .FirstOrDefaultAsync();

            if (teacher == null)
            {
                throw new EntityNotFoundException($"Teacher with name: {name} does not exist.");
            }

            return teacher;
        }



        public IEnumerable<Teacher> GetAllTeachers()
        {
            var teachers = _context.Teachers.ToList();

            if (teachers.Count == 0)
            {
                throw new EntityNotFoundException("No teachers found in the database.");
            }
            return teachers;
        }

        public async Task<GeneralResponse> UpdateTeacher(uint id, TeacherDTO teacherDTO)
        {

            var errors = ValidationService.ValidatePersonDTO(teacherDTO);

            if (errors != null && errors.Any())
            {
                var errorMessage = new StringBuilder();
                foreach (var error in errors)
                {
                    errorMessage.Append(error);
                }
                errorMessage.Length -= 2;
                return new GeneralResponse(false, errorMessage.ToString());
            }

            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return new GeneralResponse(false, "Teacher not found.");
            }

            _mapper.Map(teacherDTO, teacher);

            // Add students
            AddStudents(teacherDTO, teacher);

            await _context.SaveChangesAsync();

            return new GeneralResponse(true, "Teacher updated successfully.");
        }

        public async Task<GeneralResponse> DeleteTeacher(uint id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return new GeneralResponse(false, "Teacher not found.");
            }
            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "Teacher deleted successfully.");
        }

        private static void AddStudents(TeacherDTO teacherDTO, Teacher teacher)
        {
            if (teacherDTO.StudentsIds != null && teacherDTO.StudentsIds.Count > 0)
            {
                teacher.StudentTeachers = new List<StudentTeachers>();
                foreach (var studentId in teacherDTO.StudentsIds)
                {
                    teacher.StudentTeachers.Add(new StudentTeachers { TeacherId = studentId });
                }
            }
        }
    }
}
