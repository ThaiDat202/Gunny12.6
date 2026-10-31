using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.ServerModels;

public class StrengThenExp
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public int ID { get; set; }

	public int Level { get; set; }

	public int Exp { get; set; }

	public int NecklaceStrengthExp { get; set; }

	public int NecklaceStrengthPlus { get; set; }
}
