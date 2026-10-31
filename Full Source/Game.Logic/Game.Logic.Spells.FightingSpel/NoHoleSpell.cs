using Game.Logic.Effects;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic.Spells.FightingSpell;

[SpellAttibute(9)]
public class NoHoleSpell : ISpellHandler
{
	public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
	{
		if (player.IsLiving)
		{
			player.IsNoHole = true;
			player.Game.SendPlayerPicture(player, 5, state: true);
			new NoHole_Active(item.Property3).Start(player);
		}
		else if (game.CurrentLiving != null && game.CurrentLiving is Player && game.CurrentLiving.Team == player.Team)
		{
			game.CurrentLiving.IsNoHole = true;
			game.CurrentLiving.Game.SendPlayerPicture(game.CurrentLiving, 5, state: true);
			new NoHole_Active(item.Property3).Start(game.CurrentLiving);
		}
	}
}
