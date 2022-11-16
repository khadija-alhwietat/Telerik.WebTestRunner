using System.Globalization;
using System.Runtime.Serialization;

namespace Telerik.WebTestRunner.Model
{
	[DataContract(Name = "CultureViewModel")]
	public class WcfCultureViewModel
	{
		private string displayName;

		private string shortName;

		private string[] sitesNames = new string[0];

		private string[] sitesUsingCultureAsDefault = new string[0];

		[DataMember]
		public string Key
		{
			get;
			set;
		}

		[DataMember]
		public string Culture
		{
			get;
			set;
		}

		[DataMember]
		public string UICulture
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
					CultureInfo instanceCulture = GetInstanceCulture();
					displayName = instanceCulture.EnglishName;
				}
				return displayName;
			}
			set
			{
				displayName = value;
			}
		}

		[DataMember]
		public string ShortName
		{
			get
			{
				if (string.IsNullOrEmpty(shortName))
				{
					CultureInfo cultureInfo = CultureInfo.GetCultureInfo(Culture);
					shortName = cultureInfo.Name;
				}
				return shortName;
			}
			set
			{
				shortName = value;
			}
		}

		[DataMember]
		public bool IsDefault
		{
			get;
			set;
		}

		public bool ShowSpecificName
		{
			get;
			set;
		}

		[DataMember]
		public string[] SitesNames
		{
			get
			{
				return sitesNames;
			}
			set
			{
				sitesNames = value;
			}
		}

		[DataMember]
		public string[] SitesUsingCultureAsDefault
		{
			get
			{
				return sitesUsingCultureAsDefault;
			}
			set
			{
				sitesUsingCultureAsDefault = value;
			}
		}

		public WcfCultureViewModel(CultureInfo cultureInfo)
		{
			Key = $"{cultureInfo.EnglishName}-{cultureInfo.Name}".ToLowerInvariant();
			UICulture = cultureInfo.Name;
			Culture = cultureInfo.Name;
			ShortName = cultureInfo.Name;
		}

		private CultureInfo GetInstanceCulture()
		{
			string text = ShowSpecificName ? Culture : UICulture;
			if (!string.IsNullOrEmpty(text) && !(text.ToUpperInvariant() == "INVARIANT"))
			{
				return CultureInfo.GetCultureInfo(text);
			}
			return CultureInfo.InvariantCulture;
		}
	}
}
