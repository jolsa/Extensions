//	ToOtherExtensions: Created 12/23/2015 - Johnny Olsa

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace System.Linq
{
	/// <summary>
	/// Conversion Extensions
	/// </summary>
	public static class ToOtherExtensions
	{
		/// <summary>
		/// Get the properties and fields of a Type in a dictionary by name
		/// </summary>
		/// <param name="type">Type to parse</param>
		public static Dictionary<string, MemberInfo> TypeToMemberDict(Type type)
		{
			return type.GetMembers(BindingFlags.Instance | BindingFlags.Public).ToDictionary(k => k.Name, v => v, StringComparer.OrdinalIgnoreCase);
		}
		/// <summary>
		/// Get the common properties and fields by name that can be "read" from the source and can be "written" in the target.
		/// Create "Member Dictionaries" using TypeToMemberDict.
		/// Returns a list of member names.
		/// </summary>
		/// <param name="sourceMembers">Source Members</param>
		/// <param name="targetMembers">Target Members</param>
		/// <returns>List of names</returns>
		public static List<string> GetCopyableMembers(Dictionary<string, MemberInfo> sourceMembers, Dictionary<string, MemberInfo> targetMembers)
		{
			PropertyInfo pi;
			return sourceMembers.Join(targetMembers, s => s.Key, t => t.Key, (s, t) => new { s, t }, StringComparer.OrdinalIgnoreCase)
				.Where(j => (j.s.Value is FieldInfo || ((pi = j.s.Value as PropertyInfo) != null && pi.CanRead)
						   && j.t.Value is FieldInfo || ((pi = j.t.Value as PropertyInfo) != null && pi.CanWrite)))
				.Select(j => j.s.Key)
				.ToList();
		}

		//	Private "shared" method used by public version and ToOtherList method
		private static TTarget ToOtherType<TTarget>(this object value, Dictionary<string, MemberInfo> sourceMembers, Dictionary<string, MemberInfo> targetMembers, List<string> setProps) where TTarget : new()
		{
			var target = new TTarget();
			//	Set the common properties/fields
			setProps.ForEach(name =>
			{
				var t = targetMembers[name];
				var s = sourceMembers[name];

				//	Is it a property or field?
				var tp = t as PropertyInfo;
				var tf = t as FieldInfo;
				var sp = s as PropertyInfo;
				var sf = s as FieldInfo;

				var sv = sp != null ? sp.GetValue(value) : sf.GetValue(value);
				if (tp != null)
					tp.SetValue(target, sv);
				else
					tf.SetValue(target, sv);
			});
			return target;
		}
		/// <summary>
		/// Create a new Target instance whose properties and fields are set by the source (if the names match).
		/// </summary>
		/// <typeparam name="TTarget">Target type</typeparam>
		public static TTarget ToOtherType<TTarget>(this object value) where TTarget : new()
		{
			var sourceMembers = TypeToMemberDict(value.GetType());
			var targetMembers = TypeToMemberDict(typeof(TTarget));

			return value.ToOtherType<TTarget>(sourceMembers, targetMembers, GetCopyableMembers(sourceMembers, targetMembers));
		}

		/// <summary>
		/// Returns a list of Target type whose properties and fields are set by the items in the source (if the names match).
		/// </summary>
		/// <typeparam name="TTarget">Target Type</typeparam>
		public static List<TTarget> ToOtherList<TTarget>(this IEnumerable<object> value) where TTarget : new()
		{
			//	If we have none, return an empty list
			if (!value.Any()) return new List<TTarget>();

			//	Get the first item to find the type
			var first = value.First();

			//	Get the source properties and the target properties
			var sourceMembers = TypeToMemberDict(first.GetType());
			var targetMembers = TypeToMemberDict(typeof(TTarget));

			//	Create the new list
			return value.Select(v => v.ToOtherType<TTarget>(sourceMembers, targetMembers, GetCopyableMembers(sourceMembers, targetMembers))).ToList();
		}

		/// <summary>
		/// Slightly better than .ToString()
		/// </summary>
		public static string DumpObject(this object value)
		{
			if (value == null)
				return "null";
			if (value is string)
				return (string)value;

			Func<string, string> cleanTypeString = s => Regex.Replace(s, @"`\d+", "");

			if (value is Collections.IEnumerable)
				return cleanTypeString(value.ToString());

			return string.Join("; ",
				value.GetType().GetMembers(BindingFlags.Instance | BindingFlags.Public)
					.Select(m => new { m.Name, m = m, p = m as PropertyInfo, f = m as FieldInfo })
					.Where(m => m.f != null || (m.p != null && m.p.CanRead && !m.p.GetIndexParameters().Any()))
					.Select(m =>
					{
						var val = m.f != null ? m.f.GetValue(value) : m.p.GetValue(value);
						if (val == null)
							val = "null";
						return cleanTypeString(string.Format("{0}={1}", m.Name, val));
					}));
		}
	}
}
