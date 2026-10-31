using System.Collections.Generic;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Newtonsoft.Json;
using SqlDataProvider.Data;

namespace Game.Server.SanXiao.Handle;

[SanXiaoHandleAttbute(1)]
public class CreateMap : ISanXiaoCommandHadler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		Dictionary<int, Dictionary<int, SXCellDataInfo>> dictionary = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SXCellDataInfo>>>(player.Actives.Info.SXMapInfoData);
		int num = packet.ReadInt();
		for (int i = 0; i < num * 3; i += 3)
		{
			int key = packet.ReadInt();
			int key2 = packet.ReadInt();
			int type = packet.ReadInt();
			dictionary[key][key2].type = type;
		}
		player.Actives.Info.SXMapInfoData = JsonConvert.SerializeObject(dictionary);
		return 0;
	}
}
