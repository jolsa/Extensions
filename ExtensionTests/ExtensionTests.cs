using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml;
using System.Text.RegularExpressions;
using System.IO;
using System.Data.SqlClient;

namespace ExtensionTests
{

	[TestClass]
	public class ExtensionTests
	{
		[TestMethod]
		public void Test_DateDiffs()
		{
			var thisDate = new DateTime(2016, 1, 2, 10, 0, 0, 11);
			var otherDate = new DateTime(2016, 1, 3, 10, 0, 0, 10);

			string diff;
			diff = new DateTime(1967, 9, 10, 12, 0, 0).DateDiff(DateTime.Now).ToString();
			diff = new DateTime(1973, 8, 7).DateDiff(DateTime.Now.Date).ToString();
			diff = new DateTime(1967, 9, 10).DateDiff(new DateTime(1973, 8, 7)).ToString();
			diff = new DateTime(1967, 9, 10).DateDiff(new DateTime(1989, 8, 21)).ToString();

			diff = thisDate.DateDiff(otherDate).ToString();
			diff = thisDate.DateDiff(otherDate).ToString(true);

		}
		[TestMethod]
		public void Test_DictionaryMerge()
		{

			var nd = new NullDictionary<string, int?>(StringComparer.Ordinal)
			{
				{ "One", 1 },
				{ "two", 22 }
			};

			var d = new Dictionary<string, int?>()
			{
				{ "Two", 2 },
				{ "Three", 3 }
			};

			nd.MergeDictionary(d, DictionaryMergeOptions.OverwriteDuplicateKey);

			return;


		}
		[TestMethod]
		public void Test_DataExtensions()
		{
			string connect = "Data Source=.;Integrated Security=SSPI;Initial Catalog=master;";
			string query = "SELECT TOP 5 name, object_id, other = CAST(NULL AS int) FROM sys.tables ORDER BY name";

			using (var cn = new SqlConnection(connect).Opened())
			using (var cmd = new SqlCommand(query, cn))
			using (var ds = cmd.ToDataSet())
			using (var dt = cmd.ToDataTable())
			{
				var records = cmd.GetRecords();

				var o1 = dt.Rows[0].GetColumn("name");
				var o2 = dt.Rows[0].GetColumn("object_id");
				var o3 = dt.Rows[0].GetColumn("other");

				var c1 = dt.Rows[0].GetColumn<string>("name");
				var c2 = dt.Rows[0].GetColumn<int>("object_id");
				var c3 = dt.Rows[0].GetColumn<int?>("other");

				o1 = records[0].GetColumn("name");
				o2 = records[0].GetColumn("object_id");
				o3 = records[0].GetColumn("other");

				c1 = records[0].GetColumn<string>("name");
				c2 = records[0].GetColumn<int>("object_id");
				c3 = records[0].GetColumn<int>("other");

				return;
			}
		}
		[TestMethod]
		public void Test_CleanPath()
		{
			string path = Path.GetTempPath().ToUpper();
			string clean = IOUtilties.CleanPath(path);
		}
		[TestMethod]
		public void Test_RegexExtension()
		{
			var text = "Hello";
			var test = text.RegexReplace("[eo]", ".");
			var m = text.RegexMatches("[eo]").ToList();
		}

