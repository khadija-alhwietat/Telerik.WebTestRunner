using System;
using System.Runtime.Serialization;

namespace Telerik.WebTestRunner.Model
{
	[DataContract(Name = "LocalizationSettingsModel")]
	public class WcfLocalizationSettingsModel
	{
		[DataMember]
		public WcfCultureViewModel[] Cultures
		{
			get;
			set;
		}

		[DataMember]
		public WcfCultureViewModel[] BackendCultures
		{
			get;
			set;
		}

		[DataMember]
		public string MonolingualCulture
		{
			get;
			set;
		}

		[DataMember]
		public string DefaultLocalizationStrategy
		{
			get;
			set;
		}

		[DataMember]
		[Obsolete("Use DefaultStrategySettings")]
		public WcfCultureSettingViewModel[] SubdomainStrategySettings
		{
			get;
			set;
		}

		[DataMember]
		public WcfCultureSettingViewModel[] DefaultStrategySettings
		{
			get;
			set;
		}
	}
}
