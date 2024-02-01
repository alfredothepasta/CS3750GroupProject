﻿using LMSEarlyBird.Interfaces;
using LMSEarlyBird.Models;
using LMSEarlyBird.Data;
using Microsoft.EntityFrameworkCore;

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
            _context.Add(address); 
            return Save();
        }

        public async Task<IEnumerable<Address>> getUserAddress(string userID)
        {
            return await _context.Addresses.Where(x => x.UserID == userID).ToListAsync();
        }

        public bool removeUserAddress(Address address)
        {
            throw new NotImplementedException();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;
        }

        public bool updateUserAddress(Address address)
        {
            throw new NotImplementedException();
        }
    }
}
