//	XmlNamespaceHelper: Created 11/21/2015 - Johnny Olsa

using System.Linq;

namespace System.Xml.Linq
{

	/// <summary>
	/// Simplify XPath expressions by loading namespaces from XDocument
	/// </summary>
	public class XmlNamespaceHelper
	{
		private readonly XmlNamespaceManager _nsm;
		private readonly string _defaultPrefix;
		private readonly string _defaultToken;
		public XmlNamespaceHelper(XDocument xml, string defaultPrefix)
		{
			_defaultPrefix = defaultPrefix;
			_nsm = new XmlNamespaceManager(new NameTable());
			xml.Root.Attributes()
				.Where(a => a.IsNamespaceDeclaration)
				.GroupBy(a => a.Name.Namespace == XNamespace.None ? defaultPrefix : a.Name.LocalName, a => XNamespace.Get(a.Value))
				.ToList()
				.ForEach(ns => _nsm.AddNamespace(string.IsNullOrWhiteSpace(ns.Key) ? defaultPrefix : ns.Key, ns.First().NamespaceName));
			_defaultToken = _nsm.HasNamespace(defaultPrefix) ? defaultPrefix + ":" : "";
		}
		/// <summary>
		/// Returns the default prefix used
		/// </summary>
		public string DefaultPrefix { get { return _defaultPrefix; } }
		/// <summary>
		/// Returns the default token (e.g. "x:" or "" if not default namespace is specified)
		/// </summary>
		public string DefaultToken { get { return _defaultToken; } }
		/// <summary>
		/// Returns the NamespaceManager built from the XDocument
		/// </summary>
		public XmlNamespaceManager NamespaceManager { get { return _nsm; } }
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
			return XName.Get(element, _nsm.LookupNamespace(prefix ?? _defaultPrefix) ?? "");
		}
	}
}