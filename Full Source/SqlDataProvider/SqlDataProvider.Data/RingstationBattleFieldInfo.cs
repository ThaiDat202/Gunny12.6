using System;

namespace SqlDataProvider.Data;

public class RingstationBattleFieldInfo : DataObject
{
	public int ID { get; set; }

	public int UserID { get; set; }

	public bool DareFlag { get; set; }

	public string UserName { get; set; }

	public bool SuccessFlag { get; set; }

	public int Level { get; set; }

	public DateTime BattleTime { get; set; }
}
