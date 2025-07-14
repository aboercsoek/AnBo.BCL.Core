//--------------------------------------------------------------------------
// File:    DateTimeHelper.cs
// Content:	Implementation of class DateTimeUtlities
// Author:	Andreas Börcsök
//--------------------------------------------------------------------------
#region Using directives

using System.Data.SqlTypes;
using System.Globalization;
using System.Xml;

#endregion

namespace AnBo.Core
{
    /// <summary>
	/// Methods to help in date and time manipulation.
	/// </summary>
	public static class DateTimeHelper
    {

        #region DateTime & TimeSpan Enums

        /// <summary>
        /// Enumeration for day part
        /// </summary>
        public enum DayPartType
        {
            /// <summary>Beginn of day = 00:00:00</summary>
            BeginOfDay,
            /// <summary>High Noon = 12:00:00</summary>
            HighNoon,
            /// <summary>End of day = 23:59:59</summary>
            EndOfDay,
        }

        /// <summary>
        /// 
        /// </summary>
        public enum TimeSpanAssumption
        {
            /// <summary>
            /// 
            /// </summary>
            None,

            /// <summary>
            /// 
            /// </summary>
            Days,

            /// <summary>
            /// 
            /// </summary>
            Hours,

            /// <summary>
            /// 
            /// </summary>
            Minutes,

            /// <summary>
            /// 
            /// </summary>
            Seconds
        }

        #endregion

        #region Day, Month Helper Methods

        /// <summary>
        /// Parses the month.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns></returns>
        public static Month ParseMonth(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentNullException("str");
            }
            if (str.Length < 3)
            {
                throw new ArgumentException("Month should be at least 3 characters (" + str + ")");
            }
            str = str.ToLower().Trim().Substring(0, 3);
            switch (str)
            {
                case "jan":
                    return Month.January;
                case "feb":
                    return Month.February;
                case "mar":
                    return Month.March;
                case "apr":
                    return Month.April;
                case "may":
                    return Month.May;
                case "jun":
                    return Month.June;
                case "jul":
                    return Month.July;
                case "aug":
                    return Month.August;
                case "sep":
                    return Month.September;
                case "oct":
                    return Month.October;
                case "nov":
                    return Month.November;
                case "dec":
                    return Month.December;
                default:
                    throw new ArgumentOutOfRangeException("str", str, "Unknown month " + str);
            }
        }

