using Game.Base.Packets;
using Game.Logic.Phy.Object;

namespace Game.Logic.Cmd;

[GameCommand(130, "战胜关卡中Boss翻牌")]
public class BossTakeCardCommand : ICommandHandler
{
	public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
	{
		int num = packet.ReadByte();
		if (game is PVEGame)
		{
			PVEGame pVEGame = (PVEGame)game;
			if (pVEGame.BossCardCount + 1 > 0)
			{
				if ((num < 0 || num > pVEGame.BossCards.Length) && pVEGame.IsBossWar != "")
				{
					pVEGame.TakeBossCard(player);
				}
				else
				{
					pVEGame.TakeCard(player, num, isAuto: false);
				}
			}
		}
		else if (game is PVPGame && num == 100)
		{
			game.Stop();
		}
	}
}
