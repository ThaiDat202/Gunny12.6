using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HomeSystem.Handle;

[HomeSystemHandleAttbute(13)]
public class HouseExit : IHomeSystemCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int val = packet.ReadInt();
		GSPacketIn gSPacketIn = new GSPacketIn(285);
		gSPacketIn.WriteByte(13);
		gSPacketIn.WriteInt(val);
		Player.SendTCP(gSPacketIn);
		return true;
	}
}
