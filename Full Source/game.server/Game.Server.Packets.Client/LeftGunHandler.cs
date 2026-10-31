using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(128, "物品炼化")]
public class LeftGunHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		int num2 = num;
		int num3 = num2;
		if (num3 == 1)
		{
			client.Player.Roulette.SendLeftGunStart();
		}
		return 1;
	}
}
