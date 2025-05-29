namespace BankingAPI.Models.DTOs.Banking
{
    public class AddNewTransactionDto
    {
        public int TransactionId { get; set; } 
        public int AccountNo { get; set; } 
        public double Amount { get; set; } 
        public string? TransactionType { get; set; } 
        public DateTime TransactionDate { get; set; } 

    }
}