using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.BombKing;

public interface IBombKingProcessor
{
	void OnGameData(GamePlayer player, GSPacketIn packet);
}
