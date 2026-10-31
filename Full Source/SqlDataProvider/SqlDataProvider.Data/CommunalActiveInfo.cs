using System;

namespace SqlDataProvider.Data;

public class CommunalActiveInfo
{
	public int ActiveID { get; set; }

	public DateTime BeginTime { get; set; }

	public DateTime EndTime { get; set; }

	public int LimitGrade { get; set; }

	public int DayMaxScore { get; set; }

	public int MinScore { get; set; }

	public string AddPropertyByMoney { get; set; }

	public string AddPropertyByProp { get; set; }

	public bool CanReset { get; set; }

	public bool CanSendAward { get; set; }

	public bool IsOpen { get; set; }

	public bool CanAwardArea { get; set; }
}
