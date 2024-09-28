using System.Data;
using Npgsql;
using web_technologies_itmo_2024.Models.Supabase;

namespace web_technologies_itmo_2024.Services;

public class DatabaseQueryService
{
	private const string _logTag = $"[{nameof(DatabaseQueryService)}]";

	private readonly ILogger<DatabaseQueryService> _logger;
	private readonly SupabaseConfigurationModel _configuration;
	private readonly string _connectionString;

	public DatabaseQueryService(
		ILogger<DatabaseQueryService> logger,
		SupabaseConfigurationModel configuration
		)
	{
		_logger = logger;
		_configuration = configuration;

		_connectionString = _configuration.DbConnectionStringConfig.ToString();
	}

	public async Task<DataTable> ExecuteQueryAsync(string query)
	{
		_logger.LogInformation($"{_logTag} Attempting to send SQL query");
		_logger.LogDebug($"{_logTag} Query content: {query}");

		using(var connection = new NpgsqlConnection(_connectionString))
		{
			await connection.OpenAsync();

			using(var command = new NpgsqlCommand(query, connection))
			{
				using(var reader = await command.ExecuteReaderAsync())
				{
					var dataTable = new DataTable();
					dataTable.Load(reader);

					return dataTable;
				}
			}
		}
	}
}