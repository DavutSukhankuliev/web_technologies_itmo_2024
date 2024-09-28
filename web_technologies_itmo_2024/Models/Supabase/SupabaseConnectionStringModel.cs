namespace web_technologies_itmo_2024.Models.Supabase;

public class SupabaseConnectionStringModel
{
	public string Host { get; set; }
	public string Database { get; set; }
	public int Port { get; set; }
	public string User { get; set; }
	public string Password { get; set; }

	public override string ToString()
	{
		return $"Host={Host};Port={Port};Username={User};Password={Password};Database={Database}";
	}

	public static SupabaseConnectionStringModel FromConnectionString(string connectionString)
	{
		var uri = new Uri(connectionString);
		var userInfo = uri.UserInfo.Split(':');
		return new SupabaseConnectionStringModel
		{
			Host = uri.Host,
			Database = uri.AbsolutePath.Trim('/'),
			Port = uri.Port,
			User = userInfo[0],
			Password = userInfo[1],
		};
	}
}