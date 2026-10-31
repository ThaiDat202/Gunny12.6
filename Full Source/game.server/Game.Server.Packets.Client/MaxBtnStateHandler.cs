using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(538, "打开物品")]
public class MaxBtnStateHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		byte b = packet.ReadByte();
		GSPacketIn gSPacketIn = new GSPacketIn(538);
		gSPacketIn.WriteByte(b);
		bool val = false;
		byte b2 = b;
		byte b3 = b2;
		if (b3 == 2)
		{
			val = packet.ReadBoolean();
		}
		gSPacketIn.WriteBoolean(val);
		client.SendTCP(gSPacketIn);
		return 1;
	}
}
