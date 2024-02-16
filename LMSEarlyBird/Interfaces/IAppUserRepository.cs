using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMSEarlyBird.Models;

namespace LMSEarlyBird.Interfaces
{
    public interface IAppUserRepository
    {
        Task<AppUser> GetUser(string id);

        bool UpdateUser(AppUser user);

    }
}