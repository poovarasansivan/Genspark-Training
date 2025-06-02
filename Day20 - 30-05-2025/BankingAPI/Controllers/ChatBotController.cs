using System;
using System.Threading.Tasks;
using BankingAPI.Interfaces;
using BankingAPI.Models.DTOs.Banking;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BankingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatBotController : ControllerBase
    {
        private readonly IChatBot _chatBotService;
        private readonly ILogger<ChatBotController> _logger;

        public ChatBotController(IChatBot chatBotService, ILogger<ChatBotController> logger)
        {
            _chatBotService = chatBotService;
            _logger = logger;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> AskQuestionAsync([FromBody] QuestionRequestDto questionDto)
        {
            if (string.IsNullOrWhiteSpace(questionDto.Question))
                return BadRequest("Question cannot be null or empty.");

            try
            {
                var response = await _chatBotService.AskQuestions(questionDto.Question);
                return Ok(new ChatResponseDto { Answer = response });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing question: {Question}", questionDto.Question);
                return StatusCode(500, "An error occurred while processing your question.");
            }
        }
    }
}
