using LMSEarlyBird.Models;

namespace LMSEarlyBird.Interfaces
{
    public interface IRoomRepository
    {
        public Task<List<Room>> GetRooms();
        public Task<List<Room>> GetRoomByBuilding(int buildingId);

        public Task<Room> GetRoomById(int roomId);
    }
}
