using System.ComponentModel.DataAnnotations;

namespace EntityDatabase.ServerModels;

public class TS_HomeTempAdvance_Template
{
	[Key]
	public int Level { get; set; }

	public int Item1 { get; set; }

	public int Count1 { get; set; }

	public int Item2 { get; set; }

	public int Count2 { get; set; }

	public int Blood { get; set; }

	public int MagicDefend { get; set; }

	public int Toughness { get; set; }

	public int AvoidInjury { get; set; }

	public int TricRevolt { get; set; }

	public int Guardian { get; set; }

	public string Name { get; set; }
}
