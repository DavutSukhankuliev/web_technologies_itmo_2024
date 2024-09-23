using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace web_technologies_itmo_2024.HuggingFaceController;

[ApiController]
[Route("generate_text")]
public class HuggingFaceController : ControllerBase
{
	private const string _logTag = $"[{nameof(HuggingFaceController)}]";

	private readonly HttpClient _httpClient;
	private readonly TelegramSenderService _telegramSenderService;
	private readonly ILogger<HuggingFaceController> _logger;

	private readonly string _modelApi;
	private readonly string _apiKey;

	public HuggingFaceController(HttpClient httpClient, TelegramSenderService telegramSenderService, ILogger<HuggingFaceController> logger, IConfiguration configuration)
	{
		_httpClient = httpClient;
		_telegramSenderService = telegramSenderService;
		_logger = logger;
		var localConfiguration = configuration;

		_modelApi = localConfiguration["HuggingFace:ApiUrl"] + localConfiguration["HuggingFace:ModelEndpoint"];
		_apiKey = localConfiguration["HuggingFace:AuthToken"];

		if (string.IsNullOrEmpty(_modelApi) || string.IsNullOrEmpty(_apiKey))
		{
			_logger.LogError($"{_logTag} Your HuggingFace configuration is null or empty. Check your appsettings.json");
			throw new InvalidOperationException($"{_logTag} Your HuggingFace configuration is null or empty. Check your appsettings.json");
		}

		_logger.LogInformation($"{_logTag} HuggingFaceController initialized with Model API: {_modelApi}");
	}

	[HttpPost]
	public async Task<IActionResult> GetQuestion([FromBody] PromptRequestModel request)
	{
		if (string.IsNullOrEmpty(request.Prompt))
		{
			_logger.LogWarning($"{_logTag} Received empty or null prompt.");
			return BadRequest("Parameter 'prompt' is required");
		}

		try
		{
			_logger.LogInformation($"{_logTag} Sending prompt to HuggingFace API.");
			var apiResult = await TrySendPromptAsync(request.Prompt);
			_logger.LogInformation($"{_logTag} Received API result.");

			_logger.LogInformation($"{_logTag} Parsing API result.");
			var answer = ParseApiResult(apiResult);

			_logger.LogInformation($"{_logTag} Preparing message to send.");
			var message = PrepareMessageToSend(request.Prompt, answer);

			_logger.LogInformation($"{_logTag} Sending message.");
			return await TrySendMessageAsync(message);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"{_logTag} Error occurred while processing the request.");
			return StatusCode(500, "Internal server error");
		}
	}

	private async Task<string> TrySendPromptAsync(string prompt)
	{
		var payload = new
		{
			inputs = prompt
		};

		var stringContent = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

		var request = new HttpRequestMessage(HttpMethod.Post, _modelApi)
		{
			Content = stringContent
		};

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

	private string PrepareMessageToSend(string prompt, string answer)
	{
		var message = $"Prompt: {prompt}" +
		              $"\r\nAnswer: {answer}";
		_logger.LogInformation($"{_logTag} Prepared message to send: {message}");
		return message;
	}

	private string ParseApiResult(string apiResult)
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
			_logger.LogInformation($"{_logTag} Parsed first result: {firstResult}");
			return firstResult;
		}
		catch (JsonException ex)
		{
			_logger.LogError(ex, $"{_logTag} Error parsing API result: {ex.Message}");
			throw new ApplicationException("Error parsing API result.", ex);
		}
	}

	private async Task<IActionResult> TrySendMessageAsync(string textToSend)
	{
		try
		{
			_logger.LogInformation($"{_logTag} Attempting to send message to Telegram.");

			var response = await _telegramSenderService.SendMessageAsync(textToSend);

			if (response.IsSuccessStatusCode)
			{
				var apiResult = await response.Content.ReadAsStringAsync();
				_logger.LogInformation($"{_logTag} Message successfully sent to Telegram.");
				return Ok(apiResult);
			}
			else
			{
				var errorMessage = await response.Content.ReadAsStringAsync();
				_logger.LogWarning($"{_logTag} Error calling Telegram API: {response.StatusCode} - {errorMessage}");
				return StatusCode((int)response.StatusCode, $"Error calling Telegram API: {errorMessage}");
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"{_logTag} Ошибка при отправке сообщения в Telegram: {ex.Message}");
			return StatusCode(500, "Internal Server Error");
		}
	}
}