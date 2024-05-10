namespace DataArtApp.DTOs
{
    public class TeacherDTO :PersonDTO
    {
        public string Subject { get; set; }
        public double Experience { get; set; }
        public List<uint>? StudentsIds { get; set; }
    }
}
