using System.ComponentModel.DataAnnotations;

namespace EntityDatabase.ServerModels;

public class TS_ButterflyTask
{
	[Key]
	public int TaskId { get; set; }

	public string TaskName { get; set; }

	public int GradeMin { get; set; }

	public int GradeMax { get; set; }

	public int NeedFeeling { get; set; }

	public int NeedTime { get; set; }

	public int RewardGp { get; set; }

	public int RewardItemId { get; set; }

	public string TaskDescr { get; set; }
}
