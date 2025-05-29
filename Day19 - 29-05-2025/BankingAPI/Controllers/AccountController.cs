using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BankingAPI.Interfaces;
using BankingAPI.Models;
using BankingAPI.Models.DTOs.Banking;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BankingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountServices _accountService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            IAccountServices accountService,
            ILogger<AccountController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        [HttpPost]

        public async Task<IActionResult> AddNewAccountAsync([FromBody] AddNewAccountDto dto)
        {
            if (dto == null)
                return BadRequest("Account model cannot be null.");

            if (dto.CustomerId <= 0)
                return BadRequest("Customer ID must be greater than zero.");

            try
            {
                var account = await _accountService.AddNewAccountAsync(dto);
                return Ok(account);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new account {@AccountModel}", dto);
                return StatusCode(500, "An error occurred while creating the account.");
            }
        }

        [HttpGet("{accountNumber:int}")]
        public async Task<ActionResult<AccountModel>> GetAccountByNumberAsync(int accountNumber)
        {
            if (accountNumber <= 0)
                return BadRequest("Account number must be greater than zero.");

            try
            {
                var account = await _accountService.GetAccountByNumberAsync(accountNumber);
                if (account == null)
                    return NotFound($"No account found with number {accountNumber}.");

                return Ok(account);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching account {AccountNumber}", accountNumber);
                return StatusCode(500, "An error occurred while retrieving the account.");
            }
        }

        [HttpGet("customer/{customerId:int}")]
        public async Task<ActionResult<ICollection<AccountModel>>> GetAccountsByCustomerIdAsync(int customerId)
        {
            if (customerId <= 0)
                return BadRequest("Customer ID must be greater than zero.");

            try
            {
                var accounts = await _accountService.GetAccountsByCustomerIdAsync(customerId);
                return Ok(accounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching accounts for customer {CustomerId}", customerId);
                return StatusCode(500, "An error occurred while retrieving accounts.");
            }
        }

        [HttpPost("transfer")]
        public async Task<ActionResult> TransferAmountAsync([FromBody] TransferAmountDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.Amount <= 0)
                return BadRequest("Transfer amount must be positive.");

            try
            {
                var success = await _accountService.TransferAmountAsync(dto);
                if (!success)
                    return BadRequest("Transfer failed: check account numbers and balance.");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error transferring amount {@Dto}", dto);
                return StatusCode(500, "An error occurred while processing the transfer.");
            }
        }


        [HttpPost("{accountNumber:int}/deposit")]
        public async Task<ActionResult> DepositAsync(int accountNumber, [FromBody] AmountDto amt)
        {
            if (accountNumber <= 0)
                return BadRequest("Account number must be greater than zero.");
            if (amt.Amount <= 0)
                return BadRequest("Deposit amount must be positive.");

            try
            {
                var success = await _accountService.DepositAsync(accountNumber, amt.Amount);
                if (!success)
                    return BadRequest("Deposit failed: check account number.");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error depositing to account {AccountNumber}", accountNumber);
                return StatusCode(500, "An error occurred while depositing amount.");
            }
        }


        [HttpPost("{accountNumber:int}/withdraw")]
        public async Task<ActionResult> WithdrawAsync(int accountNumber, [FromBody] AmountDto amt)
        {
            if (accountNumber <= 0)
                return BadRequest("Account number must be greater than zero.");
            if (amt.Amount <= 0)
                return BadRequest("Withdrawal amount must be positive.");

            try
            {
                var success = await _accountService.WithdrawAsync(accountNumber, amt.Amount);
                if (!success)
                    return BadRequest("Withdrawal failed: insufficient funds or invalid account.");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error withdrawing from account {AccountNumber}", accountNumber);
                return StatusCode(500, "An error occurred while withdrawing amount.");
            }
        }
    }


    public class AmountDto
    {
        public double Amount { get; set; }
    }
}
