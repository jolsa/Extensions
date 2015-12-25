//	IOUtilties: Created 12/11/2014 - Johnny Olsa

using System.Linq;
using System.Text.RegularExpressions;

namespace System.IO
{
	/// <summary>
	/// IO Utilities
	/// </summary>
	public static class IOUtilties
	{
		/// <summary>
		/// Convert a mis-cased path to a properly cased one
		/// </summary>
		public static string CleanPath(string fullPath)
		{
			//	Remove trailing slashes (if any), but add one back if any
			bool appendSlash = fullPath.EndsWith("\\");
			if (appendSlash)
				fullPath = Regex.Match(fullPath, @"(^.*?)\\+$").Result("$1");

			//	If not a valid file or directory, this won't work
			if (!Directory.Exists(fullPath) && !File.Exists(fullPath))
				throw new DirectoryNotFoundException("\"" + fullPath + "\" is not a valid file or directory.");

			fullPath = Path.GetFullPath(fullPath);
			//	Get the root
			string root = Directory.GetDirectoryRoot(fullPath);
			//	If it has a ':', assume it's a drive and capitalize; otherwise, leave it alone
			if (root.IndexOf(':') >= 0)
				root = root.ToUpper();
			else if (!root.EndsWith("\\"))
				root += '\\';
			//	Check each part and aggregate
			if (fullPath.Length > root.Length)
				root = fullPath.Substring(root.Length).Split('\\')
					.Aggregate(root, (r, p) => new DirectoryInfo(r).GetFileSystemInfos(p)[0].FullName);
			if (appendSlash)
				root += '\\';
			return root;
		}
	}
}