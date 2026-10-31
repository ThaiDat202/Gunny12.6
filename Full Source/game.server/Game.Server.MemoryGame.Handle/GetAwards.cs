using System.Collections.Generic;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Newtonsoft.Json;
using SqlDataProvider.Data;

namespace Game.Server.MemoryGame.Handle;

[MemoryGameHandleAttbute(16)]
public class GetAwards : IMemoryGameCommandHadler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		List<MemoryGameInfo> list = JsonConvert.DeserializeObject<List<MemoryGameInfo>>(player.Actives.Info.MemoryGameData);
		if (list == null)
		{
			return 1;
		}
		ThreeClearPointAwardInfo threeClearPointAwardInfo = AwardMgr.FindThreeClearPointAward(num);
		if (threeClearPointAwardInfo != null && threeClearPointAwardInfo.Point <= player.Actives.Info.MemoryGameScore && threeClearPointAwardInfo.Type == 0)
		{
			List<int> list2 = JsonConvert.DeserializeObject<List<int>>(player.Actives.Info.MemoryGameAwardGetData);
			if (list2 == null)
			{
				list2 = new List<int>();
			}
			if (!list2.Contains(num))
			{
				list2.Add(num);
				player.Actives.Info.MemoryGameAwardGetData = JsonConvert.SerializeObject(list2);
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(threeClearPointAwardInfo.ItemID), threeClearPointAwardInfo.Count, 105);
				itemInfo.IsBinds = threeClearPointAwardInfo.IsBind;
				itemInfo.ValidDate = threeClearPointAwardInfo.Valid;
				player.AddTemplate(itemInfo);
				player.Actives.SendMemoryGameData(list, isOld: true);
			}
		}
		return 1;
	}
}
