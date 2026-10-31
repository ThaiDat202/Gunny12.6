using System;
using System.Collections.Generic;
using System.Drawing;
using Bussiness;
using Game.Base.Packets;
using Game.Server.ConsortiaDomain;
using Game.Server.Games;
using Game.Server.Packets.Package;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(371, "user ac action")]
public class ConsortiaDomainHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		ConsortiaDomainPackageType consortiaDomainPackageType = (ConsortiaDomainPackageType)packet.ReadByte();
		switch (consortiaDomainPackageType)
		{
		case ConsortiaDomainPackageType.RemovePlayer:
			if (client.Player.CurrentRoom != null)
			{
				client.Player.CurrentRoom.RemoveAllPlayer();
			}
			break;
		case ConsortiaDomainPackageType.OtherInfo:
			if (client.Player.CurrentRoom != null && client.Player.PlayerCharacter.ConsortiaID > 0)
			{
				List<UserConsortiaDomainInfo> allPlayerDomain = GameMgr.ConsortiaDomain.GetAllPlayerDomain(client.Player.PlayerCharacter.ConsortiaID);
				if (allPlayerDomain.Count > 0)
				{
					GSPacketIn pkg = GameMgr.ConsortiaDomain.SendOtherPlayers(allPlayerDomain);
					client.SendTCP(pkg);
				}
			}
			break;
		case ConsortiaDomainPackageType.MOVE:
		{
			UserConsortiaDomainInfo player = GameMgr.ConsortiaDomain.GetPlayer(client.Player.PlayerId);
			if (player != null)
			{
				int num = packet.ReadInt();
				Point[] array = new Point[num];
				for (int i = 0; i < num; i++)
				{
					Point point = (array[i] = new Point(packet.ReadInt(), packet.ReadInt()));
				}
				player.Postion = array[num - 1];
				GameMgr.ConsortiaDomain.SendUpdatePostionPlayer(player, array);
			}
			break;
		}
		case ConsortiaDomainPackageType.Active:
		{
			ConsortiaDomainInfo consortiaDomain2 = GameMgr.ConsortiaDomain.GetConsortiaDomain(client.Player.PlayerCharacter.ConsortiaID);
			if (consortiaDomain2 == null)
			{
				consortiaDomain2 = new ConsortiaDomainInfo();
				consortiaDomain2.ConsortiaID = client.Player.PlayerCharacter.ConsortiaID;
				consortiaDomain2.BeginTime = DateTime.Now;
				consortiaDomain2.EndTime = DateTime.Now;
				consortiaDomain2.UnFightBuildList = "";
				if (GameMgr.ConsortiaDomain.AddConsortiaDomain(consortiaDomain2))
				{
					GSPacketIn gSPacketIn4 = new GSPacketIn(371);
					gSPacketIn4.WriteByte(7);
					client.SendTCP(gSPacketIn4);
					client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.ConsortiaDomain.ActiveSuccess"));
				}
				else
				{
					client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.ConsortiaDomain.ActiveError"));
				}
			}
			else
			{
				client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.ConsortiaDomain.ActiveError"));
			}
			break;
		}
		case ConsortiaDomainPackageType.GetConsortiaInfo:
		{
			GSPacketIn gSPacketIn3 = new GSPacketIn(371);
			gSPacketIn3.WriteByte(12);
			if (client.Player.PlayerCharacter.ConsortiaID > 0)
			{
				ConsortiaDomainInfo consortiaDomain = GameMgr.ConsortiaDomain.GetConsortiaDomain(client.Player.PlayerCharacter.ConsortiaID);
				if (consortiaDomain != null)
				{
					gSPacketIn3.WriteBoolean(val: true);
					gSPacketIn3.WriteInt(1);
					gSPacketIn3.WriteDateTime(consortiaDomain.BeginTime);
					gSPacketIn3.WriteInt(0);
					gSPacketIn3.WriteInt(0);
					gSPacketIn3.WriteInt(0);
					gSPacketIn3.WriteInt(0);
					gSPacketIn3.WriteInt(0);
					gSPacketIn3.WriteInt(0);
					gSPacketIn3.WriteInt(0);
					gSPacketIn3.WriteInt(0);
					gSPacketIn3.WriteDateTime(consortiaDomain.EndTime);
					client.SendTCP(gSPacketIn3);
				}
			}
			gSPacketIn3.WriteBoolean(val: false);
			client.SendTCP(gSPacketIn3);
			break;
		}
		case ConsortiaDomainPackageType.ActiveSate:
		{
			GSPacketIn gSPacketIn2 = new GSPacketIn(371);
			gSPacketIn2.WriteByte(15);
			gSPacketIn2.WriteInt(0);
			client.SendTCP(gSPacketIn2);
			break;
		}
		case ConsortiaDomainPackageType.buildRepairInfo:
		{
			GSPacketIn gSPacketIn = new GSPacketIn(371);
			gSPacketIn.WriteByte(17);
			gSPacketIn.WriteInt(5);
			gSPacketIn.WriteInt(1);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(2);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(3);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(4);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(5);
			gSPacketIn.WriteInt(0);
			client.SendTCP(gSPacketIn);
			break;
		}
		default:
			Console.WriteLine("ConsortiaDomain: " + consortiaDomainPackageType);
			break;
		}
		return 1;
	}
}
