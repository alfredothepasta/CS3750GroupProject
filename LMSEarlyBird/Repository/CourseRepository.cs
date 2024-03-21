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

        public async Task<bool> Delete(Course course)
        {
            List<StudentCourse> studentCourses = await _context.StudentCourses.Where(sc => sc.CourseId == course.id).ToListAsync();
            List<InstructorCourse> instructorCourses = await _context.InstructorCourses.Where(ic => ic.CourseId == course.id).ToListAsync();
            List<Assignment> courseAssignments = await _context.Assignments.Where(c => c.CourseId == course.id).ToListAsync();
            _context.StudentCourses.RemoveRange(studentCourses);
            _context.InstructorCourses.RemoveRange(instructorCourses);
            _context.Assignments.RemoveRange(courseAssignments);
            _context.Courses.Remove(course);
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
            return await _context.Courses.Include(x => x.Department).Where(x => x.id == id).FirstAsync();
        }

        public async Task<List<Course>> GetCoursesByTeacher(string teacherId)
        {
            return await _context.Courses
                .Include(x => x.Department)
                .Where(c => c.InstructorCourses
                    .Where(i => i.UserId == teacherId)
                    .Any())
                .ToListAsync();
        }

        public async Task<List<Course>> GetCoursesByStudent(string studentId)
        {
            return await _context.Courses
                .Where(c => c.StudentCourses
                    .Where(i => i.UserId == studentId)
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