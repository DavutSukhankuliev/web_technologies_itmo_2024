using web_technologies_itmo_2024;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddScoped<TelegramPhotoSenderService>(provider =>
{
	var configuration = provider.GetRequiredService<IConfiguration>();
	var logger = provider.GetRequiredService<ILogger<TelegramPhotoSenderService>>();
	var botKey = configuration["TelegramBotKey"];
	var chatId = configuration["TelegramChatId"];

	if (botKey == null)
		throw new InvalidOperationException("TelegramBotKey is missing");
	if (chatId == null)
		throw new InvalidOperationException("TelegramChatId is missing");

	return new TelegramPhotoSenderService(provider.GetRequiredService<HttpClient>(), logger, botKey, chatId);
});

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();