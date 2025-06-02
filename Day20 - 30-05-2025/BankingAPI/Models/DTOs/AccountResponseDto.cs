namespace BankingAPI.Models.DTOs.Banking
{
    public class AccountResponseDto
    {
        public int AccountNo { get; set; }
        public string? AccountType { get; set; }
        public double Balance { get; set; }
    }
}