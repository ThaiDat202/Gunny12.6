using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.PlayerModels;

public class Sys_Users_TeamActive
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int ID { get; set; }

	public int teamID { get; set; }

	public string NickName { get; set; }

	public int UserActive { get; set; }

	public int ActiveType { get; set; }
}
