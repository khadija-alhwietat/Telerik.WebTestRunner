using System.Linq;
using System.Xml.Serialization;

namespace System.Xml.Linq
{
	public static class XmlSerializerExtensions
	{
		public static XElement SerializeToXElement(this XmlSerializer serializer, object objToSerialize, bool includeNamespaces = false)
		{
			XDocument xDocument = new XDocument();
			using (XmlWriter xmlWriter = xDocument.CreateWriter())
			{
				serializer.Serialize(xmlWriter, objToSerialize);
			}
			XElement root = xDocument.Root;
			if (!includeNamespaces)
			{
				(from x in root.Attributes()
					where x.IsNamespaceDeclaration
					select x).Remove();
				(from x in root.Attributes()
					where x.Name.LocalName == "xmlns"
					select x).Remove();
			}
			root.Remove();
			return root;
		}
	}
}
