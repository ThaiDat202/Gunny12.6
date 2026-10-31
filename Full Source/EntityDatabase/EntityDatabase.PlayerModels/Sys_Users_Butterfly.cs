using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.PlayerModels;

public class Sys_Users_Butterfly
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int ID { get; set; }

	public int UserID { get; set; }

	public int isHasButterfly { get; set; }

	public int butterflyExp { get; set; }

	public bool isGetLevelReward { get; set; }

	public int MoonAll { get; set; }

	public int feedCount { get; set; }

	public int touchCount { get; set; }

	public int curTaskID { get; set; }

	public int taskState { get; set; }

	public int level { get; set; }

	public DateTime taskDate { get; set; }
}
