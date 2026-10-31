using System;
using Game.Base.Packets;
using Game.Server.Rooms;

namespace Game.Server.Packets.Client;

[PacketHandler(16, "Player enter scene.")]
public class UserEnterSceneHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		client.Player.BeginChanges();
		switch (num)
		{
		case 1:
			client.Player.PlayerState = ePlayerState.Online;
			break;
		case 2:
			client.Player.PlayerState = ePlayerState.Away;
			break;
		case 8:
			client.Player.PlayerState = ePlayerState.HappyGame;
			break;
		default:
			Console.WriteLine("UserEnterScene: typeScene {0}", num);
			break;
		}
		client.Player.CommitChanges();
		RoomMgr.EnterWaitingRoom(client.Player);
		return 1;
	}
}
