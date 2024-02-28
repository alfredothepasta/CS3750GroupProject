using LMSEarlyBird.Models;

namespace LMSEarlyBird.Interfaces
{
    public interface IBalanceRepository
    {
        bool addUserBalance(BalanceHistory balanceHistory);

        Task<List<BalanceHistory>> GetBalanceHistory(string userId);

        Task<decimal> GetCurrentBalance(string userId);

        bool Save();

        Task<bool> UpdateBalanceAddCourse(string userId, int courseHours, string courseName);

        Task<bool> UpdateBalanceDropCourse(string userId, int courseHours, string courseName);

        Task<bool> UpdateBalancePayment(string userId, decimal payment);
    }
}
