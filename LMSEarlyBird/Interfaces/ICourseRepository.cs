using LMSEarlyBird.Models;

namespace LMSEarlyBird.Interfaces
{
    public interface ICoarseRepository
    {
        Task<IEnumerable<Course>> GetAllCourses();

        Task<Course> GetCourseId(int id);

        bool Add(Course course);
        bool Update(Course course);
        bool Delete(Course course);
        bool Save();
    }
}