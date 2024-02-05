using LMSEarlyBird.Models;

namespace LMSEarlyBird.Interfaces
{
    public interface ICourseRepository
    {
        Task<IEnumerable<Course>> GetAllCourses();
        Task<Course> GetCourse(int id);

        Task<List<Course>> GetCoursesByTeacher(string teacherId); // <---gets all courses by teacher id>>
        bool Add(Course course);
        bool Update(Course course);
        bool Delete(Course course);
        bool Save();
    }
}