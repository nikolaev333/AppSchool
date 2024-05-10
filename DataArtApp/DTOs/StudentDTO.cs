namespace DataArtApp.DTOs
{
    public class StudentDTO : PersonDTO
    {
        public string Major { get; set; }
        public float GPA { get; set; }
        public List<uint>? TeacherIds { get; set; }
    }
}
