using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.ServerModels;

public class GM_Active_Reward
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public string giftId { get; set; }

	public int templateId { get; set; }

	public int count { get; set; }

	public int isBind { get; set; }

	public int occupationOrSex { get; set; }

	public int rewardType { get; set; }

	public int validDate { get; set; }

	public string property { get; set; }

	public string remain1 { get; set; }

	public int allGiftGetTimes { get; set; }
}
