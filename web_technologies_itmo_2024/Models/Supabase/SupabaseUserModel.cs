using Newtonsoft.Json;

namespace web_technologies_itmo_2024.Models.Supabase;

public class SupabaseUserModel
{
	[JsonProperty("username")]
	public string Username { get; set; }

	[JsonProperty("password")]
	public string Password { get; set; }
}