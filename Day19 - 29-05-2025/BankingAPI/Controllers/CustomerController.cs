using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using BankingAPI.Models.DTOs.Banking;
using BankingAPI.Interfaces;

namespace BankingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerServices _customerService;

        public CustomerController(ICustomerServices customerService)
        {
            _customerService = customerService;
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer([FromBody] AddNewCustomerDto newCustomer)
        {
            if (newCustomer == null)
                return BadRequest("Customer data is required.");

            try
            {
                var result = await _customerService.AddCustomerAsync(newCustomer);
                return result ? Ok("Customer added successfully.") : StatusCode(500, "Failed to add customer.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomer(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid customer ID.");

            try
            {
                var customer = await _customerService.GetCustomerById(id);
                return customer != null ? Ok(customer) : NotFound("Customer not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] AddNewCustomerDto updatedCustomer)
        {
            if (updatedCustomer == null || id != updatedCustomer.CustomerId)
                return BadRequest("Customer data is invalid or ID mismatch.");

            try
            {
                var result = await _customerService.UpdateCustomerAsync(updatedCustomer);
                return result ? Ok("Customer updated successfully.") : StatusCode(500, "Failed to update customer.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid customer ID.");

            try
            {
                var result = await _customerService.DeleteCustomerAsync(id);
                return result ? Ok("Customer deleted successfully.") : StatusCode(500, "Failed to delete customer.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}
