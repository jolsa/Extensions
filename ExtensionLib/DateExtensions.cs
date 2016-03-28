//	DateExtensions: Created 03/27/2016 - Johnny Olsa
using System.Linq;

namespace System
{
	/// <summary>
	/// Date Extensions
	/// </summary>
	public static class DateExtensions
	{
		/// <summary>
		/// Represents the difference between 2 DateTime instances
		/// </summary>
		public class DateTimeDiff
		{
			public int Years { get; private set; }
			public int Months { get; private set; }
			public int Days { get; private set; }
			public int Hours { get; private set; }
			public int Minutes { get; private set; }
			public int Seconds { get; private set; }
			public int Milliseconds { get; private set; }
			public DateTimeDiff(int years, int months, int days)
			{
				Years = years;
				Months = months;
				Days = days;
			}
			public DateTimeDiff(int years, int months, int days, int hours, int minutes, int seconds) : this(years, months, days)
			{
				Hours = hours;
				Minutes = minutes;
				Seconds = seconds;
			}
			public DateTimeDiff(int years, int months, int days, int hours, int minutes, int seconds, int milliseconds) : this(years, months, days, hours, minutes, seconds)
			{
				Milliseconds = milliseconds;
			}
			public override string ToString()
			{
				return ToString(false);
			}
			/// <param name="longFormat">If true, time (if any) will be shown as x hours, x minutest, etc. Otherwise, it will be hh:mm:ss.fff</param>
			public string ToString(bool longFormat)
			{
				string format = "";
				Action<int, string> append = (n, label) =>
				{
					if (n != 0)
					{
						if (format != "") format += ", ";
						format += $"{n:#,0} " + label;
						if (n > 1)
							format += 's';
					}
				};
				append(Years, "year");
				append(Months, "month");
				append(Days, "day");

				TimeSpan time;
				if (longFormat)
				{
					append(Hours, "hour");
					append(Minutes, "minute");
					append(Seconds, "second");
					append(Milliseconds, "millisecond");
				}
				else if ((time = new TimeSpan(0, Hours, Minutes, Seconds, Milliseconds)) != default(TimeSpan))
				{
					if (format != "") format += " ";
					format += time.ToString(@"hh\:mm\:ss\:fff");
				}
				if (format == "") format = "no difference";
				return format;
			}
		}
		/// <summary>
		/// Returns the End of month for the specified date
		/// </summary>
		public static DateTime Eom(this DateTime date)
		{
			return date.Date.AddDays(-date.Day + 1).AddMonths(1).AddDays(-1);
		}
		/// <summary>
		/// Returns the difference in years, months, days, hours, minutes, seconds, and milliseconds between the two dates
		/// </summary>
		public static DateTimeDiff DateDiff(this DateTime thisDate, DateTime otherDate)
		{
			//	Always return a positive, so dt1 is the greater of the two
			var dt1 = thisDate < otherDate ? otherDate : thisDate;
			var dt2 = thisDate < otherDate ? thisDate : otherDate;

			var y1 = dt1.Year;
			var y2 = dt2.Year;
			var M1 = dt1.Month;
			var M2 = dt2.Month;
			var d1 = dt1.Day;
			var d2 = dt2.Day;
			var h1 = dt1.Hour;
			var h2 = dt2.Hour;
			var m1 = dt1.Minute;
			var m2 = dt2.Minute;
			var s1 = dt1.Second;
			var s2 = dt2.Second;
			var f1 = dt1.Millisecond;
			var f2 = dt2.Millisecond;

			if (f1 < f2)
			{
				f1 += 1000;
				s1--;
			}
			if (s1 < s2)
			{
				s1 += 60;
				m1--;
			}
			if (m1 < m2)
			{
				m1 += 60;
				h1--;
			}
			if (h1 < h2)
			{
				h1 += 24;
				d1--;
			}
			if (d1 < d2)
			{
				d1 += dt2.Eom().Day;
				M1--;
			}
			if (M1 < M2)
			{
				M1 += 12;
				y1--;
			}
			return new DateTimeDiff(y1 - y2, M1 - M2, d1 - d2, h1 - h2, m1 - m2, s1 - s2, f1 - f2);
		}
	}
}
