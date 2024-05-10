using AutoMapper;
using DataArtApp.Data;
using DataArtApp.DTOs;
using DataArtApp.Entities;
using DataArtApp.Entities.Enum;
using DataArtApp.Exceptions;
using DataArtApp.Responses;
using DataArtApp.Services.Interfaces;
using DataArtApp.Validations;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace DataArtApp.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public StudentService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<GeneralResponse> CreateStudent(StudentDTO studentDTO)
        {
            // Validate the personal information part of the DTO
            var errors = ValidationService.ValidatePersonDTO(studentDTO);

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

            var student = _mapper.Map<Student>(studentDTO);

            student.GPAStatus = CalculateGPAStatus(student.GPA);
            student.YearOfStudy = CalculateClassByAge(student.DateOfBirth);

            AddTeachers(studentDTO, student);

            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "Student successfully created.");
        }

        public async Task<Student> GetStudentById(uint id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                throw new EntityNotFoundException($"Student with ID: {id} does not exist.");
            }

            return student;
        }

        public async Task<Student> GetStudentByName(string name)
        {
            var student = await _context.Students
                               .Where(s => s.Name == name)
                               .FirstOrDefaultAsync();

            if (student == null)
            {
                throw new EntityNotFoundException($"Student with name: {name} does not exist.");
            }
            return student;
        }

        public IEnumerable<Student> GetAllStudents()
        {
            var students = _context.Students.ToList();

            if (students.Count == 0)
            {
                throw new EntityNotFoundException("No students found in the database.");
            }
            return students;
        }

        public IEnumerable<Student> GetStudentsByTeacherName(string name = null)
        {
            var query = _context.Students.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(s => s.StudentTeachers.Any(st => st.Teacher.Name.Contains(name)));
            }

            return query.ToList();
        }

        public async Task<GeneralResponse> UpdateStudent(uint id, StudentDTO studentDTO)
        {
           var errors = ValidationService.ValidatePersonDTO(studentDTO);

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

            uint studentId = Convert.ToUInt32(id);

            var student = await _context.Students.FindAsync(studentId);

            if (student == null)
            {
                return new GeneralResponse(false, "Student not found.");
            }

            _mapper.Map(studentDTO, student);

            student.GPAStatus = CalculateGPAStatus(student.GPA);

            AddTeachers(studentDTO, student);

            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "Student updated successfully.");
        }

        public async Task<GeneralResponse> DeleteStudent(uint id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return new GeneralResponse(false, "Student not found.");
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return new GeneralResponse(true, "Student deleted successfully.");
        }

        private int CalculateClassByAge(DateOnly birthDate)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - birthDate.Year;
            if (birthDate > today.AddYears(-age))
                age--;

            if (age < 7)
            {
                return 0;
            }
            else if (age >= 7 && age <= 18)
            {
                return age - 6;
            }
            else
            {
                return 12;
            }
        }

        private GPAStatus CalculateGPAStatus(float gpa)
        {
            if (gpa < 2.0)
            {
                throw new ArgumentOutOfRangeException("GPA must be at least 2.0");
            }
            else if (gpa < 3.0)
            {
                return GPAStatus.Poor;
            }
            else if (gpa < 3.5)
            {
                return GPAStatus.Average;
            }
            else if (gpa < 4.5)
            {
                return GPAStatus.Good;
            }
            else if (gpa < 5.5)
            {
                return GPAStatus.VeryGood;
            }
            else if (gpa <= 6.0)
            {
                return GPAStatus.Excellent;
            }
            else
            {
                throw new ArgumentOutOfRangeException("GPA cannot exceed 6.0");
            }
        }

        private static void AddTeachers(StudentDTO studentDTO, Student student)
        {
            if (studentDTO.TeacherIds != null && studentDTO.TeacherIds.Count > 0)
            {
                student.StudentTeachers = new List<StudentTeachers>();
                foreach (var teacherId in studentDTO.TeacherIds)
                {
                    student.StudentTeachers.Add(new StudentTeachers { TeacherId = teacherId });
                }
            }
        }
    }
}
