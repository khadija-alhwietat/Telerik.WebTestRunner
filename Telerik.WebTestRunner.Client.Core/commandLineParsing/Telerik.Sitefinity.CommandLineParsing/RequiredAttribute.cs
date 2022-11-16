using System;

namespace Telerik.Sitefinity.CommandLineParsing
{
	[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public sealed class RequiredAttribute : Attribute
	{
	}
}
