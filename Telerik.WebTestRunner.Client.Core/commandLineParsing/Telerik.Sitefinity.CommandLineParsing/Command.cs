using System;

namespace Telerik.Sitefinity.CommandLineParsing
{
	public abstract class Command
	{
		public virtual string Name
		{
			get
			{
				string name = GetType().Name;
				if (name.Contains("Command"))
				{
					return name.Remove(name.LastIndexOf("Command", StringComparison.Ordinal));
				}
				return name;
			}
		}

		public abstract void Execute();
	}
}
