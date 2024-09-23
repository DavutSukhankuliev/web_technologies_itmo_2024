using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace web_technologies_itmo_2024.PicsumService;

[ApiController]
[Route("hw3")]
public class PicsumController : ControllerBase
{
	private readonly TelegramPhotoSenderService _telegramPhotoSenderService;
	private readonly ILogger<PicsumController> _logger;

	public PicsumController(TelegramPhotoSenderService telegramPhotoSenderService, ILogger<PicsumController> logger)
	{
		_telegramPhotoSenderService = telegramPhotoSenderService;
		_logger = logger;
	}

	[HttpPost]
	public async Task<IActionResult> GetPhotoId([FromQuery] string id)
	{
		if (string.IsNullOrEmpty(id))
		{
			return BadRequest("Parameter 'id' is required");
		}

		var photoUrl = $"{PicsumConstants.URL}{id}{PicsumConstants.SQUARE_IMAGE_SUFFIX}";
		var caption = $"Тест работоспособности Telegram API. Проект C#";

		return await SendPhoto(photoUrl, caption);
	}

	private async Task<IActionResult> SendPhoto(string photoUrl, string caption = "")
	{
		try
		{
			var response = await _telegramPhotoSenderService.SendPhotoAsync(photoUrl, caption);

			if (response.IsSuccessStatusCode)
			{
				var apiResult = await response.Content.ReadAsStringAsync();
				return Ok(apiResult);
			}
			else
			{
				var errorMessage = await response.Content.ReadAsStringAsync();
				return StatusCode((int)response.StatusCode, $"Error calling Telegram API: {errorMessage}");
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Ошибка при отправке фотографии в Telegram");
			return StatusCode(500, "Internal Server Error");
		}
	}
}