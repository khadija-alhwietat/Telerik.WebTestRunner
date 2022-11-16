using System;
using System.Runtime.Serialization;

namespace Telerik.WebTestRunner.Model
{
	[DataContract]
	public class WcfSiteDataSourceLinkViewModel
	{
		[DataMember]
		public Guid Id
		{
			get;
			set;
		}

		[DataMember]
		public string ProviderName
		{
			get;
			set;
		}

		[DataMember]
		public string ProviderTitle
		{
			get;
			set;
		}

		[DataMember]
		public Guid SiteId
		{
			get;
			set;
		}

		[DataMember]
		public string DataSourceName
		{
			get;
			set;
		}

		[DataMember]
		public bool IsDefault
		{
			get;
			set;
		}
	}
}
