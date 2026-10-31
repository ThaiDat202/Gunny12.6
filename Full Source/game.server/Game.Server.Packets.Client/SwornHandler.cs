using System;
using System.Collections.Generic;
using System.Linq;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;

namespace Game.Server.Packets.Client;

[PacketHandler(669, "结义系统")]
public class SwornHandler : IPacketHandler
{
	private bool isAccept = false;

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		byte b = packet.ReadByte();
		switch (b)
		{
		case 1:
			SendSwornInfo(client.Player);
			break;
		case 2:
		{
			int userId3 = packet.ReadInt();
			SendSworInvatenAlert(client.Player, userId3);
			break;
		}
		case 3:
		{
			int userId2 = packet.ReadInt();
			SendSwornAcceptAlert(client.Player, userId2);
			break;
		}
		case 4:
			RelieveSworn(client.Player);
			break;
		case 5:
			SendSwornSceneInfo(client.Player);
			break;
		case 6:
		{
			int userId = packet.ReadInt();
			SendSwornSceneInvate(client.Player, userId);
			break;
		}
		case 7:
		{
			int sceneAcceptUserId = packet.ReadInt();
			SendSwornSceneAccept(client.Player, sceneAcceptUserId);
			break;
		}
		case 9:
			SendEnterSwornScene(client.Player);
			break;
		case 10:
			SendSwornLeaveScene(client.Player);
			break;
		case 11:
		{
			int posX = packet.ReadInt();
			int posY = packet.ReadInt();
			string str = packet.ReadString();
			MoveHandler(client.Player, posX, posY, str);
			break;
		}
		default:
			Console.WriteLine("SwornHandler cmd:" + b);
			break;
		}
		return 0;
	}

	private void MoveHandler(GamePlayer player, int posX, int posY, string str)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(669);
		gSPacketIn.WriteByte(11);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(posX);
		gSPacketIn.WriteInt(posY);
		gSPacketIn.WriteString(str);
		player.SendTCP(gSPacketIn);
	}

	private void SendEnterSwornScene(GamePlayer player)
	{
		throw new NotImplementedException();
	}

	private void SendSwornLeaveScene(GamePlayer player)
	{
		throw new NotImplementedException();
	}

	private void SendSwornSceneAccept(GamePlayer player, int sceneAcceptUserId)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(669);
		gSPacketIn.WriteByte(7);
		gSPacketIn.WriteBoolean(val: true);
		player.SendTCP(gSPacketIn);
	}

	private void SendSwornSceneInvate(GamePlayer player, int userId)
	{
		GamePlayer playerById = WorldMgr.GetPlayerById(userId);
		if (playerById == null)
		{
			player.SendMessage(LanguageMgr.GetTranslation("SwornHandler.SendSworInvatenAlert.Offline"));
			return;
		}
		GSPacketIn gSPacketIn = new GSPacketIn(669);
		gSPacketIn.WriteByte(6);
		gSPacketIn.WriteString(player.PlayerCharacter.NickName);
		gSPacketIn.WriteInt(player.PlayerId);
		playerById.SendTCP(gSPacketIn);
	}

	private void SendSwornSceneInfo(GamePlayer player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(669);
		gSPacketIn.WriteByte(5);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(0);
		player.SendTCP(gSPacketIn);
	}

	private void RelieveSworn(GamePlayer player)
	{
		throw new NotImplementedException();
	}

	private void SendSwornAcceptAlert(GamePlayer player, int userId)
	{
		GamePlayer playerById = WorldMgr.GetPlayerById(userId);
		if (playerById == null)
		{
			player.SendMessage(LanguageMgr.GetTranslation("SwornHandler.SendSworInvatenAlert.Offline"));
			return;
		}
		List<int> list = (string.IsNullOrEmpty(player.PlayerCharacter.SwornInfo.swornUserIDs) ? new List<int>() : player.PlayerCharacter.SwornInfo.swornUserIDs.Split(',').ToList().Select(int.Parse)
			.ToList());
		if (list.Contains(userId))
		{
			player.SendMessage(LanguageMgr.GetTranslation("SwornHandler.SendSwornAcceptAlert.Fail"));
			return;
		}
		if (list.Count >= 4)
		{
			player.SendMessage(LanguageMgr.GetTranslation("SwornHandler.SendSwornAcceptAlert.Max"));
			return;
		}
		list.Add(userId);
		player.PlayerCharacter.SwornInfo.swornUserIDs = string.Join(",", list);
		player.PlayerCharacter.SwornInfo.swornId = 1;
		isAccept = true;
		List<int> list2 = (string.IsNullOrEmpty(playerById.PlayerCharacter.SwornInfo.swornUserIDs) ? new List<int>() : playerById.PlayerCharacter.SwornInfo.swornUserIDs.Split(',').ToList().Select(int.Parse)
			.ToList());
		if (list2.Contains(player.PlayerId))
		{
			playerById.SendMessage(LanguageMgr.GetTranslation("SwornHandler.SendSwornAcceptAlert.Fail"));
			return;
		}
		if (list2.Count >= 4)
		{
			playerById.SendMessage(LanguageMgr.GetTranslation("SwornHandler.SendSwornAcceptAlert.Max"));
			return;
		}
		list2.Add(player.PlayerId);
		playerById.PlayerCharacter.SwornInfo.swornUserIDs = string.Join(",", list2);
		playerById.PlayerCharacter.SwornInfo.swornId = 1;
		GSPacketIn gSPacketIn = new GSPacketIn(669);
		gSPacketIn.WriteByte(3);
		gSPacketIn.WriteBoolean(isAccept);
		player.SendTCP(gSPacketIn);
		SendSwornInfo(playerById);
	}

	private void SendSworInvatenAlert(GamePlayer player, int userId)
	{
		GamePlayer playerById = WorldMgr.GetPlayerById(userId);
		if (playerById == null)
		{
			player.SendMessage(LanguageMgr.GetTranslation("SwornHandler.SendSworInvatenAlert.Offline"));
			return;
		}
		GSPacketIn gSPacketIn = new GSPacketIn(669);
		gSPacketIn.WriteByte(2);
		gSPacketIn.WriteInt(player.PlayerId);
		gSPacketIn.WriteString(player.PlayerCharacter.NickName);
		playerById.SendTCP(gSPacketIn);
	}

	private void SendSwornInfo(GamePlayer player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(669);
		gSPacketIn.WriteByte(1);
		gSPacketIn.WriteInt(player.PlayerCharacter.SwornInfo.swornId);
		gSPacketIn.WriteInt(player.PlayerCharacter.SwornInfo.swornCount);
		gSPacketIn.WriteInt(player.PlayerCharacter.SwornInfo.todayGetSwornCount);
		gSPacketIn.WriteInt(player.PlayerCharacter.SwornInfo.isHasSwornItem);
		gSPacketIn.WriteInt(player.PlayerCharacter.SwornInfo.pveCount);
		gSPacketIn.WriteInt(player.PlayerCharacter.SwornInfo.pvpCount);
		gSPacketIn.WriteInt(player.PlayerCharacter.SwornInfo.giveCount);
		gSPacketIn.WriteInt(player.PlayerCharacter.SwornInfo.skillId);
		List<int> list = (string.IsNullOrEmpty(player.PlayerCharacter.SwornInfo.swornUserIDs) ? new List<int>() : player.PlayerCharacter.SwornInfo.swornUserIDs.Split(',').ToList().Select(int.Parse)
			.ToList());
		gSPacketIn.WriteInt(list.Count);
		foreach (int item in list)
		{
			gSPacketIn.WriteInt(item);
			gSPacketIn.WriteString(WorldMgr.GetUserNickNameByUserId(item));
			gSPacketIn.WriteString("");
		}
		gSPacketIn.WriteBoolean(player.PlayerCharacter.IsMarried);
		if (player.PlayerCharacter.IsMarried)
		{
			gSPacketIn.WriteDateTime(player.PlayerCharacter.MarryDate);
		}
		player.SendTCP(gSPacketIn);
	}
}
