using Game.Logic.Effects;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic.Spells.FightingSpell;

[SpellAttibute(85)]
public class PowerNextTurnSpell : ISpellHandler
{
	public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
	{
		if (player.IsLiving)
		{
			new PowerNextTurn(1, 200).Start(player);
		}
		else if (game.CurrentLiving != null && game.CurrentLiving is Player && game.CurrentLiving.Team == player.Team)
		{
			new PowerNextTurn(1, 200).Start(game.CurrentLiving);
		}
	}
}
