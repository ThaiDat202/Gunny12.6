using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.PlayerModels;

public class Sys_Users_Team_User
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int ID { get; set; }

	public int teamID { get; set; }

	public int UserID { get; set; }

	public int ActiveWeekScore { get; set; }

	public int ActiveTotalScore { get; set; }

	public int ActiveSeasonScore { get; set; }

	public int BattleScore { get; set; }

	public bool IsSignToday { get; set; }

	public bool IsCaption { get; set; }

	public DateTime CreateDate { get; set; }
}
