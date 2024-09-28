using Newtonsoft.Json;

namespace web_technologies_itmo_2024.Models.TelegramWebhookRespondModel;

public class ChatGroupModel
{
	[JsonProperty("id")]
	public long Id { get; set; }

	[JsonProperty("title")]
	public string? Title { get; set; }

	[JsonProperty("type")]
	public string Type { get; set; }

	[JsonProperty("all_members_are_administrators")]
	public bool? AllMembersAreAdministrators { get; set; }
}