using Newtonsoft.Json;

namespace web_technologies_itmo_2024.HuggingFaceController;

public class PromptRequestModel
{
	[JsonProperty("prompt")] public string Prompt { get; set; } = string.Empty;
}