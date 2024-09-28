namespace web_technologies_itmo_2024.Services;

public class TelegramBotCommandParserService
{
	private const string _logTag = $"[{nameof(TelegramBotCommandParserService)}]";

	private readonly ILogger<TelegramBotCommandParserService> _logger;

	private readonly string _botName;

	public TelegramBotCommandParserService(
		ILogger<TelegramBotCommandParserService> logger,
		IConfiguration configuration)
	{
		_logger = logger;
		_botName = configuration["TelegramBotName"];
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
		    && !text.StartsWith($"@{_botName}", StringComparison.OrdinalIgnoreCase))
		{
			_logger.LogError($"{_logTag} TryParseCommand received not a bot command. Method parses only bot commands");
			return false;
		}

		_logger.LogDebug($"{_logTag} {text}");

		(command, arguments) = GetCommandAndArguments(text);

		if (string.IsNullOrEmpty(arguments) || command == TelegramBotCommands.Unknown)
		{
			_logger.LogError($"{_logTag} GetCommandAndArguments error while parsing command");
			return false;
		}

		return true;
	}

	private (TelegramBotCommands, string) GetCommandAndArguments(string text)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			_logger.LogError($"{_logTag} GetCommandAndArguments received null or empty string. Default values...");
			return (TelegramBotCommands.Unknown, string.Empty);
		}

		var cleanedCommand = text
			.Replace($"@{_botName}", string.Empty)
			.Replace($"/{_botName}_", string.Empty)
			.Trim();

		_logger.LogDebug($"{_logTag} Cleaned command: {cleanedCommand}");

		var parts = cleanedCommand.Split(new char[] { ' ', ':' }, 2, StringSplitOptions.RemoveEmptyEntries);
		if (parts.Length < 2)
		{
			_logger.LogError($"{_logTag} Command part is empty or contains only command without arguments.");
			return (TelegramBotCommands.Unknown, string.Empty);
		}

		var commandPart = parts[0];
		var userInputPart = parts.Length > 1 ? parts[1].Trim() : string.Empty;

		var command = MapCommand(commandPart.ToLower());

		_logger.LogDebug($"{_logTag} Parsed command: {command}, Arguments: {userInputPart}");

		return (command, userInputPart);
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
			command = MapCommand(commandName);
			return true;
		}

		return false;
	}

	private TelegramBotCommands MapCommand(string commandName)
	{
		return commandName.ToLower() switch
		{
			"ask" => TelegramBotCommands.Ask,
			"draw" => TelegramBotCommands.Draw,
			_ => TelegramBotCommands.Unknown,
		};
	}
}