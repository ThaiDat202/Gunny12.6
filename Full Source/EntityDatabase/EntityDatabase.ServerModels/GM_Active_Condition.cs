using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.ServerModels;

public class GM_Active_Condition
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public string giftbagId { get; set; }

	public int conditionIndex { get; set; }

	public int conditionValue { get; set; }

	public int remain1 { get; set; }

	public string remain2 { get; set; }
}
