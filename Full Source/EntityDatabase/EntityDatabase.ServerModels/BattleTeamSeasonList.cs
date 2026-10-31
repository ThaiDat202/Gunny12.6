using System;
using System.ComponentModel.DataAnnotations;

namespace EntityDatabase.ServerModels;

public class BattleTeamSeasonList
{
	[Key]
	public int SeasonId { get; set; }

	public DateTime? StartDate { get; set; }

	public DateTime? EndDate { get; set; }

	public string SubDate { get; set; }

	public string DayHour { get; set; }

	public int AwardWeek { get; set; }

	public string RankGift { get; set; }
}
