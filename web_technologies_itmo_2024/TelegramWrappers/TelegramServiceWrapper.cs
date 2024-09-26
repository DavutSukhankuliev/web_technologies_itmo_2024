using Microsoft.AspNetCore.Mvc;

namespace web_technologies_itmo_2024.TelegramWrappers;

public class TelegramServiceWrapper
{
	private const string _logTag = $"[{nameof(TelegramServiceWrapper)}]";

	private readonly TelegramSenderService _telegramSenderService;
	private readonly ILogger<TelegramServiceWrapper> _logger;

	public TelegramServiceWrapper(TelegramSenderService telegramSenderService, ILogger<TelegramServiceWrapper> logger)
	{
		_telegramSenderService = telegramSenderService;
		_logger = logger;
	}

	public async Task<IActionResult> SendMessageAsync(string textToSend)
	{
		try
		{
			_logger.LogInformation($"{_logTag} Attempting to send message to Telegram.");
			var response = await _telegramSenderService.SendMessageAsync(textToSend);

			if (response.IsSuccessStatusCode)
			{
				var apiResult = await response.Content.ReadAsStringAsync();
				_logger.LogInformation($"{_logTag} Message successfully sent to Telegram.");
				return new OkObjectResult(apiResult);
			}
			else
			{
				var errorMessage = await response.Content.ReadAsStringAsync();
				_logger.LogWarning($"{_logTag} Error calling Telegram API: {response.StatusCode} - {errorMessage}");
				return new StatusCodeResult((int)response.StatusCode);
			}
		}
		catch (HttpRequestException ex)
		{
			_logger.LogError(ex, $"{_logTag} HttpRequestException while sending message to Telegram: {ex.Message}");
			return new StatusCodeResult(503); // Service Unavailable
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"{_logTag} Unknown error while sending message to Telegram: {ex.Message}");
			return new StatusCodeResult(500); // Internal Server Error
		}
	}
}