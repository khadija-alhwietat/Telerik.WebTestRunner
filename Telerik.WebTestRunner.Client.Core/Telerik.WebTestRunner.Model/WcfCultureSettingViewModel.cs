using System.Globalization;
using System.Runtime.Serialization;

namespace Telerik.WebTestRunner.Model
{
	[DataContract(Name = "CultureSettingViewModel")]
	public class WcfCultureSettingViewModel
	{
		private string displayName;

		[DataMember]
		public string Key
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
		public string Setting
		{
			get;
			set;
		}

		[DataMember]
		public string DisplayName
		{
			get
			{
				if (string.IsNullOrEmpty(displayName))
				{
					CultureInfo invariantCulture = CultureInfo.InvariantCulture;
					displayName = invariantCulture.EnglishName;
				}
				return displayName;
			}
			set
			{
				displayName = value;
			}
		}
	}
}
