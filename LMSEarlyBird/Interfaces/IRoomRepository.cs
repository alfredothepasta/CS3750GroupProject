using LMSEarlyBird.Models;

namespace LMSEarlyBird.Interfaces
{
    public interface IRoomRepository
    {
        public List<Room> GetRooms();
        public List<Room> GetRoomByBuilding(int buildingId);

        public Room GetRoom(int roomId);
    }
}
