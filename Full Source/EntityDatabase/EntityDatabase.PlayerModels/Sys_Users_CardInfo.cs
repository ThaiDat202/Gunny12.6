using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.PlayerModels;

public class Sys_Users_CardInfo
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int ID { get; set; }

	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public int UserID { get; set; }

	public int CardLevel { get; set; }

	public int Point { get; set; }

	public int Armor { get; set; }

	public int Crits { get; set; }

	public int Trick { get; set; }

	public int Viole { get; set; }

	public int Attack { get; set; }

	public int Defence { get; set; }

	public int Agility { get; set; }

	public int Lucky { get; set; }

	public int SuitLevel { get; set; }

	public string CardMainStr { get; set; }

	public string CardSuitStr { get; set; }
}
