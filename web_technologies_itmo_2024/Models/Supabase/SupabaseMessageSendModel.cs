using Newtonsoft.Json;

namespace web_technologies_itmo_2024.Models.Supabase;

public class SupabaseMessageSendModel : SupabaseUserModel
{
	[JsonProperty("message")]
	public SupabaseMessageModel Message { get; set; }
}