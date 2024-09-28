using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using web_technologies_itmo_2024.Models.Supabase;
using web_technologies_itmo_2024.Models.SupabaseResponseModels;
using web_technologies_itmo_2024.Services;

namespace web_technologies_itmo_2024.Controllers;

[ApiController]
[Route("api")]
public class SupabaseController : ControllerBase
{
	private const string _logTag = $"[{nameof(SupabaseController)}]";

	private readonly ILogger<SupabaseController> _logger;
	private readonly PasswordHasher<string> _passwordHasher;
	private readonly SupabaseService _supabaseService;

	public SupabaseController(
		ILogger<SupabaseController> logger, 
		PasswordHasher<string> passwordHasher, 
		SupabaseService supabaseService
		)
	{
		_logger = logger;
		_passwordHasher = passwordHasher;
		_supabaseService = supabaseService;
	}

	[HttpPost]
	[Route("register-user")]
	public async Task<IActionResult> Post([FromBody] SupabaseUserModel userModel)
	{
		_logger.LogInformation($"{_logTag} A register-user received!");

		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}

		userModel.Password = _passwordHasher.HashPassword(userModel.Username, userModel.Password);

		string queryString = $"INSERT INTO users (username, password) " +
		                     $"VALUES ('{userModel.Username}', '{userModel.Password}');";

		_logger.LogDebug($"{_logTag} {queryString}");

		SqlValidatorService sqlValidator = new SqlValidatorService();
		var errors = sqlValidator.GetValidationErrors(queryString);

		if (errors.Count == 0)
		{
			_logger.LogInformation($"{_logTag} Query: \"{queryString}\" is valid.");
		}
		else
		{
			_logger.LogError($"{_logTag} Query: \"{queryString}\" has errors.");
			foreach (var error in errors)
			{
				_logger.LogDebug($"\t{error}");
			}
			return BadRequest("Your SQL query string is invalid.");
		}

		SupabaseAuthorizationResponseModel response;
		try
		{
			response = await _supabaseService.ExecuteSqlQueryThroughApi<SupabaseAuthorizationResponseModel>(queryString);
		}
		catch (Exception ex)
		{
			_logger.LogError($"{_logTag} Internal server error." +
			                 $"\r\n{ex}");
			return StatusCode(500, $"Internal server error.");
		}

		SupabaseAuthorizationPublicModel publicResponse = new SupabaseAuthorizationPublicModel
		{
			Id = response.Id,
			Username = response.Username
		};
		return Ok(publicResponse);
	}

	[HttpPost("check-user")]
	public async Task<IActionResult> CheckUser([FromBody] SupabaseUserModel userModel)
	{
		_logger.LogInformation($"{_logTag} A check-user received!");

		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}

		string queryString = $"SELECT id, username, password " +
		                     $"FROM users " +
		                     $"WHERE username = '{userModel.Username}';";

		SqlValidatorService sqlValidator = new SqlValidatorService();
		var errors = sqlValidator.GetValidationErrors(queryString);

		if (errors.Count == 0)
		{
			_logger.LogInformation($"{_logTag} Query: \"{queryString}\" is valid.");
		}
		else
		{
			_logger.LogError($"{_logTag} Query: \"{queryString}\" has errors.");
			foreach (var error in errors)
			{
				_logger.LogDebug($"\t{error}");
			}
			return BadRequest("Your SQL query string is invalid.");
		}

		List<SupabaseAuthorizationResponseModel> responses;
		try
		{
			responses = await _supabaseService.ExecuteSqlQueryThroughApi<List<SupabaseAuthorizationResponseModel>>(queryString);
		}
		catch (Exception ex)
		{
			_logger.LogError($"{_logTag} Internal server error." +
			                 $"\r\n{ex}");
			return StatusCode(500, $"Internal server error.");
		}

		switch (responses.Count)
		{
			case 0:
				return Unauthorized("No such user");
			case > 1:
				return Unauthorized($"There are more than one user with the name {responses[0].Username}");
		}

		if (responses[0].Id == 0)
		{
			return Unauthorized("Invalid credentials");
		}

		var result = _passwordHasher.VerifyHashedPassword(responses[0].Username, responses[0].Password, userModel.Password);

		if (result == PasswordVerificationResult.Success)
		{
			SupabaseAuthorizationPublicModel publicResponse = new SupabaseAuthorizationPublicModel
			{
				Id = responses[0].Id,
				Username = responses[0].Username
			};
			return Ok(publicResponse);
		}

		return Unauthorized("Unhandled error");
	}
}