using AutoMapper;
using DataArtApp.Data;
using DataArtApp.DTOs;
using DataArtApp.Entities;
using DataArtApp.Entities.Enum;
using DataArtApp.Exceptions;
using DataArtApp.Services.Implementations;
using DataArtApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;


namespace DataArtAppTests
{
    namespace DataArtAppTests
    {
        public class StudentServiceTests
        {
            private IStudentService _studentService;
            private AppDbContext _context;
            private Mock<IMapper> _mockMapper;

            [SetUp]
            public void Setup()
            {
                var options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(databaseName: "TestDb")
                    .Options;

                _context = new AppDbContext(options);
                _mockMapper = new Mock<IMapper>();
                _studentService = new StudentService(_context, _mockMapper.Object);

                // Ensure the database is clean before each test
                _context.Database.EnsureDeleted();
                _context.Database.EnsureCreated();
            }

            [TearDown]
            public void Teardown()
            {
                _context.Dispose();
            }

            [Test]
            public async Task CreateStudent_ShouldAssignTeachersCorrectly()
            {
                StudentDTO studentDto = CreateStudentDTO();

                _mockMapper.Setup(m => m.Map<Student>(It.IsAny<StudentDTO>()))
                    .Returns(CreateStudent());

                var response = await _studentService.CreateStudent(studentDto);

                Assert.IsTrue(response.Success);
                var savedStudent = await _context.Students.Include(s => s.StudentTeachers).FirstOrDefaultAsync(s => s.Name == "StudentTest");
                Assert.IsNotNull(savedStudent);
                Assert.AreEqual(2, savedStudent.StudentTeachers.Count);
                Assert.AreEqual(studentDto.Name, savedStudent.Name);
                Assert.AreEqual(studentDto.DateOfBirth, savedStudent.DateOfBirth);
                Assert.AreEqual(studentDto.GPA, savedStudent.GPA);
                Assert.AreEqual(studentDto.Email, savedStudent.Email);
                Assert.AreEqual(studentDto.Major, savedStudent.Major);
                Assert.AreEqual(studentDto.Sex, savedStudent.Sex);
            }

            [Test]
            public void CreateStudent_ShouldCalculateGPAStatusCorrectly()
            {
                var studentDto = CreateStudentDTO();

                _mockMapper.Setup(m => m.Map<Student>(It.IsAny<StudentDTO>()))
                    .Returns(CreateStudent());

                Assert.DoesNotThrowAsync(async () => await _studentService.CreateStudent(studentDto));

                var createdStudent = _context.Students.FirstOrDefault(s => s.Name == studentDto.Name);
                Assert.IsNotNull(createdStudent);
                Assert.AreEqual(GPAStatus.VeryGood, createdStudent.GPAStatus);
            }

            [Test]
            public void CreateStudent_ShouldCalculateYearOfStudyCorrectly()
            {
                var studentDto = CreateStudentDTO();

                _mockMapper.Setup(m => m.Map<Student>(It.IsAny<StudentDTO>()))
                    .Returns(CreateStudent());

                Assert.DoesNotThrowAsync(async () => await _studentService.CreateStudent(studentDto));

                var createdStudent = _context.Students.FirstOrDefault(s => s.Name == studentDto.Name);
                Assert.IsNotNull(createdStudent);
                Assert.AreEqual(12, createdStudent.YearOfStudy);
            }

            [Test]
            public async Task GetStudentById_ShouldReturnStudent_WhenStudentExists()
            {
                var student = CreateStudent();
                await _context.Students.AddAsync(student);
                await _context.SaveChangesAsync();

                var result = await _studentService.GetStudentById(1);

                Assert.IsNotNull(result);
                Assert.AreEqual(student.Id, result.Id);
                Assert.AreEqual(student.Name, result.Name);
                Assert.AreEqual(student.GPA, result.GPA);
                Assert.AreEqual(student.Major, result.Major);
                Assert.AreEqual(student.Email, result.Email);
                Assert.AreEqual(student.PIN, result.PIN);
            }

            [Test]
            public void GetStudentById_ShouldThrowEntityNotFoundException_WhenStudentDoesNotExist()
            {
                var ex = Assert.ThrowsAsync<EntityNotFoundException>(async () => await _studentService.GetStudentById(1));
                Assert.That(ex.Message, Is.EqualTo("Student with ID: 1 does not exist."));
            }

            [Test]
            public async Task DeleteStudent_ShouldReturnErrorResponse_WhenStudentNotFound()
            {
                var result = await _studentService.DeleteStudent(1);

                Assert.IsFalse(result.Success);
                Assert.AreEqual("Student not found.", result.Message);
            }

            [Test]
            public async Task DeleteStudent_ShouldReturnSuccessResponse_WhenStudentIsDeleted()
            {
                var student = CreateStudent();
                _context.Students.Add(student);
                await _context.SaveChangesAsync();

                var result = await _studentService.DeleteStudent(1);

                Assert.IsTrue(result.Success);
                Assert.AreEqual("Student deleted successfully.", result.Message);
            }

            [Test]
            public void GetAllStudents_ShouldReturnAllStudents()
            {
                var students = new[]
                {
                CreateStudent(), CreateStudent()
            };

                _context.Students.AddRange(students);
                _context.SaveChanges();

                var result = _studentService.GetAllStudents();

                Assert.AreEqual(2, result.Count());
                Assert.Contains(students[0], result.ToList());
                Assert.Contains(students[1], result.ToList());
            }

