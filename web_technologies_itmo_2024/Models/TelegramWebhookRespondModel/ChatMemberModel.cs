using Newtonsoft.Json;

namespace web_technologies_itmo_2024.Models.TelegramWebhookRespondModel;

public class ChatMemberModel
{
	[JsonProperty("user")]
	public UserModel User { get; set; }

	[JsonProperty("status")]
	public string Status { get; set; }
}