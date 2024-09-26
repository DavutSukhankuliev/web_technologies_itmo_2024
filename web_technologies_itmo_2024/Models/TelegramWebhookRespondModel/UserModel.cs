using Newtonsoft.Json;

namespace web_technologies_itmo_2024.Models.TelegramWebhookRespondModel;

public class UserModel
{
	[JsonProperty("id")]
	public long Id { get; set; }

	[JsonProperty("is_bot")]
	public bool IsBot { get; set; }

	[JsonProperty("first_name")]
	public string? FirstName { get; set; }

	[JsonProperty("last_name")]
	public string? LastName { get; set; }

	[JsonProperty("username")]
	public string Username { get; set; }

	[JsonProperty("language_code")]
	public string? LanguageCode { get; set; }
}