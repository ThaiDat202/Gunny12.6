using Bussiness;
using EntityDatabase.PlayerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.CardNewSystem.Handle;

[CardNewSystemHandle(7)]
public class CardEquipSuits : ICardNewSystemCommandHandler
{
	public int CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		foreach (Sys_Users_CardActive activeCard in Player.PlayerCharacter.ActiveCards)
		{
			activeCard.IsActive = activeCard.MainID == num;
		}
		CardNewSystemMgr.Out.SendCardInfo(Player);
		Player.UpdateCardInfo();
		Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("CardNew.CardEquipSuits.Success"));
		return 0;
	}
}
