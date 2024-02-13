using LMSEarlyBird.Models;

namespace LMSEarlyBird.ViewModels
{
    public class DashboardViewModel
    {
        public List<string>? DepartmentNames { get; set; }
        public List<Room>? RoomList { get; set; }
        public List<Department>? DepartmentList { get; set; }
        public List<Building>? BuildingList { get; set; } = new List<Building>();
    }
}
