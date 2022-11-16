using System;

namespace Telerik.WebTestRunner.Model
{
	[Serializable]
	public enum WcfAuthenticationMode
	{
		Default,
		Claims,
		Tfis,
		OpenId
	}
}
