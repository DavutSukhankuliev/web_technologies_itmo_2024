using System.Text;
using Newtonsoft.Json;
using web_technologies_itmo_2024.Models;

namespace web_technologies_itmo_2024.Services;

public class HuggingFaceService
{
	private const string _logTag = $"[{nameof(HuggingFaceService)}]";

	private readonly HttpClient _httpClient;
	private readonly ILogger<HuggingFaceService> _logger;

	private readonly string _textModelApi;
	private readonly string _imageModelApi;
	private readonly string _apiKey;

	public HuggingFaceService(HttpClient httpClient, ILogger<HuggingFaceService> logger, IConfiguration configuration)
	{
		_httpClient = httpClient;
		_logger = logger;

		_textModelApi = configuration["HuggingFace:ApiUrl"] + configuration["HuggingFace:TextModelEndpoint"];
		_imageModelApi = configuration["HuggingFace:ApiUrl"] + configuration["HuggingFace:ImageModelEndpoint"];
		_apiKey = configuration["HuggingFace:AuthToken"];

		ValidateHuggingFaceConfigs(new Dictionary<string, string> 
		{
			{ nameof(_textModelApi), _textModelApi },
			{ nameof(_imageModelApi), _imageModelApi },
			{ nameof(_apiKey), _apiKey }
		});
	}

	public async Task<string> SendPromptAsync(string prompt)
	{
		var payload = new { inputs = prompt };
		using var stringContent = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
		using var request = new HttpRequestMessage(HttpMethod.Post, _textModelApi) { Content = stringContent };
		request.Headers.Add("Authorization", $"Bearer {_apiKey}");

		try
		{
			_logger.LogInformation($"{_logTag} Sending request to HuggingFace API.");
			var response = await _httpClient.SendAsync(request);

			if (response.IsSuccessStatusCode)
			{
				_logger.LogInformation($"{_logTag} Received successful response from HuggingFace API.");
				return await response.Content.ReadAsStringAsync();
			}
			else
			{
				var errorMessage = await response.Content.ReadAsStringAsync();
				_logger.LogError($"{_logTag} Error calling HuggingFace API: {response.StatusCode} - {errorMessage}");
				throw new HttpRequestException($"Error calling HuggingFace API: {response.StatusCode} - {errorMessage}");
			}
		}
		catch (HttpRequestException ex)
		{
			_logger.LogError(ex, $"{_logTag} HttpRequestException while sending prompt to HuggingFace: {ex.Message}");
			throw new ApplicationException("HttpRequestException while sending prompt to HuggingFace.", ex);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"{_logTag} Unknown error while sending prompt to HuggingFace");
			throw;
		}
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

	private void ValidateHuggingFaceConfigs(Dictionary<string, string> configs)
	{
		if (configs == null || configs.Count == 0)
		{
			_logger.LogError($"{_logTag} ConfigValidationError: Nothing to validate. Empty list received. If you want to use HuggingFace models, please pass configuration dictionary for validation");
			throw new InvalidOperationException($"{_logTag} ConfigValidationError: Nothing to validate. Empty list received. If you want to use HuggingFace models, please pass configuration dictionary for validation");
		}

		foreach (var config in configs)
		{
			if (string.IsNullOrEmpty(config.Value))
			{
				_logger.LogError($"{_logTag} ConfigValidationError: {config.Key} is null or empty. Please check your appsettings.json file");
				throw new InvalidOperationException($"{_logTag} ConfigValidationError: {config.Key} is null or empty. Please check your appsettings.json file");
			}
			_logger.LogDebug($"{_logTag} Debug: {config.Key} passed with {config.Value}");
		}

		_logger.LogInformation($"{_logTag} Configuration Validation passed.");
	}
}