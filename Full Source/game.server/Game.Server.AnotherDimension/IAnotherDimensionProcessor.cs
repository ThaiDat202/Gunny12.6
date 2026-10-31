using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.AnotherDimension;

public interface IAnotherDimensionProcessor
{
	void OnGameData(GamePlayer player, GSPacketIn packet);
}
