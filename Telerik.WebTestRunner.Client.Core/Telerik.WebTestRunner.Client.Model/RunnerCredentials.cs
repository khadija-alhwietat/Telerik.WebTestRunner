namespace Telerik.WebTestRunner.Client.Model
{
	public class RunnerCredentials
	{
		private string username;

		private string password;

		private string provider;

		public string UserName
		{
			get
			{
				return username;
			}
			set
			{
				username = value;
			}
		}

		public string Password
		{
			get
			{
				return password;
			}
			set
			{
				password = value;
			}
		}

		public string Provider
		{
			get
			{
				return provider;
			}
			set
			{
				provider = value;
			}
		}

		public RunnerCredentials(string username, string password)
			: this(username, password, string.Empty)
		{
		}

		public RunnerCredentials(string username, string password, string provider)
		{
			UserName = username;
			Password = password;
			Provider = provider;
		}

		public RunnerCredentials(RunnerCredentials credentials)
		{
			UserName = credentials.UserName;
			Password = credentials.Password;
			Provider = credentials.Provider;
		}

		public RunnerCredentials()
		{
		}
	}
}
