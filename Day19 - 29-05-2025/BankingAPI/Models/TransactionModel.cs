using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankingAPI.Models
{
    public class TransactionModel
    {
        [Key]
        public int TransactionId { get; set; }
        public int AccountNumber { get; set; }
        public double Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? TransactionType { get; set; } // e.g., Deposit, Withdrawal
        public double balance { get; set; } // Balance after the transaction

        [ForeignKey("AccountNumber")]
        public AccountModel? Account { get; set; }


    }
}