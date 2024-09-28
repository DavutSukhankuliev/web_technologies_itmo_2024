using Newtonsoft.Json;
using web_technologies_itmo_2024.Models;

namespace web_technologies_itmo_2024.Services;

public class HuggingFaceApiResultParser
{
	private const string _logTag = $"[{nameof(HuggingFaceApiResultParser)}]";

	private readonly ILogger<HuggingFaceApiResultParser> _logger;

	public HuggingFaceApiResultParser(ILogger<HuggingFaceApiResultParser> logger)
	{
		_logger = logger;
	}

	public string ParseApiResult(string apiResult)
	{
		try
		{
			var resList = JsonConvert.DeserializeObject<List<FlanT5BaseModel>>(apiResult);

			if (resList == null || resList.Count == 0)
			{
				_logger.LogWarning($"{_logTag} No results found in the API response.");
				return "No result found";
			}

			var firstResult = resList[0].GeneratedText;
			_logger.LogDebug($"{_logTag} Parsed first result: {firstResult}");
			return firstResult;
		}
		catch (JsonException ex)
		{
			_logger.LogError(ex, $"{_logTag} Error parsing API result: {ex.Message}");
			throw new ApplicationException("Error parsing API result.", ex);
		}
	}
}