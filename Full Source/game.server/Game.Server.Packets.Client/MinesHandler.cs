using System;
using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameUtils;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(414, "user ac action")]
public class MinesHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		switch (packet.ReadByte())
		{
		case 1:
		{
			int num6 = packet.ReadInt();
			if (client.Player.Mines.Data.LastTimeDig.AddSeconds(GameProperties.OnlineArmPerTimeDig) > DateTime.Now)
			{
				return 0;
			}
			if (client.Player.Extra.Info.MissionEnergy < GameProperties.OnlineArmCostEnergy)
			{
				client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.Mines.Msg3"));
				return 0;
			}
			client.Player.Mines.Data.LastTimeDig = DateTime.Now;
			bool val = false;
			string str = "Unknown";
			int num7 = 0;
			if (client.Player.MinesBag.FindFirstEmptySlot() != -1)
			{
				ItemTemplateInfo items2 = client.Player.Mines.GetItems(num6);
				if (items2 != null)
				{
					ItemInfo itemInfo3 = ItemInfo.CreateFromTemplate(items2, 1, 103);
					itemInfo3.IsBinds = true;
					if (client.Player.MinesBag.AddTemplate(itemInfo3, itemInfo3.Count))
					{
						str = itemInfo3.Template.Name;
						num7 = itemInfo3.Count;
					}
					else
					{
						client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.Mines.Msg2"));
					}
				}
			}
			else
			{
				val = true;
				num7 = 1;
			}
			if (num7 > 0)
			{
				GSPacketIn gSPacketIn = new GSPacketIn(414);
				gSPacketIn.WriteByte(1);
				gSPacketIn.WriteInt(num6);
				gSPacketIn.WriteBoolean(val);
				gSPacketIn.WriteString(str);
				gSPacketIn.WriteInt(num7);
				client.SendTCP(gSPacketIn);
			}
			break;
		}
		case 2:
			client.Out.SendOpenMines(client.Player.Mines.Data);
			break;
		case 3:
		{
			int slot2 = packet.ReadInt();
			int num5 = packet.ReadInt();
			if (num5 <= 0)
			{
				return 0;
			}
			ItemInfo itemAt2 = client.Player.MinesBag.GetItemAt(slot2);
			if (itemAt2 == null || itemAt2.Count < num5)
			{
				return 0;
			}
			int exp2 = itemAt2.Template.Property2 * num5;
			client.Player.MinesBag.RemoveCountFromStack(itemAt2, num5);
			client.Player.Mines.AddExp(0, exp2);
			client.Out.SendMinesLevelUpTool(client.Player.Mines.Data);
			break;
		}
		case 4:
		{
			int id = packet.ReadInt();
			int count = packet.ReadInt();
			int itemCount = client.Player.MinesBag.GetItemCount(201669);
			MineManageExchangeInfo singleExchange = MinesMgr.GetSingleExchange(id);
			if (singleExchange != null && singleExchange.Price <= itemCount && MinesMgr.TryExchange(id, count))
			{
				GSPacketIn pkg = client.Out.SendMinesShopInfo();
				WorldMgr.SendToNoGame(pkg, client.Player);
				client.Player.MinesBag.RemoveTemplate(201669, singleExchange.Price);
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(singleExchange.TemplateID), count, 103);
				itemInfo.AttackCompose = singleExchange.attackCompose;
				itemInfo.DefendCompose = singleExchange.defendCompose;
				itemInfo.AgilityCompose = singleExchange.agilityCompose;
				itemInfo.LuckCompose = singleExchange.luckCompose;
				itemInfo.StrengthenLevel = singleExchange.strengthLevel;
				itemInfo.IsBinds = singleExchange.isBind;
				itemInfo.ValidDate = singleExchange.validDate;
				client.Player.AddTemplate(itemInfo);
				client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.Mines.Msg7"));
			}
			else
			{
				client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.Mines.Msg6"));
			}
			break;
		}
		case 5:
		{
			int num3 = packet.ReadInt();
			int num4 = packet.ReadInt();
			MineManageItemInfo singleDrop = MinesMgr.GetSingleDrop(num3);
			int itemCount2 = client.Player.MinesBag.GetItemCount(num3);
			if (singleDrop != null && itemCount2 >= num4 && MinesMgr.TrySell(num3, num4))
			{
				GSPacketIn pkg2 = client.Out.SendMinesShopInfo();
				WorldMgr.SendToNoGame(pkg2, client.Player);
				client.Player.MinesBag.RemoveTemplate(num3, num4);
				ItemInfo itemInfo2 = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(201669), singleDrop.Price, 103);
				itemInfo2.IsBinds = true;
				client.Player.MinesBag.AddTemplate(itemInfo2, singleDrop.Price);
				client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.Mines.Msg7"));
			}
			else
			{
				client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.Mines.Msg5"));
			}
			break;
		}
		case 6:
		{
			int type = packet.ReadInt();
			int slot = packet.ReadInt();
			int num2 = packet.ReadInt();
			if (num2 <= 0)
			{
				return 0;
			}
			ItemInfo itemAt = client.Player.MinesBag.GetItemAt(slot);
			if (itemAt == null || itemAt.Count < num2)
			{
				return 0;
			}
			int exp = itemAt.Template.Property2 * num2;
			client.Player.MinesBag.RemoveCountFromStack(itemAt, num2);
			client.Player.Mines.AddExp(type, exp);
			client.Out.SendMinesLevelUpArm(client.Player.Mines.Data);
			client.Player.EquipBag.UpdatePlayerProperties();
			break;
		}
		case 7:
		{
			PlayerInventory minesBag = client.Player.MinesBag;
			List<ItemInfo> items = minesBag.GetItems();
			if (items.Count > 0)
			{
				minesBag.BeginChanges();
				try
				{
					if (minesBag.FindFirstEmptySlot(minesBag.BeginSlot) != -1)
					{
						for (int i = 1; minesBag.FindFirstEmptySlot(minesBag.BeginSlot) < items[items.Count - i].Place; i++)
						{
							minesBag.MoveItem(items[items.Count - i].Place, minesBag.FindFirstEmptySlot(minesBag.BeginSlot), items[items.Count - i].Count);
						}
					}
				}
				finally
				{
					try
					{
						items = minesBag.GetItems();
						List<int> list = new List<int>();
						for (int j = 0; j < items.Count; j++)
						{
							if (list.Contains(j))
							{
								continue;
							}
							for (int num = items.Count - 1; num > j; num--)
							{
								if (!list.Contains(num) && items[j].TemplateID == items[num].TemplateID && items[j].CanStackedTo(items[num]))
								{
									minesBag.MoveItem(items[num].Place, items[j].Place, items[num].Count);
									list.Add(num);
								}
							}
						}
					}
					finally
					{
						items = minesBag.GetItems();
						if (minesBag.FindFirstEmptySlot(minesBag.BeginSlot) != -1)
						{
							for (int k = 1; minesBag.FindFirstEmptySlot(minesBag.BeginSlot) < items[items.Count - k].Place; k++)
							{
								minesBag.MoveItem(items[items.Count - k].Place, minesBag.FindFirstEmptySlot(minesBag.BeginSlot), items[items.Count - k].Count);
							}
						}
					}
					minesBag.CommitChanges();
				}
			}
			client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.Mines.Msg4"));
			break;
		}
		case 8:
			client.Out.SendMinesShopInfo();
			break;
		}
		return 1;
	}
}
