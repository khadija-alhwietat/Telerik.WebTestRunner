using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Telerik.Sitefinity.CommandLineParsing
{
	public static class CommandLineParser
	{
		static CommandLineParser()
		{
			TypeDescriptor.AddAttributes(typeof(DirectoryInfo), new TypeConverterAttribute(typeof(DirectoryInfoConverter)));
			TypeDescriptor.AddAttributes(typeof(FileInfo), new TypeConverterAttribute(typeof(FileInfoConverter)));
		}

		public static void ParseArguments(this object valueToPopulate, IEnumerable<string> args)
		{
			CommandLineDictionary commandLineDictionary = CommandLineDictionary.FromArguments(args);
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(valueToPopulate);
			foreach (PropertyDescriptor item in properties)
			{
				if (item.Attributes.Cast<Attribute>().Any((Attribute attribute) => attribute is RequiredAttribute) && !commandLineDictionary.ContainsKey(item.Name))
				{
					throw new ArgumentException("A value for the " + item.Name + " property is required.");
				}
			}
			foreach (KeyValuePair<string, string> item2 in commandLineDictionary)
			{
				PropertyDescriptor propertyDescriptor = MatchProperty(item2.Key, properties, valueToPopulate.GetType());
				if (string.IsNullOrEmpty(item2.Value) && (propertyDescriptor.PropertyType == typeof(bool) || propertyDescriptor.PropertyType == typeof(bool?)))
				{
					propertyDescriptor.SetValue(valueToPopulate, true);
				}
				else
				{
					object value;
					switch (propertyDescriptor.PropertyType.Name)
					{
					case "IEnumerable`1":
					case "ICollection`1":
					case "IList`1":
					case "List`1":
					{
						MethodInfo method = typeof(CommandLineParser).GetMethod("FromCommaSeparatedList", BindingFlags.Static | BindingFlags.NonPublic);
						Type[] genericArguments = propertyDescriptor.PropertyType.GetGenericArguments();
						MethodInfo methodInfo = method.MakeGenericMethod(genericArguments);
						value = methodInfo.Invoke(null, new object[1]
						{
							item2.Value
						});
						break;
					}
					default:
					{
						TypeConverter converter = TypeDescriptor.GetConverter(propertyDescriptor.PropertyType);
						if (!(converter?.CanConvertFrom(typeof(string)) ?? false))
						{
							throw new ArgumentException("Unable to convert from a string to a property of type " + propertyDescriptor.PropertyType + ".");
						}
						value = converter.ConvertFromInvariantString(item2.Value);
						break;
					}
					}
					propertyDescriptor.SetValue(valueToPopulate, value);
				}
			}
		}

		private static PropertyDescriptor MatchProperty(string keyName, PropertyDescriptorCollection properties, Type targetType)
		{
			foreach (PropertyDescriptor property in properties)
			{
				if (property.Name.Equals(keyName, StringComparison.OrdinalIgnoreCase))
				{
					return property;
				}
			}
			throw new ArgumentException("A matching public property of name " + keyName + " on type " + targetType + " could not be found.");
		}

		public static void PrintUsage(object component)
		{
			IEnumerable<PropertyDescriptor> source = TypeDescriptor.GetProperties(component).Cast<PropertyDescriptor>();
			IEnumerable<string> names = source.Select((PropertyDescriptor property) => property.Name);
			IEnumerable<string> descriptions = source.Select((PropertyDescriptor property) => property.Description);
			IEnumerable<string> enumerable = FormatNamesAndDescriptions(names, descriptions, Console.WindowWidth);
			Console.WriteLine("Possible arguments:");
			foreach (string item in enumerable)
			{
				Console.WriteLine(item);
			}
		}

		public static void PrintCommands(IEnumerable<Command> commands)
		{
			IEnumerable<string> names = commands.Select((Command command) => command.Name);
			IEnumerable<string> descriptions = commands.Select((Command command) => command.GetAttribute<DescriptionAttribute>().Description);
			IEnumerable<string> enumerable = FormatNamesAndDescriptions(names, descriptions, Console.WindowWidth);
			Console.WriteLine("Possible commands:");
			foreach (string item in enumerable)
			{
				Console.WriteLine(item);
			}
		}

		public static string ToString(object valueToConvert)
		{
			IEnumerable<PropertyDescriptor> first = TypeDescriptor.GetProperties(valueToConvert).Cast<PropertyDescriptor>();
			IEnumerable<PropertyDescriptor> second = TypeDescriptor.GetProperties(valueToConvert.GetType().BaseType).Cast<PropertyDescriptor>();
			first = first.Except(second);
			CommandLineDictionary commandLineDictionary = new CommandLineDictionary();
			foreach (PropertyDescriptor item in first)
			{
				commandLineDictionary[item.Name] = item.GetValue(valueToConvert).ToString();
			}
			return commandLineDictionary.ToString();
		}

		private static IEnumerable<string> FormatNamesAndDescriptions(IEnumerable<string> names, IEnumerable<string> descriptions, int maxLineLength)
		{
			if (names.Count() != descriptions.Count())
			{
				throw new ArgumentException("Collection sizes are not equal", "names");
			}
			int num = names.Max((string commandName) => commandName.Length);
			List<string> list = new List<string>();
			for (int i = 0; i < names.Count(); i++)
			{
				string text = names.ElementAt(i);
				text = text.PadRight(num + 2);
				foreach (string item in WordWrap(descriptions.ElementAt(i), maxLineLength - num - 3))
				{
					text += item;
					list.Add(text);
					text = new string(' ', num + 2);
				}
			}
			return list;
		}

		private static List<T> FromCommaSeparatedList<T>(this string commaSeparatedList)
		{
			List<T> list = new List<T>();
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
			if (converter.CanConvertFrom(typeof(string)))
			{
				StringBuilder stringBuilder = new StringBuilder();
				bool flag = false;
				foreach (char c in commaSeparatedList)
				{
					if (flag)
					{
						stringBuilder.Append(c);
						flag = false;
					}
					else if (c == '\\' && !flag)
					{
						flag = true;
					}
					else if (c == ',' && !flag)
					{
						list.Add((T)converter.ConvertFromInvariantString(stringBuilder.ToString()));
						stringBuilder.Length = 0;
					}
					else
					{
						stringBuilder.Append(c);
					}
				}
				if (stringBuilder.Length > 0 || list.Count > 0)
				{
					list.Add((T)converter.ConvertFromInvariantString(stringBuilder.ToString()));
				}
			}
			return list;
		}

		private static T GetAttribute<T>(this object value) where T : Attribute
		{
			IEnumerable<Attribute> source = TypeDescriptor.GetAttributes(value).Cast<Attribute>();
			return (T)source.First((Attribute attribute) => attribute is T);
		}

		private static IEnumerable<string> WordWrap(string text, int maxLineLength)
		{
			List<string> list = new List<string>();
			string text2 = string.Empty;
			string[] array = text.Split(' ');
			foreach (string text3 in array)
			{
				if (text2.Length + text3.Length > maxLineLength)
				{
					list.Add(text2);
					text2 = string.Empty;
				}
				text2 += text3;
				if (text2.Length != maxLineLength)
				{
					text2 += " ";
				}
			}
			if (text2.Trim() != string.Empty)
			{
				list.Add(text2);
			}
			return list;
		}
	}
}
