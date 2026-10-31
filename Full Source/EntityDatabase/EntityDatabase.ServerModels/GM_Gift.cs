using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.ServerModels;

public class GM_Gift
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public string giftbagId { get; set; }

	public string activityId { get; set; }

	public int rewardMark { get; set; }

	public int giftbagOrder { get; set; }
}
