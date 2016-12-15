//	StringExtensions: Created 12/15/2016 - Johnny Olsa
using System;
using System.Globalization;
using System.Linq;

namespace ExtensionLib
{
    public static class StringExtensions
    {
        public static bool IgnoreDiacriticsContains(this string value, string search)
        {
            return CultureInfo.CurrentCulture.CompareInfo.IndexOf(value, search, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) > -1;
        }
        public static bool IgnoreDiacriticsEquals(this string value, string other)
        {
            return CultureInfo.CurrentCulture.CompareInfo.Compare(value, other, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) == 0;
        }
    }
}
