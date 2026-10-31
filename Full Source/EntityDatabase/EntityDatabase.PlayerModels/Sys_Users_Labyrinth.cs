using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.PlayerModels;

public class Sys_Users_Labyrinth
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int ID { get; set; }

	public int UserID { get; set; }

	public int maxWeekFinishLevel { get; set; }

	public int currentFinishLevel { get; set; }

	public int sweepTotalTimeRemaining { get; set; }

	public int sweepTotalExp { get; set; }

	public int sweepTotalTime { get; set; }

	public int sweepNeedMoney { get; set; }

	public bool isUseDoubleProp { get; set; }

	public bool isPlayGame { get; set; }

	public bool isSweep { get; set; }

	public bool isMultiple { get; set; }

	public string buffList { get; set; }

	public int nowContinueCount { get; set; }

	public int maxFinishLevelTurn { get; set; }

	public int maxFinishLevel { get; set; }

	public int tryAgainMoney { get; set; }

	public DateTime LastDate { get; set; }

	public string ProcessAward { get; set; }
}
