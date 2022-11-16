using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Telerik.WebTestRunner.Model
{
	[DataContract]
	public class WcfCollectionContext<T>
	{
		private IEnumerable<T> items;

		private static WcfCollectionContext<T> empty;

		[DataMember]
		public int TotalCount
		{
			get;
			set;
		}

		[DataMember]
		public bool IsGeneric
		{
			get;
			set;
		}

		[DataMember]
		public IDictionary<string, string> Context
		{
			get;
			set;
		}

		[DataMember]
		public IEnumerable<T> Items
		{
			get
			{
				if (items == null)
				{
					items = new Collection<T>();
				}
				return items;
			}
			set
			{
				items = value;
			}
		}

		public static WcfCollectionContext<T> Empty
		{
			get
			{
				if (empty == null)
				{
					empty = new WcfCollectionContext<T>(new T[0])
					{
						TotalCount = 0
					};
				}
				return empty;
			}
		}

		public WcfCollectionContext()
		{
		}

		public WcfCollectionContext(IEnumerable<T> items)
		{
			this.items = items;
		}
	}
}
