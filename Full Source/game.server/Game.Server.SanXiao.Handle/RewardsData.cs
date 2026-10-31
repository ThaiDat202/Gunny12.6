using System.Collections.Generic;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Newtonsoft.Json;

namespace Game.Server.SanXiao.Handle;

[SanXiaoHandleAttbute(17)]
public class RewardsData : ISanXiaoCommandHadler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		List<int> list = JsonConvert.DeserializeObject<List<int>>(player.Actives.Info.SXRewardsGet);
		if (list != null)
		{
			player.Actives.SendSXRewardsData(list);
		}
		return 1;
	}
}
