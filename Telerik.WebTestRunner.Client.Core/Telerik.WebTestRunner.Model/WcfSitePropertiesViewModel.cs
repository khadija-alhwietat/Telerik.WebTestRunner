using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Telerik.WebTestRunner.Model
{
	[DataContract]
	public class WcfSitePropertiesViewModel
	{
		[DataMember]
		public Guid Id
		{
			get;
			set;
		}

		[DataMember]
		public string Name
		{
			get;
			set;
		}

		[DataMember]
		public string StagingUrl
		{
			get;
			set;
		}

		[DataMember]
		public string LiveUrl
		{
			get;
			set;
		}

		[DataMember]
		public IList<WcfCultureViewModel> PublicContentCultures
		{
			get;
			set;
		}

		[DataMember]
		public bool IsOffline
		{
			get;
			set;
		}

		[DataMember]
		public IList<string> DomainAliases
		{
			get;
			set;
		}

		[DataMember]
		public bool RequiresSsl
		{
			get;
			set;
		}

		[DataMember]
		public Guid HomePageId
		{
			get;
			set;
		}

		[DataMember]
		public Guid FrontEndLoginPageId
		{
			get;
			set;
		}

		[DataMember]
		public string FrontEndLoginPageUrl
		{
			get;
			set;
		}

		[DataMember]
		public Guid SiteMapRootNodeId
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

		[DataMember]
		public string OfflineSiteMessage
		{
			get;
			set;
		}

		[DataMember]
		public Guid OfflinePageToRedirect
		{
			get;
			set;
		}

		[DataMember]
		public bool RedirectIfOffline
		{
			get;
			set;
		}

		[DataMember]
		public Guid SourcePagesSiteId
		{
			get;
			set;
		}

		[DataMember]
		public bool IsAllowedStartStop
		{
			get;
			set;
		}

		[DataMember]
		public bool IsCurrentSite
		{
			get;
			set;
		}
	}
}
