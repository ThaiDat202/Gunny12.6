using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.ServerModels;

public class TS_NecklaceCasting
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public int Level { get; set; }

	public int NeedItemCount1 { get; set; }

	public int NeedItemCount2 { get; set; }

	public int HP { get; set; }

	public int Toughness { get; set; }

	public int AvoidInjury { get; set; }

	public int TricRevolt { get; set; }

	public int Guardian { get; set; }
}
