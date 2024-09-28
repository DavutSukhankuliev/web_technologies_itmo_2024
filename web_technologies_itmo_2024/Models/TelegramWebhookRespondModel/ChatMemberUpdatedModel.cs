using Newtonsoft.Json;

namespace web_technologies_itmo_2024.Models.TelegramWebhookRespondModel;

public class ChatMemberUpdatedModel
{
	[JsonProperty("chat")]
	public ChatGroupModel Chat { get; set; }

	[JsonProperty("from")]
	public UserModel From { get; set; }

	[JsonProperty("date")]
	public long Date { get; set; }

	[JsonProperty("old_chat_member")]
	public ChatMemberModel OldChatMember { get; set; }

	[JsonProperty("new_chat_member")]
	public ChatMemberModel NewChatMember { get; set; }
}