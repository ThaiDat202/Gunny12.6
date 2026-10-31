using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(608, "恶魔转盘")]
public class DevilTurnHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		client.Player.DevilTurnHandler.ProcessData(client.Player, packet);
		return 1;
	}
}
