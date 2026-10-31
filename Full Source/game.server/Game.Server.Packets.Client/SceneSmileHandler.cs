using Game.Base.Packets;
using Game.Server.Rooms;

namespace Game.Server.Packets.Client;

[PacketHandler(20, "用户场景表情")]
public class SceneSmileHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		packet.ClientID = client.Player.PlayerCharacter.ID;
		if (client.Player.CurrentRoom != null)
		{
			if (client.Player.CurrentRoom.BattleServer == null)
			{
				client.Player.CurrentRoom.SendToAll(packet);
			}
			else if (client.Player.CurrentRoom.Game != null)
			{
				packet.Parameter2 = client.Player.CurrentRoom.Game.Id;
				client.Player.CurrentRoom.BattleServer.Server.SendSmileChatMessage(packet);
			}
		}
		else
		{
			if (client.Player.CurrentMarryRoom != null)
			{
				client.Player.CurrentMarryRoom.SendToAllForScene(packet, client.Player.MarryMap);
			}
			if (client.Player.CurrentHotSpringRoom != null)
			{
				client.Player.CurrentHotSpringRoom.SendToALL(packet);
			}
			else
			{
				RoomMgr.WaitingRoom.SendToALL(packet);
			}
		}
		return 1;
	}
}
