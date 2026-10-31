using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.WorshipTheMoon.Handle;

[WorshipTheMoonHandleAttbute(5)]
public class WorshipTheMoonReceiveMoonReward : IWorshipTheMoonCommandHadler
{
	public bool CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int num = int.Parse(GameProperties.WorshipMoonGift.Split('|')[0]);
		if (player.Actives.Info.updateWorshipedCounts < num)
		{
			player.SendMessage(LanguageMgr.GetTranslation("WorshipTheMoonReceiveMoonReward.Fail"));
		}
		else
		{
			ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(int.Parse(GameProperties.WorshipMoonGift.Split('|')[1]));
			if (itemTemplateInfo != null)
			{
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 105);
				itemInfo.IsBinds = true;
				player.AddTemplate(itemInfo);
				player.SendMessage(LanguageMgr.GetTranslation("WorshipTheMoonReceiveMoonReward.Success"));
			}
			player.Actives.Info.update200State++;
		}
		player.Actives.SendUpdateFreeCount();
		return false;
	}
}
