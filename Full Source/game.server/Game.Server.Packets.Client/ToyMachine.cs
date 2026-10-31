using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(324, "打开物品")]
public class ToyMachine : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(324);
		gSPacketIn.WriteDateTime(client.Player.Extra.Info.silverTime);
		gSPacketIn.WriteInt(client.Player.Extra.Info.silverCount);
		gSPacketIn.WriteDateTime(client.Player.Extra.Info.goldTime);
		client.Player.SendTCP(gSPacketIn);
		return 1;
	}
}
