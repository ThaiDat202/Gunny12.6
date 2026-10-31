using Game.Base.Packets;
using Game.Server.WonderFul;

namespace Game.Server.Packets.Client;

[PacketHandler(405, "初始化精彩活动")]
public class WonderfulActivityInitHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		WonderFulActivityManager.WonderFulActivityInit(client.Player, packet.ReadInt());
		return 0;
	}
}
