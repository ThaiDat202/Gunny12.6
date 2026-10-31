using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.ServerModels;

public class TS_HonorSystem_Template
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public int Level { get; set; }

	public string Name { get; set; }

	public int Exp { get; set; }

	public int Blood { get; set; }

	public int StrengthRate { get; set; }

	public int AdvanceRate { get; set; }

	public int GoldRate { get; set; }

	public int SpiritRate { get; set; }

	public int FusionRate { get; set; }

	public int LevelGift { get; set; }
}
