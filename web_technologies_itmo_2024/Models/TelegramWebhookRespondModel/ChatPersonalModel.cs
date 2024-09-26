using Newtonsoft.Json;

namespace web_technologies_itmo_2024.Models.TelegramWebhookRespondModel;

public class ChatPersonalModel
{
	[JsonProperty("id")]
	public long Id { get; set; }

	[JsonProperty("first_name")]
	public string? FirstName { get; set; }

	// Некоторым полям так же допустимо быть null
	[JsonProperty("last_name")]
	public string? LastName { get; set; }

	[JsonProperty("username")]
	public string Username { get; set; }

	[JsonProperty("type")]
	public string Type { get; set; }
}