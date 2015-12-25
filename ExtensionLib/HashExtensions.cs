//	HashExtensions: Created 09/27/2015 - Johnny Olsa

using System;
using System.Linq;
using System.Text;

namespace System.Security.Cryptography
{
	/// <summary>
	/// Hash Extensions
	/// </summary>
	public static class HashExtensions
	{
		/// <summary>
		/// Returns an MD5 Hash that matches the deprecated MD5 Hash
		/// </summary>
		public static string GetMD5Hash(this string value)
		{
			using (var hash = MD5.Create())
				return string.Join("", hash.ComputeHash(Encoding.UTF8.GetBytes(value)).Select(b => b.ToString("X2")));
		}
	}
}