using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.PlayerModels;

public class Sys_Users_Property
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int ID { get; set; }

	public int UserID { get; set; }

	public string Texp { get; set; }

	public string Card { get; set; }

	public string Pet { get; set; }

	public string Suit { get; set; }

	public string Gem { get; set; }

	public string Bead { get; set; }

	public string Avatar { get; set; }

	public string MagicStone { get; set; }

	public string Temp { get; set; }

	public string PetAtlas { get; set; }

	public string Horse { get; set; }

	public string HorsePicCherish { get; set; }

	public string Enchant { get; set; }

	public string CardAchievement { get; set; }

	public string Mark { get; set; }

	public string marKing { get; set; }

	public string titleAdd { get; set; }

	public string magicHouse { get; set; }

	public string SeloPet { get; set; }
}
