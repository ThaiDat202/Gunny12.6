using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.MemoryGame;

public interface IMemoryGameProcessor
{
	void OnGameData(GamePlayer player, GSPacketIn packet);
}
