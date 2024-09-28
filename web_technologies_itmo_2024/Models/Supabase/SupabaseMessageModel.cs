using Newtonsoft.Json;

namespace web_technologies_itmo_2024.Models.Supabase;

public class SupabaseMessageModel
{
	[JsonProperty("to")]
	public string To { get; set; }

	[JsonProperty("text")]
	public string Text { get; set; }
}