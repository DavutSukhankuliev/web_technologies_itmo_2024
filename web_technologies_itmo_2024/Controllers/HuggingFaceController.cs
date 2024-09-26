using Microsoft.AspNetCore.Mvc;
using web_technologies_itmo_2024.Models;
using web_technologies_itmo_2024.Services;

namespace web_technologies_itmo_2024.Controllers;

[ApiController]
[Route("api/hugging-face")]
public class HuggingFaceController : ControllerBase
{
	private const string _logTag = $"[{nameof(HuggingFaceController)}]";

	private readonly HuggingFaceService _huggingFaceService;
	private readonly TelegramServiceWrapper _telegramServiceWrapper;
	private readonly ILogger<HuggingFaceController> _logger;

	public HuggingFaceController(
		HuggingFaceService huggingFaceService, 
		TelegramServiceWrapper telegramServiceWrapper, 
		ILogger<HuggingFaceController> logger
		)
	{
		_huggingFaceService = huggingFaceService;
		_telegramServiceWrapper = telegramServiceWrapper;
		_logger = logger;
	}

	[HttpPost]
	[Route("generate-text")]
	public async Task<IActionResult> GetQuestion([FromBody] PromptRequestModel request)
	{
		if (string.IsNullOrEmpty(request.Prompt))
		{
			_logger.LogWarning($"{_logTag} Received empty or null prompt.");
			return BadRequest("Parameter 'prompt' is required");
		}

		try
		{
			var apiResult = await _huggingFaceService.SendPromptAsync(request.Prompt);
			var answer = _huggingFaceService.ParseApiResult(apiResult);
			var message = PrepareMessageToSend(request.Prompt, answer);
			return await _telegramServiceWrapper.SendMessageAsync(message);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"{_logTag} Error occurred while processing the request.");
			return StatusCode(500, "Internal server error");
		}
	}

	private string PrepareMessageToSend(string prompt, string answer)
	{
		var message = $"Prompt: {prompt}\r\nAnswer: {answer}";
		_logger.LogDebug($"{_logTag} Prepared message to send: {message}");
		return message;
	}
}