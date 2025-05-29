using BankingAPI.Models.DTOs.Banking;

namespace BankingAPI.Interfaces
{
    public interface ICustomerServices
    {
        Task<bool> AddCustomerAsync(AddNewCustomerDto customerDto);
        Task<AddNewCustomerDto?> GetCustomerById(int customerId);
        Task<bool> UpdateCustomerAsync(AddNewCustomerDto customerDto);
        Task<bool> DeleteCustomerAsync(int customerId);
    }
}
