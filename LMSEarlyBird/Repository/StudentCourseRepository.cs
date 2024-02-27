using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMSEarlyBird.Data;
using LMSEarlyBird.Interfaces;
using LMSEarlyBird.Models;
using Microsoft.EntityFrameworkCore;

namespace LMSEarlyBird.Repository
{
    public class StudentCourseRepository : IStudentCourseRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentCourseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Add(StudentCourse course)
        {
            _context.Add(course);
            return Save();
        }

        public bool Delete(StudentCourse course)
        {
            _context.Remove(course);
            return Save();
        }

        public async Task<IEnumerable<StudentCourse>> GetAllStudentCourses()
        {
            return await _context.StudentCourses.ToListAsync();
        }

        
        public async Task<List<AppUser>> GetStudentsByCourse(int courseId)
        {
            var studentIds = await _context.StudentCourses
                .Where(sc => sc.CourseId == courseId)
                .Select(sc => sc.UserId)
                .ToListAsync();

            return await _context.AppUsers
                .Where(au => studentIds.Contains(au.Id))
                .ToListAsync();
        }
        
        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}