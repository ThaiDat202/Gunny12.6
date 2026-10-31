using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.BombKing.Handle;

[BombKingHandleAttbute(4)]
public class BombKingBattleLog : IBombKingCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		return false;
	}
}
