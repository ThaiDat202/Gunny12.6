using System;
using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(68, "宠物系统")]
public class PetHandler : IPacketHandler
{
	public static Random random = new Random();

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		if (client.Player.PetHandler != null)
		{
			client.Player.PetHandler.ProcessData(client.Player, packet);
			return 1;
		}
		return 0;
	}
}
