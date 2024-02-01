using LMSEarlyBird.Models;

namespace LMSEarlyBird.Interfaces
{
    public interface IAddressRepository
    {
        Task<IEnumerable<Address>> getUserAddress();
        bool addUserAddress(Address address);
        bool updateUserAddress(Address address);
        bool removeUserAddress(Address address);
        bool Save();
    }
}
