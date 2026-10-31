using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.ServerModels;

public class God_Of_Wealth_Reward
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int ID { get; set; }

	public int NeedMoney { get; set; }

	public int AwardMin { get; set; }

	public int AwardMax { get; set; }
}
