//	InterpolationPlus: Created 06/05/2016 - Johnny Olsa

using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace ExtensionLib
{
	public static class InterpolationPlus
	{
		/// <summary>
		/// Formats a "template" string with the values passed<br/>
		/// NOTE: Pass formats before values!
		/// </summary>
		/// <param name="template">e.g. "{val1:fmt}, {val2.fmt}"</param>
		/// <param name="values">e.g. new { fmt = "#,0" }, new { val1 = 1000, val2 = 2000 }</param>
		public static string FormatTemplate(string template, params object[] values)
		{
			values.ToList().ForEach(v => template = FormatTemplate(template, v));
			return template;
		}

		/// <summary>
		/// Formats a "template" string with the values passed
		/// </summary>
		/// <param name="template">e.g. "{val1}, {val2}"</param>
		/// <param name="values">e.g. new { val1 = 1000, val2 = 2000 }</param>
		public static string FormatTemplate(string template, object values)
		{
			//  This code assumes values is an anonymous object other objects may work, but
			//  Indexed properties will fail, it doesn't check public, internal, or private, and it ignores methods
			var type = values.GetType();
			var dict = type.GetProperties().Select(p => new { p.Name, Value = type.GetProperty(p.Name).GetValue(values, null) })
			  .ToDictionary(k => k.Name, v => v.Value);

			//  Convert the keys to an "or" pattern within braces
			return Regex.Replace(template, $"({string.Join("|", dict.Keys.Select(k => $@"{{\s*{k}\s*.*?}}"))})", match =>
			{
				//  Find the token within the match
				string token = Regex.Match(match.Value, @"\{\s*(.*?)[:,\}]").Result("$1").Trim();
				//  Format the value (remove whitespace around the token)
				return string.Format(Regex.Replace(match.Value, $@"\s*{token}\s*", "0"), dict[token]);
			}, RegexOptions.None); // Intentionally not case-insensitive (like string interpolation)
		}
	}
}
