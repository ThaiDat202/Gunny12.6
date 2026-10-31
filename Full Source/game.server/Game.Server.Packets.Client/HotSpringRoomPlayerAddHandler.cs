using System;
using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(198, "礼堂数据")]
public class HotSpringRoomPlayerAddHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		if (client.Player.CurrentHotSpringRoom != null)
		{
		}
		Console.WriteLine("HotSpringRoomPlayerAddHandler");
		return 0;
	}
}
