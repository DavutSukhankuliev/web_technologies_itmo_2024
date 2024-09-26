namespace web_technologies_itmo_2024.Models;

public class TelegramBotCommandModel
{
	public long ChatId { get; set; }
	public string Author { get; set; }
	public TelegramBotCommands Command { get; set; }
	public string Text { get; set; }
}