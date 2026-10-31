using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.PlayerModels;

public class Sys_Users_SecondPro
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int ID { get; set; }

	public int UserID { get; set; }

	public int Crit { get; set; }

	public int Tenacity { get; set; }

	public int SunderArmor { get; set; }

	public int AvoidInjury { get; set; }

	public int Kill { get; set; }

	public int WillFight { get; set; }

	public int ViolenceInjury { get; set; }

	public int Speed { get; set; }

	public int Guard { get; set; }
}
