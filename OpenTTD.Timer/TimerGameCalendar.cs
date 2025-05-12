namespace OpenTTD.Timer;

/// <summary>
/// Timer that is increased every 27ms, and counts towards ticks / days / months / years.<br/>
/// <br/>
/// The amount of days in a month depends on the month and year (leap-years).<br/>
/// There are always 74 ticks in a day (and with 27ms, this makes 1 day 1.998 seconds).<br/>
/// <br/>
/// Calendar time is used for technology and time-of-year changes, including:<br/>
/// - Vehicle, airport, station, object introduction and obsolescence<br/>
/// - NewGRF variables for visual styles or behavior based on year or time of year (e.g. variable snow line)<br/>
/// - Inflation, since it is tied to original game years. One interpretation of inflation is that it compensates for faster and higher capacity vehicles,<br/>
/// another is that it compensates for more established companies. Each of these point to a different choice of calendar versus economy time, but we have to pick one<br/>
/// so we follow a previous decision to tie inflation to original TTD game years.
/// </summary>
public class TimerGameCalendar : TimerGame<Calendar>
{
    public static Year Year; // Current year, starting at 0
    public static Month Month; // Current month (0..11)
    public static Date Date; // Current date in days (day counter)
    public static DateFract DateFract; // Fractional part of the day
    public static ushort SubDateFract; // Subpart of DateFract that we use when calendar days are slower than economy days

    public static YearMonthDay ConvertDateToYMD(Date date)
    {

    }

    public static Date ConvertYMDToDate(Year year, Month month, Date date)
    {

    }

    public static void SetDate(Date date, DateFract fract)
    {

    }
}

/// <summary>
/// Storage class for Calendar time constants.
/// </summary>
public class CalendarTime : TimerGameConst<Calendar>
{
    public const int DEF_MINUTES_PER_YEAR = 12;
    public const int FROZEN_MINUTES_PER_YEAR = 0;
    public const int MAX_MINUTES_PER_YEAR = 10080; // One week of realtime, the actual max that doesn't overflow TimerGameCalendar.SubDateFract is 10627, but this is neater
}