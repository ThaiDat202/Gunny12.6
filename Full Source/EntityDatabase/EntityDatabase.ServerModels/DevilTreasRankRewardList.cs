using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.ServerModels;

public class DevilTreasRankRewardList
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public int ID { get; set; }

	public int RankMin { get; set; }

	public int RankMax { get; set; }

	public string Desc { get; set; }

	public float Percent { get; set; }

	public int Reward { get; set; }
}
