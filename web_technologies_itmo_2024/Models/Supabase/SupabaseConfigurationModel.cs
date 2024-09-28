namespace web_technologies_itmo_2024.Models.Supabase;

public class SupabaseConfigurationModel
{
	public string ApiUrl { get; set; }

	private string _sqlQueryEndpoint;
	public string SqlQueryEndpoint
	{
		get
		{
			if (!_isEndpointInited)
			{
				_sqlQueryEndpoint = _sqlQueryEndpoint.Replace("%PROJECT_REF%", ProjectRef);
				_isEndpointInited = true;
			}

			return _sqlQueryEndpoint;
		}
		set
		{
			if (_isEndpointInited)
				return;
			_sqlQueryEndpoint = value;
		}
	}

	public string ProjectRef
	{
		get => _projectRef;
		set
		{
			_projectRef = value;
			if (!string.IsNullOrEmpty(_sqlQueryEndpoint))
			{
				_sqlQueryEndpoint = _sqlQueryEndpoint.Replace("%PROJECT_REF%", value);
				_isEndpointInited = true;
			}
		}
	}

	public string AuthToken { get; set; }
	public SupabaseConnectionStringModel DbConnectionStringConfig { get; set; }


	private bool _isEndpointInited;
	private string _projectRef;
}