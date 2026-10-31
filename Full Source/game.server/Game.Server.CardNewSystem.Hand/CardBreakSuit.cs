using System.Linq;
using Bussiness;
using EntityDatabase.ServerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.CardNewSystem.Handle;

[CardNewSystemHandle(8)]
public class CardBreakSuit : ICardNewSystemCommandHandler
{
	public int CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		TS_UpgradeTemplate tS_UpgradeTemplate = CardNewSystemMgr.UpgradeTemplate.FirstOrDefault((TS_UpgradeTemplate p) => p.Types == 105 && p.Grades == Player.PlayerCharacter.CardInfo.SuitLevel);
		if (tS_UpgradeTemplate != null)
		{
			if (Player.PlayerCharacter.CardSoul >= tS_UpgradeTemplate.Param1 && Player.PropBag.GetAllItemByTemplateID(0, 12221).Sum((ItemInfo p) => p.Count) >= tS_UpgradeTemplate.Param2)
			{
				Player.RemoveCardSoul(tS_UpgradeTemplate.Param1);
				Player.PropBag.RemovePropItem(Player, 12221, tS_UpgradeTemplate.Param2);
				Player.PlayerCharacter.CardInfo.SuitLevel = tS_UpgradeTemplate.Grades + 1;
				Player.UpdateCardInfo();
				CardNewSystemMgr.Out.SendCardInfo(Player);
				CardNewSystemMgr.Out.SendCardSoul(Player);
				Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("CardNew.CardBreakSuit.Success"));
			}
			else
			{
				Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("CardNew.CardBreakSuit.Failed"));
			}
		}
		return 0;
	}
}
