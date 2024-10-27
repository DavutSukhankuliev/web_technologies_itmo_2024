using Microsoft.AspNetCore.Mvc;
using web_technologies_itmo_2024.Models;
using web_technologies_itmo_2024.Models.TelegramWebhookRespondModel;
using web_technologies_itmo_2024.Services;

namespace web_technologies_itmo_2024.Controllers;

[ApiController]
[Route("api")]
public class TelegramBotUpdateReceiverController : ControllerBase
{
	private const string _logTag = $"[{nameof(TelegramBotUpdateReceiverController)}]";

	private readonly ILogger<TelegramBotUpdateReceiverController> _logger;
	private readonly TelegramUpdateService _update;
	private readonly TelegramBotCommandHandlerService _commandHandler;
	private readonly TelegramServiceWrapper _telegramServiceWrapper;

	private readonly string _botName;

	public TelegramBotUpdateReceiverController(
		ILogger<TelegramBotUpdateReceiverController> logger, 
		TelegramUpdateService update, 
		TelegramBotCommandHandlerService commandHandler, 
		TelegramServiceWrapper telegramServiceWrapper
		)
	{
		_logger = logger;
		_update = update;
		_commandHandler = commandHandler;
		_telegramServiceWrapper = telegramServiceWrapper;
		_botName = Environment.GetEnvironmentVariable("TELEGRAM_BOT_NAME");
	}

	[HttpPost]
	[Route("telegram-bot-update-receiver")]
	public async Task<IActionResult> Post([FromBody] UpdateModel updateModel)
	{
		_logger.LogInformation($"{_logTag} An update received!");

		var cachedRequestBody = HttpContext.Items["CachedRequestBody"] as string;

		_logger.LogDebug($"{_logTag} {cachedRequestBody}");

		if (!IsMessageRelevant(updateModel))
		{
			return Ok("Message not relevant.");
		}

		try
		{
			var commandModel = _update.Deserialize(updateModel);
			var sendModel = await _commandHandler.HandleCommandModel(commandModel);
			switch (sendModel)
			{
				case TelegramSendMessageModel sendMessageModel:
					return await _telegramServiceWrapper.SendMessageAsync(sendMessageModel);
					break;
				case TelegramSendPhotoModel sendPhotoModel:
					return await _telegramServiceWrapper.SendPhotoAsync(sendPhotoModel);
					break;
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"Error occurred while processing the request.");
			return StatusCode(500, $"{ex} Internal server error");
		}

		return Ok();
	}

	private bool IsMessageRelevant(UpdateModel updateModel)
	{
		if (updateModel == null)
		{
			_logger.LogWarning($"{_logTag} updateModel is null.");
			return false;
		}

		if (updateModel.Message == null
		    || updateModel.Message.NewChatMembers != null && updateModel.Message.NewChatMembers.Length != 0)
		{
			_logger.LogInformation($"{_logTag} Non-message update received. Skip.");
			return false;
		}

		BaseMessageModel message = updateModel.Message;

		if (string.IsNullOrWhiteSpace(message.Text))
		{
			_logger.LogInformation($"{_logTag} Message has no text. Skip.");
			return false;
		}

		if (!message.Text.StartsWith($"@{_botName}") && !message.Text.StartsWith($"/{_botName}"))
		{
			_logger.LogDebug($"{_logTag} Not a bot command. Skip.");
			return false;
		}

		return true;
	}
}