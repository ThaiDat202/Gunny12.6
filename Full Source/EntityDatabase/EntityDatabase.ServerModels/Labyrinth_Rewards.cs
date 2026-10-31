using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.ServerModels;

public class Labyrinth_Rewards
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public int ID { get; set; }

	public int HardType { get; set; }

	public int Level { get; set; }

	public int MissionID { get; set; }

	public string FirstRewards { get; set; }

	public string Buffs { get; set; }

	public string Rewards { get; set; }

	public int Rank { get; set; }
}
