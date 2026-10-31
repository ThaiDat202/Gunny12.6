using System;
using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(245, "场景用户离开")]
public class RequestAwardsHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		client.Player.CaddyType = num;
		if (client.Server.Configuration.DebugMode)
		{
			Console.WriteLine("RequestAwardsHandler.Type {0}", num);
		}
		return 0;
	}
}
