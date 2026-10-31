using System;
using System.Collections.Generic;
using System.Linq;
using Bussiness;
using EntityDatabase.PlayerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Packets.Client;

[PacketHandler(355, "烟花红包")]
public class FireWorksHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		switch (packet.ReadByte())
		{
		case 1:
			SendFireworksInfoAttribute(client.Player);
			break;
		case 2:
		{
			string sendMsg = packet.ReadString();
			int sendType = packet.ReadInt();
			int templateId = packet.ReadInt();
			int sendCount = packet.ReadInt();
			SendFireworksPut(client, sendMsg, sendType, templateId, sendCount);
			break;
		}
		case 3:
		{
			int grabId = packet.ReadInt();
			SendFireworksGrab(client.Player, grabId);
			break;
		}
		case 4:
			SendFireworksPutHistory(client.Player);
			break;
		case 5:
			SendFireworksGrabHistory(client.Player);
			break;
		}
		return 0;
	}

	private void SendFireworksGrabHistory(GamePlayer player)
	{
		PlayerData playerData = new PlayerData();
		GSPacketIn gSPacketIn = new GSPacketIn(355);
		gSPacketIn.WriteByte(5);
		List<Sys_Users_FireWorks_Data> list = playerData.Sys_Users_FireWorks_Data.Where((Sys_Users_FireWorks_Data fw) => fw.UserID == player.PlayerId).ToList();
		int num = 0;
		int num2 = 0;
		int val = 0;
		foreach (Sys_Users_FireWorks_Data item in list)
		{
			num += item.GetMoney;
			num2++;
		}
		gSPacketIn.WriteInt(num);
		gSPacketIn.WriteInt(num2);
		gSPacketIn.WriteInt(val);
		gSPacketIn.WriteInt(list.Count);
		foreach (Sys_Users_FireWorks_Data item2 in list)
		{
			gSPacketIn.WriteInt(item2.ID);
			gSPacketIn.WriteInt(item2.GetMoney);
			gSPacketIn.WriteDateTime(item2.GetDate);
		}
		player.SendTCP(gSPacketIn);
	}

	private void SendFireworksPutHistory(GamePlayer player)
	{
		PlayerData playerData = new PlayerData();
		GSPacketIn gSPacketIn = new GSPacketIn(355);
		gSPacketIn.WriteByte(4);
		List<Sys_Users_FireWorks> list = playerData.Sys_Users_FireWorks.Where((Sys_Users_FireWorks fw) => fw.UserID == player.PlayerId).ToList();
		int num = 0;
		int num2 = 0;
		foreach (Sys_Users_FireWorks item in list)
		{
			num += item.SendMoney;
			num2++;
		}
		gSPacketIn.WriteInt(num);
		gSPacketIn.WriteInt(num2);
		gSPacketIn.WriteInt(list.Count);
		foreach (Sys_Users_FireWorks item2 in list)
		{
			gSPacketIn.WriteInt(item2.ID);
			gSPacketIn.WriteInt(item2.SendType);
			gSPacketIn.WriteDateTime(item2.SendDate);
		}
		player.SendTCP(gSPacketIn);
	}

	private void SendFireworksGrab(GamePlayer player, int grabId)
	{
		PlayerData playerData = new PlayerData();
		Sys_Users_FireWorks sys_Users_FireWorks = playerData.Sys_Users_FireWorks.FirstOrDefault((Sys_Users_FireWorks fw) => fw.ID == grabId);
		Sys_Users_FireWorks_Data sys_Users_FireWorks_Data = playerData.Sys_Users_FireWorks_Data.FirstOrDefault((Sys_Users_FireWorks_Data fwd) => fwd.ID == grabId && fwd.UserID == player.PlayerId);
		GSPacketIn gSPacketIn = new GSPacketIn(355);
		gSPacketIn.WriteByte(3);
		if (sys_Users_FireWorks_Data == null)
		{
			gSPacketIn.WriteInt(4);
			player.AddGiftToken(sys_Users_FireWorks.SendMoney);
			Sys_Users_FireWorks_Data entity = new Sys_Users_FireWorks_Data
			{
				UserID = player.PlayerId,
				FireWorksID = grabId,
				IsGet = true,
				GetMoney = sys_Users_FireWorks.SendMoney,
				GetDate = DateTime.Now
			};
			playerData.Sys_Users_FireWorks_Data.Add(entity);
			playerData.SaveChanges();
		}
		else if (sys_Users_FireWorks_Data.IsGet)
		{
			gSPacketIn.WriteInt(2);
		}
		else if (sys_Users_FireWorks == null)
		{
			gSPacketIn.WriteInt(1);
		}
		else if (sys_Users_FireWorks.SendCount == 0)
		{
			gSPacketIn.WriteInt(3);
		}
		gSPacketIn.WriteInt(sys_Users_FireWorks.SendMoney);
		gSPacketIn.WriteInt(grabId);
		player.SendTCP(gSPacketIn);
	}

	private void SendFireworksPut(GameClient client, string sendMsg, int sendType, int templateId, int sendCount)
	{
		int num = 0;
		if (!client.Player.RemoveTemplate(templateId, 1))
		{
			client.Player.SendMessage(LanguageMgr.GetTranslation("FireworksHandler.SendFireworksPut.Failed"));
			return;
		}
		num = templateId switch
		{
			12549 => 100, 
			12550 => 50, 
			_ => 200, 
		};
		Sys_Users_FireWorks entity = new Sys_Users_FireWorks
		{
			UserID = client.Player.PlayerId,
			SendMoney = num,
			SendCount = sendCount,
			SendMessage = sendMsg,
			SendType = sendType,
			SendDate = DateTime.Now
		};
		PlayerData playerData = new PlayerData();
		int iD = playerData.Sys_Users_FireWorks.Add(entity).ID;
		playerData.SaveChanges();
		GSPacketIn gSPacketIn = new GSPacketIn(355);
		gSPacketIn.WriteByte(2);
		gSPacketIn.WriteInt(iD);
		gSPacketIn.WriteInt(sendCount);
		gSPacketIn.WriteInt(client.Player.PlayerId);
		gSPacketIn.WriteString(client.Player.PlayerCharacter.NickName);
		gSPacketIn.WriteString(sendMsg);
		client.SendTCP(gSPacketIn);
	}

	private void SendFireworksInfoAttribute(GamePlayer player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(355);
		gSPacketIn.WriteByte(1);
		gSPacketIn.WriteInt(player.PropBag.GetItemCount(12549));
		gSPacketIn.WriteInt(player.PropBag.GetItemCount(12550));
		gSPacketIn.WriteInt(player.PropBag.GetItemCount(12551));
		gSPacketIn.WriteInt(player.PropBag.GetItemCount(12552));
		gSPacketIn.WriteInt(player.PropBag.GetItemCount(12553));
		player.SendTCP(gSPacketIn);
	}
}
