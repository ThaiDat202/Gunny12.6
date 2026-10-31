using System;
using System.Collections.Generic;

namespace Game.Server.ConsortiaTask.Data;

public class ConsortiaTaskInfo
{
	public int ID { get; set; }

	public int Experience { get; set; }

	public int Offer { get; set; }

	public int Contribution { get; set; }

	public int Riches { get; set; }

	public int BuffID { get; set; }

	public DateTime BeginTime { get; set; }

	public int Time { get; set; }

	public int Level { get; set; }

	public int Points { get; set; }

	public bool Completed { get; set; }

	public List<ConsortiaTaskConditionInfo> Conditions { get; set; }
}
