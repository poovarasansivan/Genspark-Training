using Microsoft.EntityFrameworkCore;
using BankingAPI.Interfaces;
using BankingAPI.Contexts;
using BankingAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankingAPI.Repositories
{
    public class CustomerRepository : Repository<int, CustomerModel>
    {
        public CustomerRepository(BankingContext context) : base(context)
        {
        }

        public override async Task<CustomerModel> GetByIdAsync(int id)
        {
            var customer = await _context.Customers.SingleOrDefaultAsync(c => c.CustomerId == id);
            return customer ?? throw new Exception("No customer with the given ID");
        }

        public override async Task<ICollection<CustomerModel>> GetAllAsync()
        {
            var customers = _context.Customers;
            if (await customers.CountAsync() == 0)
                throw new Exception("No customers in the database");
            return await customers.ToListAsync();
        }
    }
}
