using Microsoft.EntityFrameworkCore;
using BankingAPI.Models;
using BankingAPI.Models.DTOs.Banking;

namespace BankingAPI.Interfaces
{
    public interface IAccountServices
    {
        public Task<AccountModel> AddNewAccountAsync(AddNewAccountDto dto);
        public Task<AccountModel> GetAccountByNumberAsync(int accountNumber);
        public Task<ICollection<AccountModel>> GetAccountsByCustomerIdAsync(int customerId);
        public Task<bool> TransferAmountAsync(TransferAmountDto transferAmountDto);
        public Task<bool> DepositAsync(int accountNumber, double amount);
        public Task<bool> WithdrawAsync(int accountNumber, double amount);
    }
}