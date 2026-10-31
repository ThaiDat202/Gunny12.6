using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.SanXiao.Handle;

[SanXiaoHandleAttbute(9)]
public class HitsEnd : ISanXiaoCommandHadler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		if (player.Actives.Info.SXStepRemain > 0)
		{
			player.Actives.Info.SXStepRemain--;
			SpecialItemBoxInfo specialValue = new SpecialItemBoxInfo();
			List<ItemInfo> list = new List<ItemInfo>();
			if (ItemBoxMgr.CreateItemBox(GameProperties.ThreeCleanAwardBox, list, ref specialValue))
			{
				if (specialValue.CrystalScore > 0)
				{
					player.Actives.Info.SXCrystal += specialValue.CrystalScore;
				}
				if (list.Count > 0)
				{
					foreach (ItemInfo item in list)
					{
						player.AddTemplate(item);
					}
					player.Actives.SendSXGainedDropItems(list);
				}
			}
			player.Actives.SendSXData();
		}
		return 1;
	}
}
