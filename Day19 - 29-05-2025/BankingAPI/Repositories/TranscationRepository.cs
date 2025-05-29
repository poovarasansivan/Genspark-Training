using Microsoft.EntityFrameworkCore;
using BankingAPI.Interfaces;
using BankingAPI.Contexts;
using BankingAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankingAPI.Repositories
{
    public class TransactionRepository : Repository<int, TransactionModel>
    {
        public TransactionRepository(BankingContext context) : base(context)
        {
        }

        public override async Task<TransactionModel> GetByIdAsync(int id)
        {
            var transaction = await _context.Transactions.SingleOrDefaultAsync(t => t.TransactionId == id);
            return transaction ?? throw new Exception("No transaction with the given ID");
        }
        public override async Task<ICollection<TransactionModel>> GetAllAsync()
        {
            var transactions = _context.Transactions;
            if (await transactions.CountAsync() == 0)
                throw new Exception("No transactions in the database");
            return await transactions.ToListAsync();
        }
    }
}
