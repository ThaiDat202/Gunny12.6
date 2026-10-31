using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.ActiveSystem.Handle;

[ActiveSystemHandleAttbute(93)]
public class BoguAdventureAcquireAward : IActiveSystemCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		if (Player.Actives.IsAcquireAward(num) == 0)
		{
			List<BoguAdventureRewardInfo> list = ActiveSystermAwardMgr.FindBoguAdventureReward(num + 1);
			List<ItemInfo> list2 = new List<ItemInfo>();
			foreach (BoguAdventureRewardInfo item in list)
			{
				ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(item.TemplateID);
				if (itemTemplateInfo != null)
				{
					ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 105);
					itemInfo.Count = item.Count;
					itemInfo.IsBinds = item.IsBinds;
					itemInfo.StrengthenLevel = item.StrengthenLevel;
					itemInfo.AttackCompose = item.AttackCompose;
					itemInfo.DefendCompose = item.DefendCompose;
					itemInfo.AgilityCompose = item.AgilityCompose;
					itemInfo.LuckCompose = item.LuckCompose;
					list2.Add(itemInfo);
				}
			}
			if (list2.Count > 0)
			{
				BoguAdventureDataInfo boguAdventure = Player.Actives.BoguAdventure;
				string translation = LanguageMgr.GetTranslation("BoguAdventureAcquireAward.Newbie");
				switch (num)
				{
				case 0:
					boguAdventure.isAcquireAward1 = 1;
					break;
				case 1:
					boguAdventure.isAcquireAward2 = 1;
					translation = LanguageMgr.GetTranslation("BoguAdventureAcquireAward.Pro");
					break;
				case 2:
					boguAdventure.isAcquireAward3 = 1;
					translation = LanguageMgr.GetTranslation("BoguAdventureAcquireAward.Master");
					break;
				}
				WorldEventMgr.SendItemsToMails(list2, Player.PlayerCharacter.ID, Player.PlayerCharacter.NickName, Player.ZoneId, LanguageMgr.GetTranslation("BoguAdventureAcquireAward.Title", translation));
				Player.Actives.SaveBoguAdventureDatabase();
				Player.SendMessage(LanguageMgr.GetTranslation("BoguAdventureUpdateCell.GetAwardSuccess"));
				Player.Actives.SendBoguAdventureEnter();
			}
			else
			{
				Player.SendMessage(LanguageMgr.GetTranslation("BoguAdventureUpdateCell.AwardError", num + 1));
			}
		}
		else
		{
			Player.SendMessage(LanguageMgr.GetTranslation("BoguAdventureUpdateCell.GotAward"));
		}
		return true;
	}
}
