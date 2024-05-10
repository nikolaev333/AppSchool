using DataArtApp.Entities.Enum;

namespace DataArtApp.Entities
{
    public class Student : Person
    {
        public string Major { get; set; }
        public int YearOfStudy { get; set; }

        public float GPA { get; set; }

        public GPAStatus GPAStatus { get; set; }

        public ICollection<StudentTeachers> StudentTeachers { get; set; }

    }
  }


