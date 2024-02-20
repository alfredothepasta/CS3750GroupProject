using LMSEarlyBird.Data;
using LMSEarlyBird.Interfaces;
using LMSEarlyBird.Models;
using Microsoft.EntityFrameworkCore;

namespace LMSEarlyBird.Repository
{
    public class LinksRepository : ILinksRepository
    {
        private readonly ApplicationDbContext _context;

        public LinksRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool addUserLinks(UserLinks links)
        {
            _context.Add(links);
            return Save();
        }

        // check if the user has a Links built
        public bool hasUserLinks(string userID)
        {
            return _context.UserLinks.Where(a => a.UserId == userID).Any();
        }

        public async Task<UserLinks> getUserLinks(string userID)
        {
            var returnLinks = await _context.UserLinks.FirstOrDefaultAsync(x => x.UserId == userID);
            if (returnLinks == null)
            {
                throw new Exception("No links found for user");
            }
            return returnLinks;
        }

        public async Task<AppUser> GetUser(string id)
        {
            return await _context.Users.Include(x => x.StudentCourses).FirstAsync(x => x.Id == id);
        }

        public bool UpdateLinks(UserLinks links)
        {
            _context.Update(links);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;
        }
    }
}
