namespace BankingAPI.Interfaces
{
    public interface IChatBot
    {
        Task<string> AskQuestions(string Message);
    }
}