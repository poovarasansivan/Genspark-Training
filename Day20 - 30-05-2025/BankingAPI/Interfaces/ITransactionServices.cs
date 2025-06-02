using Microsoft.EntityFrameworkCore;
using BankingAPI.Models;

namespace BankingAPI.Interfaces
{
    public interface ITransactionServices
    {
        Task<ICollection<TransactionModel>> GetTransactionsByAccountNumberAsync(int accountNumber);
        Task<TransactionModel> GetTransactionByIdAsync(int transactionId);
        Task<bool> AddTransactionAsync(TransactionModel transaction);
    }
}