using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.PlayerModels;

public class User_GmActivityCondition
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int ID { get; set; }

	public int UserID { get; set; }

	public string ActivityID { get; set; }

	public string GiftBagID { get; set; }

	public int StatusID { get; set; }

	public int StatusValue { get; set; }
}
