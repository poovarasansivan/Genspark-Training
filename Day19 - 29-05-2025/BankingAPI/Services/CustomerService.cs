using System;
using System.Threading.Tasks;
using BankingAPI.Interfaces;
using BankingAPI.Models;
using BankingAPI.Models.DTOs.Banking;

namespace BankingAPI.Services
{
    public class CustomerService : ICustomerServices
    {
        private readonly IRepository<int, CustomerModel> _customerRepository;

        public CustomerService(IRepository<int, CustomerModel> customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<bool> AddCustomerAsync(AddNewCustomerDto customerDto)
        {
            try
            {
                var customer = new CustomerModel
                {
                    CustomerName = customerDto.CustomerName,
                    Email = customerDto.Email,
                    PhoneNumber = customerDto.PhoneNumber,
                    Address = customerDto.Address,
                    DateOfBirth = customerDto.DateOfBirth,
                    Status = customerDto.Status
                };
                await _customerRepository.AddAsync(customer);
                return true;
            }
            catch (Exception e)
            {
                throw new Exception($"Error adding customer: {e.Message}");
            }
        }

        public async Task<AddNewCustomerDto?> GetCustomerById(int customerId)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(customerId);
                if (customer == null) return null;

                return new AddNewCustomerDto
                {
                    CustomerId = customer.CustomerId,
                    CustomerName = customer.CustomerName,
                    Email = customer.Email,
                    PhoneNumber = customer.PhoneNumber,
                    Address = customer.Address,
                    DateOfBirth = customer.DateOfBirth,
                    Status = customer.Status
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Error retrieving customer: {e.Message}");
            }
        }

        public async Task<bool> UpdateCustomerAsync(AddNewCustomerDto customerDto)
        {
            try
            {
                var existingCustomer = await _customerRepository.GetByIdAsync(customerDto.CustomerId);
                if (existingCustomer == null) return false;

                existingCustomer.CustomerName = customerDto.CustomerName;
                existingCustomer.Email = customerDto.Email;
                existingCustomer.PhoneNumber = customerDto.PhoneNumber;
                existingCustomer.Address = customerDto.Address;
                existingCustomer.DateOfBirth = customerDto.DateOfBirth;
                existingCustomer.Status = customerDto.Status;

                await _customerRepository.UpdateAsync(existingCustomer);
                return true;
            }
            catch (Exception e)
            {
                throw new Exception($"Error updating customer: {e.Message}");
            }
        }

        public async Task<bool> DeleteCustomerAsync(int customerId)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(customerId);
                if (customer == null) return false;

                // Use DeleteAsync with key as per IRepository interface
                await _customerRepository.DeleteAsync(customerId);
                return true;
            }
            catch (Exception e)
            {
                throw new Exception($"Error deleting customer: {e.Message}");
            }
        }

    }
}