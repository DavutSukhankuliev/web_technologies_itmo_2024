using Microsoft.AspNetCore.Mvc;
using web_technologies_itmo_2024.Models.TelegramWebhookRespondModel;
using web_technologies_itmo_2024.Services;

namespace web_technologies_itmo_2024.Controllers;

[ApiController]
[Route("api")]
public class TelegramBotUpdateReceiverController : ControllerBase
{
	private const string _logTag = $"[{nameof(TelegramBotReceiveMessageService)}]";

	private readonly ILogger<TelegramBotReceiveMessageService> _logger;
	private readonly TelegramUpdateService _update;
	private readonly TelegramBotCommandHandlerService _commandHandler;

	public TelegramBotUpdateReceiverController(
		ILogger<TelegramBotReceiveMessageService> logger, 
		TelegramUpdateService update, 
		TelegramBotCommandHandlerService commandHandler
		)
	{
		_logger = logger;
		_update = update;
		_commandHandler = commandHandler;
	}

	[HttpPost]
	[Route("telegram-bot-update-receiver")]
	public async Task<IActionResult> Post([FromBody] UpdateModel updateModel)
	{
		if (updateModel?.Message == null)
		{
			_logger.LogWarning($"{_logTag} Invalid update received: message is null");
			return BadRequest("Invalid update received: message is null");
		}
		

		try
		{
			var commandModel = _update.Deserialize(updateModel);
			var sendModel = await _commandHandler.HandleCommandModel(commandModel);

			/*if (TryParseCommand(text, out var command))
			{
				var stringContent = await HandleCommand(command, author, chatId);
				return await TrySendMessageAsync(stringContent);
			}*/
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"Error occurred while processing the request.");
			return StatusCode(500, "Internal server error");
		}

		return Ok();
	}
}