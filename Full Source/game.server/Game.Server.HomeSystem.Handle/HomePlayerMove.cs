using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HomeSystem.Handle;

[HomeSystemHandleAttbute(3)]
public class HomePlayerMove : IHomeSystemCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int val = packet.ReadInt();
		int val2 = packet.ReadInt();
		GSPacketIn gSPacketIn = new GSPacketIn(285);
		gSPacketIn.WriteByte(3);
		gSPacketIn.WriteInt(Player.PlayerId);
		gSPacketIn.WriteInt(val);
		gSPacketIn.WriteInt(val2);
		Player.SendTCP(gSPacketIn);
		return false;
	}
}
