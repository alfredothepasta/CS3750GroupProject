using LMSEarlyBird.Data;
using LMSEarlyBird.Interfaces;
using LMSEarlyBird.Models;
using Microsoft.EntityFrameworkCore;

namespace LMSEarlyBird.Repository
{
    public class RoomRepository : IRoomRepository
    {
        private readonly ApplicationDbContext _context;

        public RoomRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Room> GetRoomById(int roomId)
        {
            return await _context.Rooms.FindAsync(roomId);
        }

        public async Task<List<Room>> GetRoomByBuilding(int buildingId)
        {
            return await _context.Rooms.Where(r => r.BuildingID == buildingId).ToListAsync();
        }

        public async Task<List<Room>> GetRooms()
        {
            return await _context.Rooms.ToListAsync();
        }
    }
}
