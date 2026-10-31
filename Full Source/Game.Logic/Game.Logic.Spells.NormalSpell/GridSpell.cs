using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic.Spells.NormalSpell;

[SpellAttibute(129)]
public class GridSpell : ISpellHandler
{
	public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
	{
		if (player.IsLiving)
		{
			player.SetBall(129);
		}
	}
}
