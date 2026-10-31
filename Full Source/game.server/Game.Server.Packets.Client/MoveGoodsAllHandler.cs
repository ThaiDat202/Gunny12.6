using System;
using System.Collections.Generic;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.GameUtils;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(124, "整理背包")]
public class MoveGoodsAllHandler : IPacketHandler
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		bool flag = packet.ReadBoolean();
		int num = packet.ReadInt();
		int bageType = packet.ReadInt();
		if (DateTime.Compare(client.Player.LastMoveGoodsAllTime.AddSeconds(3.0), DateTime.Now) > 0)
		{
			client.Player.SendMessage("请勿频繁整理背包,谢谢~");
			return 0;
		}
		client.Player.LastMoveGoodsAllTime = DateTime.Now;
		PlayerInventory inventory = client.Player.GetInventory((eBageType)bageType);
		int maxSlot = inventory.Capalility;
		if (inventory.BagType == 0)
		{
			maxSlot = 80;
		}
		if (inventory.BagType == 21)
		{
			maxSlot = 179;
		}
		List<ItemInfo> items = inventory.GetItems(inventory.BeginSlot, maxSlot);
		if (num == items.Count)
		{
			inventory.BeginChanges();
			try
			{
				ItemInfo[] rawSpaces = inventory.GetRawSpaces();
				inventory.ClearBag();
				for (int i = 0; i < num; i++)
				{
					int num2 = packet.ReadInt();
					int num3 = packet.ReadInt();
					ItemInfo itemInfo = rawSpaces[num2];
					if (itemInfo != null)
					{
						if (itemInfo.Template.CategoryID == 18 || itemInfo.Template.CategoryID == 19 || itemInfo.Template.CategoryID == 40 || itemInfo.Template.CategoryID == 9 || itemInfo.Template.CategoryID == 2)
						{
							client.Player.AddTemplate(itemInfo);
						}
						else if (!inventory.AddItemTo(itemInfo, num3))
						{
							throw new Exception($"整理背包时出错(更改道具位置): 整理前位置:{num2} 整理后位置:{num3} 整理物品类型:{itemInfo.Template.CategoryID}");
						}
						continue;
					}
					throw new Exception($"整理背包时出错(更改道具位置): 整理前位置:{num2} 整理后位置:{num3} 整理物品类型:{itemInfo.Template.CategoryID}");
				}
			}
			catch (Exception ex)
			{
				log.ErrorFormat("整理背包错误玩家ID:{0} 错误信息:{1}", client.Player.PlayerId, ex.Message);
			}
			finally
			{
				if (flag)
				{
					items = inventory.GetItems();
					List<int> list = new List<int>();
					for (int j = 0; j < items.Count; j++)
					{
						if (list.Contains(j))
						{
							continue;
						}
						for (int num4 = items.Count - 1; num4 > j; num4--)
						{
							if (!list.Contains(num4) && items[j].TemplateID == items[num4].TemplateID && items[j].CanStackedTo(items[num4]))
							{
								inventory.MoveItem(items[num4].Place, items[j].Place, items[num4].Count);
								list.Add(num4);
							}
						}
					}
					items = inventory.GetItems();
					if (inventory.FindFirstEmptySlot() != -1)
					{
						for (int k = 1; inventory.FindFirstEmptySlot() < items[items.Count - k].Place; k++)
						{
							inventory.MoveItem(items[items.Count - k].Place, inventory.FindFirstEmptySlot(), items[items.Count - k].Count);
						}
					}
				}
				inventory.CommitChanges();
				if (inventory.BagType == 21)
				{
					inventory.UpdateChangedPlaces();
				}
			}
		}
		return 0;
	}
}
