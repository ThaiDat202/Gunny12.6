using System;
using System.Collections.Generic;
using System.Linq;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(326, "打开物品")]
public class ToyGrapple : IPacketHandler
{
	private string[] prices = GameProperties.ToyMachinePrice.Split('|');

	private static Random rand = new Random();

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int index = packet.ReadInt();
		int num = packet.ReadInt();
		bool flag = packet.ReadBoolean();
		bool flag2 = packet.ReadBoolean();
		if (prices.Length != 4)
		{
			client.Player.SendMessage(LanguageMgr.GetTranslation("ToyGrapple.ErroPrice"));
			return 0;
		}
		int num2 = int.Parse(prices[0]);
		List<EventAwardInfo> list = new List<EventAwardInfo>();
		List<EventAwardInfo> list2 = EventAwardMgr.FindEventAward(eEventType.TOY_MACHINE_S);
		if (list2.Count == 0)
		{
			client.Player.SendMessage(LanguageMgr.GetTranslation("ToyGrapple.ErroAward"));
			return 0;
		}
		int num3 = index;
		int num4 = num3;
		if (num4 == 1)
		{
			if (num == 10)
			{
				if (!flag)
				{
					flag = true;
				}
				num2 = int.Parse(prices[1]) * num;
				IEnumerable<EventAwardInfo> source = list2.Where((EventAwardInfo s) => s.SelectIndex == index);
				if (source.Count() > 0)
				{
					EventAwardInfo awards = GetAwards(source.ToList());
					if (awards != null)
					{
						list.Add(awards);
					}
				}
			}
			else
			{
				num2 = int.Parse(prices[0]);
				int silverCount = client.Player.Extra.Info.silverCount;
				if (silverCount > 0)
				{
					if (client.Player.Extra.UseKingBless(2))
					{
						num = 1;
					}
					else
					{
						client.Player.Extra.Info.silverTime = DateTime.Now;
						client.Player.Extra.Info.silverTime = DateTime.Now.AddMinutes(10.0);
					}
					if (!flag)
					{
						client.Player.Extra.Info.silverCount--;
					}
				}
			}
		}
		else
		{
			list2 = EventAwardMgr.FindEventAward(eEventType.TOY_MACHINE_G);
			if (num == 10)
			{
				if (!flag)
				{
					flag = true;
				}
				num2 = int.Parse(prices[3]) * num;
				IEnumerable<EventAwardInfo> source2 = list2.Where((EventAwardInfo s) => s.SelectIndex == index);
				if (source2.Count() > 0)
				{
					EventAwardInfo awards2 = GetAwards(source2.ToList());
					if (awards2 != null)
					{
						list.Add(awards2);
					}
				}
			}
			else
			{
				num2 = int.Parse(prices[2]);
				if (!flag)
				{
					client.Player.Extra.Info.goldTime = DateTime.Now;
					client.Player.Extra.Info.goldTime = DateTime.Now.AddDays(3.0);
				}
			}
		}
		if (flag)
		{
			if (client.Player.MoneyDirect(num2))
			{
				int num5 = num;
				int num6 = num5;
				if (num6 == 1)
				{
					list.Add(GetAwards(list2));
				}
				else
				{
					int num7 = 0;
					while (list.Count < 10)
					{
						list.Add(GetAwards(list2));
						num7++;
					}
				}
			}
			else
			{
				list.Clear();
			}
		}
		else
		{
			list.Add(GetAwards(list2));
		}
		if (list.Count > 0)
		{
			List<ItemInfo> list3 = new List<ItemInfo>();
			foreach (EventAwardInfo item in list)
			{
				ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(item.TemplateID);
				if (itemTemplateInfo != null)
				{
					ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, item.Count, 105);
					itemInfo.TemplateID = item.TemplateID;
					itemInfo.IsBinds = item.IsBinds;
					itemInfo.ValidDate = item.ValidDate;
					itemInfo.Count = item.Count;
					itemInfo.StrengthenLevel = item.StrengthenLevel;
					itemInfo.AttackCompose = item.AttackCompose;
					itemInfo.DefendCompose = item.DefendCompose;
					itemInfo.AgilityCompose = item.AgilityCompose;
					itemInfo.LuckCompose = item.LuckCompose;
					itemInfo.MagicAttack = item.MagicAttack;
					itemInfo.MagicDefence = item.MagicDefence;
					list3.Add(itemInfo);
				}
			}
			GSPacketIn gSPacketIn = new GSPacketIn(326);
			gSPacketIn.WriteInt(index);
			gSPacketIn.WriteByte((byte)list.Count);
			foreach (ItemInfo item2 in list3)
			{
				gSPacketIn.WriteInt(item2.TemplateID);
				gSPacketIn.WriteInt(item2.Count);
				gSPacketIn.WriteBoolean(item2.IsBinds);
				gSPacketIn.WriteInt(item2.ValidDate);
				gSPacketIn.WriteInt(item2.StrengthenLevel);
				gSPacketIn.WriteInt(item2.AttackCompose);
				gSPacketIn.WriteInt(item2.DefendCompose);
				gSPacketIn.WriteInt(item2.AttackCompose);
				gSPacketIn.WriteInt(item2.LuckCompose);
				if (Equip.isMagicStone(item2.Template.CategoryID))
				{
					gSPacketIn.WriteInt(item2.MagicAttack);
					gSPacketIn.WriteInt(item2.MagicDefence);
				}
				else
				{
					gSPacketIn.WriteInt(0);
					gSPacketIn.WriteInt(0);
				}
				client.Player.AddTemplate(item2);
			}
			client.Player.SendTCP(gSPacketIn);
		}
		return 1;
	}

	private static EventAwardInfo GetAwards(List<EventAwardInfo> unFiltInfos)
	{
		int num = 1;
		int maxRound = rand.Next(unFiltInfos.Select((EventAwardInfo s) => s.Random).Max());
		List<EventAwardInfo> list = unFiltInfos.Where((EventAwardInfo s) => s.Random >= maxRound).ToList();
		int num2 = list.Count();
		if (num2 > 0)
		{
			num = ((num > num2) ? num2 : num);
			int[] randomUnrepeatArray = GetRandomUnrepeatArray(0, num2 - 1, num);
			int[] array = randomUnrepeatArray;
			foreach (int index in array)
			{
				EventAwardInfo eventAwardInfo = list[index];
				if (eventAwardInfo != null)
				{
					return eventAwardInfo;
				}
			}
		}
		return null;
	}

	private static int[] GetRandomUnrepeatArray(int minValue, int maxValue, int count)
	{
		int[] array = new int[count];
		for (int i = 0; i < count; i++)
		{
			int num = rand.Next(minValue, maxValue + 1);
			int num2 = 0;
			for (int j = 0; j < i; j++)
			{
				if (array[j] == num)
				{
					num2++;
				}
			}
			if (num2 == 0)
			{
				array[i] = num;
			}
			else
			{
				i--;
			}
		}
		return array;
	}
}
