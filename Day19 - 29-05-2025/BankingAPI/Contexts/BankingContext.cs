using Microsoft.EntityFrameworkCore;
using BankingAPI.Models;
using BankingAPI.Models.DTOs.Banking;


namespace BankingAPI.Contexts
{
    public class BankingContext : DbContext
    {
        public BankingContext(DbContextOptions<BankingContext> options) : base(options)
        {
        }

        public async Task<List<AccountResponseDto>> GetAccountsByCustomerId(int accountNo)
        {
            return await this.Set<AccountResponseDto>()
                        .FromSqlInterpolated($"SELECT * FROM get_account_summary({accountNo})")
                        .ToListAsync();
        }
    
        public DbSet<CustomerModel> Customers { get; set; }
        public DbSet<AccountModel> Accounts { get; set; }
        public DbSet<TransactionModel> Transactions { get; set; }
    }
}