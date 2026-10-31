using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.DevilTurn.Handle;

[DevilTurnHandle(3)]
public class UpdateJackpot : IDevilTurnCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		DevilTurnMgr.Out.SendUpdateJackpot(player);
		return 0;
	}
}
