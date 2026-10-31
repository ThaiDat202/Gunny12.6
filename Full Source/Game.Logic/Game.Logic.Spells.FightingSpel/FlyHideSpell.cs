using Game.Logic.Effects;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic.Spells.FightingSpell;

[SpellAttibute(80)]
public class FlyHideSpell : ISpellHandler
{
	public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
	{
		if (player.IsLiving)
		{
			new HideEffect(item.Property3).Start(player);
			player.SetBall(3);
		}
		else if (game.CurrentLiving != null && game.CurrentLiving is Player && game.CurrentLiving.Team == player.Team)
		{
			new HideEffect(item.Property3).Start(game.CurrentLiving);
			(game.CurrentLiving as Player).SetBall(3);
		}
	}
}
