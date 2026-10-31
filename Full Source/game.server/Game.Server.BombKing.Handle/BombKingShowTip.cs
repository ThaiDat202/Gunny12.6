using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.BombKing.Handle;

[BombKingHandleAttbute(9)]
public class BombKingShowTip : IBombKingCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(263);
		gSPacketIn.WriteByte(9);
		gSPacketIn.WriteBoolean(BombKingMgr.IsBombKingOpen);
		Player.SendTCP(gSPacketIn);
		return false;
	}
}
