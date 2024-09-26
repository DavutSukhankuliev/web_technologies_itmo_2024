using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using web_technologies_itmo_2024.Models.TelegramWebhookRespondModel;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace web_technologies_itmo_2024.Utils;

public class MessageModelJsonConverter : JsonConverter<BaseMessageModel>
{
	public override BaseMessageModel ReadJson(JsonReader reader, Type objectType, BaseMessageModel existingValue, bool hasExistingValue, JsonSerializer serializer)
	{
		var jsonObject = JObject.Load(reader);
		var chatType = jsonObject["chat"]?["type"]?.ToString();

		BaseMessageModel messageModel;

		switch (chatType)
		{
			case "private":
				messageModel = new MessagePersonalModel();
				break;
			case "group":
			case "supergroup":
				messageModel = new MessageGroupModel();
				break;
			default:
				throw new NotSupportedException($"Unsupported chat type: {chatType}");
		}

		serializer.Populate(jsonObject.CreateReader(), messageModel);
		return messageModel;
	}

	public override void WriteJson(JsonWriter writer, BaseMessageModel value, JsonSerializer serializer)
	{
		serializer.Serialize(writer, value, value.GetType());
	}
}