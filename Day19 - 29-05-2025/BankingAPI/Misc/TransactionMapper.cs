using BankingAPI.Models;
using BankingAPI.Models.DTOs.Banking;

namespace BankingAPI.Mappers
{
    public static class TransactionMapper
    {
        public static AddNewTransactionDto ModelToDTo(TransactionModel model)
        {
            return new AddNewTransactionDto
            {
                TransactionId = model.TransactionId,
                AccountNo = model.AccountNumber,
                Amount = model.Amount,
                TransactionType = model.TransactionType,
                TransactionDate = model.TransactionDate,
            };
        }

        public static TransactionModel DtoToModel(AddNewTransactionDto dto)
        {
            return new TransactionModel
            {
                TransactionId = dto.TransactionId,
                AccountNumber = dto.AccountNo,
                Amount = dto.Amount,
                TransactionType = dto.TransactionType,
                TransactionDate = dto.TransactionDate,
            };
        }
    }
}
