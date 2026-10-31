using System.Collections.Generic;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Newtonsoft.Json;

namespace Game.Server.SanXiao.Handle;

[SanXiaoHandleAttbute(5)]
public class Data : ISanXiaoCommandHadler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		player.Actives.SendSXData();
		List<int> list = JsonConvert.DeserializeObject<List<int>>(player.Actives.Info.SXRewardsGet);
		if (list != null)
		{
			player.Actives.SendSXRewardsData(list);
		}
		Dictionary<int, int> dictionary = JsonConvert.DeserializeObject<Dictionary<int, int>>(player.Actives.Info.MiniShopBuyCount);
		if (dictionary != null)
		{
			player.Actives.SendSXStoreData(dictionary);
		}
		return 1;
	}
}
