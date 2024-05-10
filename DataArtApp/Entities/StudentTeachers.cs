namespace DataArtApp.Entities
{
    public class StudentTeachers
    {
        public uint StudentId { get; set; }
        public Student Student { get; set; }

        public uint TeacherId { get; set; }
        public Teacher Teacher { get; set; }
    }
}
