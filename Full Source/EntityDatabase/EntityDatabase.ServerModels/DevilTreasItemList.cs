using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.ServerModels;

public class DevilTreasItemList
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public int ID { get; set; }

	public int Type { get; set; }

	public int TemplateID { get; set; }

	public int Value { get; set; }

	public int Random { get; set; }
}
