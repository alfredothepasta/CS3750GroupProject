using LMSEarlyBird.Interfaces;
using LMSEarlyBird.Models;
using LMSEarlyBird.Data;
using Microsoft.EntityFrameworkCore;

namespace LMSEarlyBird.Repository{
    public class CourseRepository : ICourseRepository
    {
        private readonly ApplicationDbContext _context;

        public CourseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Add(Course course, AppUser instructor)
        {
            InstructorCourse instructorCourse = new InstructorCourse
            {
                Course = course,
                User = instructor
            };

            _context.Add(course);
            _context.Add(instructorCourse);
            return Save();
        }

        public bool Delete(Course course)
        {
            List<StudentCourse> studentCourses = _context.StudentCourses.Where(sc => sc.CourseId == course.id).ToList();
            List<InstructorCourse> instructorCourses = _context.InstructorCourses.Where(ic => ic.CourseId == course.id).ToList();
            _context.RemoveRange(studentCourses);
            _context.RemoveRange(instructorCourses);
            _context.Remove(course);
            return Save();
        }

        public async Task<IEnumerable<Course>> GetAllCourses()
        {
            return await _context.Courses.ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetAllCoursesWithInstructor()
        {
            return await _context.Courses.Include(x => x.InstructorCourses)
                             .ThenInclude(ic => ic.User)
                             .ToListAsync();
        }

        public async Task<Course> GetCourse(int id)
        {
            return await _context.Courses.FindAsync(id);
        }

        public async Task<List<Course>> GetCoursesByTeacher(string teacherId)
        {
            return await _context.Courses
                .Where(c => c.InstructorCourses
                    .Where(i => i.UserId == teacherId)
                    .Any())
                .ToListAsync();
        }

        public async Task<List<Course>> GetCourseByRoomId(int id)
        {
            return await _context.Courses.Where(c => c.RoomId == id).ToListAsync();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Course course)
        {
            _context.Update(course);
            return Save();
        }
    }
}