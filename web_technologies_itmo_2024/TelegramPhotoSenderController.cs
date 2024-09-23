using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace web_technologies_itmo_2024;

[ApiController]
[Route("hw3")]
public class TelegramPhotoSenderController : ControllerBase
{
	private readonly HttpClient _httpClient;
	private readonly ILogger<TelegramPhotoSenderController> _logger;

	public TelegramPhotoSenderController(HttpClient httpClient, ILogger<TelegramPhotoSenderController> logger)
	{
		_httpClient = httpClient;
		_logger = logger;
	}

	[HttpPost]
	public async Task<IActionResult> SendPhoto([FromQuery] string id)
	{
		if (string.IsNullOrEmpty(id))
		{
			return BadRequest("Parameter 'id' is required");
		}

		var telegramUrl = $"https://api.telegram.org/bot7521746390:AAERSKb4h7yiBnLFIckdh2NmmGjn5Rs2sts/sendPhoto";
		var photoUrl = $"https://picsum.photos/id/{id}/200.jpg";

		var stringContent = new StringContent(JsonConvert.SerializeObject(new
		{
			chat_id = "-4526431218",
			photo = photoUrl,
			caption = "Тест работоспособности Telegram API. Проект C#"
		}), Encoding.UTF8, "application/json");

		try
		{
			var response = await _httpClient.PostAsync(telegramUrl, stringContent);

			if (response.IsSuccessStatusCode)
			{
				var apiResult = await response.Content.ReadAsStringAsync();
				_logger.LogInformation($"Успешный ответ от Telegram API: {apiResult}");
				return Ok(apiResult);
			}
			else
			{
				var errorMessage = await response.Content.ReadAsStringAsync();
				_logger.LogError($"Ошибка при вызове Telegram API: {errorMessage}");
				return StatusCode((int)response.StatusCode, "Error calling Telegram API");
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Ошибка при вызове Telegram API");
			return StatusCode(500, "Internal Server Error");
		}
	}
}