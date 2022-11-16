using System.Configuration;

namespace Telerik.WebTestRunner.Client.Configuration.Collections
{
	public abstract class ConfigurationElementCollection<K, V> : ConfigurationElementCollection where V : ConfigurationElement, new()
	{
		public abstract override ConfigurationElementCollectionType CollectionType
		{
			get;
		}

		public V this[K key] => (V)BaseGet(key);

		public V this[int index] => (V)BaseGet(index);

		protected abstract override string ElementName
		{
			get;
		}

		public ConfigurationElementCollection()
		{
		}

		public void Add(V value)
		{
			BaseAdd(value);
		}

		public void Remove(K key)
		{
			BaseRemove(key);
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new V();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return GetElementKey((V)element);
		}

		protected abstract K GetElementKey(V element);
	}
}
