using System.Linq;
using Bussiness;
using EntityDatabase.PlayerModels;
using EntityDatabase.ServerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.CardNewSystem.Handle;

[CardNewSystemHandle(6)]
public class CardActiveSuits : ICardNewSystemCommandHandler
{
	public int CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int id = packet.ReadInt();
		TS_CardSuit info = CardNewSystemMgr.CardSuits.FirstOrDefault((TS_CardSuit p) => p.SuitTemplateId == id);
		if (info == null || Player.PlayerCharacter.ActiveCards.FirstOrDefault((Sys_Users_CardActive p) => p.MainID == info.SuitTemplateId) != null)
		{
			Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("CardNew.CardActiveSuits.ServerError"));
			return 0;
		}
		Sys_Users_CardActive item = new Sys_Users_CardActive
		{
			UserID = Player.PlayerId,
			MainID = info.SuitTemplateId,
			IsActive = false
		};
		Player.PlayerCharacter.ActiveCards.Add(item);
		CardNewSystemMgr.Out.SendCardInfo(Player);
		Player.UpdateCardInfo();
		Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("CardNew.CardActiveSuits.Success"));
		return 0;
	}
}
