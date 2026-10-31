using System;
using System.Data.Entity;
using System.Linq;
using Bussiness.Managers;
using EntityDatabase.PlayerModels;
using EntityDatabase.ServerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(660, "弹王荣誉")]
public class HonorHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadByte();
		switch (num)
		{
		case 1:
			SendHonorInfo(client);
			break;
		case 2:
			SendHonorRecord(client);
			break;
		case 3:
		{
			int userID = packet.ReadInt();
			int type = packet.ReadInt();
			HonorPraisePeople(client, userID, type);
			break;
		}
		case 4:
		{
			int id = packet.ReadInt();
			string msg = packet.ReadString();
			string nickName = packet.ReadString();
			HonorReport(client, id, msg, nickName);
			break;
		}
		case 5:
			GetHonorReward(client);
			break;
		default:
			Console.WriteLine("HonorCmd:" + num);
			break;
		}
		return 0;
	}

	private void HonorReport(GameClient client, int Id, string Msg, string NickName)
	{
		Console.WriteLine("SendHonorReport not found!");
	}

	private void SendHonorInfo(GameClient client)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(660);
		gSPacketIn.WriteByte(1);
		gSPacketIn.WriteBoolean(client.Player.PlayerCharacter.HonorInfo.IsGet);
		gSPacketIn.WriteInt(client.Player.PlayerCharacter.HonorInfo.HonorExp);
		gSPacketIn.WriteInt(client.Player.PlayerCharacter.HonorInfo.HonorLevel);
		gSPacketIn.WriteDateTime(client.Player.PlayerCharacter.HonorInfo.GoodPeopleCardTime);
		client.SendTCP(gSPacketIn);
	}

	private void SendHonorRecord(GameClient client)
	{
		PlayerData playerData = new PlayerData();
		client.Player.PlayerCharacter.HonorRecords = playerData.Sys_Users_HonorRecord.Where((Sys_Users_HonorRecord p) => p.UserID == client.Player.PlayerCharacter.ID && p.Type == 0).ToList();
		GSPacketIn gSPacketIn = new GSPacketIn(660);
		gSPacketIn.WriteByte(2);
		gSPacketIn.WriteInt(playerData.Sys_Users_HonorRecord.Where((Sys_Users_HonorRecord p) => p.UserID == client.Player.PlayerCharacter.ID && p.Type == 0).ToList().Count);
		gSPacketIn.WriteInt(playerData.Sys_Users_HonorRecord.Where((Sys_Users_HonorRecord p) => p.UserID == client.Player.PlayerCharacter.ID && p.Type == 1).ToList().Count);
		gSPacketIn.WriteInt(client.Player.PlayerCharacter.HonorRecords.Count);
		foreach (Sys_Users_HonorRecord honorRecord in client.Player.PlayerCharacter.HonorRecords)
		{
			gSPacketIn.WriteString(honorRecord.NickName);
			gSPacketIn.WriteInt(honorRecord.Type);
			gSPacketIn.WriteDateTime(honorRecord.Date);
		}
		client.SendTCP(gSPacketIn);
	}

	private void GetHonorReward(GameClient client)
	{
		ServerData serverData = new ServerData();
		int levelGift = serverData.TS_HonorSystem_Template.FirstOrDefault((TS_HonorSystem_Template e) => e.Level == client.Player.PlayerCharacter.HonorInfo.HonorLevel).LevelGift;
		if (levelGift == 0)
		{
			throw new NullReferenceException("HonorHandler.giftID is null");
		}
		ItemTemplateInfo goods = ItemMgr.FindItemTemplate(levelGift);
		ItemInfo cloneItem = ItemInfo.CreateFromTemplate(goods, 1, 101);
		client.Player.AddTemplate(cloneItem);
		client.Player.SendMessage("领取成功.");
		client.Player.PlayerCharacter.HonorInfo.IsGet = true;
		SendHonorInfo(client);
		SendHonorRecord(client);
	}

	private void HonorPraisePeople(GameClient client, int UserID, int Type)
	{
		if (client.Player.PlayerCharacter.HonorInfo.Count <= 0)
		{
			client.Player.SendMessage("点赞/踩次数不足,明天再来试试吧.");
			return;
		}
		client.Player.PlayerCharacter.HonorInfo.Count--;
		PlayerData playerData = new PlayerData();
		Sys_Users_HonorRecord sys_Users_HonorRecord = new Sys_Users_HonorRecord();
		sys_Users_HonorRecord.UserID = UserID;
		sys_Users_HonorRecord.Type = Type;
		sys_Users_HonorRecord.NickName = client.Player.PlayerCharacter.NickName;
		sys_Users_HonorRecord.Date = DateTime.Now;
		playerData.Sys_Users_HonorRecord.Add(sys_Users_HonorRecord);
		playerData.SaveChanges();
		GamePlayer playerById = WorldMgr.GetPlayerById(UserID);
		if (playerById != null && Type == 0)
		{
			playerById.AddHonorExp(5);
			GSPacketIn gSPacketIn = new GSPacketIn(660);
			gSPacketIn.WriteByte(6);
			gSPacketIn.WriteInt(Type);
			gSPacketIn.WriteInt(client.Player.PlayerCharacter.ID);
			gSPacketIn.WriteString(client.Player.PlayerCharacter.NickName);
			playerById.SendTCP(gSPacketIn);
		}
		else
		{
			Sys_Users_Honor offlinePlayer = playerData.Sys_Users_Honor.FirstOrDefault((Sys_Users_Honor p) => p.UserID == UserID);
			if (offlinePlayer != null)
			{
				offlinePlayer.HonorExp++;
				ServerData serverData = new ServerData();
				int exp = serverData.TS_HonorSystem_Template.FirstOrDefault((TS_HonorSystem_Template e) => e.Level == offlinePlayer.HonorLevel).Exp;
				if (offlinePlayer.HonorExp >= exp)
				{
					offlinePlayer.HonorLevel++;
				}
				playerData.Entry(offlinePlayer).State = EntityState.Modified;
			}
			else
			{
				offlinePlayer = new Sys_Users_Honor();
				offlinePlayer.UserID = UserID;
				offlinePlayer.HonorLevel = 1;
				offlinePlayer.HonorExp = 1;
				offlinePlayer.Count = 3;
				offlinePlayer.GoodPeopleCardTime = DateTime.Now;
				playerData.Sys_Users_Honor.Add(offlinePlayer);
			}
			playerData.SaveChanges();
		}
		SendHonorInfo(client);
		SendHonorRecord(client);
	}
}
