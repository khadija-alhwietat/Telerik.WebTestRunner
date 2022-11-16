namespace Telerik.Sitefinity.HttpClientCore.HttpClients
{
	public enum UserLoggingReason
	{
		Success,
		UserLimitReached,
		UserNotFound,
		UserLoggedFromDifferentIp,
		SessionExpired,
		UserLoggedOff,
		UserLoggedFromDifferentComputer,
		Unknown,
		NeedAdminRights,
		UserAlreadyLoggedIn,
		UserRevoked,
		SiteAccessNotAllowed
	}
}
