using web_technologies_itmo_2024.Models;

namespace web_technologies_itmo_2024.Services;

public class TelegramBotCommandHandlerService
{
	private const string _logTag = $"[{nameof(TelegramBotCommandHandlerService)}]";

	private ILogger<TelegramBotCommandHandlerService> _logger;
	private HuggingFaceService _huggingFaceService;

	public TelegramBotCommandHandlerService(
		ILogger<TelegramBotCommandHandlerService> logger, 
		HuggingFaceService huggingFaceService
		)
	{
		_logger = logger;
		_huggingFaceService = huggingFaceService;
	}

	public async Task<BaseTelegramSendModel> HandleCommandModel(TelegramBotCommandModel model)
	{
		BaseTelegramSendModel telegramSendModel = new BaseTelegramSendModel()
		{
			ChatId = model.ChatId,
			Author = model.Author,
		};
		TelegramSendMessageModel telegramSendMessageModel = telegramSendModel as TelegramSendMessageModel;
		TelegramSendPhotoModel telegramSendPhotoModel = telegramSendModel as TelegramSendPhotoModel;

		switch (model.Command)
		{
			case TelegramBotCommands.BotName:
				telegramSendMessageModel.Text = $"Hi! I'm a bot you can use ask and draw commands. Try it out now ;)" +
				                                $"\r\nExample: /testBotForIctBot_ask What's your name?";
				break;
			case TelegramBotCommands.Ask:
				if (string.IsNullOrEmpty(model.Text))
				{
					telegramSendMessageModel.Text =
						$"Empty prompt received. Write something." +
						$"\r\nExample: /testBotForIctBot_ask What's your name?";
				}
				else
				{
					var apiResult = await _huggingFaceService.SendPromptAsync(model.Text);
					var answer = _huggingFaceService.ParseApiResult(apiResult);
					telegramSendMessageModel.Text = answer;
				}
				break;
			case TelegramBotCommands.Draw:
				if (string.IsNullOrEmpty(model.Text))
				{
					telegramSendMessageModel.Text =
						$"Empty prompt received. Write something." +
						$"\r\nExample: /testBotForIctBot_draw Macaroni demon";
				}
				else
				{
					// todo: imageGenerator
				}
				break;
			case TelegramBotCommands.Unknown:
				telegramSendMessageModel = new TelegramSendMessageModel
				{
					ChatId = model.ChatId,
					Author = model.Author,
					Text = $"Unknown command received. Please use /..._ask or /..._draw commands" +
					       $"\r\nExample: /testBotForIctBot_ask What's your name?",
				};
				break;
		}

		return string.IsNullOrEmpty(telegramSendMessageModel.Text) ? telegramSendPhotoModel : telegramSendMessageModel;
	}
}