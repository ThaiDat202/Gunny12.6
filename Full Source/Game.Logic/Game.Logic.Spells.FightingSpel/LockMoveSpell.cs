using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic.Spells.FightingSpell;

[SpellAttibute(87)]
public class LockMoveSpell : ISpellHandler
{
	public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
	{
		if (player.IsLiving)
		{
			player.LockMove = true;
		}
		else if (game.CurrentLiving != null && game.CurrentLiving is Player && game.CurrentLiving.Team == player.Team)
		{
			game.CurrentLiving.LockMove = true;
		}
	}
}
