using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(630, "user ac action")]
public class UpdateFightPowerHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		client.Player.UpdateFightPower();
		return 0;
	}
}
