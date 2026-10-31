using System;
using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(361, "添加好友")]
public class CardAchievementTypeHandler : IPacketHandler
{
	public static Random random;

	static CardAchievementTypeHandler()
	{
		random = new Random();
	}

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int type = packet.ReadInt();
		client.Player.CardAchievementInventory.Finish(type);
		client.Player.EquipBag.UpdatePlayerProperties();
		return 0;
	}
}
