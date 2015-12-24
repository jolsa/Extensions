namespace System.Text.RegularExpressions
{
	/// <summary>
	/// Regex extensions
	/// </summary>
	public static class RegExExtension
	{
		public static string RegexReplace(this string value, string pattern, string replacement, RegexOptions options = RegexOptions.Singleline | RegexOptions.IgnoreCase)
		{
			return Regex.Replace(value, pattern, replacement, options);
		}
	}
}