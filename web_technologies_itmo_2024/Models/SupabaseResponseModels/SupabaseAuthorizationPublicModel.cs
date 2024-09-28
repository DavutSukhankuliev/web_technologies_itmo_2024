using Newtonsoft.Json;

namespace web_technologies_itmo_2024.Models.SupabaseResponseModels;

public class SupabaseAuthorizationPublicModel
{
	[JsonProperty("id")]
	public uint Id { get; set; }

	[JsonProperty("username")]
	public string Username { get; set; }
}