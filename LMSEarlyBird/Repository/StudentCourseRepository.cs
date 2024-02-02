using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMSEarlyBird.Data;
using LMSEarlyBird.Interfaces;
using LMSEarlyBird.Models;

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

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}