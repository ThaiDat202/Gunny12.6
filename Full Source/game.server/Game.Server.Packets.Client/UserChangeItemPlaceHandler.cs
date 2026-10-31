using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameUtils;
using Game.Server.Managers;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(49, "改变物品位置")]
public class UserChangeItemPlaceHandler : IPacketHandler
{
	public static readonly ILog log = LogManager.GetLogger("ItemLogger");

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		eBageType eBageType = (eBageType)packet.ReadByte();
		int num = packet.ReadInt();
		eBageType eBageType2 = (eBageType)packet.ReadByte();
		int num2 = packet.ReadInt();
		int num3 = packet.ReadInt();
		bool flag = packet.ReadBoolean();
		PlayerInventory inventory = client.Player.GetInventory(eBageType);
		PlayerInventory inventory2 = client.Player.GetInventory(eBageType2);
		ItemInfo itemAt = inventory.GetItemAt(num);
		if (itemAt == null)
		{
			return 0;
		}
		if (num3 < 0 || num3 > itemAt.Count)
		{
			client.Disconnect();
			log.ErrorFormat("client disconnect bagType: {0}, place: {1}, toBagType: {2}, toPlace: {3}, count: {4}, allMove: {5}", eBageType, num, eBageType2, num2, num3, flag);
			return 0;
		}
		if (client.Server.Configuration.DebugMode)
		{
			Console.WriteLine("bagType: {0}, place: {1}, toBagType: {2}, toPlace: {3}, count: {4}, allMove: {5}", eBageType, num, eBageType2, num2, num3, flag);
		}
		if (eBageType == eBageType2 && num == num2)
		{
			return 0;
		}
		inventory.BeginChanges();
		inventory2.BeginChanges();
		try
		{
			if (eBageType2 == eBageType.Consortia)
			{
				ConsortiaInfo consortiaInfo = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
				if (consortiaInfo != null)
				{
					inventory2.Capalility = consortiaInfo.StoreLevel * 10;
				}
			}
			if (eBageType2 == eBageType.MagicHouseBag)
			{
				inventory2.Capalility = client.Player.MagicHouse.Info?.depotCount ?? 10;
			}
			if (num2 == -1)
			{
				bool flag2 = false;
				if (eBageType == eBageType.CaddyBag && eBageType2 == eBageType.BeadBag)
				{
					num2 = inventory2.FindFirstEmptySlot(inventory2.BeginSlot);
					if (inventory2.AddItemTo(itemAt, num2))
					{
						inventory.TakeOutItem(itemAt);
					}
					else
					{
						flag2 = true;
					}
				}
				else if (eBageType == eBageType2 && eBageType2 == eBageType.EquipBag)
				{
					num2 = inventory2.FindFirstEmptySlot(inventory2.BeginSlot, 80);
					if (Equip.isAvatar(itemAt.Template))
					{
						num2 = inventory2.FindFirstEmptySlot(81);
					}
					if (!inventory.MoveItem(num, num2, num3))
					{
						flag2 = true;
					}
				}
				else if (inventory2.StackItemToAnother(itemAt) || inventory2.AddItem(itemAt))
				{
					inventory.TakeOutItem(itemAt);
				}
				else
				{
					flag2 = true;
				}
				if (flag2)
				{
					client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full"));
					return 0;
				}
			}
			else if (eBageType == eBageType2)
			{
				if (eBageType2 == eBageType.EquipBag && num2 < inventory.BeginSlot)
				{
					itemAt.IsBinds = true;
				}
				ItemInfo itemAt2 = inventory.GetItemAt(num2);
				if (itemAt2 != null && num2 >= inventory.BeginSlot)
				{
					num2 = inventory2.FindFirstEmptySlot(inventory2.BeginSlot, 80);
					if (Equip.isAvatar(itemAt.Template))
					{
						num2 = inventory2.FindFirstEmptySlot(81);
					}
					inventory.MoveItem(num, num2, num3);
				}
				else
				{
					inventory.MoveItem(num, num2, num3);
				}
				client.Player.OnNewGearEvent(itemAt.Template.CategoryID);
			}
			else
			{
				switch (eBageType)
				{
				case eBageType.Store:
					MoveFromStore(client, inventory, itemAt, num2, inventory2, num3);
					break;
				default:
					if (eBageType != eBageType.MagicHouseBag)
					{
						switch (eBageType2)
						{
						case eBageType.Store:
							if (itemAt.IsAdvanceDate())
							{
								itemAt.StrengthenExp = 0;
								itemAt.AdvanceDate = DateTime.Now;
							}
							MoveToStore(client, inventory, itemAt, num2, inventory2, num3);
							break;
						default:
							if (eBageType2 != eBageType.MagicHouseBag)
							{
								if (inventory2.AddItemTo(itemAt, num2))
								{
									inventory.TakeOutItem(itemAt);
									log.ErrorFormat("User: {1} Add item to: {0}", inventory2.BagType, client.Player.PlayerCharacter.UserName);
								}
								break;
							}
							goto case eBageType.Consortia;
						case eBageType.Consortia:
						{
							ItemInfo itemAt3 = inventory2.GetItemAt(num2);
							if (itemAt3 != null)
							{
								num2 = inventory2.FindFirstEmptySlot();
								MoveToBank(num, num2, inventory, inventory2, itemAt);
							}
							else
							{
								MoveToBank(num, num2, inventory, inventory2, itemAt);
							}
							break;
						}
						}
						break;
					}
					goto case eBageType.Consortia;
				case eBageType.Consortia:
				{
					ItemInfo itemAt4 = inventory2.GetItemAt(num2);
					if (itemAt4 != null)
					{
						num2 = inventory2.FindFirstEmptySlot(inventory2.BeginSlot, 80);
						if (Equip.isAvatar(itemAt.Template))
						{
							num2 = inventory2.FindFirstEmptySlot(81);
						}
						MoveFromBank(client, num, num2, inventory, inventory2, itemAt);
						break;
					}
					if (eBageType2 == eBageType.EquipBag && num2 < inventory2.BeginSlot)
					{
						num2 = inventory2.FindFirstEmptySlot(inventory2.BeginSlot, 80);
						if (Equip.isAvatar(itemAt.Template))
						{
							num2 = inventory2.FindFirstEmptySlot(81);
						}
					}
					MoveFromBank(client, num, num2, inventory, inventory2, itemAt);
					break;
				}
				}
			}
		}
		finally
		{
			inventory.CommitChanges();
			inventory2.CommitChanges();
		}
		return 0;
	}

	public void MoveFromStore(GameClient client, PlayerInventory storeBag, ItemInfo item, int toSlot, PlayerInventory bag, int count)
	{
		if (client.Player == null || item == null || storeBag == null || bag == null || item.Template.BagType != (eBageType)bag.BagType)
		{
			return;
		}
		if (toSlot < bag.BeginSlot || toSlot > bag.Capalility)
		{
			if (bag.StackItemToAnother(item))
			{
				storeBag.RemoveItem(item, eItemRemoveType.Stack);
				return;
			}
			string key = $"temp_place_{item.ItemID}";
			if (client.Player.TempProperties.ContainsKey(key))
			{
				toSlot = (int)storeBag.Player.TempProperties[key];
				storeBag.Player.TempProperties.Remove(key);
			}
			else
			{
				toSlot = bag.FindFirstEmptySlot();
			}
		}
		if (bag.StackItemToAnother(item) || bag.AddItemTo(item, toSlot))
		{
			storeBag.TakeOutItem(item);
			return;
		}
		toSlot = bag.FindFirstEmptySlot();
		if (bag.AddItemTo(item, toSlot))
		{
			storeBag.TakeOutItem(item);
			return;
		}
		ItemInfo item2 = item.Clone();
		storeBag.RemoveItem(item);
		client.Player.SendItemToMail(item2, LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full"), LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full"), eMailType.ItemOverdue);
		client.Player.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
	}

	public void MoveToStore(GameClient client, PlayerInventory bag, ItemInfo item, int toSlot, PlayerInventory storeBag, int count)
	{
		if (client.Player == null || bag == null || item == null || storeBag == null)
		{
			return;
		}
		int place = item.Place;
		ItemInfo itemAt = storeBag.GetItemAt(toSlot);
		if (itemAt != null)
		{
			if (item.Count == 1 && item.BagType == itemAt.BagType)
			{
				bag.TakeOutItem(item);
				storeBag.TakeOutItem(itemAt);
				bag.AddItemTo(itemAt, place);
				storeBag.AddItemTo(item, toSlot);
			}
			else
			{
				PlayerInventory itemInventory = client.Player.GetItemInventory(itemAt.Template);
				string key = $"temp_place_{itemAt.ItemID}";
				if (itemInventory.BagType == 0)
				{
					if (client.Player.TempProperties.ContainsKey(key))
					{
						int place2 = (int)client.Player.TempProperties[key];
						client.Player.TempProperties.Remove(key);
						if (itemInventory.AddItemTo(itemAt, place2))
						{
							storeBag.TakeOutItem(itemAt);
						}
					}
				}
				else if (itemInventory.StackItemToAnother(itemAt))
				{
					storeBag.RemoveItem(itemAt, eItemRemoveType.Stack);
				}
				else if (itemInventory.AddItem(itemAt))
				{
					storeBag.TakeOutItem(itemAt);
				}
				else
				{
					client.Player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full"));
				}
			}
		}
		if (!storeBag.IsEmpty(toSlot))
		{
			return;
		}
		if (item.Count == 1)
		{
			if (!storeBag.AddItemTo(item, toSlot))
			{
				return;
			}
			bag.TakeOutItem(item);
			if (item.Template.BagType == eBageType.EquipBag && place < 31)
			{
				string key = $"temp_place_{item.ItemID}";
				if (client.Player.TempProperties.ContainsKey(key))
				{
					client.Player.TempProperties[key] = place;
				}
				else
				{
					client.Player.TempProperties.Add(key, place);
				}
			}
		}
		else
		{
			ItemInfo itemInfo = item.Clone();
			itemInfo.Count = count;
			if (bag.RemoveCountFromStack(item, count, eItemRemoveType.Stack) && !storeBag.AddItemTo(itemInfo, toSlot))
			{
				bag.AddCountToStack(item, count);
			}
		}
	}

	private static void MoveToBank(int place, int toplace, PlayerInventory bag, PlayerInventory bank, ItemInfo item)
	{
		if (bag == null || item == null)
		{
			return;
		}
		ItemInfo itemAt = bank.GetItemAt(toplace);
		if (itemAt != null)
		{
			if (item.CanStackedTo(itemAt) && item.Count + itemAt.Count <= item.Template.MaxCount)
			{
				if (bank.AddCountToStack(itemAt, item.Count))
				{
					bag.RemoveCountFromStack(item, item.Count);
				}
			}
			else if (itemAt.Template.BagType == (eBageType)bag.BagType)
			{
				bag.TakeOutItem(item);
				bank.TakeOutItem(itemAt);
				bag.AddItemTo(itemAt, place);
				bank.AddItemTo(item, toplace);
			}
		}
		else if (bank.AddItemTo(item, toplace))
		{
			bag.TakeOutItem(item);
		}
	}

	private static void MoveFromBank(GameClient client, int place, int toplace, PlayerInventory bag, PlayerInventory tobag, ItemInfo item)
	{
		if (item == null)
		{
			return;
		}
		PlayerInventory itemInventory = client.Player.GetItemInventory(item.Template);
		if (itemInventory == tobag)
		{
			ItemInfo itemAt = itemInventory.GetItemAt(toplace);
			if (itemAt == null)
			{
				if (itemInventory.AddItemTo(item, toplace))
				{
					bag.TakeOutItem(item);
				}
			}
			else if (item.CanStackedTo(itemAt) && item.Count + itemAt.Count <= item.Template.MaxCount)
			{
				if (itemInventory.AddCountToStack(itemAt, item.Count))
				{
					bag.RemoveCountFromStack(item, item.Count);
				}
			}
			else
			{
				itemInventory.TakeOutItem(itemAt);
				bag.TakeOutItem(item);
				itemInventory.AddItemTo(item, toplace);
				bag.AddItemTo(itemAt, place);
			}
		}
		else if (itemInventory.AddItem(item))
		{
			bag.TakeOutItem(item);
		}
	}
}
