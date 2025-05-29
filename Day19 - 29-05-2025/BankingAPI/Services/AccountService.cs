using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BankingAPI.Interfaces;
using BankingAPI.Models;
using BankingAPI.Models.DTOs.Banking;

namespace BankingAPI.Services
{
    public class AccountService : IAccountServices
    {
        private readonly IRepository<int, AccountModel> _accountRepository;
        private readonly IRepository<int, CustomerModel> _customerRepository;

        public AccountService(
            IRepository<int, AccountModel> accountRepository,
            IRepository<int, CustomerModel> customerRepository)
        {
            _accountRepository = accountRepository;
            _customerRepository = customerRepository;
        }


        public async Task<AccountModel> AddNewAccountAsync(AddNewAccountDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.CustomerId <= 0)
                throw new ArgumentException("Customer ID must be greater than zero.", nameof(dto.CustomerId));

            var customer = await _customerRepository.GetByIdAsync(dto.CustomerId);
            if (customer == null)
                throw new Exception("Customer not found.");

            var newAccount = new AccountModel
            {
                AccountNumber = dto.accountno,
                Customerid = dto.CustomerId,
                Accountholdername = dto.AccountHolderName,
                Accounttype = dto.AccountType,
                Balance = dto.InitialBalance,
                Status = dto.Status,
                Customer = null
            };

            return await _accountRepository.AddAsync(newAccount);
        }
        public async Task<AccountModel> GetAccountByNumberAsync(int accountNumber)
        {
            return await _accountRepository.GetByIdAsync(accountNumber);
        }

        public async Task<ICollection<AccountModel>> GetAccountsByCustomerIdAsync(int customerId)
        {
            var allAccounts = await _accountRepository.GetAllAsync();
            var customerAccounts = new List<AccountModel>();

            foreach (var acc in allAccounts)
            {
                if (acc.Customerid == customerId)
                    customerAccounts.Add(acc);
            }

            return customerAccounts;
        }

        public async Task<bool> TransferAmountAsync(TransferAmountDto transferAmountDto)
        {
            var fromAccount = await _accountRepository.GetByIdAsync(transferAmountDto.FromAccountNumber);
            var toAccount = await _accountRepository.GetByIdAsync(transferAmountDto.ToAccountNumber);

            if (fromAccount == null || toAccount == null)
                return false;

            if (fromAccount.Balance < transferAmountDto.Amount)
                return false; // insufficient funds

            fromAccount.Balance -= transferAmountDto.Amount;
            toAccount.Balance += transferAmountDto.Amount;

            await _accountRepository.UpdateAsync(fromAccount);
            await _accountRepository.UpdateAsync(toAccount);

            return true;
        }

        public async Task<bool> DepositAsync(int accountNumber, double amount)
        {
            if (amount <= 0)
                return false;

            var account = await _accountRepository.GetByIdAsync(accountNumber);
            if (account == null) return false;

            account.Balance += amount;
            await _accountRepository.UpdateAsync(account);

            return true;
        }

        public async Task<bool> WithdrawAsync(int accountNumber, double amount)
        {
            if (amount <= 0)
                return false;

            var account = await _accountRepository.GetByIdAsync(accountNumber);
            if (account == null) return false;

            if (account.Balance < amount)
                return false; // insufficient funds

            account.Balance -= amount;
            await _accountRepository.UpdateAsync(account);

            return true;
        }
    }
}
