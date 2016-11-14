//	RandomExtensions: Created 11/13/2016 - Johnny Olsa

using System.Linq;

namespace System
{
	public static class RandomExtensions
	{
		private const int A = 'A';
		private const int Z = 'Z';
		private const int CapDiff = 'a' - A;
		/// <summary>
		/// Returns an alphabetic random string with a length between minLength and maxLength
		/// </summary>
		public static string Alpha(this Random rnd, int minLength, int maxLength)
		{
			return new string(
				new int[rnd.Next(minLength, maxLength + 1)]
					.Select(n => (char)(rnd.Next(A, Z + 1) + CapDiff * rnd.Next(2)))
					.ToArray());
		}
		/// <summary>
		/// Returns an alphabetic random string with a length between minLength and maxLength.
		/// The first letter will be capitalized and the rest will be lower-case.
		/// </summary>
		public static string Name(this Random rnd, int minLength, int maxLength)
		{
			string name = rnd.Alpha(minLength, maxLength);
			return name.Substring(0, 1).ToUpper() + name.Substring(1).ToLower();
		}

	}
}
