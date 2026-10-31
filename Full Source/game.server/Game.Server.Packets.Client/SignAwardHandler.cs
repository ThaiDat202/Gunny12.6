using Game.Base.Packets;
using Game.Server.Managers;

namespace Game.Server.Packets.Client;

[PacketHandler(90, "场景用户离开")]
public class SignAwardHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int dailyLog = packet.ReadInt();
		string message = "每日领取成功";
		if (AwardMgr.AddSignAwards(client.Player, dailyLog))
		{
			client.Out.SendMessage(eMessageType.Normal, message);
		}
		return 0;
	}
}
