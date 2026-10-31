using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(213, "场景用户离开")]
public class UseLogHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		int num2 = num;
		int num3 = num2;
		if (num3 != -1)
		{
		}
		return 0;
	}
}
