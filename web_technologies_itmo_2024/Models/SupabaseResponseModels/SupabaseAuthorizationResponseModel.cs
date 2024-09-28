using Newtonsoft.Json;

namespace web_technologies_itmo_2024.Models.SupabaseResponseModels;

public class SupabaseAuthorizationResponseModel : SupabaseAuthorizationPublicModel
{
	[JsonProperty("password")]
	public string Password { get; set; }
}