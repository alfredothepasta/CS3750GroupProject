using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMSEarlyBird.Interfaces;
using LMSEarlyBird.Models;
using Microsoft.AspNetCore.Identity;

namespace LMSEarlyBird.Repository
{
    public class UserIdentityService : IUserIdentityService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserIdentityService(UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUserId()
        {
            var user = _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User).Result;
            return user?.Id;
        }
    }
}