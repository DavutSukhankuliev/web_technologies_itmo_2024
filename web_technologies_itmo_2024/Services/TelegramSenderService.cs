using System.Text;
using Newtonsoft.Json;

namespace web_technologies_itmo_2024.Services;

public class TelegramSenderService
{
	private readonly HttpClient _httpClient;
	private readonly ILogger<TelegramSenderService> _logger;
	private readonly string _sendPhotoApiUrl;
	private readonly string _sendMessageApiUrl;
	private readonly long _chatId;

	public TelegramSenderService(HttpClient httpClient, ILogger<TelegramSenderService> logger, string botKey, string chatId)
	{
		_httpClient = httpClient;
		_logger = logger;
		_sendPhotoApiUrl = $"https://api.telegram.org/bot{botKey}/sendPhoto";
		_sendMessageApiUrl = $"https://api.telegram.org/bot{botKey}/sendMessage";
		_chatId = long.Parse(chatId);
		
	}

	public async Task<HttpResponseMessage> SendPhotoAsync(string photoUrl, string caption = "")
	{
		return await SendPhotoInternalAsync(photoUrl, caption, _chatId);
	}

	public async Task<HttpResponseMessage> SendPhotoAsync(string photoUrl, long chatId, string caption = "")
	{
		return await SendPhotoInternalAsync(photoUrl, caption, chatId);
	}

	public async Task<HttpResponseMessage> SendPhotoAsync(byte[] bytes, string caption = "")
	{
		return await SendPhotoFromBytesAsync(bytes, caption, _chatId);
	}

	public async Task<HttpResponseMessage> SendPhotoAsync(byte[] bytes, long chatId, string caption = "")
	{
		return await SendPhotoFromBytesAsync(bytes, caption, chatId);
	}

	public async Task<HttpResponseMessage> SendMessageAsync(string message)
	{
		return await SendMessageInternalAsync(message, _chatId);
	}

	public async Task<HttpResponseMessage> SendMessageAsync(string message, long chatId)
	{
		return await SendMessageInternalAsync(message, chatId);
	}

	private async Task<HttpResponseMessage> SendPhotoFromBytesAsync(byte[] imageBytes, string caption, long chatId)
	{
		using var stream = new MemoryStream(imageBytes);
		var form = new MultipartFormDataContent
		{
			{ new StringContent(chatId.ToString()), "chat_id" },
			{ new StringContent(caption), "caption" },
			{ new StreamContent(stream), "photo", "image.jpg" }
		};

		try
		{
			_logger.LogInformation($"{_logger} Attempting to send message to Telegram API.");
			_logger.LogDebug($"{_logger} Attempting to send message to Telegram API. Payload: {form}");
			var response = await _httpClient.PostAsync(_sendPhotoApiUrl, form);

			if (response.IsSuccessStatusCode)
			{
				_logger.LogInformation($"{_logger} Successfully received response from Telegram API.");
			}
			else
			{
				var errorMessage = await response.Content.ReadAsStringAsync();
				_logger.LogError($"{_logger} Error calling Telegram API: {response.StatusCode} - {errorMessage}");
			}

			return response;
		}
		catch (HttpRequestException ex)
		{
			_logger.LogError(ex, $"{_logger} HttpRequestException when calling Telegram API: {ex.Message}");
			throw;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"{_logger} Unknown error when calling Telegram API: {ex.Message}");
			throw;
		}
	}

	private async Task<HttpResponseMessage> SendPhotoInternalAsync(string photoUrl, string caption, long chatId)
	{
		var payload = new
		{
			chat_id = chatId,
			photo = photoUrl,
			caption = caption
		};

		var jsonPayload = JsonConvert.SerializeObject(payload);
		var stringContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

		try
		{
			_logger.LogInformation($"{_logger} Attempting to send message to Telegram API.");
			_logger.LogDebug($"{_logger} Attempting to send message to Telegram API. Payload: {jsonPayload}");
			var response = await _httpClient.PostAsync(_sendPhotoApiUrl, stringContent);

			if (response.IsSuccessStatusCode)
			{
				_logger.LogInformation($"{_logger} Successfully received response from Telegram API.");
			}
			else
			{
				var errorMessage = await response.Content.ReadAsStringAsync();
				_logger.LogError($"{_logger} Error calling Telegram API: {response.StatusCode} - {errorMessage}");
			}

			return response;
		}
		catch (HttpRequestException ex)
		{
			_logger.LogError(ex, $"{_logger} HttpRequestException when calling Telegram API: {ex.Message}");
			throw;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"{_logger} Unknown error when calling Telegram API: {ex.Message}");
			throw;
		}
	}

	private async Task<HttpResponseMessage> SendMessageInternalAsync(string message, long chatId)
	{
		var payload = new
		{
			chat_id = chatId,
			text = message,
		};

		var jsonPayload = JsonConvert.SerializeObject(payload);
		var stringContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

		try
		{
			_logger.LogInformation($"{_logger} Attempting to send message to Telegram API.");
			_logger.LogDebug($"{_logger} Attempting to send message to Telegram API. Payload: {jsonPayload}");
			var response = await _httpClient.PostAsync(_sendMessageApiUrl, stringContent);

			if (response.IsSuccessStatusCode)
			{
				_logger.LogInformation($"{_logger} Successfully received response from Telegram API.");
			}
			else
			{
				var errorMessage = await response.Content.ReadAsStringAsync();
				_logger.LogError($"{_logger} Error calling Telegram API: {response.StatusCode} - {errorMessage}");
			}

			return response;
		}
		catch (HttpRequestException ex)
		{
			_logger.LogError(ex, $"{_logger} HttpRequestException when calling Telegram API: {ex.Message}");
			throw;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"{_logger} Unknown error when calling Telegram API: {ex.Message}");
			throw;
		}
	}
}