using web_technologies_itmo_2024.Models;
using web_technologies_itmo_2024.Models.TelegramWebhookRespondModel;

namespace web_technologies_itmo_2024.Services;

public class TelegramUpdateService
{
	private const string _logTag = $"[{nameof(TelegramUpdateService)}]";

	private readonly ILogger<TelegramUpdateService> _logger;
	private readonly TelegramBotCommandParserService _parser;

	public TelegramUpdateService(
		ILogger<TelegramUpdateService> logger, 
		TelegramBotCommandParserService parser
		)
	{
		_logger = logger;
		_parser = parser;
	}

	public TelegramBotCommandModel Deserialize(UpdateModel update)
	{
		TelegramBotCommandModel telegramBotCommandModel;

		var message = update.Message;
		var text = message.Text;

		if (!_parser.TryParseCommand(text, out var command, out var userInput))
		{
			_logger.LogError($"{_logTag} Error occured while parsing the command");
		}

		var author = $"@{message.From?.Username}";
		var chatId = GetChatId(message);

		telegramBotCommandModel = new TelegramBotCommandModel
		{
			ChatId = chatId,
			Author = author,
			Command = command,
			Text = userInput
		};

		return telegramBotCommandModel;
	}

	private long GetChatId(BaseMessageModel message)
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
}