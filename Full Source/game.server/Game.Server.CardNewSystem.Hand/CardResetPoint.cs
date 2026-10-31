using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.CardNewSystem.Handle;

[CardNewSystemHandle(4)]
public class CardResetPoint : ICardNewSystemCommandHandler
{
	public int CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		if (Player.PlayerCharacter.Money >= GameProperties.CardGrooveRevert)
		{
			Player.RemoveMoney(GameProperties.CardGrooveRevert);
			Player.PlayerCharacter.CardInfo.Point += (Player.PlayerCharacter.CardInfo.Armor + Player.PlayerCharacter.CardInfo.Crits + Player.PlayerCharacter.CardInfo.Trick + Player.PlayerCharacter.CardInfo.Viole) / 10;
			Player.PlayerCharacter.CardInfo.Armor = 0;
			Player.PlayerCharacter.CardInfo.Crits = 0;
			Player.PlayerCharacter.CardInfo.Trick = 0;
			Player.PlayerCharacter.CardInfo.Viole = 0;
			Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("CardNew.CardResetPoint.Success"));
			Player.UpdateCardInfo();
			CardNewSystemMgr.Out.SendCardInfo(Player);
		}
		else
		{
			Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("CardNew.CardResetPoint.failed"));
		}
		return 0;
	}
}
