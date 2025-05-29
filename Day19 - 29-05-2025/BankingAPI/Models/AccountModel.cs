using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankingAPI.Models
{
    public class AccountModel
    {
        [Key]
        public int AccountNumber { get; set; }
        public int Customerid { get; set; }
        public string? Accountholdername { get; set; }
        public string? Accounttype { get; set; }
        public double Balance { get; set; }
        public string? Status { get; set; }

        [ForeignKey("customerid")]
        public CustomerModel? Customer { get; set; }
    }
}