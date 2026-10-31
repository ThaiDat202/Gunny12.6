using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Statics;

namespace Game.Server.Labyrinth.Handle;

[LabyrinthHandle(9)]
public class TryAgain : ILabyrinthCommandHandler
{
	public int CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		packet.ReadInt();
		bool flag = packet.ReadBoolean();
		bool flag2 = packet.ReadBoolean();
		if (flag)
		{
			if (!Player.MoneyDirect((!flag2) ? MoneyType.Money : MoneyType.DDTMoney, Player.PlayerCharacter.LabyrinthInfo.tryAgainMoney))
			{
				return 0;
			}
			Player.PlayerCharacter.LabyrinthInfo.isPlayGame = false;
			Player.SendMessage(LanguageMgr.GetTranslation("Labyrinth.TryAgain.Success"));
		}
		else
		{
			Player.SendMessage(LanguageMgr.GetTranslation("Labyrinth.TryAgain.Failed"));
		}
		return 0;
	}
}
