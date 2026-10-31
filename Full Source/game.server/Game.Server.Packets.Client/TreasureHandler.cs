using System;
using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(135, "场景用户离开")]
public class TreasureHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		int iD = client.Player.PlayerCharacter.ID;
		GSPacketIn gSPacketIn = new GSPacketIn(135, iD);
		switch (num)
		{
		case 0:
		{
			client.Player.Treasure.UpdateLoginDay();
			UserTreasureInfo currentTreasure3 = client.Player.Treasure.CurrentTreasure;
			List<TreasureDataInfo> treasureData = client.Player.Treasure.TreasureData;
			List<TreasureDataInfo> treasureDig = client.Player.Treasure.TreasureDig;
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(currentTreasure3.logoinDays);
			gSPacketIn.WriteInt(currentTreasure3.treasure);
			gSPacketIn.WriteInt(currentTreasure3.treasureAdd);
			gSPacketIn.WriteInt(currentTreasure3.friendHelpTimes);
			gSPacketIn.WriteBoolean(currentTreasure3.isEndTreasure);
			gSPacketIn.WriteBoolean(currentTreasure3.isBeginTreasure);
			gSPacketIn.WriteInt(treasureData.Count);
			for (int i = 0; i < treasureData.Count; i++)
			{
				gSPacketIn.WriteInt(treasureData[i].TemplateID);
				gSPacketIn.WriteInt(treasureData[i].ValidDate);
				gSPacketIn.WriteInt(treasureData[i].Count);
			}
			gSPacketIn.WriteInt(treasureDig.Count);
			for (int i = 0; i < treasureDig.Count; i++)
			{
				gSPacketIn.WriteInt(treasureDig[i].TemplateID);
				gSPacketIn.WriteInt(treasureDig[i].pos);
				gSPacketIn.WriteInt(treasureDig[i].ValidDate);
				gSPacketIn.WriteInt(treasureDig[i].Count);
			}
			client.Player.Out.SendTCP(gSPacketIn);
			break;
		}
		case 6:
		{
			UserTreasureInfo currentTreasure2 = client.Player.Treasure.CurrentTreasure;
			currentTreasure2.isBeginTreasure = true;
			gSPacketIn.WriteInt(6);
			gSPacketIn.WriteBoolean(currentTreasure2.isBeginTreasure);
			client.Player.SendTCP(gSPacketIn);
			client.Player.Treasure.UpdateUserTreasure(currentTreasure2);
			break;
		}
		case 3:
		{
			int num3 = packet.ReadInt();
			int index = num3 - 1;
			bool flag = true;
			TreasureDataInfo treasureDataInfo = client.Player.Treasure.TreasureData[index];
			if (treasureDataInfo == null)
			{
				return 0;
			}
			UserTreasureInfo currentTreasure4 = client.Player.Treasure.CurrentTreasure;
			if (currentTreasure4.treasure > 0)
			{
				currentTreasure4.treasure--;
			}
			else if (currentTreasure4.treasureAdd > 0)
			{
				currentTreasure4.treasureAdd--;
			}
			else
			{
				flag = false;
			}
			if (flag)
			{
				gSPacketIn.WriteInt(3);
				gSPacketIn.WriteInt(treasureDataInfo.TemplateID);
				gSPacketIn.WriteInt(num3);
				gSPacketIn.WriteInt(treasureDataInfo.Count);
				gSPacketIn.WriteInt(currentTreasure4.treasure);
				gSPacketIn.WriteInt(currentTreasure4.treasureAdd);
				client.Player.Out.SendTCP(gSPacketIn);
				treasureDataInfo.pos = num3;
				client.Player.Treasure.AddTreasureDig(treasureDataInfo, index);
				client.Player.Treasure.UpdateUserTreasure(currentTreasure4);
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(treasureDataInfo.TemplateID), treasureDataInfo.Count, 105);
				itemInfo.IsBinds = true;
				itemInfo.ValidDate = treasureDataInfo.ValidDate;
				client.Player.AddTemplate(itemInfo, itemInfo.Template.BagType, treasureDataInfo.Count, eGameView.RouletteTypeGet);
				client.Player.SendHideMessage(LanguageMgr.GetTranslation("TreasureHandler.Msg1", itemInfo.Template.Name, treasureDataInfo.Count));
			}
			break;
		}
		case 1:
		{
			int num2 = packet.ReadInt();
			gSPacketIn.WriteInt(1);
			gSPacketIn.WriteInt(0);
			client.Player.SendTCP(gSPacketIn);
			GamePlayer playerById = WorldMgr.GetPlayerById(num2);
			if (playerById == null)
			{
				using PlayerBussiness playerBussiness = new PlayerBussiness();
				playerBussiness.RemoveIsArrange(num2);
				playerBussiness.UpdateFriendHelpTimes(num2);
			}
			else
			{
				playerById.Farm.ClearIsArrange();
				playerById.Treasure.AddfriendHelpTimes();
				gSPacketIn.ClientID = playerById.PlayerCharacter.ID;
				playerById.SendTCP(gSPacketIn);
			}
			break;
		}
		case 2:
		{
			UserTreasureInfo currentTreasure = client.Player.Treasure.CurrentTreasure;
			currentTreasure.isBeginTreasure = false;
			currentTreasure.isEndTreasure = true;
			gSPacketIn.WriteInt(2);
			gSPacketIn.WriteBoolean(currentTreasure.isEndTreasure);
			client.Player.SendTCP(gSPacketIn);
			client.Player.Treasure.UpdateUserTreasure(currentTreasure);
			client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("TreasureHandler.Msg2"));
			break;
		}
		default:
		{
			TreasurePackageType treasurePackageType = (TreasurePackageType)num;
			Console.WriteLine("//treasure_cmd: " + treasurePackageType);
			break;
		}
		}
		return 0;
	}
}
