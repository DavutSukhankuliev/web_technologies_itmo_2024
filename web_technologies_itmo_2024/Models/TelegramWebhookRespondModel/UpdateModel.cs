using Newtonsoft.Json;

namespace web_technologies_itmo_2024.Models.TelegramWebhookRespondModel;

public class UpdateModel
{
	[JsonProperty("update_id")]
	public long UpdateId { get; set; }

	[JsonProperty("message")]
	public BaseMessageModel? Message { get; set; }

	[JsonProperty("my_chat_member")]
	public ChatMemberUpdatedModel? MyChatMember { get; set; }

	[JsonProperty("chat_member")]
	public ChatMemberUpdatedModel? ChatMember { get; set; }
}