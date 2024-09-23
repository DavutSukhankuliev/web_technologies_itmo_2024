using System.Text;
using Newtonsoft.Json;

namespace web_technologies_itmo_2024;

public class TelegramSenderService
{
	private readonly HttpClient _httpClient;
	private readonly ILogger<TelegramSenderService> _logger;
	private readonly string _sendPhotoApiUrl;
	private readonly string _sendMessageApiUrl;
	private readonly string _chatId;

	public TelegramSenderService(HttpClient httpClient, ILogger<TelegramSenderService> logger, string botKey, string chatId)
	{
		_httpClient = httpClient;
		_logger = logger;
		_sendPhotoApiUrl = $"https://api.telegram.org/bot{botKey}/sendPhoto";
		_sendMessageApiUrl = $"https://api.telegram.org/bot{botKey}/sendMessage";
		_chatId = chatId;
	}

	public async Task<HttpResponseMessage> SendPhotoAsync(string photoUrl, string caption = "")
	{
		var stringContent = new StringContent(JsonConvert.SerializeObject(new
		{
			chat_id = _chatId,
			photo = photoUrl,
			caption = caption
		}), Encoding.UTF8, "application/json");

		try
		{
			var response = await _httpClient.PostAsync(_sendPhotoApiUrl, stringContent);

			if (response.IsSuccessStatusCode)
			{
				_logger.LogInformation("Успешный ответ от Telegram API.");
			}
			else
			{
				_logger.LogError($"Ошибка при вызове Telegram API: {await response.Content.ReadAsStringAsync()}");
			}

			return response;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Ошибка при вызове Telegram API");
			throw;
		}
	}

	public async Task<HttpResponseMessage> SendMessageAsync(string message)
	{
		var stringContent = new StringContent(JsonConvert.SerializeObject(new
		{
			chat_id = _chatId,
			text = message,
		}), Encoding.UTF8, "application/json");

		try
		{
			_logger.LogInformation("Attempting to send message to Telegram API.");
			var response = await _httpClient.PostAsync(_sendMessageApiUrl, stringContent);

			if (response.IsSuccessStatusCode)
			{
				_logger.LogInformation("Successfully received response from Telegram API.");
			}
			else
			{
				var errorMessage = await response.Content.ReadAsStringAsync();
				_logger.LogError($"Error calling Telegram API: {response.StatusCode} - {errorMessage}");
			}

			return response;
		}
		catch (HttpRequestException ex)
		{
			_logger.LogError(ex, $"HttpRequestException when calling Telegram API: {ex.Message}");
			throw;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"Unknown error when calling Telegram API: {ex.Message}");
			throw;
		}
	}
}