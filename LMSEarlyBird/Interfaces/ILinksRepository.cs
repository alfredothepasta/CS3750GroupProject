using LMSEarlyBird.Models;

namespace LMSEarlyBird.Interfaces
{
    public interface ILinksRepository
    {
        Task<AppUser> GetUser(string id);

        bool addUserLinks(UserLinks links);

        bool hasUserLinks(string userID);

        Task<UserLinks> getUserLinks(string userID);

        bool UpdateLinks(UserLinks links);

        bool Save();
    }
}
