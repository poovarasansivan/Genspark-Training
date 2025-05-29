using Microsoft.EntityFrameworkCore;
using BankingAPI.Interfaces;
using BankingAPI.Contexts;
using BankingAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankingAPI.Repositories
{
    public class AccountRepository : Repository<int, AccountModel>
    {
        public AccountRepository(BankingContext context) : base(context)
        {
        }
        public override async Task<AccountModel> GetByIdAsync(int id)
        {
            var account = await _context.Accounts
                .Include(a => a.Customer)
                .SingleOrDefaultAsync(a => a.AccountNumber == id);
            return account ?? throw new Exception("No account with the given ID");
        }
        public override async Task<ICollection<AccountModel>> GetAllAsync()
        {
            var accounts = _context.Accounts.Include(a => a.Customer);
            if (await accounts.CountAsync() == 0)
                throw new Exception("No accounts in the database");
            return await accounts.ToListAsync();
        }
    }
}