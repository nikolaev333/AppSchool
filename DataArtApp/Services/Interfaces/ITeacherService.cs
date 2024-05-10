using DataArtApp.DTOs;
using DataArtApp.Entities;
using DataArtApp.Responses;

namespace DataArtApp.Services.Interfaces
{
    public interface ITeacherService
    {
        IEnumerable<Teacher> GetAllTeachers();
        Task<Teacher> GetTeacherById(uint id);
        Task<Teacher> GetTeacherByName(string name);
        Task<GeneralResponse> CreateTeacher(TeacherDTO teacher);
        Task<GeneralResponse> UpdateTeacher(uint id, TeacherDTO teacher);
        Task<GeneralResponse> DeleteTeacher(uint id);
    }
}
