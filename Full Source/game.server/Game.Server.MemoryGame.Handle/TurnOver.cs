using System;
using System.Collections.Generic;
using System.Linq;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Newtonsoft.Json;
using SqlDataProvider.Data;

namespace Game.Server.MemoryGame.Handle;

[MemoryGameHandleAttbute(12)]
public class TurnOver : IMemoryGameCommandHadler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		List<MemoryGameInfo> list = JsonConvert.DeserializeObject<List<MemoryGameInfo>>(player.Actives.Info.MemoryGameData);
		if (list != null)
		{
			int index = packet.ReadInt();
			MemoryGameInfo memoryGameInfo = list.SingleOrDefault((MemoryGameInfo a) => a.place1 == index || a.place2 == index);
			if (memoryGameInfo != null)
			{
				if (player.Actives.Info.MemoryGameCount <= 0)
				{
					player.SendMessage(LanguageMgr.GetTranslation("MemoryGame.NoTurn.Msg"));
					return 1;
				}
				player.Actives.Info.MemoryGameCount--;
				bool val = true;
				int num = 0;
				int num2 = 300;
				if (index == memoryGameInfo.place1)
				{
					memoryGameInfo.show1 = true;
				}
				else
				{
					memoryGameInfo.show2 = true;
				}
				List<MemoryGameInfo> list2 = list.Where((MemoryGameInfo a) => (a.show1 || a.show2) && !a.isGet).ToList();
				if (list2.Count > 1)
				{
					val = false;
					foreach (MemoryGameInfo item in list2)
					{
						if (item != memoryGameInfo)
						{
							num = (item.show1 ? item.place1 : item.place2);
						}
						item.show1 = false;
						item.show2 = false;
					}
				}
				else if (list2[0].show1 && list2[0].show2)
				{
					list2[0].isGet = true;
					num2 = 600;
					ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(list2[0].TemplateID), list2[0].count, 105);
					itemInfo.ValidDate = 0;
					itemInfo.IsBinds = true;
					player.AddTemplate(itemInfo);
				}
				player.Actives.Info.MemoryGameScore += num2;
				GSPacketIn gSPacketIn = new GSPacketIn(329);
				gSPacketIn.WriteByte(12);
				gSPacketIn.WriteInt(player.Actives.Info.MemoryGameCount);
				gSPacketIn.WriteInt(player.Actives.Info.MemoryGameScore);
				gSPacketIn.WriteInt(list2.Count);
				foreach (MemoryGameInfo item2 in list2)
				{
					int num3 = 0;
					num3 = ((item2 != memoryGameInfo) ? num : index);
					gSPacketIn.WriteInt(item2.TemplateID);
					gSPacketIn.WriteInt(item2.count);
					gSPacketIn.WriteInt(num3);
					gSPacketIn.WriteBoolean(val);
					gSPacketIn.WriteBoolean(item2.isGet);
				}
				player.SendTCP(gSPacketIn);
				player.Actives.Info.MemoryGameData = JsonConvert.SerializeObject(list);
			}
			else
			{
				Console.WriteLine("//MemoryGame CellNull: " + index);
			}
		}
		return 1;
	}
}
