using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.ServerModels;

public class GM_Activity
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public string activityId { get; set; }

	public string activityName { get; set; }

	public int activityType { get; set; }

	public int activityChildType { get; set; }

	public int getWay { get; set; }

	public string desc { get; set; }

	public string rewardDesc { get; set; }

	public DateTime beginTime { get; set; }

	public DateTime beginShowTime { get; set; }

	public DateTime endTime { get; set; }

	public DateTime endShowTime { get; set; }

	public int icon { get; set; }

	public int isContinue { get; set; }

	public int status { get; set; }

	public int remain1 { get; set; }

	public string remain2 { get; set; }

	public bool isActive { get; set; }
}
