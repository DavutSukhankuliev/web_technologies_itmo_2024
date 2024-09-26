using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using web_technologies_itmo_2024.Models;
using web_technologies_itmo_2024.Models.TelegramWebhookRespondModel;
using static System.String;

namespace web_technologies_itmo_2024;

[ApiController]
[Route("api")]
public class TelegramBotReceiveMessageService : ControllerBase
{
	private const string _logTag = $"[{nameof(TelegramBotReceiveMessageService)}]";

	private readonly HttpClient _httpClient;
	private readonly TelegramSenderService _telegramSenderService;
	private readonly ILogger<TelegramBotReceiveMessageService> _logger;

	private readonly string _modelApi;
	private readonly string _apiKey;
	private readonly string _botName;

	public TelegramBotReceiveMessageService(
		HttpClient httpClient, 
		TelegramSenderService telegramSenderService, 
		ILogger<TelegramBotReceiveMessageService> logger, 
		IConfiguration configuration)
	{
		_httpClient = httpClient;
		_telegramSenderService = telegramSenderService;
		_logger = logger;
		var localConfiguration = configuration;

		_modelApi = localConfiguration["HuggingFace:ApiUrl"] + localConfiguration["HuggingFace:ImageModelEndpoint"];
		_apiKey = localConfiguration["HuggingFace:AuthToken"];
		_botName = localConfiguration["TelegramBotName"];

		if (IsNullOrEmpty(_modelApi) || IsNullOrEmpty(_apiKey) || IsNullOrEmpty(_botName))
		{
			_logger.LogError($"{_logTag} Your configuration is null or empty. Check your appsettings.json");
			throw new InvalidOperationException($"{_logTag} Your configuration is null or empty. Check your appsettings.json");
		}

		_logger.LogInformation($"{_logTag} TelegramBotReceiveMessageService initialized with Model API: {_modelApi}");
	}

	[HttpPost("text_to_image")]
	public async Task<IActionResult> Post([FromBody] UpdateModel updateModel)
	{
		_logger.LogInformation($"Received update: {updateModel}");

		if (updateModel?.Message == null)
		{
			_logger.LogWarning("Invalid update received: message is null");
			return BadRequest("Invalid update received: message is null");
		}

		var message = updateModel.Message;
		var text = message.Text;
		var from = message.From;
		var author = from?.Username;

		var chatId = GetChatId(message);

		if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(author))
		{
			_logger.LogWarning("Invalid message or author: text or author is null or empty");
			return BadRequest("Invalid message or author: text or author is null or empty");
		}
		
		_logger.LogInformation($"Message from @{author}: {text}");

		try
		{
			if (TryParseCommand(text, out var command))
			{
				var stringContent = await HandleCommand(command, author, chatId);
				return await TrySendMessageAsync(stringContent);
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"Error occurred while processing the request.");
			return StatusCode(500, "Internal server error");
		}

		return Ok();
	}

	private static long GetChatId(BaseMessageModel message)
	{
		long chatId;
		var messageModel = message;
		if (messageModel is MessagePersonalModel)
		{
			var newMessageModel = messageModel as MessagePersonalModel;
			chatId = newMessageModel.Chat.Id;
		}
		else
		{
			var newMessageModel = messageModel as MessageGroupModel;
			chatId = newMessageModel.Chat.Id;
		}

		return chatId;
	}

	private bool TryParseCommand(string text, out string command)
	{
		command = null;
		if (!IsNullOrEmpty(text) && (text.StartsWith("/") || text.Contains($"@{_botName}")))
		{
			var parts = text.Split(new[] { ' ' }, 2);
			if (parts[0].Contains($"@{_botName}"))
			{
				command = parts.Length > 1 ? parts[1] : null;
				return true;
			}
			if (parts[0].StartsWith($"/{_botName}"))
			{
				command = parts.Length > 1 ? parts[1] : null;
				return true;
			}
		}
		return false;
	}

	private async Task<TelegramSendPhotoModel> HandleCommand(string command, string author, long chatId)
	{
		TelegramSendPhotoModel model = new TelegramSendPhotoModel
		{
			Author = author,
			ChatId = chatId,
			Caption = command
		};

		switch (command?.ToLower())
		{
			case "напиши свое имя":
				model.Caption = $"Я бот, и мое имя @{_botName}!";
				break;
			default:
				model.PhotoBytes = await TrySendPromptAsync(command);
				_logger.LogInformation($"{_logTag} {model.PhotoBytes}");
				break;
		}
		return model;
	}

	private async Task<byte[]> TrySendPromptAsync(string prompt)
	{
		var payload = new
		{
			inputs = prompt
		};

		var stringContent = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

		var request = new HttpRequestMessage(HttpMethod.Post, _modelApi)
		{
			Content = stringContent
		};

		request.Headers.Add("Authorization", $"Bearer {_apiKey}");

		try
		{
			_logger.LogInformation($"{_logTag} Sending request to HuggingFace API.");

			var response = await _httpClient.SendAsync(request);

			if (response.IsSuccessStatusCode)
			{
				_logger.LogInformation($"{_logTag} Received successful response from HuggingFace API.");
				return await response.Content.ReadAsByteArrayAsync();
			}
			else
			{
				var errorMessage = await response.Content.ReadAsStringAsync();
				_logger.LogError($"{_logTag} Error calling HuggingFace API: {response.StatusCode} - {errorMessage}");
				throw new HttpRequestException($"Error calling HuggingFace API: {response.StatusCode} - {errorMessage}");
			}
		}
		catch (HttpRequestException ex)
		{
			_logger.LogError(ex, $"{_logTag} HttpRequestException while sending prompt to HuggingFace: {ex.Message}");
			throw new ApplicationException("HttpRequestException while sending prompt to HuggingFace.", ex);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"{_logTag} Unknown error while sending prompt to HuggingFace");
			throw;
		}
	}

	private async Task<IActionResult> TrySendMessageAsync(TelegramSendPhotoModel messageContents)
	{
		if (messageContents == null)
		{
			_logger.LogError($"{_logTag} messageContents is null.");
			return BadRequest("Message contents cannot be null.");
		}

		var content = messageContents.PhotoBytes;
		var author = messageContents.Author;
		var chatId = messageContents.ChatId;
		var caption = $"Вот ваш: {messageContents.Caption}";

		try
		{
			_logger.LogInformation($"{_logTag} Attempting to send message to Telegram.");

			caption += $"\n\r\n\rЭто сообщение отправлено при помощи TelegramBot вебхука в ответ на сообщение от пользователя @{author}";

			HttpResponseMessage response;
			if (content == null || content.Length == 0)
			{
				response = await _telegramSenderService.SendMessageAsync(caption, chatId);
			}
			else
			{
				response = await _telegramSenderService.SendPhotoAsync(content, chatId, caption);
			}

			if (response.IsSuccessStatusCode)
			{
				_logger.LogInformation($"{_logTag} Successfully sent message to Telegram.");
				return Ok();
			}
			else
			{
				_logger.LogError($"{_logTag} Error sending message to Telegram: {await response.Content.ReadAsStringAsync()}");
				return StatusCode((int)response.StatusCode, "Failed to send message to Telegram.");
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"{_logTag} Ошибка при отправке сообщения в Telegram: {ex.Message}");
			return StatusCode(500, "Internal Server Error");
		}
	}
}