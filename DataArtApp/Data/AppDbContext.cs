using DataArtApp.Entities;
using DataArtApp.Entities.Enum;
using Microsoft.EntityFrameworkCore;

namespace DataArtApp.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }

        public DbSet<StudentTeachers> StudentTeachers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<StudentTeachers>()
                       .HasKey(st => new { st.StudentId, st.TeacherId });

            modelBuilder.Entity<StudentTeachers>()
                .HasOne(st => st.Student)
                .WithMany(s => s.StudentTeachers)
                .HasForeignKey(st => st.StudentId);

            modelBuilder.Entity<StudentTeachers>()
                .HasOne(st => st.Teacher)
                .WithMany(t => t.StudentTeachers)
                .HasForeignKey(st => st.TeacherId);

            modelBuilder.Entity<Person>()
                 .HasKey(p => p.Id);


            modelBuilder.Entity<Person>()
                 .Property(p => p.Sex)
                 .HasConversion(
            v => v.ToString(),
            v => (Gender)Enum.Parse(typeof(Gender), v));

            modelBuilder.Entity<Student>()
                .Property(s => s.GPAStatus)
                .HasConversion(
                    v => v.ToString(),
                    v => (GPAStatus)Enum.Parse(typeof(GPAStatus), v));
        }
    }
}
