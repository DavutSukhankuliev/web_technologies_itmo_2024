using Newtonsoft.Json;

namespace web_technologies_itmo_2024.Models.SupabaseResponseModels;

public class SupabaseMessageSendResponseModel
{
	[JsonProperty("id")]
	public uint MessageId { get; set; }
}