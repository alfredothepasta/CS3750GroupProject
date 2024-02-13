using LMSEarlyBird.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LMSEarlyBird.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Many to many relationship for Student Course
            builder.Entity<StudentCourse>().HasKey(sc => new
            {
                sc.UserId,
                sc.CourseId
            });
            builder.Entity<StudentCourse>().HasOne(c => c.User).WithMany(c => c.StudentCourses).HasForeignKey(c => c.UserId);
            builder.Entity<StudentCourse>().HasOne(c => c.Course).WithMany(c => c.StudentCourses).HasForeignKey(c => c.CourseId);

            // Many to many relationship for Instructor Course
            builder.Entity<InstructorCourse>().HasKey(sc => new
            {
                sc.UserId,
                sc.CourseId
            });
            builder.Entity<InstructorCourse>().HasOne(c => c.User).WithMany(c => c.InstructorCourses).HasForeignKey(c => c.UserId);
            builder.Entity<InstructorCourse>().HasOne(c => c.Course).WithMany(c => c.InstructorCourses).HasForeignKey(c => c.CourseId);

            // One to One relationship between users and addresses
            builder.Entity<Address>().HasOne(x => x.User).WithOne(x => x.Address).HasForeignKey<Address>(a => a.UserID);


            base.OnModelCreating(builder);
        }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Building> Buildings { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<InstructorCourse> InstructorCourses { get; set; }

        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<StudentAssignment> StudentAssignments { get; set; }
    }
}
