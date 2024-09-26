namespace web_technologies_itmo_2024.Models;

public class TelegramSendPhotoModel : BaseTelegramSendModel
{
	public byte[] PhotoBytes { get; set; }
	public string Caption { get; set; }
}