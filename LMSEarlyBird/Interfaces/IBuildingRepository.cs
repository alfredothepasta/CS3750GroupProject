using LMSEarlyBird.Models;

namespace LMSEarlyBird.Interfaces
{
    public interface IBuildingRepository
    {
        public Task<List<Building>> GetBuildings();

        public Task<Building> GetBuildingById(int id);
    }
}
