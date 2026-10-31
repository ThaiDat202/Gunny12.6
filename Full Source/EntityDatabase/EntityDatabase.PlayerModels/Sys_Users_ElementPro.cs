using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.PlayerModels;

public class Sys_Users_ElementPro
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int ID { get; set; }

	public int UserID { get; set; }

	public int FireAttack { get; set; }

	public int WaterAttack { get; set; }

	public int WindAttack { get; set; }

	public int LandAttack { get; set; }

	public int FireDefence { get; set; }

	public int WaterDefence { get; set; }

	public int WindDefence { get; set; }

	public int LandDefence { get; set; }
}
