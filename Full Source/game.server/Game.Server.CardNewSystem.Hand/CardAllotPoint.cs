using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.CardNewSystem.Handle;

[CardNewSystemHandle(3)]
public class CardAllotPoint : ICardNewSystemCommandHandler
{
	public int CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		int num2 = packet.ReadInt();
		int num3 = packet.ReadInt();
		int num4 = packet.ReadInt();
		int num5 = num + num2 + num3 + num4;
		if (num5 > Player.PlayerCharacter.CardInfo.Point)
		{
			Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("CardNew.CardAllotPoint.Failed"));
		}
		Player.PlayerCharacter.CardInfo.Point -= num5;
		Player.PlayerCharacter.CardInfo.Armor += num * 10;
		Player.PlayerCharacter.CardInfo.Crits += num2 * 10;
		Player.PlayerCharacter.CardInfo.Trick += num3 * 10;
		Player.PlayerCharacter.CardInfo.Viole += num4 * 10;
		Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("CardNew.CardAllotPoint.Success"));
		Player.UpdateCardInfo();
		CardNewSystemMgr.Out.SendCardInfo(Player);
		return 0;
	}
}
