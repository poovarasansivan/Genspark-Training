using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BankingAPI.Interfaces;
using BankingAPI.Models;
using BankingAPI.Models.DTOs.Banking;

namespace BankingAPI.Services
{
    public class TransactionService : ITransactionServices
    {
        private readonly IRepository<int, TransactionModel> _transactionRepository;
        private readonly IRepository<int, AccountModel> _accountRepository;

        public TransactionService(
            IRepository<int, TransactionModel> transactionRepository,
            IRepository<int, AccountModel> accountRepository)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
        }

        public async Task<bool> AddTransactionAsync(TransactionModel transaction)
        {
            if (transaction == null)
                return false;

            var account = await _accountRepository.GetByIdAsync(transaction.AccountNumber);
            if (account == null)
                return false;

            if (transaction.TransactionType == "Deposit")
                account.Balance += transaction.Amount;
            else if (transaction.TransactionType == "Withdrawal")
            {
                if (account.Balance < transaction.Amount)
                    return false;

                account.Balance -= transaction.Amount;
            }

            transaction.balance = account.Balance;
            transaction.TransactionDate = DateTime.UtcNow;

            await _accountRepository.UpdateAsync(account);
            await _transactionRepository.AddAsync(transaction);

            return true;
        }

        public async Task<TransactionModel> GetTransactionByIdAsync(int transactionId)
        {
            return await _transactionRepository.GetByIdAsync(transactionId);
        }

        public async Task<ICollection<TransactionModel>> GetTransactionsByAccountNumberAsync(int accountNumber)
        {
            var allTransactions = await _transactionRepository.GetAllAsync();
            var result = new List<TransactionModel>();

            foreach (var t in allTransactions)
            {
                if (t.AccountNumber == accountNumber)
                    result.Add(t);
            }

            return result;
        }
    }
}
