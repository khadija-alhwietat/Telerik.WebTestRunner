using System.Runtime.Serialization;

namespace Telerik.WebTestRunner.Model
{
	[DataContract(Name = "ItemContext")]
	public class WcfItemContext<T>
	{
		private T item;

		[DataMember]
		public virtual T Item
		{
			get
			{
				return item;
			}
			set
			{
				item = value;
			}
		}
	}
}
