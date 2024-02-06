using LMSEarlyBird.Data;
using LMSEarlyBird.Interfaces;
using LMSEarlyBird.Models;
using Microsoft.EntityFrameworkCore;

namespace LMSEarlyBird.Repository
{
    public class BuildingRepository : IBuildingRepository
    {
        private readonly ApplicationDbContext _context;

        public BuildingRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Building>> GetBuildings()
        {
            return await _context.Buildings.ToListAsync();
        }

        public async Task<Building> GetBuildingById(int id)
        {
            return await _context.Buildings.FindAsync(id);
        }
    }
}
