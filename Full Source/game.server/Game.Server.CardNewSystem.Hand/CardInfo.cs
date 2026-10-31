using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.CardNewSystem.Handle;

[CardNewSystemHandle(1)]
public class CardInfo : ICardNewSystemCommandHandler
{
	public int CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		CardNewSystemMgr.Out.SendCardInfo(Player);
		return 0;
	}
}
