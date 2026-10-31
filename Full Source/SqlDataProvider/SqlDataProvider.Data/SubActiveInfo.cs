using System;

namespace SqlDataProvider.Data;

public class SubActiveInfo
{
	public int ID { get; set; }

	public int ActiveID { get; set; }

	public int SubID { get; set; }

	public bool IsOpen { get; set; }

	public DateTime StartDate { get; set; }

	public DateTime StartTime { get; set; }

	public DateTime EndDate { get; set; }

	public DateTime EndTime { get; set; }

	public bool IsContinued { get; set; }

	public string ActiveInfo { get; set; }

	public bool IsValid()
	{
		if (StartDate.Date < DateTime.Now.Date && EndDate.Date > DateTime.Now.Date)
		{
			return true;
		}
		return false;
	}

	public bool OnActive(int Id, int subId)
	{
		return ActiveID == Id && SubID == subId;
	}
}
