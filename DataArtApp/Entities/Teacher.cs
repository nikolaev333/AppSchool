namespace DataArtApp.Entities
{
    public class Teacher : Person
    {
        public string Subject { get; set; }

        public double Experience {  get; set; }

        public ICollection<StudentTeachers> StudentTeachers { get; set; }
    }
}
