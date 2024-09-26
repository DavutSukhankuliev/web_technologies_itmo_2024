namespace web_technologies_itmo_2024;

public enum TelegramBotCommands : ushort
{
	BotName = 0,
	Ask = 1,
	Draw = 2,
	Unknown = 65535,	// ushort.MaxValue
}