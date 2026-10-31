using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HomeSystem.Handle;

[HomeSystemHandleAttbute(23)]
public class Prosperity : IHomeSystemCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int val = packet.ReadInt();
		GSPacketIn gSPacketIn = new GSPacketIn(285);
		gSPacketIn.WriteByte(23);
		gSPacketIn.WriteInt(val);
		gSPacketIn.WriteInt(1);
		gSPacketIn.WriteInt(2);
		gSPacketIn.WriteInt(3);
		Player.SendTCP(gSPacketIn);
		return false;
	}
}
