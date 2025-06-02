using BankingAPI.Models;
using BankingAPI.Models.DTOs.Banking;

namespace BankingAPI.Mappers
{
    public static class AccountMapper
    {
        // Map from AccountModel to AddNewAccountDto
        public static AddNewAccountDto ToDto(AccountModel model)
        {
            return new AddNewAccountDto
            {
                accountno = model.AccountNumber,
                CustomerId = model.Customerid,
                AccountHolderName = model.Accountholdername,
                AccountType = model.Accounttype,
                InitialBalance = model.Balance,
                Status = model.Status
            };
        }

        // Map from AddNewAccountDto to AccountModel
        public static AccountModel ToModel(AddNewAccountDto dto)
        {
            return new AccountModel
            {
                AccountNumber = dto.accountno,
                Customerid = dto.CustomerId,
                Accountholdername = dto.AccountHolderName,
                Accounttype = dto.AccountType,
                Balance = dto.InitialBalance,
                Status = dto.Status
            };
        }
    }
}
