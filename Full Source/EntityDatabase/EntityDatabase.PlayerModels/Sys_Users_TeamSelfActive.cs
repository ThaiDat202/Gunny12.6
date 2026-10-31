using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.PlayerModels;

public class Sys_Users_TeamSelfActive
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int ID { get; set; }

	public int UserID { get; set; }

	public int haveScore { get; set; }

	public int Times { get; set; }

	public int Type { get; set; }

	public DateTime LastDate { get; set; }
}
