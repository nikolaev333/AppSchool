using NUnit.Framework;
using DataArtApp.Entities;
using DataArtApp.DTOs;
using Microsoft.EntityFrameworkCore;
using Moq;
using AutoMapper;
using DataArtApp.Data;
using DataArtApp.Services.Implementations;
using DataArtApp.Exceptions;
using DataArtApp.Services.Interfaces;
namespace DataArtAppTests
{
    public class TeacherServiceTests
    {
        private ITeacherService _teacherService;
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
            _teacherService = new TeacherService(_context, _mockMapper.Object);

            // Ensure the database is clean before each test
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        //Teardown method is called to dispose of or "tear down" the context (_context) after the completion of tests 
        [TearDown]
        public void Teardown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task GetTeacherByName_ShouldReturnTeacher_WhenTeacherExists()
        {
            var teacher = CreateTeacher();
            await _context.Teachers.AddAsync(teacher);
            await _context.SaveChangesAsync();

            var result = await _teacherService.GetTeacherByName("TeacherTest");

            Assert.IsNotNull(result);
            Assert.AreEqual("TeacherTest", result.Name);
        }

        [Test]
        public void GetTeacherByName_ShouldThrowEntityNotFoundException_WhenTeacherDoesNotExist()
        {
            var ex = Assert.ThrowsAsync<EntityNotFoundException>(async () => await _teacherService.GetTeacherByName("NoName"));
            Assert.That(ex.Message, Is.EqualTo("Teacher with name: NoName does not exist."));
        }

        [Test]
        public void GetAllTeachers_ShouldReturnAllTeachers()
        {
            var teachers = new[]
            {
               CreateTeacher(), CreateTeacher()
            };

            _context.Teachers.AddRange(teachers);
            _context.SaveChanges();

            var result = _teacherService.GetAllTeachers();

            Assert.AreEqual(2, result.Count());
            Assert.Contains(teachers[0], result.ToList());
            Assert.Contains(teachers[1], result.ToList());
        }

        [Test]
        public void GetAllTeachers_ShouldThrowWhenNoTeachers()
        {
            var ex = Assert.Throws<EntityNotFoundException>(() => _teacherService.GetAllTeachers());
            Assert.That(ex.Message, Is.EqualTo("No teachers found in the database."));
        }

        [Test]
        public async Task UpdateTeacher_ShouldReturnSuccessResponse_WhenTeacherIsUpdated()
        {
            var existingTeacher = CreateTeacher();
            existingTeacher.Id = 1;
            await _context.Teachers.AddAsync(existingTeacher);
            await _context.SaveChangesAsync();

            var updatedTeacherDto = CreateUpdatedTeacherDTO();
            _mockMapper.Setup(m => m.Map<TeacherDTO, Teacher>(updatedTeacherDto, existingTeacher)).Callback(() =>
            {
                existingTeacher.Name = updatedTeacherDto.Name;
                existingTeacher.Subject = updatedTeacherDto.Subject;
                existingTeacher.Experience = updatedTeacherDto.Experience;
            });

            var result = await _teacherService.UpdateTeacher(existingTeacher.Id, updatedTeacherDto);

            Assert.IsTrue(result.Success);
            Assert.AreEqual("Teacher updated successfully.", result.Message);
            var updatedTeacher = await _context.Teachers.FindAsync(existingTeacher.Id);
            Assert.AreEqual(updatedTeacherDto.Name, updatedTeacher.Name);
            Assert.AreEqual(updatedTeacherDto.Subject, updatedTeacher.Subject);
            Assert.AreEqual(updatedTeacherDto.Experience, updatedTeacher.Experience);
        }

        [Test]
        public async Task UpdateTeacher_ShouldReturnErrorResponse_WhenTeacherNotFound()
        {
            var teacherDto = CreateTeacherDTO();

            var result = await _teacherService.UpdateTeacher(999, teacherDto);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Teacher not found.", result.Message);
        }

        [Test]
        public async Task DeleteTeacher_ShouldReturnSuccessResponse_WhenTeacherIsDeleted()
        {
            var teacher = CreateTeacher();
            await _context.Teachers.AddAsync(teacher);
            await _context.SaveChangesAsync();

            var result = await _teacherService.DeleteTeacher(1);

            Assert.IsTrue(result.Success);
            Assert.AreEqual("Teacher deleted successfully.", result.Message);
            Assert.AreEqual(0, await _context.Teachers.CountAsync());
        }

        [Test]
        public async Task DeleteTeacher_ShouldReturnErrorResponse_WhenTeacherNotFound()
        {
            var result = await _teacherService.DeleteTeacher(1);  

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Teacher not found.", result.Message);
        }

        private static TeacherDTO CreateTeacherDTO()
        {
            return new TeacherDTO
            {
                Name = "TeacherTest",
                DateOfBirth = DateOnly.FromDateTime(DateTime.Parse("1980-08-15")),
                Subject = "Physics",
                Experience = 10,
                StudentsIds = new List<uint> { 101, 102 },
                Email = "teachertest@example.com",
                PIN = "0987654321"
            };
        }

        private static TeacherDTO CreateUpdatedTeacherDTO()
        {
            return new TeacherDTO
            {
                Name = "Updated Teacher",
                DateOfBirth = DateOnly.FromDateTime(DateTime.Parse("1981-09-16")),
                Subject = "Mathematics",
                Experience = 12,
                Email = "updatedteacher@example.com",
                PIN = "1234567890"
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
