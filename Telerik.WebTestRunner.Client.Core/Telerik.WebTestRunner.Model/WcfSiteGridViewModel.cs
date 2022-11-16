using System;
using System.Runtime.Serialization;

namespace Telerik.WebTestRunner.Model
{
	[DataContract]
	public class WcfSiteGridViewModel
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
		public bool IsOffline
		{
			get;
			set;
		}

		[DataMember]
		public string UIStatus
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
		public bool IsDeleteable
		{
			get;
			set;
		}

		[DataMember]
		public bool IsAllowedSetPermissions
		{
			get;
			set;
		}

		[DataMember]
		public bool IsAllowedConfigureModules
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
		public bool IsAllowedCreateEdit
		{
			get;
			set;
		}

		[DataMember]
		public string[] CultureDisplayNames
		{
			get;
			set;
		}

		[DataMember]
		public string SiteUrl
		{
			get;
			set;
		}
	}
}
