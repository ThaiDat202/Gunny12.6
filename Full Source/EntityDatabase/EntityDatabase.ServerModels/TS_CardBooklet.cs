using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.ServerModels;

public class TS_CardBooklet
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public int ID { get; set; }

	public int TemplateId { get; set; }

	public string TemplateName { get; set; }

	public int Profile { get; set; }

	public int Weight { get; set; }

	public int Hp { get; set; }

	public int Attack { get; set; }

	public int Defence { get; set; }

	public int Agility { get; set; }

	public int Lucky { get; set; }

	public int MagicAttack { get; set; }

	public int MagicDefence { get; set; }

	public int Crit { get; set; }

	public int SunderArmor { get; set; }

	public int ViolenceInjury { get; set; }

	public int Speed { get; set; }

	public int TricKill { get; set; }

	public int Damage { get; set; }

	public int Armor { get; set; }

	public int RecyclCount { get; set; }

	public string Desc { get; set; }
}
