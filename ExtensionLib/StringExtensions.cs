//	StringExtensions: Created 12/15/2016 - Johnny Olsa
using System;
using System.Globalization;
using System.Linq;

namespace ExtensionLib
{
	public static class StringExtensions
	{
		public static int IgnoreDiacriticsIndexOf(this string value, string search)
		{
			if (value == null || search == null)
				return value == search ? 0 : -1;
			return CultureInfo.CurrentCulture.CompareInfo.IndexOf(value, search, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace);
		}
		public static bool IgnoreDiacriticsContains(this string value, string search)
		{
			return IgnoreDiacriticsIndexOf(value, search) > -1;
		}
		public static bool IgnoreDiacriticsEquals(this string value, string other)
		{
			if (value == null || other == null)
				return value == other;
			return CultureInfo.CurrentCulture.CompareInfo.Compare(value, other, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) == 0;
		}
	}
}
