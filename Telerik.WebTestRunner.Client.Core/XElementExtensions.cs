using System.Xml;
using System.Xml.Linq;

public static class XElementExtensions
{
	public static XmlElement ToXmlElement(this XElement element)
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.Load(element.CreateReader());
		return xmlDocument.DocumentElement;
	}
}
