namespace BankingAPI.Models.DTOs.Banking
{
    public class AddNewAccountDto
    {
        public int accountno { get; set; } // Unique identifier for the account
        public int CustomerId { get; set; }
        public string? AccountHolderName { get; set; }
        public string? AccountType { get; set; } // e.g., Savings, Checking
        public double InitialBalance { get; set; } // Initial deposit amount
        public string? Status { get; set; } // e.g., Active, Inactive
    }
}