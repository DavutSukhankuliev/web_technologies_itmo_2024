using Microsoft.AspNetCore.Mvc;
using web_technologies_itmo_2024.Models;

namespace web_technologies_itmo_2024.Services;

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

	public async Task<IActionResult> SendMessageAsync(TelegramSendMessageModel messageModel)
	{
		var chatId = messageModel.ChatId;
		var text = $"{messageModel.Text}";

		if (!string.IsNullOrEmpty(messageModel.Author)) 
			text += $"\r\n\r\nThis message was sent for @{messageModel.Author}";

		try
		{
			_logger.LogInformation($"{_logTag} Attempting to send message to Telegram.");

			HttpResponseMessage response = chatId == 0
				? await _telegramSenderService.SendMessageAsync(text)
				: await _telegramSenderService.SendMessageAsync(text, chatId);

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
			return new StatusCodeResult(503);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"{_logTag} Unknown error while sending message to Telegram: {ex.Message}");
			return new StatusCodeResult(500);
		}
	}

	public async Task<IActionResult> SendPhotoAsync(TelegramSendPhotoModel photoModel)
	{
		var chatId = photoModel.ChatId;
		var photo = photoModel.PhotoBytes;
		var caption = $"{photoModel.Caption}";

		if (!string.IsNullOrEmpty(photoModel.Author)) 
			caption += $"\r\n\r\nThis message was sent for @{photoModel.Author}";

		try
		{
			_logger.LogInformation($"{_logTag} Attempting to send message to Telegram.");
			var response = await _telegramSenderService.SendPhotoAsync(photo, chatId, caption);

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
			return new StatusCodeResult(503);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"{_logTag} Unknown error while sending message to Telegram: {ex.Message}");
			return new StatusCodeResult(500);
		}
	}
}