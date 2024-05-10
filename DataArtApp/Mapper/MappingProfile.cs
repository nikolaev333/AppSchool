using AutoMapper;
using DataArtApp.DTOs;
using DataArtApp.Entities;

namespace DataArtApp.Mapper
{
    public class MappingProfile: Profile
    {
        public MappingProfile() {

            CreateMap<TeacherDTO, Teacher>();
            CreateMap<StudentDTO, Student>();
        }
    }
}
