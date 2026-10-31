using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic.Spells.FightingSpell;

[SpellAttibute(82)]
public class TiredShootSpell : ISpellHandler
{
	public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
	{
		if (player.IsLiving)
		{
			player.TiredShoot = true;
		}
		else if (game.CurrentLiving != null && game.CurrentLiving is Player && game.CurrentLiving.Team == player.Team)
		{
			game.CurrentLiving.TiredShoot = true;
		}
	}
}
