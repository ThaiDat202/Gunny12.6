using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.PlayerModels;

public class Sys_Users_DevilTurn_Rank
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int ID { get; set; }

	public int UserID { get; set; }

	public string NickName { get; set; }

	public int Score { get; set; }

	public int ServerId { get; set; }

	public string ServerName { get; set; }
}
