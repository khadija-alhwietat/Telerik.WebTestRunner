using System.ComponentModel;
using System.Configuration;

namespace Telerik.WebTestRunner.Client.Configuration.Elements
{
	public class TeamMemberConfigElement : ConfigurationElement
	{
		[ConfigurationProperty("fullName", IsRequired = true, IsKey = true)]
		public string FullName
		{
			get
			{
				return (string)base["fullName"];
			}
			set
			{
				base["fullName"] = value;
			}
		}

		[ConfigurationProperty("aliases")]
		[TypeConverter(typeof(CommaDelimitedStringCollectionConverter))]
		public CommaDelimitedStringCollection Aliases => (CommaDelimitedStringCollection)base["aliases"];
	}
}
