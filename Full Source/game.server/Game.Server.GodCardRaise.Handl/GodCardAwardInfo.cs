using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.GodCardRaise.Handle;

[GodCardRaiseHandleAttbute(25)]
public class GodCardAwardInfo : IGodCardRaiseCommandHadler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(329);
		gSPacketIn.WriteByte(25);
		gSPacketIn.WriteInt(0);
		player.Out.SendTCP(gSPacketIn);
		return 1;
	}
}
