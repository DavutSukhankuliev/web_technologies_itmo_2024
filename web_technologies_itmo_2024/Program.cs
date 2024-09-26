using web_technologies_itmo_2024;
using web_technologies_itmo_2024.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddScoped<TelegramSenderService>(provider =>
{
	var configuration = provider.GetRequiredService<IConfiguration>();
	var logger = provider.GetRequiredService<ILogger<TelegramSenderService>>();
	var botKey = configuration["TelegramBotKey"];
	var chatId = configuration["TelegramChatId"];

	if (string.IsNullOrEmpty(botKey))
		throw new InvalidOperationException("TelegramBotKey is missing");
	if (string.IsNullOrEmpty(chatId))
		throw new InvalidOperationException("TelegramChatId is missing");

	return new TelegramSenderService(provider.GetRequiredService<HttpClient>(), logger, botKey, chatId);
});

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
	options.SerializerSettings.Converters.Add(new MessageModelJsonConverter());
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapControllers();

app.Run();