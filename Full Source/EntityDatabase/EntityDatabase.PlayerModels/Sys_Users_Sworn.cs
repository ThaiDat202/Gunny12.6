using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.PlayerModels;

public class Sys_Users_Sworn
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int ID { get; set; }

	public int UserID { get; set; }

	public int swornId { get; set; }

	public int swornCount { get; set; }

	public int todayGetSwornCount { get; set; }

	public int isHasSwornItem { get; set; }

	public int pveCount { get; set; }

	public int pvpCount { get; set; }

	public int giveCount { get; set; }

	public int skillId { get; set; }

	public string swornUserIDs { get; set; }
}
