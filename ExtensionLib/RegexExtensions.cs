//	RegularExpressions: Created 12/07/2015 - Johnny Olsa

using System.Collections.Generic;
using System.Linq;

namespace System.Text.RegularExpressions
{
	/// <summary>
	/// Regex extensions
	/// </summary>
	public static class RegExExtension
	{
		/// <summary>
		/// Returns a Regex Match object
		/// </summary>
		/// <param name="options">Optional.  Default = RegexOptions.Singleline | RegexOptions.IgnoreCase</param>
		/// <returns></returns>
		public static Match RegexMatch(this string value, string pattern, RegexOptions options = RegexOptions.Singleline | RegexOptions.IgnoreCase)
		{
			return Regex.Match(value, pattern, options);
		}
		/// <summary>
		/// Returns an IEnumerable&lt;Match&gt; for the specified Regex pattern
		/// </summary>
		/// <param name="options">Optional.  Default = RegexOptions.Singleline | RegexOptions.IgnoreCase</param>
		/// <returns></returns>
		public static IEnumerable<Match> RegexMatches(this string value, string pattern, RegexOptions options = RegexOptions.Singleline | RegexOptions.IgnoreCase)
		{
			return Regex.Matches(value, pattern, options).Cast<Match>();
		}
		/// <summary>
		/// Returns a string with the specified Regex Replacements
		/// </summary>
		/// <param name="options">Optional.  Default = RegexOptions.Singleline | RegexOptions.IgnoreCase</param>
		/// <returns></returns>
		public static string RegexReplace(this string value, string pattern, string replacement, RegexOptions options = RegexOptions.Singleline | RegexOptions.IgnoreCase)
		{
			return Regex.Replace(value, pattern, replacement, options);
		}
	}
}