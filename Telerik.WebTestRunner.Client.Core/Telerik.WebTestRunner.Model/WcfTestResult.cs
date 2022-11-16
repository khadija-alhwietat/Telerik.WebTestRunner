using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace Telerik.WebTestRunner.Model
{
	[Serializable]
	[DataContract]
	public class WcfTestResult
	{
		private string authorName;

		private string fixtureName;

		private string assemblyName;

		private string testMethodName;

		[DataMember]
		public string FixtureName
		{
			get
			{
				return fixtureName;
			}
			set
			{
				fixtureName = value;
			}
		}

		[DataMember]
		public string TestMethodName
		{
			get
			{
				return testMethodName;
			}
			set
			{
				testMethodName = value;
			}
		}

		[DataMember]
		public string AssemblyName
		{
			get
			{
				return assemblyName;
			}
			set
			{
				assemblyName = value;
			}
		}

		[DataMember]
		public string AuthorName
		{
			get
			{
				return authorName;
			}
			set
			{
				authorName = value;
			}
		}

		[DataMember]
		public Guid Id
		{
			get;
			set;
		}

		[DataMember]
		public string TestFilePath
		{
			get;
			set;
		}

		[DataMember]
		public WcfTestResultEnum Result
		{
			get;
			set;
		}

		[DataMember]
		public string Message
		{
			get;
			set;
		}

		[DataMember]
		[XmlElement(DataType = "dateTime", ElementName = "StartTime")]
		public DateTime StartTime
		{
			get;
			set;
		}

		[DataMember]
		[XmlElement(DataType = "dateTime", ElementName = "EndTime")]
		public DateTime EndTime
		{
			get;
			set;
		}

		[DataMember]
		[XmlIgnore]
		public TimeSpan TestDuration
		{
			get;
			set;
		}

		[Browsable(false)]
		[XmlElement(DataType = "duration", ElementName = "TestDuration")]
		public string TestDurationString
		{
			get
			{
				return XmlConvert.ToString(TestDuration);
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					TestDuration = TimeSpan.Zero;
				}
				else
				{
					TestDuration = XmlConvert.ToTimeSpan(value);
				}
			}
		}
	}
}
