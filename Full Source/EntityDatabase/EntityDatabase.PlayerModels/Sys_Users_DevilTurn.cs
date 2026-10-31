using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.PlayerModels;

public class Sys_Users_DevilTurn
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int ID { get; set; }

	public int UserID { get; set; }

	public int lotteryCount { get; set; }

	public int myRankScore { get; set; }

	public int beadCount1 { get; set; }

	public int beadCount2 { get; set; }

	public int beadCount3 { get; set; }

	public int beadCount4 { get; set; }

	public int beadCount5 { get; set; }

	public string getGiftProgress { get; set; }

	public int freeTimes { get; set; }

	public DateTime freeDate { get; set; }
}
