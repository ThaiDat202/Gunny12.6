using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.DevilTurn.Handle;

[DevilTurnHandle(10)]
public class GetInfo : IDevilTurnCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		DevilTurnMgr.Out.SendUpdateInfo(player);
		return 0;
	}
}
