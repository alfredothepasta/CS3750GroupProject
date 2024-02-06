using LMSEarlyBird.Models;

namespace LMSEarlyBird.Interfaces
{
    public interface IDepartmentRepository
    {
        public Task<List<Department>> GetAllDepartments();

        public Task<Department> GetDepartmentById(int id);
    }
}
