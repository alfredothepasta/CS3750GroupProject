using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMSEarlyBird.Models;

namespace LMSEarlyBird.Interfaces
{
    public interface IStudentCourseRepository
    {
        bool Add(StudentCourse course);
        bool Delete(StudentCourse course);
        bool Save();

        Task<IEnumerable<StudentCourse>> GetAllStudentCourses();
        Task<List<AppUser>> GetStudentsByCourse(int courseId);
    }
}