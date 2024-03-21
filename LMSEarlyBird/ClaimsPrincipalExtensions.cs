using LMSEarlyBird.Data;
using LMSEarlyBird.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LMSEarlyBird
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.NameIdentifier).Value;
        }
    }
}