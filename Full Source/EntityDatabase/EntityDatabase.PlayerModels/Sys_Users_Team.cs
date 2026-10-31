using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.PlayerModels;

public class Sys_Users_Team
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int teamID { get; set; }

	public string teamName { get; set; }

	public string teamTag { get; set; }

	public int teamGrade { get; set; }

	public int teamWinTime { get; set; }

	public int teamTotalTime { get; set; }

	public int teamDivision { get; set; }

	public int teamScore { get; set; }

	public int teamDuty { get; set; }

	public int teamPersonalScore { get; set; }

	public int freeInvitedUsedCnt { get; set; }

	public DateTime createDate { get; set; }

	public DateTime teamLoginDate { get; set; }

	public int totalMember { get; set; }

	public int totalActive { get; set; }

	public int season { get; set; }

	public int active { get; set; }

	public string AreaName { get; set; }
}
