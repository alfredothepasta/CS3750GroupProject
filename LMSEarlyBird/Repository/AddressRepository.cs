using LMSEarlyBird.Interfaces;
using LMSEarlyBird.Models;
using LMSEarlyBird.Data;

namespace LMSEarlyBird.Repository
{
    public class AddressRepository : IAddressRepository
    {
        private readonly ApplicationDbContext _context; 

        public AddressRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool addUserAddress(Address address)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Address>> getUserAddress(string userID)
        {
            // return await _context.Addresses.Where;
            throw new NotImplementedException();
        }

        public bool removeUserAddress(Address address)
        {
            throw new NotImplementedException();
        }

        public bool Save()
        {
            throw new NotImplementedException();
        }

        public bool updateUserAddress(Address address)
        {
            throw new NotImplementedException();
        }
    }
}
