using LMSEarlyBird.Data;
using LMSEarlyBird.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LMSEarlyBird
{
    public static class ClaimsPrincipalExtensions
    {
        private static readonly ApplicationDbContext _context;

        //public ClaimsPrincipalExtensions(ApplicationDbContext context)
        //{
        //    _context = context;
        //}

        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        public static async Task<List<StudentAssignment>> GetStudentAssignments(string studentId)
        {
            var user = await _context.AppUsers
            .Include(x => x.StudentAssignment)
            .ThenInclude(sa => sa.Assignment)
            .FirstAsync(x => x.Id == studentId);

            return user.StudentAssignment;
        }
    }
}