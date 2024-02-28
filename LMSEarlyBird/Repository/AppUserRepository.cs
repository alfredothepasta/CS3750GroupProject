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
    public class AppUserRepository : IAppUserRepository
    {
        private readonly ApplicationDbContext _context;

        public AppUserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AppUser> GetUser(string id)
        {
            return await _context.Users.Include(x => x.StudentCourses).Include(x=> x.InstructorCourses).FirstAsync(x => x.Id == id);
        }

        public bool UpdateUser(AppUser user)
        {
            _context.Update(user);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;
        }
    }
}