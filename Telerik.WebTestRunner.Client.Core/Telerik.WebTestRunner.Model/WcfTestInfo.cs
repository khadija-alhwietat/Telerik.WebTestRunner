using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Telerik.WebTestRunner.Model
{
	[Serializable]
	[DataContract]
	public class WcfTestInfo
	{
		[DataMember]
		public string FixtureName
		{
			get;
			set;
		}

		[DataMember]
		public string TestMethodName
		{
			get;
			set;
		}

		[DataMember]
		public string AssemblyName
		{
			get;
			set;
		}

		[DataMember]
		public string AuthorName
		{
			get;
			set;
		}

		[DataMember]
		public bool IsToIgnore
		{
			get;
			set;
		}

		[DataMember]
		public bool IsMultilingual
		{
			get;
			set;
		}

		[DataMember]
		public string IdentificationKey
		{
			get;
			set;
		}

		[DataMember]
		public string MultilingualExecutionMode
		{
			get;
			set;
		}

		[DataMember]
		public Dictionary<string, string> Parameters
		{
			get;
			set;
		}

		[DataMember]
		public WcfAuthenticationMode AuthenticationMode
		{
			get;
			set;
		}

		[DataMember]
		public WcfCredentials Credentials
		{
			get;
			set;
		}

		[DataMember]
		public bool HasCustomAuthentication
		{
			get;
			set;
		}

		[DataMember]
		public string Area
		{
			get;
			set;
		}
	}
}
