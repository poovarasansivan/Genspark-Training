using Microsoft.AspNetCore.Mvc;
using BankingAPI.Interfaces;
using BankingAPI.Models;
using BankingAPI.Models.DTOs.Banking;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionServices _transactionServices;

        public TransactionController(ITransactionServices transactionServices)
        {
            _transactionServices = transactionServices;
        }

        [HttpGet("account/{accountNumber}")]
        public async Task<ActionResult<ICollection<TransactionModel>>> GetTransactionsByAccountNumber(int accountNumber)
        {
            var transactions = await _transactionServices.GetTransactionsByAccountNumberAsync(accountNumber);
            return Ok(transactions);
        }

        [HttpGet("{transactionId}")]
        public async Task<ActionResult<TransactionModel>> GetTransactionById(int transactionId)
        {
            var transaction = await _transactionServices.GetTransactionByIdAsync(transactionId);
            if (transaction == null) return NotFound();
            return Ok(transaction);
        }

        [HttpPost]
        public async Task<ActionResult> AddTransaction([FromBody] AddNewTransactionDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var transaction = new TransactionModel
            {
                TransactionId = dto.TransactionId,
                AccountNumber = dto.AccountNo,
                Amount = dto.Amount,
                TransactionType = dto.TransactionType,
                TransactionDate = dto.TransactionDate,
            };

            var result = await _transactionServices.AddTransactionAsync(transaction);
            if (!result) return BadRequest("Transaction failed.");

            return Ok("Transaction completed successfully.");
        }
    }
}
