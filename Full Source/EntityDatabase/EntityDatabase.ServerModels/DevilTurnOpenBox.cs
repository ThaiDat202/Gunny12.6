using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.ServerModels;

public class DevilTurnOpenBox
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public int ID { get; set; }

	public int BoxId { get; set; }

	public int BoxIndex { get; set; }

	public int NeedMoney { get; set; }
}
