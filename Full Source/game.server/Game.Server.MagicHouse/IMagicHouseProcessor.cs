using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.MagicHouse;

public interface IMagicHouseProcessor
{
	void OnGameData(GamePlayer player, GSPacketIn packet);
}
