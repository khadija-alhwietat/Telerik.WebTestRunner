using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace Telerik.Sitefinity.CommandLineParsing
{
	public class FileInfoConverter : TypeConverter
	{
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string && value != null)
			{
				return new FileInfo((string)value);
			}
			return null;
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (value is FileInfo && destinationType == typeof(string))
			{
				return ((FileInfo)value).FullName;
			}
			return null;
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(string);
		}
	}
}
