namespace web_technologies_itmo_2024.Services;

public class TelegramBotCommandParserService
{
	private const string _logTag = $"[{nameof(TelegramBotCommandParserService)}]";

	private readonly Logger<TelegramBotCommandParserService> _logger;

	private readonly string _botName;

	public TelegramBotCommandParserService(
		Logger<TelegramBotCommandParserService> logger,
		string botName)
	{
		_logger = logger;
		_botName = botName;
	}

	public bool TryParseCommand(string text, out TelegramBotCommands command, out string arguments)
	{
		command = TelegramBotCommands.Unknown;
		arguments = string.Empty;

		if (string.IsNullOrEmpty(text))
		{
			_logger.LogError($"{_logTag} TryParseCommand received null or empty string. Returning false...");
			return false;
		}

		if (IsBotNameOnly(text))
		{
			command = TelegramBotCommands.BotName;
			return true;
		}

		if (IsCommandWithoutArguments(text, TelegramBotCommands.Ask.ToString().ToLowerInvariant(), out command) ||
		    IsCommandWithoutArguments(text, TelegramBotCommands.Draw.ToString().ToLowerInvariant(), out command))
		{
			return true;
		}

		if (!text.StartsWith($"/{_botName}", StringComparison.OrdinalIgnoreCase)
		    && !text.Contains($"@{_botName}", StringComparison.OrdinalIgnoreCase))
		{
			_logger.LogError($"{_logTag} TryParseCommand received not a bot command. Method parses only bot commands");
			return false;
		}

		var parts = text.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
		if (parts.Length == 0)
		{
			_logger.LogError($"{_logTag} Error while trying to parse {text.Remove(20)}...");
			return false;
		}

		string commandPart = ExtractCommandPart(parts[0]);

		command = ParseCommand(commandPart);
		arguments = parts.Length > 1 ? parts[1] : null;
		return true;

	}

	private bool IsBotNameOnly(string text)
	{
		return text.Equals($"/{_botName}", StringComparison.OrdinalIgnoreCase)
		       || text.Equals($"@{_botName}", StringComparison.OrdinalIgnoreCase);
	}

	private bool IsCommandWithoutArguments(string text, string commandName, out TelegramBotCommands command)
	{
		command = TelegramBotCommands.Unknown;

		if (text.Equals($"/{_botName}_{commandName}", StringComparison.OrdinalIgnoreCase) ||
		    text.Equals($"@{_botName} {commandName}:", StringComparison.OrdinalIgnoreCase))
		{
			command = ParseCommand(commandName);
			return true;
		}

		return false;
	}

	private string ExtractCommandPart(string commandPart)
	{
		if (commandPart.Contains($"@{_botName}", StringComparison.OrdinalIgnoreCase))
		{
			return commandPart.Replace($"@{_botName}", "", StringComparison.OrdinalIgnoreCase)
				.Replace(":", "", StringComparison.OrdinalIgnoreCase)
				.Trim();
		}
		if (commandPart.StartsWith($"/{_botName}_", StringComparison.OrdinalIgnoreCase))
		{
			return commandPart.Replace($"/{_botName}_", "", StringComparison.OrdinalIgnoreCase)
				.Trim();
		}
		return commandPart;
	}

	private TelegramBotCommands ParseCommand(string commandName)
	{
		return commandName.ToLower() switch
		{
			"ask" => TelegramBotCommands.Ask,
			"draw" => TelegramBotCommands.Draw,
			_ => TelegramBotCommands.Unknown,
		};
	}
}