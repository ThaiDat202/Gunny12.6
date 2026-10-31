using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(323, "打开物品")]
public class IsMasterHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		if (client.Player.Level <= 1)
		{
			client.Player.AddGP(16341, canDupeExp: false, canAddTop: false);
		}
		return 1;
	}
}
