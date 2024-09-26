namespace web_technologies_itmo_2024.Models;

public class TelegramSendPhotoModel
{
	public long ChatId { get; set; }
	public string Author { get; set; }
	public byte[] PhotoBytes { get; set; }
	public string Caption { get; set; }
}