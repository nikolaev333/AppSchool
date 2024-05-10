using DataArtApp.Entities.Enum;

namespace DataArtApp.DTOs
{
    public abstract class PersonDTO
    {
        public string Name { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string PIN { get; set; }
        public Gender Sex { get; set; }
        public string Email { get; set; }
    }
}
