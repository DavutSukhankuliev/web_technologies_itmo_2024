using Newtonsoft.Json;

namespace web_technologies_itmo_2024.Models.TelegramWebhookRespondModel;

public class MessagePersonalModel : BaseMessageModel
{
	[JsonProperty("chat")]
	public ChatPersonalModel Chat { get; set; }
}