using System.Collections.Generic;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.GodCardRaise.Handle;

[GodCardRaiseHandleAttbute(24)]
public class GodCardPointAwardAttribute : IGodCardRaiseCommandHadler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		GodCardPointRewardInfo godCardPointRewardInfo = GodCardPointRewardMgr.FindGodCardPointReward(num);
		if (godCardPointRewardInfo != null)
		{
			ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(godCardPointRewardInfo.ItemID);
			if (itemTemplateInfo != null)
			{
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 102);
				if (itemInfo != null)
				{
					itemInfo.Count = godCardPointRewardInfo.Count;
					itemInfo.IsBinds = godCardPointRewardInfo.IsBind;
					itemInfo.ValidDate = godCardPointRewardInfo.Valid;
					player.AddTemplate(itemInfo);
					player.Actives.ListAwards.Add(num);
				}
			}
		}
		List<int> listAwards = player.Actives.ListAwards;
		GSPacketIn gSPacketIn = new GSPacketIn(329);
		gSPacketIn.WriteByte(25);
		gSPacketIn.WriteInt(listAwards.Count);
		for (int i = 0; i < listAwards.Count; i++)
		{
			gSPacketIn.WriteInt(listAwards[i]);
		}
		player.Out.SendTCP(gSPacketIn);
		return 1;
	}
}
