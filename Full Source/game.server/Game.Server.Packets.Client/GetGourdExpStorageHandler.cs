using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(393, "获取经验葫芦存储值")]
public class GetGourdExpStorageHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		client.Player.Out.SendGourdExpStorage(client.Player);
		return 1;
	}
}
