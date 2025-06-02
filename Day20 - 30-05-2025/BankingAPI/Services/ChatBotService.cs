using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BankingAPI.Interfaces;
using BankingAPI.Models.DTOs.Banking;

namespace BankingAPI.Services
{
    public class ChatBotService : IChatBot
    {
        private readonly HttpClient _httpClient;

        public ChatBotService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> AskQuestions(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return "Question cannot be empty.";

            var requestDto = new QuestionRequestDto
            {
                Question = message
            };

            var flaskApiUrl = "http://localhost:5000/chatbot";

            try
            {
    
                var response = await _httpClient.PostAsJsonAsync(flaskApiUrl, requestDto);

                if (!response.IsSuccessStatusCode)
                {
                    return "Sorry, chatbot service is not available right now.";
                }

                var chatResponse = await response.Content.ReadFromJsonAsync<ChatResponseDto>();

                return chatResponse?.Answer ?? "Sorry, no answer found.";
            }
            catch (HttpRequestException)
            {
                return "Error connecting to chatbot service.";
            }
        }
    }
}
