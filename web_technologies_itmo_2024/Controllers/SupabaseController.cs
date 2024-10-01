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
	public async Task<IActionResult> RegisterUser([FromBody] SupabaseUserModel userModel)
	{
		_logger.LogInformation($"{_logTag} A register-user received!");

		var cachedUserModelPassword = userModel.Password;

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

		List<SupabaseAuthorizationResponseModel> response;
		try
		{
			response = await _supabaseService.ExecuteSqlQueryThroughApi<List<SupabaseAuthorizationResponseModel>>(queryString);
		}
		catch (Exception ex)
		{
			_logger.LogError($"{_logTag} Internal server error." +
			                 $"\r\n{ex}");
			return StatusCode(500, $"Internal server error.");
		}

		var newUserModel = new SupabaseUserModel
		{
			Username = userModel.Username,
			Password = cachedUserModelPassword
		};

		var checkResponse = await CheckUser(newUserModel);

		if (checkResponse is not OkObjectResult okResult)
		{
			return Unauthorized("User authorization failed");
		}

		if (okResult.Value is not SupabaseAuthorizationPublicModel validPublicResponse)
		{
			return Unauthorized("User authorization failed");
		}

		return Ok(validPublicResponse);
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

	[HttpPost("send-message")]
	public async Task<IActionResult> SendMessage([FromBody] SupabaseMessageSendModel supabaseMessageSendModel)
	{
		_logger.LogInformation($"{_logTag} A send-message received!");

		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}

		SupabaseUserModel userModel = new SupabaseUserModel()
		{
			Username = supabaseMessageSendModel.Username,
			Password = supabaseMessageSendModel.Password
		};

		var firstResponse = await CheckUser(userModel);

		if (firstResponse is not OkObjectResult okResult)
		{
			return Unauthorized("User authorization failed");
		}

		if (okResult.Value is not SupabaseAuthorizationPublicModel validPublicResponse)
		{
			return Unauthorized("User authorization failed");
		}

		var isReceiverExist = await CheckIfUsernameExists(supabaseMessageSendModel.Message.To);

		if (isReceiverExist is not OkObjectResult receiverOkResult)
		{
			return NotFound("Receiver doesn't exist");
		}

		if (receiverOkResult.Value is not SupabaseAuthorizationPublicModel validReceiverPublicResponse)
		{
			return NotFound("Receiver doesn't exist");
		}

		string insertIntoMessagesQueryString = $"INSERT INTO messages (\"from\", \"to\", \"text\") " +
		                                       $"VALUES ({validPublicResponse.Id}, {validReceiverPublicResponse.Id}, '{supabaseMessageSendModel.Message.Text}');";

		SqlValidatorService sqlValidator = new SqlValidatorService();
		var errors = sqlValidator.GetValidationErrors(insertIntoMessagesQueryString);

		if (errors.Count == 0)
		{
			_logger.LogInformation($"{_logTag} Query: \"{insertIntoMessagesQueryString}\" is valid.");
		}
		else
		{
			_logger.LogError($"{_logTag} Query: \"{insertIntoMessagesQueryString}\" has errors.");
			foreach (var error in errors)
			{
				_logger.LogDebug($"\t{error}");
			}
			return BadRequest("Your SQL query string is invalid.");
		}

		List<string> responsesCreated;
		try
		{
			responsesCreated = await _supabaseService.ExecuteSqlQueryThroughApi<List<string>>(insertIntoMessagesQueryString);
		}
		catch (Exception ex)
		{
			_logger.LogError($"{_logTag} Internal server error." +
			                 $"\r\n{ex}");
			return StatusCode(500, $"Internal server error.");
		}

		string checkResultQueryString = $"SELECT \"id\" " +
		                                $"FROM messages " +
		                                $"WHERE \"from\" = {validPublicResponse.Id} " +
		                                $"ORDER BY \"created_at\" DESC " +
		                                $"LIMIT 1;";

		SqlValidatorService sqlNewValidator = new SqlValidatorService();
		var newErrors = sqlNewValidator.GetValidationErrors(checkResultQueryString);

		if (newErrors.Count == 0)
		{
			_logger.LogInformation($"{_logTag} Query: \"{checkResultQueryString}\" is valid.");
		}
		else
		{
			_logger.LogError($"{_logTag} Query: \"{checkResultQueryString}\" has errors.");
			foreach (var error in newErrors)
			{
				_logger.LogDebug($"\t{error}");
			}
			return BadRequest("Your SQL query string is invalid.");
		}

		List<SupabaseMessageSendResponseModel> responses;
		try
		{
			responses = await _supabaseService.ExecuteSqlQueryThroughApi<List<SupabaseMessageSendResponseModel>>(checkResultQueryString);
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
				return StatusCode(500,"No updates received");
			case > 1:
				return StatusCode(500, $"There are more than one message returned {responses[0].MessageId}");
		}

		SupabaseMessageSendEndpointModel endpointModel = new SupabaseMessageSendEndpointModel
		{
			MessageId = responses[0].MessageId
		};

		if (endpointModel.MessageId == 0)
		{
			return StatusCode(500, $"No updates received");
		}

		return Ok(endpointModel);
	}

	private async Task<IActionResult> CheckIfUsernameExists(string username)
	{
		string checkReceiverQueryString = $"SELECT id, username " +
		                                  $"FROM users " +
		                                  $"WHERE username = '{username}';";

		SqlValidatorService sqlValidator = new SqlValidatorService();
		var errors = sqlValidator.GetValidationErrors(checkReceiverQueryString);

		if (errors.Count == 0)
		{
			_logger.LogInformation($"{_logTag} Query: \"{checkReceiverQueryString}\" is valid.");
		}
		else
		{
			_logger.LogError($"{_logTag} Query: \"{checkReceiverQueryString}\" has errors.");
			foreach (var error in errors)
			{
				_logger.LogDebug($"\t{error}");
			}
			return BadRequest("Your SQL query string is invalid.");
		}

		List<SupabaseAuthorizationPublicModel> responses;
		try
		{
			responses = await _supabaseService.ExecuteSqlQueryThroughApi<List<SupabaseAuthorizationPublicModel>>(checkReceiverQueryString);
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

		return Ok(responses[0]);
	}
}