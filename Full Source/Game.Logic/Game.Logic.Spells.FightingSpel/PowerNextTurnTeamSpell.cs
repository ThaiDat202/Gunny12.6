using System.Collections.Generic;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic.Spells.FightingSpell;

[SpellAttibute(86)]
public class PowerNextTurnTeamSpell : ISpellHandler
{
	public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
	{
		List<Player> allTeamPlayers = player.Game.GetAllTeamPlayers(player);
		foreach (Player item2 in allTeamPlayers)
		{
			new PowerNextTurn(1, 100).Start(item2);
		}
	}
}
