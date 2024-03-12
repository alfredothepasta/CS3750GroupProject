using LMSEarlyBird.Data;
using LMSEarlyBird.Interfaces;
using LMSEarlyBird.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace LMSEarlyBird.Repository
{
    public class BalanceRepository : IBalanceRepository
    {
        private readonly ApplicationDbContext _context;
        // create our context for use
        public BalanceRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        // add balance if none created
        public bool addUserBalance(BalanceHistory balanceHistory)
        {
            _context.Add(balanceHistory);
            return Save();
        }
        // gather balance history
        public async Task<List<BalanceHistory>> GetBalanceHistory(string userId)
        {
            var user = await _context.AppUsers
            .Include(x => x.BalanceHistory)
            .FirstAsync(x => x.Id == userId);

            if (user.BalanceHistory == null)
            {
                return null;
            }

            return user.BalanceHistory;
        }
        // gather the current balance by using the most recent from the list of balancehistory based on the userid
        public async Task<decimal> GetCurrentBalance(string userId)
        {
            // build a list of the balance history
            var balanceList = await _context
                .PaymentHistory
                .Where(x => x.UserId == userId)
                .ToListAsync();

            // set a default balance of 0
            decimal balance = 0;

            // check that balance list has anything
            // if it doesn't , return 0
            if (balanceList.IsNullOrEmpty())
            {
                return balance;
            }

            // build a null balance history list for checking against to find most recent balance
            BalanceHistory lastBalance = null;

            foreach (var balances in balanceList)
            {
                if (lastBalance == null || balances.Time > lastBalance.Time)
                {
                    lastBalance = balances;
                    balance = balances.Balance;
                }
            }
            // return the most recent balance
            return balance;
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;
        }

        // add course to the balance listing
        public async Task<bool> UpdateBalanceAddCourse(string userId, int courseHours, string className)
        {

            // calculate the balance to add
            decimal balancetoAdd = courseHours * 200;

            // add the balance components
            BalanceHistory newBalance = new BalanceHistory();
            newBalance.UserId = userId;
            newBalance.AppUser = await _context.AppUsers.FirstAsync(x => x.Id == userId);
            newBalance.NetChange = balancetoAdd ;
            newBalance.Time = DateTime.Now;
            newBalance.Balance = await GetCurrentBalance(userId) + balancetoAdd;
            newBalance.ClassName = className;
            newBalance.Type = "AddClass";

            // add balance
            _context.Add(newBalance);
            return Save();
        }

        // remove course from the balance listing
        public async Task<bool> UpdateBalanceDropCourse(string userId, int courseHours, string className)
        {

            // calculate the balance to add
            decimal balanceToRemove = courseHours * -200;

            // add the balance components
            BalanceHistory newBalance = new BalanceHistory();
            newBalance.UserId = userId;
            newBalance.AppUser = await _context.AppUsers.FirstAsync(x => x.Id == userId);
            newBalance.NetChange = balanceToRemove;
            newBalance.Time = DateTime.Now;
            newBalance.Balance = await GetCurrentBalance(userId) + balanceToRemove;
            newBalance.ClassName = className;
            newBalance.Type = "DropClass";

            // add balance
            _context.Add(newBalance);
            return Save();
        }

        // add a payment to the balance listing
        public async Task<bool> UpdateBalancePayment(string userId, decimal payment, string recieptNumber)
        {
            // add the balance components
            BalanceHistory newBalance = new BalanceHistory();
            newBalance.UserId = userId;
            newBalance.AppUser = await _context.AppUsers.FirstAsync(x => x.Id == userId);
            newBalance.NetChange = payment * -1;
            newBalance.Time = DateTime.Now;
            newBalance.Balance = await GetCurrentBalance(userId) - payment;
            newBalance.Type = "Payment";
            newBalance.Reciept = recieptNumber;

            // add balance
            _context.Add(newBalance);
            return Save();
        }

        // gather reciepts
        public async Task<List<BalanceHistory>> GetAllReciepts()
        {
            return await _context.PaymentHistory.ToListAsync();
            //return await _context
            //    .PaymentHistory
            //    .Where(x => x.UserId == userId)
            //    .ToListAsync();
        }

        //public async Task<List<Course>> GetCoursesByTeacher(string teacherId)
        //{
        //    return await _context.Courses
        //        .Where(c => c.InstructorCourses
        //            .Where(i => i.UserId == teacherId)
        //            .Any())
        //        .ToListAsync();
        //}
    }
}
