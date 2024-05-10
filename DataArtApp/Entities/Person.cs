using DataArtApp.Entities.Enum;

namespace DataArtApp.Entities
{
    public abstract class Person
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string PIN { get; set; }
        public Gender Sex { get; set; }
        public string Email {get; set; }
    }
}
