using System.Runtime.Serialization;

namespace Telerik.WebTestRunner.Model
{
	[DataContract]
	public class WcfCredentials
	{
		[DataMember]
		public string UserName
		{
			get;
			set;
		}

		[DataMember]
		public string Password
		{
			get;
			set;
		}

		[DataMember]
		public string Provider
		{
			get;
			set;
		}
	}
}
