using System.ComponentModel.DataAnnotations;

namespace EntityDatabase.ServerModels;

public class BattleTeamLevelList
{
	[Key]
	public int Level { get; set; }

	public int MaxPlayerNum { get; set; }

	public int NeedActive { get; set; }

	public int BuffParam { get; set; }

	public int BuffTwoParam { get; set; }
}
