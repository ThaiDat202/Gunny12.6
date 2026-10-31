using System.ComponentModel.DataAnnotations;

namespace EntityDatabase.ServerModels;

public class BattleTeamActiveTemplateList
{
	[Key]
	public int Type { get; set; }

	public string ChannelName { get; set; }

	public string ChannelDesc { get; set; }

	public int AddScore { get; set; }

	public int MaxLimit { get; set; }

	public int JumpTo { get; set; }
}