            [Test]
            public void GetAllStudents_ShouldThrowWhenNoStudents()
            {
                var ex = Assert.Throws<EntityNotFoundException>(() => _studentService.GetAllStudents());
                Assert.That(ex.Message, Is.EqualTo("No students found in the database."));
            }

            [Test]
            public async Task UpdateStudent_ShouldReturnErrorResponse_WhenStudentNotFound()
            {
                var studentDto = CreateStudentDTO();

                var result = await _studentService.UpdateStudent(999, studentDto);

                Assert.IsFalse(result.Success);
                Assert.AreEqual("Student not found.", result.Message);
            }
            [Test]
            public void GetStudentsByTeacherName_ShouldReturnStudents_WhenTeacherExistsWithStudents()
            {
                var teacher = CreateTeacher();
                var student1 = CreateStudent();
                var student2 = CreateStudent();
                _context.Teachers.Add(teacher);
                _context.Students.AddRange(student1, student2);
                _context.SaveChanges();

                _context.StudentTeachers.Add(new StudentTeachers { StudentId = student1.Id, TeacherId = teacher.Id });
                _context.StudentTeachers.Add(new StudentTeachers { StudentId = student2.Id, TeacherId = teacher.Id });
                _context.SaveChanges();

                var results = _studentService.GetStudentsByTeacherName("TeacherTest").ToList();

                Assert.IsNotNull(results);
                Assert.AreEqual(2, results.Count);
                Assert.IsTrue(results.Any(s => s.Name == "StudentTest"));
                Assert.IsTrue(results.Any(s => s.Name == "StudentTest"));
            }

            [Test]
            public void GetStudentsByTeacherName_ShouldReturnEmpty_WhenTeacherDoesNotExist()
            {
                var results = _studentService.GetStudentsByTeacherName("Nonexistent Teacher").ToList();

                Assert.IsNotNull(results);
                Assert.IsEmpty(results);
            }

            [Test]
            public void GetStudentsByTeacherName_ShouldReturnEmpty_WhenTeacherHasNoStudents()
            {
                var teacher = CreateTeacher();
                _context.Teachers.Add(teacher);
                _context.SaveChanges();

                var results = _studentService.GetStudentsByTeacherName("TeacherTest").ToList();

                Assert.IsNotNull(results);
                Assert.IsEmpty(results);
            }
            [Test]
            public async Task UpdateStudent_ShouldReturnSuccessResponse_WhenStudentIsUpdated()
            {
                var originalStudent = CreateStudent();
                await _context.Students.AddAsync(originalStudent);
                await _context.SaveChangesAsync();

                var updatedStudentDto = CreateUpdatedStudentDTO();

                _mockMapper.Setup(m => m.Map<StudentDTO, Student>(updatedStudentDto, It.IsAny<Student>())).Callback<StudentDTO, Student>((dto, student) =>
                {
                    student.Name = dto.Name;
                    student.DateOfBirth = dto.DateOfBirth;
                    student.GPA = dto.GPA;
                    student.Major = dto.Major;
                    student.Email = dto.Email;
                    student.PIN = dto.PIN;
                });

                var result = await _studentService.UpdateStudent(originalStudent.Id, updatedStudentDto);

                Assert.IsTrue(result.Success);
                Assert.AreEqual("Student updated successfully.", result.Message);

                var studentInDb = await _context.Students.FindAsync(originalStudent.Id);
                Assert.IsNotNull(studentInDb);
                Assert.AreEqual(updatedStudentDto.Name, studentInDb.Name);
                Assert.AreEqual(updatedStudentDto.DateOfBirth, studentInDb.DateOfBirth);
                Assert.AreEqual(updatedStudentDto.GPA, studentInDb.GPA);
                Assert.AreEqual(updatedStudentDto.Major, studentInDb.Major);
            }

            private static StudentDTO CreateStudentDTO()
            {
                return new StudentDTO
                {
                    Name = "StudentTest",
                    DateOfBirth = DateOnly.FromDateTime(DateTime.Parse("2005-04-01")),
                    GPA = 4.5F,
                    Major = "Physics",
                    TeacherIds = new List<uint> { 101, 102 },
                    Email = "studenttest@gmail.com",
                    PIN = "1234567890"
                };
            }

            private static Student CreateStudent()
            {
                return new Student
                {
                    Name = "StudentTest",
                    DateOfBirth = DateOnly.FromDateTime(DateTime.Parse("2005-04-01")),
                    GPA = 4.5F,
                    Major = "Physics",
                    Email = "studenttest@gmail.com",
                    PIN = "1234567890"
                };

            }
            private static StudentDTO CreateUpdatedStudentDTO()
            {
                return new StudentDTO
                {
                    Name = "Updated Student",
                    DateOfBirth = DateOnly.FromDateTime(DateTime.Parse("1991-04-07")),
                    GPA = 5.5F,
                    Major = "Updated Major",
                    Email = "updatedstudent@gmail.com",
                    PIN = "0987654321"
                };
            }
            private static Teacher CreateTeacher()
            {
                return new Teacher
                {
                    Name = "TeacherTest",
                    DateOfBirth = DateOnly.FromDateTime(DateTime.Parse("1980-08-15")),
                    Subject = "Physics",
                    Experience = 10,
                    Email = "teachertest@example.com",
                    PIN = "0987654321"
                };
            }
        }
    }
}