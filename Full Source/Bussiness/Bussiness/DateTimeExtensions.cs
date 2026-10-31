using System;
using System.Globalization;

namespace Bussiness;

public static class DateTimeExtensions
{
	private static GregorianCalendar _gc = new GregorianCalendar();

	public static int GetWeekOfMonth(this DateTime time)
	{
		DateTime time2 = new DateTime(time.Year, time.Month, 1);
		return time.GetWeekOfYear() - time2.GetWeekOfYear() + 1;
	}

	private static int WeekOfYear(this DateTime time)
	{
		return _gc.GetWeekOfYear(time, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
	}

	public static int GetWeekOfYear(this DateTime time)
	{
		CultureInfo cultureInfo = new CultureInfo("vi-VN");
		Calendar calendar = cultureInfo.Calendar;
		CalendarWeekRule calendarWeekRule = cultureInfo.DateTimeFormat.CalendarWeekRule;
		DayOfWeek firstDayOfWeek = cultureInfo.DateTimeFormat.FirstDayOfWeek;
		return calendar.GetWeekOfYear(time, calendarWeekRule, firstDayOfWeek);
	}

	public static DateTime GetStartNextMonth(this DateTime time, int totalMonth)
	{
		DateTime dateTime = time.AddMonths(totalMonth);
		return new DateTime(dateTime.Year, dateTime.Month, 1).Date;
	}
}
