using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.DevilTurn;

public interface IDevilTurnProcessor
{
	void OnGameData(GamePlayer player, GSPacketIn packet);
}
