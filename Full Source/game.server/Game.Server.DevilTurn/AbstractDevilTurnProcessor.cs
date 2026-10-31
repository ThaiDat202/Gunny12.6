using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.DevilTurn;

public class AbstractDevilTurnProcessor : IDevilTurnProcessor
{
	public virtual void OnGameData(GamePlayer player, GSPacketIn packet)
	{
	}
}
