using Newtonsoft.Json;

namespace web_technologies_itmo_2024.Models.SupabaseResponseModels;

public class SupabaseMessageSendEndpointModel
{
	[JsonProperty("message_id")]
	public uint MessageId { get; set; }
}