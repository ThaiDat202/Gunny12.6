using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.PlayerModels;

public class Sys_Users_Emblem
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int ID { get; set; }

	public int UserID { get; set; }

	public int tempId { get; set; }

	public bool isExist { get; set; }

	public int equipPos { get; set; }

	public int equipIndex { get; set; }

	public int mainType { get; set; }

	public int mainValue { get; set; }

	public string strvalue { get; set; }
}
