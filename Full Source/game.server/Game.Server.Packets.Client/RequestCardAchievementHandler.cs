using System;
using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(362, "添加好友")]
public class RequestCardAchievementHandler : IPacketHandler
{
	public static Random random = new Random();

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		client.Player.CardAchievementInventory.CheckFinish();
		return 0;
	}
}
