namespace Telerik.Sitefinity.HttpClientCore.HttpClients
{
	public class Credentials
	{
		public string Email
		{
			get;
			set;
		}

		public string Password
		{
			get;
			set;
		}

		public string MembershipProvider
		{
			get;
			set;
		}

		public Credentials(string email, string password, string membershipProvider)
		{
			Email = email;
			Password = password;
			MembershipProvider = membershipProvider;
		}
	}
}
