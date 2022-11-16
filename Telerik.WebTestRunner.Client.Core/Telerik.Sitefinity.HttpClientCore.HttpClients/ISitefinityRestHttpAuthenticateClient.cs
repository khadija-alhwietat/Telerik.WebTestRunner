namespace Telerik.Sitefinity.HttpClientCore.HttpClients
{
	public interface ISitefinityRestHttpAuthenticateClient : ISitefinityRestHttpClient
	{
		UserLoggingReason AuthenticateToServer(string membershipProvider, string userName, string password, bool rememberMe = false);

		UserLoggingReason ForceAuthenticateToServer(string membershipProvider, string userName, string password, bool rememberMe = false);

		bool Logout();

		bool Logout(string membershipProvider, string userName, string password);
	}
}
