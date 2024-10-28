using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using web_technologies_itmo_2024;
using web_technologies_itmo_2024.MiddleWare;
using web_technologies_itmo_2024.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

builder.Services.AddSingleton<TelegramSenderService>(provider =>
{
	var configuration = provider.GetRequiredService<IConfiguration>();
	var logger = provider.GetRequiredService<ILogger<TelegramSenderService>>();
	var botKey = Environment.GetEnvironmentVariable("TELEGRAM_BOT_KEY");
	var chatId = Environment.GetEnvironmentVariable("TELEGRAM_CHAT_ID");

	if (string.IsNullOrEmpty(botKey))
		throw new InvalidOperationException("TelegramBotKey is missing");
	if (string.IsNullOrEmpty(chatId))
		throw new InvalidOperationException("TelegramChatId is missing");

	return new TelegramSenderService(provider.GetRequiredService<HttpClient>(), logger, botKey, chatId);
});

builder.Services.AddSingleton<HuggingFaceService>();
builder.Services.AddSingleton<HuggingFaceApiResultParser>();
builder.Services.AddSingleton<TelegramBotCommandHandlerService>();
builder.Services.AddSingleton<TelegramBotCommandParserService>();
builder.Services.AddSingleton<TelegramServiceWrapper>();
builder.Services.AddSingleton<TelegramUpdateService>();
builder.Services.AddSingleton<PasswordHasher<string>>();

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
	options.SerializerSettings.Converters.Add(new MessageModelJsonConverter());
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<CacheRequestBodyMiddleware>();

app.MapControllers();

// Configure the app to listen on the port provided by Cloud Run
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://*:{port}");

app.Run();