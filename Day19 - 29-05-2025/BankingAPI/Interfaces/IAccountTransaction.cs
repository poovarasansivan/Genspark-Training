using Microsoft.EntityFrameworkCore;
using BankingAPI.Models;
using BankingAPI.Models.DTOs.Banking;

namespace BankingAPI.Interfaces
{
    public interface IAccountTransaction
    {
        Task<AccountResponseDto> GetAccountSummary(int accountNumber);
       
    }
}