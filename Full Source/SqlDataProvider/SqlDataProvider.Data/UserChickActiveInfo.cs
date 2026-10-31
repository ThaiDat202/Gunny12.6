using System;

namespace SqlDataProvider.Data;

public class UserChickActiveInfo
{
	public int IsKeyOpened { get; set; }

	public int KeyOpenedType { get; set; }

	public DateTime KeyOpenedTime { get; set; }

	public DateTime EveryDay { get; set; }

	public DateTime Weekly { get; set; }

	public DateTime AfterThreeDays { get; set; }

	public int CurrentLvAward { get; set; }

	public DateTime StartOfWeek(DateTime dt, DayOfWeek startOfWeek)
	{
		int num = dt.DayOfWeek - startOfWeek;
		if (num < 0)
		{
			num += 7;
		}
		return dt.AddDays(-1 * num).Date;
	}

	public void Active(int type)
	{
		IsKeyOpened = 1;
		KeyOpenedType = type;
		KeyOpenedTime = DateTime.Now;
		EveryDay = DateTime.Now.AddDays(-1.0);
		AfterThreeDays = DateTime.Now.AddDays(-1.0);
		Weekly = StartOfWeek(DateTime.Now, DayOfWeek.Saturday).AddDays(-7.0);
	}

	public bool OnThreeDay(DateTime dt)
	{
		if (dt.DayOfWeek == DayOfWeek.Friday || dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday)
		{
			return true;
		}
		return false;
	}
}
