using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Telerik.WebTestRunner.Model
{
	[DataContract]
	public class WcfSiteViewModel : WcfSitePropertiesViewModel
	{
		[DataMember]
		public IList<WcfSiteDataSourceLinkViewModel> SiteDataSourceLinks
		{
			get;
			set;
		}
	}
}
