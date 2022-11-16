using System;
using System.Runtime.Serialization;

namespace Telerik.WebTestRunner.Model
{
	[Serializable]
	[DataContract]
	public class WcfTestInfoArray
	{
		[DataMember]
		public WcfTestInfo[] Tests
		{
			get;
			set;
		}

		public WcfTestInfoArray()
		{
		}

		public WcfTestInfoArray(WcfTestInfo[] testArray)
		{
			Tests = testArray;
		}
	}
}
