using Newtonsoft.Json;

namespace web_technologies_itmo_2024.Models.TelegramWebhookRespondModel;

public class MessageGroupModel : BaseMessageModel
{
	[JsonProperty("chat")]
	public ChatGroupModel Chat { get; set; }
}