		[TestMethod]
		public void Test_XmlNamespaceHelper_WithNamespaces()
		{
			var rawXml = @"
<DIDL-Lite xmlns=""urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/"" xmlns:dc=""http://purl.org/dc/elements/1.1/"" xmlns:upnp=""urn:schemas-upnp-org:metadata-1-0/upnp/"" xmlns:dlna=""urn:dlna"" xmlns:pv=""urn:pv"">
	<item id=""0$1$8I454669"" parentID=""0$1$8"" restricted=""1"">
		<dc:title>Eudaimonia Overture</dc:title>
		<dc:date>2008-01-01</dc:date>
		<upnp:genre>Metal</upnp:genre>
		<upnp:album>Silence Followed by a Deafening Roar</upnp:album>
		<upnp:originalTrackNumber>2</upnp:originalTrackNumber>
		<dc:creator>Paul Gilbert</dc:creator>
		<upnp:albumArtURI pv:profileID=""JPEG_TN"">
			http://100.100.200.4:9000/disk/DLNA-PNJPEG_TN-CI1-FLAGS00f00000/defaultalbumart/a_u_d_i_o.jpg/O0$1$8I454669.jpg?scale=160x160
		</upnp:albumArtURI>
		<upnp:artist>Paul Gilbert</upnp:artist>
		<pv:extension>mp3</pv:extension>
		<upnp:albumArtist>Paul Gilbert</upnp:albumArtist>
		<pv:modificationTime>1315020903</pv:modificationTime>
		<pv:addedTime>1448118086</pv:addedTime>
		<pv:lastUpdated>1315020903</pv:lastUpdated>
		<pv:pvItem name=""pvItem"">
			<dlna:nested name=""Nested"" />
		</pv:pvItem>
		<upnp:album_art>/tmp/0745a659d0f96aa26a477a03c27a416ce58d11d2</upnp:album_art>
		<res bitrate=""128"" resolution="""" colorDepth=""0"" duration=""0:04:36"" size=""4627468""
			protocolInfo=""http-get:*:audio/mpeg:DLNA.ORG_PN=MP3;DLNA.ORG_OP=01;DLNA.ORG_FLAGS=01700000000000000000000000000000"">
				http://100.100.200.4:9000/disk/DLNA-PNMP3-OP01-FLAGS01700000/O0$1$8I454669.mp3</res>
		<upnp:class>object.item.audioItem.musicTrack</upnp:class>
	</item>
</DIDL-Lite>";

			var xml = XDocument.Parse(rawXml);
			var ns = new XmlNamespaceHelper(xml, "x");
			var nsm = ns.NamespaceManager;

			"".Dump("Element/Elements");
			var first = xml.Root.Element(ns.GetXName("item"));
			first.Element(ns.GetXName("album", "upnp")).Value.Dump();
			first.Element(ns.GetXName("title", "dc")).Value.Dump();
			first.Element(ns.GetXName("res")).Attribute("duration").Value.Dump();

			"".Dump("XPath");
			xml.XPathSelectElement("//x:res", nsm).Attribute("size").Value.Dump();
			xml.XPathSelectElement("//pv:lastUpdated", nsm).Dump();
			xml.XPathSelectElement("//pv:pvItem", nsm).Attribute("name").Value.Dump();
			xml.XPathSelectElement("//dlna:nested", nsm).Attribute("name").Value.Dump();

		}
		[TestMethod]
		public void Test_XmlNamespaceHelper_NoNamespaces()
		{
			string rawXml = @"
<root>
	<first>
		<item1 name=""Item 1"">
			<nested>Nested</nested>
		</item1>
		<item2>Item 2</item2>
	</first>
</root>
";

			var xml = XDocument.Parse(rawXml);
			var ns = new XmlNamespaceHelper(xml, "x");
			var nsm = ns.NamespaceManager;

			XElement first;
			"".Dump("Element/Elements");
			first = xml.Root.Element(ns.GetXName("first"));
			first.Element(ns.GetXName("item1")).Attribute("name").Value.Dump();
			first.Element(ns.GetXName("item2")).Value.Dump();

			"".Dump("XPath");
			xml.XPathSelectElement("//first", nsm).Dump();
		}
		[TestMethod]
		public void Test_ToOtherType()
		{
			var a = new ClassA() { Id = 5, Name = "Johnny", Country = "USA" };
			var b = a.ToOtherType<ClassB>();

			b = new ClassB() { Name = "Paul", Age = 73 };
			a = b.ToOtherType<ClassA>();

		}

		[TestMethod]
		public void Test_ToOtherList()
		{
			var listA = new List<ClassA>()
			{
				new ClassA() { Id = 1, Name = "John", Country = "England" },
				new ClassA() { Id = 2, Name = "Paul", Country = "England" },
				new ClassA() { Id = 3, Name = "George", Country = "England" },
				new ClassA() { Id = 4, Name = "Ringo", Country = "England" },
			};

			var toA = listA.ToOtherList<ClassA>();
			var toB = listA.ToOtherList<ClassB>();
		}

		[TestMethod]
		public void Test_DumpObject()
		{
			var a = new ClassA() { Id = 5, Name = "Johnny", Country = "USA" };
			Debug.WriteLine(a.DumpObject());
			var b = a.ToOtherType<ClassB>();
			Debug.WriteLine(b.DumpObject());
			b = new ClassB() { Name = "Paul", Age = 73 };
			Debug.WriteLine(b.DumpObject());
			b = null;
			Debug.WriteLine(b.DumpObject());
			var listA = new List<ClassA>()
			{
				new ClassA() { Id = 1, Name = "John", Country = "England" },
				new ClassA() { Id = 2, Name = "Paul", Country = "England" },
				new ClassA() { Id = 3, Name = "George", Country = "England" },
				new ClassA() { Id = 4, Name = "Ringo", Country = "England" },
			};
			Debug.WriteLine(listA.DumpObject());
			Debug.WriteLine(new Dictionary<StringComparison, ClassA>()
			{
				{ StringComparison.Ordinal, new ClassA() { Id = 4 } },
				{ StringComparison.CurrentCulture, new ClassA() { Id = 14 } },
			}.DumpObject());
			Debug.WriteLine("A string, a string, a marvelous thing!".DumpObject());


		}

		[TestMethod]
		public void Test_LeftJoin()
		{
			var listA = new List<ClassA>()
			{
				new ClassA() { Id = 1, Name = "John", Country = "England" },
				new ClassA() { Id = 2, Name = "Paul", Country = "England" },
				new ClassA() { Id = 3, Name = "George", Country = "England" },
				new ClassA() { Id = 4, Name = "Ringo", Country = "England" },
			};
			var listB = listA.Where(a => !a.Name.Equals("Ringo")).Select(a =>
			{
				var b = a.ToOtherType<ClassB>();
				b.Age = 70 + a.Id;
				return b;
			}).ToList();

			var listC = listA.LeftJoin(listB, a => a.Id, b => b.Id, (a, b) => new { a.Id, a.Name, Age = b != null ? (int?)b.Age : null, a.Country }).ToList();
			return;

		}

		[TestMethod]
		public void Test_XmlFormatting()
		{
			XDocument xml;
			xml = XDocument.Parse(@"
<root>
	<Level1 value=""value1"" add=""false"" id=""1"">D</Level1>
	<Level1 value=""value1"" add=""false"" id=""2"">C</Level1>
		<Level2>Z</Level2>
		<Level2>
			<Level3 id=""3"" />
			<Level3 id=""2"">ZZ</Level3>
			<Level3 id=""1"" />
		</Level2>
		<Level2>Y</Level2>
	<Level1 value=""value1"" add=""false"" id=""4"">B</Level1>
	<Level1 value=""value1"" add=""false"" id=""3"">A</Level1>
</root>
");

			xml.Sort(true);
			xml.ToFormattedXmlString().Dump("Sorted");

			xml = XDocument.Parse(@"
<root id=""rootNode"">
	Root Node
	<e1 name=""element1"">
		Echo 1
		<e2>Echo 2</e2>
	</e1>
</root>
");

			xml.XPathSelectAttributes("//@name | //@id").Select(x => x.GetXPath()).Dump("XPathSelectAttributes");

			xml.Element("root").Attribute("id").GetXPath().Dump("GetXPath attribute");
			xml.Root.Element("e1").GetXPath().Dump("GetXPath element");

			xml.Descendants().Select(x => x.GetXPath()).Dump("GetXPath all elements");
			xml.Descendants().SelectMany(x => x.Attributes().Select(a => a.GetXPath())).Dump("GetXPath all attributes");
			xml.Descendants().Select(x => string.Format("Level: {0,5:#,0}; XPath: {1}", x.GetLevel(), x.GetXPath())).Dump("GetLevel");

			string unformatted = @"<root><el1><el2 name=""El 2"" /></el1></root>";
			unformatted.ToFormattedXmlString().Dump("Unformatted to Formatted");
			XDocument doc;

			doc = XDocument.Parse(unformatted).Reformat();
			doc.GetLineInfo().LineNumber.ToString().Dump("Document LineNumber");
			doc.XPathSelectElement("//el1").GetLineInfo().LineNumber.ToString().Dump("Element LineNumber");
			doc.XPathSelectAttribute("//@name").GetLineInfo().LineNumber.ToString().Dump("Attribute LineNumber");

			var xws = new XmlWriterSettings() { Indent = true, IndentChars = "\t" };
			XDocument.Parse(unformatted).Reformat(xws).Declaration.ToString().Dump("Declaration");
			unformatted.ToFormattedXmlString(xws).Dump("ToFormattedXmlString");

		}

	}

	public class ClassA
	{
		public short Id { get; set; }
		public string Name { get; set; }
		public string Country { get; set; }
	}
	public class ClassB
	{
		public readonly short? Id;
		public string Name { get; set; }
		public int Age { get; set; }
		public readonly List<int> Ints = new List<int>() { 5, 10, 15, 20 };
	}
	internal static class TestExtensions
	{
		public static void Dump(this string value, string comment = null)
		{
			if (!Debugger.IsAttached) return;
			if (!string.IsNullOrEmpty(comment))
				Debug.WriteLine(comment + ':');
			if (!string.IsNullOrEmpty(value))
				Debug.WriteLine(value);
		}
		public static void Dump(this XContainer value, string comment = null)
		{
			var doc = value is XDocument ? (XDocument)value : new XDocument(value);
			var settings = XmlExtensions.GetDefaultSettings();
			if (!(value is XDocument))
				settings.OmitXmlDeclaration = true;
			Dump(doc.ToFormattedXmlString(settings), comment);
		}
		public static void Dump(this IEnumerable<string> value, string comment = null)
		{
			Dump(string.Join("\r\n", value), comment);
		}
	}


}
