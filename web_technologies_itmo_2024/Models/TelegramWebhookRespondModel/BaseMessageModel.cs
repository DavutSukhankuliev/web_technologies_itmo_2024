using Newtonsoft.Json;

namespace web_technologies_itmo_2024.Models.TelegramWebhookRespondModel;

public abstract class BaseMessageModel
{
	[JsonProperty("message_id")]
	public int MessageId { get; set; }

	[JsonProperty("from")]
	public UserModel From { get; set; }

	[JsonProperty("date")]
	public int Date { get; set; }

	[JsonProperty("text")]
	public string Text { get; set; }
}