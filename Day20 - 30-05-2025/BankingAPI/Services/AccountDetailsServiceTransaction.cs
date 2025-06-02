using System.Threading.Tasks;
using BankingAPI.Contexts;
using BankingAPI.Interfaces;
using BankingAPI.Models;
using BankingAPI.Models.DTOs.Banking;
using Microsoft.EntityFrameworkCore;

namespace BankingAPI.Services
{
    public class AccountDetailsServiceTransaction : IAccountServices
    {
        private readonly BankingContext _bankingContext;

        public AccountDetailsServiceTransaction(BankingContext bankingContext)
        {
            _bankingContext = bankingContext;
        }

        public async Task<AccountModel> GetAccountByNumberAsync(int accountNumber)
        {
            using var transaction = await _bankingContext.Database.BeginTransactionAsync();
            try
            {
                var account = await _bankingContext.Accounts
                    .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

                if (account == null)
                    throw new KeyNotFoundException("Account not found");

                await transaction.CommitAsync();
                return account;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public Task<AccountModel> AddNewAccountAsync(AddNewAccountDto dto)
        {
            throw new System.NotImplementedException();
        }

        public Task<ICollection<AccountModel>> GetAccountsByCustomerIdAsync(int customerId)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> TransferAmountAsync(TransferAmountDto transferAmountDto)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> DepositAsync(int accountNumber, double amount)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> WithdrawAsync(int accountNumber, double amount)
        {
            throw new System.NotImplementedException();
        }
    }
}
