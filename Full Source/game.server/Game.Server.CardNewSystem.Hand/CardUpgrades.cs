using System;
using System.Linq;
using Bussiness;
using EntityDatabase.ServerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.CardNewSystem.Handle;

[CardNewSystemHandle(2)]
public class CardUpgrades : ICardNewSystemCommandHandler
{
	public int CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		TS_CardMain tS_CardMain = CardNewSystemMgr.CardMains.FirstOrDefault((TS_CardMain p) => p.Level == Player.PlayerCharacter.CardInfo.CardLevel + 1);
		if (tS_CardMain == null)
		{
			throw new NullReferenceException($"nextlv is null {Player.PlayerCharacter.CardInfo.CardLevel + 1}");
		}
		if (Player.PlayerCharacter.CardSoul >= tS_CardMain.NeedItem1Count && Player.PlayerCharacter.Gold >= tS_CardMain.NeedItem2Count)
		{
			Player.RemoveCardSoul(tS_CardMain.NeedItem1Count);
			Player.RemoveGold(tS_CardMain.NeedItem2Count);
			Player.PlayerCharacter.CardInfo.CardLevel = tS_CardMain.Level;
			Player.PlayerCharacter.CardInfo.Point += tS_CardMain.Point;
			Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("CardNew.CardUpgrades.Success"));
			CardNewSystemMgr.Out.SendCardInfo(Player);
			CardNewSystemMgr.Out.SendCardSoul(Player);
			return 0;
		}
		Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("CardNew.CardUpgrades.Failed"));
		return 0;
	}
}
