//	XmlNamespaceHelper: Created 11/21/2015 - Johnny Olsa

using System.Linq;
using System.Xml.XPath;

namespace System.Xml.Linq
{

	/// <summary>
	/// Simplify XPath expressions by loading namespaces from XDocument
	/// </summary>
	public class XmlNamespaceHelper
	{
		public XmlNamespaceHelper(XDocument xml, string defaultPrefix)
		{
			DefaultPrefix = defaultPrefix;
			NamespaceManager = new XmlNamespaceManager(new NameTable());
			xml.XPathSelectElements("//*").SelectMany(e => e.Attributes())
                .Where(a => a.IsNamespaceDeclaration)
				.GroupBy(a => a.Name.Namespace == XNamespace.None ? defaultPrefix : a.Name.LocalName, a => XNamespace.Get(a.Value))
				.ToList()
				.ForEach(ns => NamespaceManager.AddNamespace(string.IsNullOrWhiteSpace(ns.Key) ? defaultPrefix : ns.Key, ns.First().NamespaceName));
			DefaultToken = NamespaceManager.HasNamespace(defaultPrefix) ? defaultPrefix + ":" : "";
		}
		/// <summary>
		/// Returns the default prefix used
		/// </summary>
		public string DefaultPrefix { get; }
		/// <summary>
		/// Returns the default token (e.g. "x:" or "" if not default namespace is specified)
		/// </summary>
		public string DefaultToken { get; }
		/// <summary>
		/// Returns the NamespaceManager built from the XDocument
		/// </summary>
		public XmlNamespaceManager NamespaceManager { get; }
		/// <summary>
		/// Get the XName for an element with no namespace
		/// </summary>
		public XName GetXName(string element)
		{
			return GetXName(element, null);
		}
		/// <summary>
		/// Get the XName for an element with the namespace for the specified prefix
		/// </summary>
		/// <param name="element">The element</param>
		/// <param name="prefix">The prefix for the namespace</param>
		public XName GetXName(string element, string prefix)
		{
			return XName.Get(element, NamespaceManager.LookupNamespace(prefix ?? DefaultPrefix) ?? "");
		}
	}
}