        /// <summary>
        /// Parses the day of week.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns></returns>
        public static DayOfWeek ParseDayOfWeek(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentNullException("str");
            }
            if (str.Length < 3)
            {
                throw new ArgumentException("Day of week should be at least 3 characters (" + str + ")");
            }
            str = str.ToLower().Trim().Substring(0, 3);
            switch (str)
            {
                case "sun":
                    return DayOfWeek.Sunday;
                case "mon":
                    return DayOfWeek.Monday;
                case "tue":
                    return DayOfWeek.Tuesday;
                case "wed":
                    return DayOfWeek.Wednesday;
                case "thu":
                    return DayOfWeek.Thursday;
                case "fri":
                    return DayOfWeek.Friday;
                case "sat":
                    return DayOfWeek.Saturday;
                default:
                    throw new ArgumentOutOfRangeException("str", str, "Unknown day of week " + str);

            }
        }

        #endregion

        #region DateTime Helper Methods

        /// <summary>
        /// Trys to parse an DateTime string into a DateTime-object
        /// </summary>
        /// <param name="value">DateTime string</param>
        /// <returns>The converted DateTime object</returns>
        public static DateTime Create(string value)
        {
            DateTime result;

            //  DateTime format parsing
            if (value.Length >= 14)
            {
                if (DateTime.TryParseExact(value, "yyyyMMdd'T'HHmmssfff", CultureInfo.CurrentCulture,
                        DateTimeStyles.None, out result) == true)
                    return result;
                if (DateTime.TryParseExact(value, "yyyy-MM-dd'T'HH:mm:ss.fff", CultureInfo.CurrentCulture,
                        DateTimeStyles.None, out result) == true)
                    return result;
                if (DateTime.TryParseExact(value, "yyyy-MM-dd'T'HHmmssfff", CultureInfo.CurrentCulture,
                        DateTimeStyles.None, out result) == true)
                    return result;
                if (DateTime.TryParseExact(value, "yyyyMMddHHmmssfff", CultureInfo.CurrentCulture,
                        DateTimeStyles.None, out result) == true)
                    return result;
                if (DateTime.TryParseExact(value, "yyyyMMdd'T'HHmmss", CultureInfo.CurrentCulture,
                        DateTimeStyles.None, out result) == true)
                    return result;
                if (DateTime.TryParseExact(value, "yyyy-MM-dd'T'HH:mm:ss", CultureInfo.CurrentCulture,
                        DateTimeStyles.None, out result) == true)
                    return result;
                if (DateTime.TryParseExact(value, "yyyy-MM-dd'T'HHmmss", CultureInfo.CurrentCulture,
                        DateTimeStyles.None, out result) == true)
                    return result;
                if (DateTime.TryParseExact(value, "yyyyMMddHHmmss", CultureInfo.CurrentCulture,
                        DateTimeStyles.None, out result) == true)
                    return result;

                if (DateTime.TryParseExact(value, "dd.MM.yyyy HH:mm:ss", CultureInfo.CurrentCulture,
                        DateTimeStyles.None, out result) == true)
                    return result;

                if (DateTime.TryParseExact(value, "MM/dd/yyyy HH:mm:ss", CultureInfo.CurrentCulture,
                        DateTimeStyles.None, out result) == true)
                    return result;
            }
            else
            {
                // Date format parsing
                if (DateTime.TryParseExact(value, "dd.MM.yy", CultureInfo.CurrentCulture,
                        DateTimeStyles.None, out result) == true)
                    return result.Date;

                if (DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.CurrentCulture,
                        DateTimeStyles.None, out result) == true)
                    return result.Date;

                if (DateTime.TryParseExact(value, "dd.MM.yy", CultureInfo.CurrentCulture,
                        DateTimeStyles.None, out result) == true)
                    return result.Date;

                if (DateTime.TryParseExact(value, "MM/dd/yyyy", CultureInfo.CurrentCulture,
                        DateTimeStyles.None, out result) == true)
                    return result.Date;

                if (DateTime.TryParseExact(value, "dd.MM.yyyy", CultureInfo.CurrentCulture,
                        DateTimeStyles.None, out result) == true)
                    return result.Date;

                if (DateTime.TryParseExact(value, "ddMMMyy", CultureInfo.CurrentCulture,
                    DateTimeStyles.None, out result) == true)
                    return result.Date;

                if (DateTime.TryParseExact(value, "ddMMMyy", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out result) == true)
                    return result.Date;

                if (DateTime.TryParseExact(value, "ddMMMyyyy", CultureInfo.CurrentCulture,
                    DateTimeStyles.None, out result) == true)
                    return result;

                if (DateTime.TryParseExact(value, "ddMMMyyyy", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out result) == true)
                    return result.Date;

                if (DateTime.TryParseExact(value, "dd MMM yy", CultureInfo.CurrentCulture,
                    DateTimeStyles.None, out result) == true)
                    return result.Date;

                if (DateTime.TryParseExact(value, "dd MMM yy", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out result) == true)
                    return result.Date;

                if (DateTime.TryParseExact(value, "dd MMM yyyy", CultureInfo.CurrentCulture,
                    DateTimeStyles.None, out result) == true)
                    return result.Date;

                if (DateTime.TryParseExact(value, "dd MMM yyyy", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out result) == true)
                    return result.Date;
            }

            return DateTime.Parse(value);
        }

        /// <summary>
        /// Creates timestamp from date and day part value
        /// </summary>
        /// <param name="date">Date</param>
        /// <param name="dayPart">part of day</param>
        /// <returns>The converted Timestamp.</returns>
        public static DateTime CreateTimestampFromDate(string date, DayPartType dayPart)
        {
            DateTime result;

            // Date format parsing
            if (DateTime.TryParseExact(date, "dd.MM.yy", CultureInfo.CurrentCulture,
                    DateTimeStyles.None, out result) == true)
                return SetTimeOfDay(result.Date, dayPart);

            if (DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.CurrentCulture,
                    DateTimeStyles.None, out result) == true)
                return SetTimeOfDay(result.Date, dayPart);

            if (DateTime.TryParseExact(date, "dd.MM.yy", CultureInfo.CurrentCulture,
                    DateTimeStyles.None, out result) == true)
                return SetTimeOfDay(result.Date, dayPart);

            if (DateTime.TryParseExact(date, "MM/dd/yyyy", CultureInfo.CurrentCulture,
                    DateTimeStyles.None, out result) == true)
                return SetTimeOfDay(result.Date, dayPart);

            if (DateTime.TryParseExact(date, "dd.MM.yyyy", CultureInfo.CurrentCulture,
                    DateTimeStyles.None, out result) == true)
                return SetTimeOfDay(result.Date, dayPart);

            if (DateTime.TryParseExact(date, "ddMMMyy", CultureInfo.CurrentCulture,
                DateTimeStyles.None, out result) == true)
                return SetTimeOfDay(result.Date, dayPart);

            if (DateTime.TryParseExact(date, "ddMMMyy", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out result) == true)
                return SetTimeOfDay(result.Date, dayPart);

            if (DateTime.TryParseExact(date, "ddMMMyyyy", CultureInfo.CurrentCulture,
                DateTimeStyles.None, out result) == true)
                return SetTimeOfDay(result.Date, dayPart);

            if (DateTime.TryParseExact(date, "ddMMMyyyy", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out result) == true)
                return SetTimeOfDay(result.Date, dayPart);

            if (DateTime.TryParseExact(date, "dd MMM yy", CultureInfo.CurrentCulture,
                DateTimeStyles.None, out result) == true)
                return SetTimeOfDay(result.Date, dayPart);

            if (DateTime.TryParseExact(date, "dd MMM yy", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out result) == true)
                return SetTimeOfDay(result.Date, dayPart);

            if (DateTime.TryParseExact(date, "dd MMM yyyy", CultureInfo.CurrentCulture,
                DateTimeStyles.None, out result) == true)
                return SetTimeOfDay(result.Date, dayPart);

            if (DateTime.TryParseExact(date, "dd MMM yyyy", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out result) == true)
                return SetTimeOfDay(result.Date, dayPart);

            return SetTimeOfDay(DateTime.Parse(date), dayPart);
        }

        /// <summary>
        /// Sets the time of the date object to a specific time. Witch time is specified through the <see cref="DayPartType">dayPart</see> parameter
        /// </summary>
        /// <param name="date">Date object</param>
        /// <param name="dayPart">Day part enumeration (BeginOfDay, HighNoon, EndOfDay)</param>
        /// <returns>The converted DateTime object.</returns>
        private static DateTime SetTimeOfDay(DateTime date, DayPartType dayPart)
        {
            DateTime result;

            switch (dayPart)
            {
                case DayPartType.BeginOfDay:
                    result = date.Date + new TimeSpan(0, 0, 0, 0, 0);
                    break;
                case DayPartType.HighNoon:
                    result = date.Date + new TimeSpan(0, 12, 0, 0, 0);
                    break;
                case DayPartType.EndOfDay:
                    result = date.Date + new TimeSpan(0, 23, 59, 59, 0);
                    break;
                default:
                    result = date.Date + new TimeSpan(0, 0, 0, 0, 0);
                    break;
            }

            return result;
        }


        /// <summary>
        /// Converts a DateTime object into a ISO 8601 conform date time string.
        /// </summary>
        /// <param name="value">DateTime object to convert.</param>
        /// <returns>ISO 8601 DateTime string</returns>
        public static string ToISO8601DateTime(DateTime value)
        {
            return XmlConvert.ToString(value, XmlDateTimeSerializationMode.Utc);
        }

        /// <summary>
        /// Converts a <see cref="DateTime"/> object into a file sortable string format
        /// </summary>
        /// <param name="value"><see cref="DateTime"/> object to convert.</param>
        /// <returns>File sortable DateTime string</returns>
        /// <remarks>File sortable DateTime string format: yyyyMMdd'T'HHmmss</remarks>
        public static string ToFileSortableDateTime(DateTime value)
        {
            return ToFileSortableDateTime(value, false);
        }

        /// <summary>
        /// Converts a <see cref="DateTime"/> object into a file sortable string format
        /// </summary>
        /// <param name="value"><see cref="DateTime"/> object to convert.</param>
        /// <param name="addMilliseconds">if <see langword="true"/> milliseconds are added to the DateTime string</param>
        /// <returns>File sortable DateTime string</returns>
        /// <remarks>
        /// <para>File sortable DateTime string format (no msec): yyyyMMdd'T'HHmmss</para>
        /// <para>File sortable DateTime string format (with msec): yyyyMMdd'T'HHmmssfff</para>
        /// </remarks>
        public static string ToFileSortableDateTime(DateTime value, bool addMilliseconds)
        {
            if (addMilliseconds)
                return value.ToString("yyyyMMdd'T'HHmmssfff", CultureInfo.CurrentCulture);

            return value.ToString("yyyyMMdd'T'HHmmss", CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Converts an file sortable DateTime string into a <see cref="DateTime"/> object.
        /// </summary>
        /// <param name="value">File sortable DateTime string.</param>
        /// <returns>Converted <see cref="DateTime"/> object</returns>
        /// <remarks>
        /// <para>File sortable DateTime string format (no msec): yyyyMMdd'T'HHmmss</para>
        /// <para>File sortable DateTime string format (with msec): yyyyMMdd'T'HHmmssfff</para>
        /// </remarks>
        /// <exception cref="ArgNullOrEmptyException">Is thrown if <paramref name="value"/> is <see langword="null"/> or empty.</exception>
        /// <exception cref="ArgOutOfRangeException{TValue}">Is thrown if <paramref name="value"/> has the wrong length.</exception>
        public static DateTime FromFileSortableDateTime(string value)
        {
            ArgChecker.ShouldNotBeNullOrEmpty(value, "value");

            string dateTimeString = value.Trim();

            if ((dateTimeString.Length == 17 || dateTimeString.Length == 20) == false)
                throw new ArgOutOfRangeException<string>(value, "value", "Argument {0} error: Wrong datetime string length! (value={1})");

            if (dateTimeString.Length == 17)
                return DateTime.ParseExact(dateTimeString, "yyyyMMdd'T'HHmmss", CultureInfo.CurrentCulture);

            return DateTime.ParseExact(dateTimeString, "yyyyMMdd'T'HHmmssfff", CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Converts an file sortable DateTime string into a <see cref="DateTime"/> object.
        /// </summary>
        /// <param name="value">File sortable DateTime string</param>
        /// <param name="hasMilliseconds">If <see langword="true"/> the <see cref="DateTime"/> string contains milliseconds</param>
        /// <returns>Converted <see cref="DateTime"/> object</returns>
        /// <remarks>
        /// <para>File sortable DateTime string format (no msec): yyyyMMdd'T'HHmmss</para>
        /// <para>File sortable DateTime string format (with msec): yyyyMMdd'T'HHmmssfff</para>
        /// </remarks>
        /// <exception cref="ArgNullOrEmptyException">Is thrown if <paramref name="value"/> is <see langword="null"/> or empty.</exception>
        /// <exception cref="ArgOutOfRangeException{T}">Is thrown if <paramref name="value"/> has the wrong length.</exception>
        public static DateTime FromFileSortableDateTime(string value, bool hasMilliseconds)
        {
            ArgChecker.ShouldNotBeNullOrEmpty(value, "value");

            string dateTimeString = value.Trim();

            if (dateTimeString.Length != 17 && hasMilliseconds == false)
                throw new ArgOutOfRangeException<string>(value, "value", "Argument {0} error: Wrong datetime string length! (value={1})");

            if (dateTimeString.Length != 20 && hasMilliseconds == true)
                throw new ArgOutOfRangeException<string>(value, "value", "Argument {0} error: Wrong datetime string length! (value={1})");

            if (hasMilliseconds)
                return DateTime.ParseExact(dateTimeString, "yyyyMMdd'T'HHmmssfff", CultureInfo.CurrentCulture);

            return DateTime.ParseExact(dateTimeString, "yyyyMMdd'T'HHmmss", CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Converts a DateTime value into a <see cref="DateTime"/> value with a precision of a <see cref="SqlDateTime">SQL Server DateTime type</see>.
        /// </summary>
        /// <param name="value"><see cref="DateTime"/> object that should be converted</param>
        /// <returns>DateTime object with <see cref="SqlDateTime">SQL Server DateTime</see> precision</returns>
        public static DateTime ToSqlServerPrecision(DateTime value)
        {
            SqlDateTime sqlValue = new SqlDateTime(value);

            return sqlValue.Value;
        }

        /// <summary>
        /// Convert a DOS datetime to a <see cref="DateTime"/> object.
        /// </summary>
        /// <param name="dosDate">DOS date value (starting 1980)</param>
        /// <param name="dosTime">DOS time value</param>
        /// <returns>The converted <see cref="DateTime"/> object.</returns>
        public static DateTime DosDateToDateTime(UInt16 dosDate, UInt16 dosTime)
        {
            int year = dosDate / 512 + 1980;
            int month = dosDate % 512 / 32;
            int day = dosDate % 512 % 32;
            int hour = dosTime / 2048;
            int minute = dosTime % 2048 / 32;
            int second = dosTime % 2048 % 32 * 2;

            if (dosDate == UInt16.MaxValue || month == 0 || day == 0)
            {
                year = 1980;
                month = 1;
                day = 1;
            }

            if (dosTime == UInt16.MaxValue)
            {
                hour = minute = second = 0;
            }

            DateTime dt;
            try
            {
                dt = new DateTime(year, month, day, hour, minute, second);
            }
            catch
            {
                dt = new DateTime();
            }
            return dt;
        }

        /// <summary>
        /// Convert a DOS datetime value into a DateTime object.
        /// </summary>
        /// <param name="dosTimestamp">The DOS timestamp value.</param>
        /// <returns>The converted <see cref="DateTime"/> object.</returns>
        public static DateTime DosDateToDateTime(UInt32 dosTimestamp)
        {
            return DosDateToDateTime((UInt16)(dosTimestamp / 65536),
                                     (UInt16)(dosTimestamp % 65536));
        }

        /// <summary>
        /// Convert a DOS datetime value into a DateTime object.
        /// </summary>
        /// <param name="dosTimestamp">The DOS timestamp value.</param>
        /// <returns>The converted <see cref="DateTime"/> object.</returns>
        public static DateTime DosDateToDateTime(Int32 dosTimestamp)
        {
            return DosDateToDateTime((UInt32)dosTimestamp);
        }

        /// <summary>
        /// Converts a DateTime value into a UNIX timestamp.
        /// </summary>
        /// <param name="timestamp">The DateTime to convert.</param>
        /// <returns>UNIX timestamp.</returns>
        public static int ToUnixTimestamp(DateTime timestamp)
        {
            DateTime unixStartTimestamp = new DateTime(1970, 1, 1, 0, 0, 0);

            return (int)((timestamp.ToUniversalTime().Ticks - unixStartTimestamp.Ticks) / 10000000);
        }

        /// <summary>
        /// Converts a UNIX timestamp into a DateTime value.
        /// </summary>
        /// <param name="timestamp">The UNIX timestamp to convert.</param>
        /// <returns>The converted DateTime value.</returns>
        public static DateTime FromUnixTimestamp(int timestamp)
        {
            DateTime unixStartTimestamp = new DateTime(1970, 1, 1, 0, 0, 0);
            return unixStartTimestamp.AddSeconds(timestamp).ToLocalTime();
        }

        /// <summary>
        /// Clones the date time as UTC.
        /// </summary>
        /// <param name="timestamp">The timestamp to clone.</param>
        /// <returns>The cloned <see cref="DateTime"/> object.</returns>
        public static DateTime CloneDateTimeAsUTC(DateTime timestamp)
        {
            return new DateTime(timestamp.Ticks, DateTimeKind.Utc);
        }


        /// <summary>
        /// Gets the last day of the specified year\month combination.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <returns>Last day of the specified year\month combination.</returns>
        /// <exception cref="ArgOutOfRangeException{T}">Is thrown if <paramref name="year"/> or <paramref name="month"/> are out of range.</exception>
        /// <remarks>Uses the <see cref="GregorianCalendar"/> to determine the last day.</remarks>
        public static DateTime GetLastDay(int year, int month)
        {
            if (year < 0)
                throw new ArgOutOfRangeException<int>(year, "year", "Argument {0} error: year value must be greater or equal 0. (value = {1})");
            if (month < 1 || month > 12)
                throw new ArgOutOfRangeException<int>(month, "month", 1, 12);

            // Start at the last day of the month, until we get to the day of the week
            // we were looking for
            DateTime lastDay = new DateTime(year, month, new GregorianCalendar().GetDaysInMonth(year, month));
            return lastDay.Date;
        }

        #endregion

        #region TimeSpan Helper Methods

        /// <summary>
        /// Returns the absolute value of the specified TimeSpan.
        /// </summary>
        /// <param name="val">The val.</param>
        /// <returns></returns>
        public static TimeSpan AbsTimeSpan(TimeSpan val)
        {
            if (IsTimeSpanNegative(val))
            {
                val = val.Negate();
            }
            return val;
        }



        /// <summary>
        /// Determines whether [is time span negative] [the specified time span].
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <returns>
        /// 	<see langword="true"/> if [is time span negative] [the specified time span]; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsTimeSpanNegative(TimeSpan timeSpan)
        {
            return timeSpan.ToString().IndexOf('-') != -1;
        }

        /// <summary>
        /// Converts the time span to double.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <returns></returns>
        public static double ConvertTimeSpanToDouble(string timeSpan)
        {
            return ConvertTimeSpanToDouble(ParseTimeSpan(timeSpan));
        }

        /// <summary>
        /// Converts the time span to integer.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <returns></returns>
        public static double ConvertTimeSpanToDouble(TimeSpan timeSpan)
        {
            return timeSpan.TotalMilliseconds;
        }

        /// <summary>
        /// Tries the parse time span.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public static bool TryParseTimeSpan(string timeSpan, out TimeSpan result)
        {
            return TryParseTimeSpan(timeSpan, TimeSpanAssumption.None, out result);
        }

        /// <summary>
        /// Parses the time span. TimeSpan.Parse does not accept
        /// a plus (+) designator, only minus (-). This parse method
        /// accepts both. Does not throw any exceptions, but returns
        /// false on failure. Return true on success.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <param name="noColonAssumption">The no colon assumption.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public static bool TryParseTimeSpan(string timeSpan, TimeSpanAssumption noColonAssumption, out TimeSpan result)
        {
            if (timeSpan.Length > 1 && timeSpan[0] == '+')
            {
                timeSpan = timeSpan.Substring(1);
            }

            timeSpan = ParseTimeSpanAssumptions(timeSpan, noColonAssumption);

            return TimeSpan.TryParse(timeSpan, out result);
        }

        /// <summary>
        /// Parses the time span.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <returns></returns>
        public static TimeSpan ParseTimeSpan(string timeSpan)
        {
            return ParseTimeSpan(timeSpan, TimeSpanAssumption.None);
        }

        /// <summary>
        /// Parses the time span. TimeSpan.Parse does not accept
        /// a plus (+) designator, only minus (-). This parse method
        /// accepts both.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <param name="noColonAssumption">The no colon assumption.</param>
        /// <returns></returns>
        public static TimeSpan ParseTimeSpan(string timeSpan, TimeSpanAssumption noColonAssumption)
        {
            if (timeSpan.Length > 1 && timeSpan[0] == '+')
            {
                timeSpan = timeSpan.Substring(1);
            }

            timeSpan = ParseTimeSpanAssumptions(timeSpan, noColonAssumption);

            return TimeSpan.Parse(timeSpan);
        }

        private static string ParseTimeSpanAssumptions(string timeSpan, TimeSpanAssumption noColonAssumption)
        {
            //if (timeSpan != null && noColonAssumption != TimeSpanAssumption.None && timeSpan.IndexOf(':') == -1)
            if (noColonAssumption != TimeSpanAssumption.None && !timeSpan.Contains(':'))
            { 
                switch (noColonAssumption)
                {
                    case TimeSpanAssumption.Seconds:
                        timeSpan = "00:00:" + timeSpan;
                        break;
                    case TimeSpanAssumption.Minutes:
                        timeSpan += ":00";
                        break;
                    case TimeSpanAssumption.Hours:
                        timeSpan += ":00:00";
                        break;
                    case TimeSpanAssumption.Days:
                        timeSpan += ".0:00:00:00";
                        break;
                }
            }

            return timeSpan;
        }

        /// <summary>
        /// Returns the value of the TimeSpan as a string, and ensures
        /// that there is a leading character specifying either whether
        /// it is positive or negative.
        /// </summary>
        /// <param name="span">The span.</param>
        /// <returns></returns>
        public static string ToStringTimeSpan(TimeSpan span)
        {
            return (IsTimeSpanNegative(span) ? "" : "+") + span;
        }

        /// <summary>
        /// Returns the value of the TimeSpan as a string, and ensures
        /// that there is a leading character specifying either whether
        /// it is positive or negative.
        /// </summary>
        /// <param name="span">The span.</param>
        /// <returns></returns>
        public static string? ToStringTimeSpan(TimeSpan? span)
        {
            if (span == null) return null;
            return ToStringTimeSpan(span.Value);
        }

        /// <summary>
        /// Trims the time span.
        /// </summary>
        /// <param name="span">The span.</param>
        /// <returns></returns>
        public static string TrimTimeSpan(string span)
        {
            return TrimTimeSpan(span, true);
        }

        /// <summary>
        /// Trims the time span.
        /// </summary>
        /// <param name="span">The span.</param>
        /// <param name="trimZeroMinutes">if set to <see langword="true"/> [trim zero minutes].</param>
        /// <returns></returns>
        public static string TrimTimeSpan(string span, bool trimZeroMinutes)
        {
            if (span.EndsWith(":00"))
            {
                span = span.Substring(0, span.Length - 3);
            }
            if (trimZeroMinutes && span.EndsWith(":00"))
            {
                span = span.Substring(0, span.Length - 3);
            }

            return span;
        }

        #endregion

    }
}
