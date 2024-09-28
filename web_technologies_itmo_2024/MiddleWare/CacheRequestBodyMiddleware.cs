namespace web_technologies_itmo_2024.MiddleWare;

public class CacheRequestBodyMiddleware
{
	private readonly RequestDelegate _next;

	public CacheRequestBodyMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		context.Request.EnableBuffering();

		using (var reader = new StreamReader(context.Request.Body, leaveOpen: true))
		{
			var body = await reader.ReadToEndAsync();

			context.Items["CachedRequestBody"] = body;

			context.Request.Body.Position = 0;
		}

		await _next(context);
	}
}