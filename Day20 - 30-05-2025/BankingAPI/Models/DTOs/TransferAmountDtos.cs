namespace BankingAPI.Models.DTOs.Banking
{
    public class TransferAmountDto
    {
        public int FromAccountNumber { get; set; }
        public int ToAccountNumber { get; set; }
        public double Amount { get; set; }
    }
}