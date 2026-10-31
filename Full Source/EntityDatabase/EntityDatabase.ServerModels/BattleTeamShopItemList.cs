using System.ComponentModel.DataAnnotations;

namespace EntityDatabase.ServerModels;

public class BattleTeamShopItemList
{
	[Key]
	public int ID { get; set; }

	public int ShopType { get; set; }

	public int NeedLevel { get; set; }

	public int Price { get; set; }

	public int Condition { get; set; }

	public int Value { get; set; }
}
