using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.ServerModels;

public class Pet_Special_Skills
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public int ID { get; set; }

	public int SkillID { get; set; }

	public int KindID { get; set; }

	public string PetName { get; set; }

	public string SkillName { get; set; }

	public string Description { get; set; }

	public int P1 { get; set; }

	public int P2 { get; set; }

	public int P3 { get; set; }

	public int P4 { get; set; }

	public bool IsPVP { get; set; }

	public bool IsAwaken { get; set; }

	public string DeleteSkillIDs { get; set; }
}
