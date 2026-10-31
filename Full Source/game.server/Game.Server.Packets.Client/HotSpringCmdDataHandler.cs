using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(191, "礼堂数据")]
public class HotSpringCmdDataHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		if (client.Player.HotSpringRoomsHandler != null)
		{
			client.Player.HotSpringRoomsHandler.ProcessData(client.Player, packet);
		}
		return 0;
	}
}
