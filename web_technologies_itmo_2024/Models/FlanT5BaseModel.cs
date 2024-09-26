using Newtonsoft.Json;

namespace web_technologies_itmo_2024.Models;

public class FlanT5BaseModel
{
	[JsonProperty("generated_text")] public string GeneratedText { get; set; } = string.Empty;
}