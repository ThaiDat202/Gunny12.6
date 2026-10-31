using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Newtonsoft.Json;
using SqlDataProvider.Data;

namespace Game.Server.SanXiao.Handle;

[SanXiaoHandleAttbute(7)]
public class GainPrise : ISanXiaoCommandHadler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		ThreeClearPointAwardInfo threeClearPointAwardInfo = AwardMgr.FindThreeClearPointAward(num);
		if (threeClearPointAwardInfo != null && threeClearPointAwardInfo.Point <= player.Actives.Info.SXScore && threeClearPointAwardInfo.Type == 1)
		{
			List<int> list = JsonConvert.DeserializeObject<List<int>>(player.Actives.Info.SXRewardsGet);
			if (list == null)
			{
				list = new List<int>();
			}
			if (!list.Contains(num))
			{
				list.Add(num);
				player.Actives.Info.SXRewardsGet = JsonConvert.SerializeObject(list);
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(threeClearPointAwardInfo.ItemID), threeClearPointAwardInfo.Count, 105);
				itemInfo.IsBinds = threeClearPointAwardInfo.IsBind;
				itemInfo.ValidDate = threeClearPointAwardInfo.Valid;
				player.AddTemplate(itemInfo);
				player.Actives.SendSXRewardsData(list);
			}
			else
			{
				player.SendMessage(LanguageMgr.GetTranslation("SanXiao.Handle.GainPrise.Fail"));
			}
		}
		return 1;
	}
}
