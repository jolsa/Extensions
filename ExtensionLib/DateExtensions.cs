//	DateExtensions: Created 03/27/2016 - Johnny Olsa

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
			var m1 = dt1.Month;
			var m2 = dt2.Month;

			//	Working dates (remove years)
			var td1 = dt1.AddYears(-y1 + 1);
			var td2 = dt2.AddYears(-y2 + 1);

			//	If month, day, time is greater in td2, subtract 1 year, add 12 months
			if (td1 < td2)
			{
				y1--;
				m1 += 12;
			}
			var y = y1 - y2;

			//	Remove Months
			td1 = td1.AddMonths(-td1.Month + 1);
			td2 = td2.AddMonths(-td2.Month + 1);

			//	Start with subtraction, if < 0, do the math
			var d = dt1.Day - dt2.Day;
			if (d < 0)
			{
				m1--;
				//	Get EOM, then subtract dt2's day and add dt1's day
				d = dt2.Eom().Day - dt2.Day + dt1.Day;
			}
			var m = m1 - m2;

			//	Remove days
			td1 = td1.AddDays(-td1.Day + 1);
			td2 = td2.AddDays(-td2.Day + 1);

			//	If time is greater in td2, subtract a day from d, but add a day for TimeSpan
			if (td1 < td2)
			{
				d--;
				td1 = td1.AddDays(1);
			}

			var timeDiff = td1 - td2;
			return new DateTimeDiff(y, m, d, timeDiff.Hours, timeDiff.Minutes, timeDiff.Seconds, timeDiff.Milliseconds);
		}
	}
}
