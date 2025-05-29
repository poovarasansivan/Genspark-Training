using BankingAPI.Models;
using BankingAPI.Models.DTOs.Banking;

// Model → DTO: To send only necessary data to the client.
// DTO → Model: To safely save client input to the database.


namespace BankingAPI.Mappers
{
    public static class CustomerMapper
    {
        // Convert CustomerModel to AddNewCustomerDto
        public static AddNewCustomerDto ToDto(CustomerModel model)
        {
            return new AddNewCustomerDto
            {
                CustomerId = model.CustomerId,
                CustomerName = model.CustomerName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                DateOfBirth = model.DateOfBirth,
                Status = model.Status
            };
        }

        // Convert AddNewCustomerDto to CustomerModel
        public static CustomerModel ToModel(AddNewCustomerDto dto)
        {
            return new CustomerModel
            {
                CustomerId = dto.CustomerId,
                CustomerName = dto.CustomerName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                DateOfBirth = dto.DateOfBirth,
                Status = dto.Status
            };
        }
    }
}
