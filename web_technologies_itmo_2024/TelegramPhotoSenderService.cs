using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace web_technologies_itmo_2024;

public class TelegramPhotoSenderService
{
	private readonly HttpClient _httpClient;
	private readonly ILogger<TelegramPhotoSenderService> _logger;
	private readonly string _telegramApiUrl;
	private readonly string _chatId;

	public TelegramPhotoSenderService(HttpClient httpClient, ILogger<TelegramPhotoSenderService> logger, string botKey, string chatId)
	{
		_httpClient = httpClient;
		_logger = logger;
		_telegramApiUrl = $"https://api.telegram.org/bot{botKey}/sendPhoto";
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
			var response = await _httpClient.PostAsync(_telegramApiUrl, stringContent);

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
}