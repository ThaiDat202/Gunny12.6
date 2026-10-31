using System;
using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.WorshipTheMoon.Handle;

[WorshipTheMoonHandleAttbute(3)]
public class WorshipTheMoon : IWorshipTheMoonCommandHadler
{
	private static Random rand = new Random();

	public bool CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		bool flag = packet.ReadBoolean();
		if (num <= 0)
		{
			return false;
		}
		int num2 = int.Parse(GameProperties.WorshipMoonPriceInfo.Split('|')[1]);
		int num3 = num * num2;
		int updateFreeCounts = player.Actives.Info.updateFreeCounts;
		if (updateFreeCounts > 0)
		{
			num3 = ((num != 1) ? (num3 - updateFreeCounts * num2) : (num3 - num2));
		}
		if (player.MoneyDirect(num3))
		{
			if (updateFreeCounts > 0)
			{
				if (num == 1)
				{
					player.Actives.Info.updateFreeCounts--;
				}
				else
				{
					player.Actives.Info.updateFreeCounts -= updateFreeCounts;
				}
			}
			player.Actives.Info.updateWorshipedCounts += num;
			List<ItemInfo> list = new List<ItemInfo>();
			GSPacketIn gSPacketIn = new GSPacketIn(281);
			gSPacketIn.WriteByte(3);
			gSPacketIn.WriteInt(num);
			for (int i = 0; i < num; i++)
			{
				int type = MoonType();
				int cakeName = GetCakeName(type);
				gSPacketIn.WriteInt(MoonType());
				gSPacketIn.WriteInt(cakeName);
				ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(cakeName);
				if (itemTemplateInfo != null)
				{
					ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 105);
					itemInfo.IsBinds = true;
					list.Add(itemInfo);
				}
			}
			player.AddTemplate(list);
			player.SendTCP(gSPacketIn);
			player.Actives.SendUpdateFreeCount();
		}
		return false;
	}

	private int MoonType()
	{
		List<int> list = new List<int>();
		string[] array = GameProperties.WorshipMoonProb.Split('|');
		int num = int.Parse(array[5]) + int.Parse(array[4]) + int.Parse(array[3]) + int.Parse(array[2]) + int.Parse(array[1]) + int.Parse(array[0]);
		for (int i = 0; i < num; i++)
		{
			if (i < int.Parse(array[5]))
			{
				list.Add(6);
			}
			if (i < int.Parse(array[5]) + int.Parse(array[4]))
			{
				list.Add(5);
			}
			if (i < int.Parse(array[5]) + int.Parse(array[4]) + int.Parse(array[3]))
			{
				list.Add(4);
			}
			if (i < int.Parse(array[5]) + int.Parse(array[4]) + int.Parse(array[3]) + int.Parse(array[2]))
			{
				list.Add(3);
			}
			if (i < int.Parse(array[5]) + int.Parse(array[4]) + int.Parse(array[3]) + int.Parse(array[2]) + int.Parse(array[1]))
			{
				list.Add(2);
			}
			else
			{
				list.Add(1);
			}
		}
		ThreadSafeRandom.ShufferListStatic(list);
		return list[rand.Next(list.Count)];
	}

	private int GetCakeName(int type)
	{
		string[] array = GameProperties.WorshipMoonReward.Split('|');
		return type switch
		{
			1 => int.Parse(array[type - 1]), 
			2 => int.Parse(array[type - 1]), 
			3 => int.Parse(array[type - 1]), 
			4 => int.Parse(array[type - 1]), 
			5 => int.Parse(array[type - 1]), 
			6 => int.Parse(array[type - 1]), 
			_ => -1, 
		};
	}
}
