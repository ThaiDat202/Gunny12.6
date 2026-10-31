using System;
using System.Collections.Generic;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Newtonsoft.Json;
using SqlDataProvider.Data;

namespace Game.Server.SanXiao.Handle;

[SanXiaoHandleAttbute(3)]
public class Boom : ISanXiaoCommandHadler
{
	private Random rand = new Random();

	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		if (player.Actives.Info.SXStepRemain <= 0)
		{
			player.SendMessage(LanguageMgr.GetTranslation("SanXiao.NoEnouchStepRemain.Msg"));
			return 1;
		}
		Dictionary<int, Dictionary<int, SXCellDataInfo>> dictionary = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SXCellDataInfo>>>(player.Actives.Info.SXMapInfoData);
		int num = packet.ReadInt();
		int num2 = packet.ReadInt();
		int num3 = packet.ReadInt();
		int num4 = packet.ReadInt();
		if (num >= 0 && num2 >= 0 && num3 >= 0 && num4 >= 0)
		{
			int type = dictionary[num][num2].type;
			int type2 = dictionary[num3][num4].type;
			dictionary[num][num2].type = type2;
			dictionary[num3][num4].type = type;
		}
		int num5 = packet.ReadInt();
		int num6 = 0;
		for (int i = 0; i < num5; i++)
		{
			int num7 = packet.ReadInt();
			for (int j = 0; j < num7 * 2; j += 2)
			{
				int num8 = packet.ReadInt();
				int num9 = packet.ReadInt();
				if (num8 >= 0 && num9 >= 0)
				{
					dictionary[num8][num9].type = 0;
					num6++;
				}
			}
		}
		player.Actives.Info.SXScore += num6;
		return 0;
	}
}
