using web_technologies_itmo_2024.Models;

namespace web_technologies_itmo_2024.Services;

public class TelegramBotCommandHandlerService
{
	private const string _logTag = $"[{nameof(TelegramBotCommandHandlerService)}]";

	private readonly ILogger<TelegramBotCommandHandlerService> _logger;
	private readonly HuggingFaceService _huggingFaceService;
	private readonly HuggingFaceApiResultParser _huggingFaceApiResultParser;

	private readonly string _botName;

	public TelegramBotCommandHandlerService(
		ILogger<TelegramBotCommandHandlerService> logger, 
		HuggingFaceService huggingFaceService,
		HuggingFaceApiResultParser huggingFaceApiResultParser
		)
	{
		_logger = logger;
		_huggingFaceService = huggingFaceService;
		_huggingFaceApiResultParser = huggingFaceApiResultParser;
		_botName = Environment.GetEnvironmentVariable("TELEGRAM_BOT_NAME");
	}

	public async Task<BaseTelegramSendModel> HandleCommandModel(TelegramBotCommandModel model)
	{
		TelegramSendMessageModel sendMessageModel = new TelegramSendMessageModel
		{
			ChatId = model.ChatId,
			Author = model.Author,
			Text = string.Empty
		};
		TelegramSendPhotoModel sendPhotoModel = new TelegramSendPhotoModel
		{
			ChatId = model.ChatId,
			Author = model.Author,
			PhotoBytes = default,
			Caption = string.Empty
		};

		switch (model.Command)
		{
			case TelegramBotCommands.BotName:
				sendMessageModel.Text = $"Hi! I'm a bot you can use ask and draw commands. Try it out now ;)" +
				                        $"\r\nExample: /{_botName}_ask What's your name?";
				break;
			case TelegramBotCommands.Ask:
				if (string.IsNullOrEmpty(model.Text))
				{
					sendMessageModel.Text =
						$"Empty prompt received. Write something." +
						$"\r\nExample: /{_botName}_ask What's your name?";
				}
				else
				{
					var apiResult = await _huggingFaceService.SendPromptAsync<string>(model.Text);
					var answer = _huggingFaceApiResultParser.ParseApiResult(apiResult);
					sendMessageModel.Text = answer;
				}
				break;
			case TelegramBotCommands.Draw:
				if (string.IsNullOrEmpty(model.Text))
				{
					sendMessageModel.Text =
						$"Empty prompt received. Write something." +
						$"\r\nExample: /{_botName}_draw Macaroni demon";
				}
				else
				{
					var apiResult = await _huggingFaceService.SendPromptAsync<byte[]>(model.Text);
					sendPhotoModel.PhotoBytes = apiResult;
				}
				break;
			case TelegramBotCommands.Unknown:
				sendMessageModel = new TelegramSendMessageModel
				{
					ChatId = model.ChatId,
					Author = model.Author,
					Text = $"Unknown command received. Please use /..._ask or /..._draw commands" +
					       $"\r\nExample: /{_botName}_ask What's your name?",
				};
				break;
		}

		return string.IsNullOrEmpty(sendMessageModel.Text) ? sendPhotoModel : sendMessageModel;
	}
}