using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace Telerik.Sitefinity.CommandLineParsing
{
	[Serializable]
	public class CommandLineDictionary : Dictionary<string, string>
	{
		public char KeyCharacter
		{
			get;
			set;
		}

		public char ValueCharacter
		{
			get;
			set;
		}

		public CommandLineDictionary()
			: base((IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase)
		{
			KeyCharacter = '/';
			ValueCharacter = '=';
		}

		protected CommandLineDictionary(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public static CommandLineDictionary FromArguments(IEnumerable<string> arguments)
		{
			return FromArguments(arguments, '/', '=');
		}

		public static CommandLineDictionary FromArguments(IEnumerable<string> arguments, char keyCharacter, char valueCharacter)
		{
			CommandLineDictionary commandLineDictionary = new CommandLineDictionary();
			commandLineDictionary.KeyCharacter = keyCharacter;
			commandLineDictionary.ValueCharacter = valueCharacter;
			foreach (string argument in arguments)
			{
				commandLineDictionary.AddArgument(argument);
			}
			return commandLineDictionary;
		}

		public override string ToString()
		{
			string text = string.Empty;
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, string> current = enumerator.Current;
					text = (string.IsNullOrEmpty(current.Value) ? (text + string.Format(CultureInfo.InvariantCulture, "{0}{1} ", new object[2]
					{
						KeyCharacter,
						current.Key
					})) : (text + string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3} ", KeyCharacter, current.Key, ValueCharacter, current.Value)));
				}
			}
			return text.TrimEnd();
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		private void AddArgument(string argument)
		{
			if (argument == null)
			{
				throw new ArgumentNullException("argument");
			}
			if (argument.StartsWith(KeyCharacter.ToString(), StringComparison.OrdinalIgnoreCase))
			{
				string[] array = argument.Substring(1).Split(ValueCharacter);
				string key = array[0];
				string value = (array.Length <= 1) ? string.Empty : string.Join("=", array, 1, array.Length - 1);
				Add(key, value);
				return;
			}
			throw new ArgumentException("Unsupported value line argument format.", argument);
		}
	}
}
