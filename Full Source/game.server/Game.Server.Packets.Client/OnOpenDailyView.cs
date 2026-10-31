using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(293, "每日签到")]
internal class OnOpenDailyView : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(293, client.Player.PlayerId);
		gSPacketIn.WriteString("");
		gSPacketIn.WriteInt(1);
		gSPacketIn.WriteInt(1);
		gSPacketIn.WriteInt(1);
		gSPacketIn.WriteInt(1);
		client.Player.Out.SendTCP(gSPacketIn);
		return 0;
	}
}
