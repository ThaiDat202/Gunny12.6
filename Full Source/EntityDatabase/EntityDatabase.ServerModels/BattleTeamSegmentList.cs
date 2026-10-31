using System.ComponentModel.DataAnnotations;

namespace EntityDatabase.ServerModels;

public class BattleTeamSegmentList
{
	[Key]
	public int SegmentID { get; set; }

	public string SegmentName { get; set; }

	public int NeedScore { get; set; }

	public string Icon { get; set; }

	public int GiftBagId { get; set; }
}
