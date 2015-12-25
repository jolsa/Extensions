//	XmlExtensions: Created 11/22/2015 - Johnny Olsa

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.XPath;

namespace System.Xml.Linq
{
	/// <summary>
	/// Xml Extensions
	/// </summary>
	public static class XmlExtensions
	{
		/// <summary>
		/// Returns the IXmlLineInfo
		/// </summary>
		public static IXmlLineInfo GetLineInfo(this XObject value)
		{
			return (IXmlLineInfo)value;
		}
		/// <summary>
		/// Returns the level for the element
		/// </summary>
		public static int GetLevel(this XElement value)
		{
			int level = 0;
			var e = value;
			while (e.Parent != null)
			{
				level++;
				e = e.Parent;
			}
			return level;
		}
		/// <summary>
		/// Returns the XPath for the element
		/// </summary>
		public static string GetXPath(this XElement value)
		{
			var names = new List<string>() { value.Name.LocalName };
			var e = value;
			while (e.Parent != null)
			{
				names.Add(e.Parent.Name.LocalName);
				e = e.Parent;
			}
			names.Reverse();
			return '/' + string.Join("/", names);
		}
		/// <summary>
		/// Returns the XPath for the attribute
		/// </summary>
		public static string GetXPath(this XAttribute value)
		{
			return value.Parent.GetXPath() + "[@" + value.Name.LocalName + ']';
		}

		/// <summary>
		/// Returns the value of THIS node excluding any child elements' values
		/// </summary>
		public static string GetThisNodeValue(this XElement element)
		{
			var n = new XElement(element);
			n.Descendants().Remove();
			return n.Value;
		}

		/// <summary>
		/// Sorts the XDocument by element, attributes, value, and line number.
		/// Optionally sorts attributes
		/// </summary>
		/// <param name="sortAttributes">True of attributes are to be sorted.</param>
		/// <param name="sortFirstAttributes">Attributes to be put first when sorting</param>
		/// <returns></returns>
		public static XDocument Sort(this XDocument value, bool sortAttributes = false, IEnumerable<string> sortFirstAttributes = null)
		{

			var sortAttrDict = (sortFirstAttributes ?? new[] { "id", "key", "name", "path" })
				.Select((name, ordinal) => new { name, ordinal })
				.ToDictionary(k => k.name, v => v.ordinal, StringComparer.OrdinalIgnoreCase);

			//	Get the ordinal for other attributes
			int nextOrd = sortAttrDict.Values.Max() + 1;

			Func<XAttribute, int> getAttributeOrdinal = a => sortAttrDict.ContainsKey(a.Name.LocalName) ? sortAttrDict[a.Name.LocalName] : nextOrd;

			//	Order elements by depth (descending)
			var elements = value.Descendants().OrderByDescending(x => x.GetXPath().Cast<char>().Count(c => c == '/')).ToList();
			elements.ForEach(e =>
			{
				if (sortAttributes && e.Attributes().Any())
				{
					var newElement = new XElement(e.Name);
					newElement.Add(e.Attributes()
						.OrderBy(a => getAttributeOrdinal(a))
						.ThenBy(a => a.Name.LocalName)
						.ThenBy(a => a.Value));
					e.Attributes().Remove();
					e.Add(newElement.Attributes());
				}

				if (e.Elements().Any())
				{
					var newElement = new XElement(e.Name);
					newElement.Add(
						e.Elements()
							.OrderBy(c => c.GetXPath())
							//	Convert attributes to tab-delimited string for secondary sort
							//	e.g.: name="Joe" value="45" becomes name{tab}Joe{tab}value{tab}45
							.ThenBy(c => string.Join("\t", c.Attributes().Select(a => a.Name.LocalName + '\t' + a.Value)))
							.ThenBy(c => c.GetThisNodeValue().Trim())
							.ThenBy(c => ((IXmlLineInfo)c).LineNumber)
						);
					e.Elements().Remove();
					e.Add(newElement.Elements());
				}
			});
			return value;
		}

		//	XPath enhancements
		public static XAttribute XPathSelectAttribute(this XContainer value, string expression)
		{
			return value.XPathSelectAttribute(expression, default(IXmlNamespaceResolver));
		}
		public static XAttribute XPathSelectAttribute(this XContainer value, string expression, IXmlNamespaceResolver resolver)
		{
			return value.XPathSelectAttributes(expression, resolver).FirstOrDefault();
		}
		public static IEnumerable<XAttribute> XPathSelectAttributes(this XContainer value, string expression)
		{
			return value.XPathSelectAttributes(expression, default(IXmlNamespaceResolver));
		}
		public static IEnumerable<XAttribute> XPathSelectAttributes(this XContainer value, string expression, IXmlNamespaceResolver resolver)
		{
			return ((IEnumerable)value.XPathEvaluate(expression, resolver)).Cast<XAttribute>();
		}

		//	Formatting:

		/// <summary>
		/// Return XDocument after formatting current XDocuments content
		/// </summary>
		/// <param name="settings">Optional Settings</param>
		public static XDocument Reformat(this XDocument value, XmlWriterSettings settings = null)
		{
			return XDocument.Parse(value.ToFormattedXmlString(settings), LoadOptions.SetLineInfo);
		}

		/// <summary>
		/// Return XDocument after formatting current string
		/// </summary>
		/// <param name="settings">Optional Settings</param>
		public static XDocument ToFormattedXml(this string xml, XmlWriterSettings settings = null)
		{
			return XDocument.Parse(xml).Reformat(settings);
		}

		/// <summary>
		/// Return formatted XML string from current string
		/// </summary>
		/// <param name="settings">Optional Settings</param>
		public static string ToFormattedXmlString(this string value, XmlWriterSettings settings = null)
		{
			return XDocument.Parse(value).ToFormattedXmlString(settings);
		}

		/// <summary>
		/// Return formatted XML string from current XDocument
		/// </summary>
		/// <param name="settings">Optional Settings</param>
		public static string ToFormattedXmlString(this XDocument value, XmlWriterSettings settings = null)
		{
			if (settings == null)
				settings = GetDefaultSettings();

			using (var sw = new StringWriterEncoded(settings.Encoding ?? Encoding.UTF8))
			using (var xw = XmlWriter.Create(sw, settings))
			{
				value.WriteTo(xw);
				xw.Flush();
				return sw.ToString();
			}
		}

		//	support classes and methods:
		public static XmlWriterSettings GetDefaultSettings()
		{
			return new XmlWriterSettings() { Indent = true, IndentChars = "\t" };
		}

		private class StringWriterEncoded : StringWriter
		{
			private readonly Encoding _encoding;
			public StringWriterEncoded(Encoding encoding) : base() { _encoding = encoding; }
			public override Encoding Encoding { get { return _encoding; } }
		}
	}
}