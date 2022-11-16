using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace Telerik.Sitefinity.CommandLineParsing
{
	public class DirectoryInfoConverter : TypeConverter
	{
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string && value != null)
			{
				return new DirectoryInfo((string)value);
			}
			return null;
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string);
		}
	}
}
