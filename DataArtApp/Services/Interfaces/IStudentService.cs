using DataArtApp.DTOs;
using DataArtApp.Entities;
using DataArtApp.Responses;

namespace DataArtApp.Services.Interfaces
{
    public interface IStudentService
    {
        IEnumerable<Student> GetAllStudents();
        IEnumerable<Student> GetStudentsByTeacherName(string name = null);
        Task<Student> GetStudentById(uint id);
        Task<Student> GetStudentByName(string name);
        Task<GeneralResponse> CreateStudent(StudentDTO student);
        Task<GeneralResponse> UpdateStudent(uint id, StudentDTO student);
        Task<GeneralResponse> DeleteStudent(uint id);
    }
}
