using System.Data;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using web_technologies_itmo_2024.Models.Supabase;

namespace web_technologies_itmo_2024.Services;

public class SupabaseService
{
	private const string _logTag = $"[{nameof(SupabaseService)}]";

	private readonly ILogger<SupabaseService> _logger;
	private readonly SupabaseConfigurationModel _config;
	private readonly DatabaseQueryService _databaseQueryService;
	private readonly HttpClient _httpClient;

	public SupabaseService(
		ILogger<SupabaseService> logger,
		SupabaseConfigurationModel config, 
		DatabaseQueryService databaseQueryService)
	{
		_config = config;
		_databaseQueryService = databaseQueryService;
		_logger = logger;

		ValidateSupabaseConfigs(new Dictionary<string, string> 
		{
			{ nameof(_config.ApiUrl), _config.ApiUrl },
			{ nameof(_config.AuthToken), _config.AuthToken },
			{ nameof(_config.SqlQueryEndpoint), _config.SqlQueryEndpoint },
			{ nameof(_config.ProjectRef), _config.ProjectRef },
			{ nameof(_config.DbConnectionStringConfig.Host), _config.DbConnectionStringConfig.Host },
			{ nameof(_config.DbConnectionStringConfig.Database), _config.DbConnectionStringConfig.Database },
			{ nameof(_config.DbConnectionStringConfig.Port), _config.DbConnectionStringConfig.Port.ToString() },
			{ nameof(_config.DbConnectionStringConfig.User), _config.DbConnectionStringConfig.User },
			{ nameof(_config.DbConnectionStringConfig.Password), _config.DbConnectionStringConfig.Password }
		});

		_httpClient = new HttpClient
		{
			BaseAddress = new Uri(config.ApiUrl)
		};
		_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", config.AuthToken);
	}

	public async Task<T> ExecuteSqlQueryThroughConnectionString<T>(string query) where T : new()
	{
		try
		{
			var dataTable = await _databaseQueryService.ExecuteQueryAsync(query);

			foreach (DataRow row in dataTable.Rows)
			{
				_logger.LogDebug($"ID: {row["id"]}, Username: {row["username"]}");
			}

			// Assuming to return the first row as the object of type T
			if (dataTable.Rows.Count == 0)
			{
				throw new Exception($"{_logTag} No rows returned for the query.");
			}

			var dataRow = dataTable.Rows[0];
			T deserializedObject = DataRowExtensions.DataRowToObject<T>(dataRow);

			return deserializedObject;
		}
		catch (Exception ex)
		{
			throw new JsonException($"{_logTag} Error while trying to deserialize SQL query response." +
			                        $"\r\n{ex}");
		}
	}

	public async Task<T> ExecuteSqlQueryThroughApi<T>(string query)
	{
		var contentString = await ExecuteSqlQueryInternal(query);
		T deserializedObject;
		deserializedObject = JsonConvert.DeserializeObject<T>(contentString);

		if (deserializedObject is null)
		{
			throw new JsonException($"{_logTag} Error while trying to deserialize SQL query response." +
			                        $"\r\nResponse: {contentString}");
		}
		_logger.LogDebug($"{_logTag} Deserialized object {deserializedObject}");

		return deserializedObject;
	}

	private async Task<string> ExecuteSqlQueryInternal(string query)
	{
		var content = new JObject
		{
			{ "query", query }
		};

		var httpContent = new StringContent(content.ToString(), Encoding.UTF8, "application/json");

		_logger.LogInformation($"{_logTag} Attempting to send SQL query");
		_logger.LogDebug($"{_logTag} Query content: {content}");

		HttpResponseMessage response = null;
		try
		{
			response = await _httpClient.PostAsync(_config.SqlQueryEndpoint, httpContent);

			if (!response.IsSuccessStatusCode)
			{
				var errorContent = await response.Content.ReadAsStringAsync();
				_logger.LogError($"{_logTag} Error while trying to execute SQL query in supabase." +
				                 $"\r\nStatus Code: {response.StatusCode}" +
				                 $"\r\nResponse Content: {errorContent}");
				throw new HttpRequestException($"{_logTag} Error while trying to execute SQL query in supabase.");
			}

			var contentString = await response.Content.ReadAsStringAsync();
			_logger.LogInformation($"{_logTag} Successfully executed SQL query!");
			_logger.LogDebug($"{_logTag} Response content: {contentString}");

			return contentString;
		}
		catch (HttpRequestException ex)
		{
			_logger.LogError($"{_logTag} HttpRequestException: {ex.Message}");
			throw;
		}
		catch (Exception ex)
		{
			_logger.LogError($"{_logTag} An unexpected exception occurred: {ex}");
			throw;
		}
		finally
		{
			response?.Dispose();
		}
	}

	private void ValidateSupabaseConfigs(Dictionary<string, string> configs)
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

		if (!configs[nameof(_config.SqlQueryEndpoint)].Contains(configs[nameof(_config.ProjectRef)]))
		{
			_logger.LogError($"{_logTag} ConfigValidationError: {configs[nameof(_config.SqlQueryEndpoint)]} has not project identifier. Check autoreplacement of environment variables.");
			throw new InvalidOperationException($"{_logTag} ConfigValidationError: {configs[nameof(_config.SqlQueryEndpoint)]} has not project identifier. Check autoreplacement of environment variables.");
		}

		_logger.LogInformation($"{_logTag} Configuration Validation passed.");
	}
}