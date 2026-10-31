using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic.Spells.FightingSpell;

[SpellAttibute(81)]
public class BombFoulSpell : ISpellHandler
{
	public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
	{
		if (player.IsLiving)
		{
			player.BombFoul = true;
		}
		else if (game.CurrentLiving != null && game.CurrentLiving is Player && game.CurrentLiving.Team == player.Team)
		{
			game.CurrentLiving.BombFoul = true;
		}
	}
}
