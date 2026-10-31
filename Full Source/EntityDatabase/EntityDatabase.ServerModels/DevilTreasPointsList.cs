using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.ServerModels;

public class DevilTreasPointsList
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public int ID { get; set; }

	public int Points { get; set; }

	public int TemplateID { get; set; }
}
