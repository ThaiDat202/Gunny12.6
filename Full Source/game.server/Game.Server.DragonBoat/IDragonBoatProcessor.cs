using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.DragonBoat;

public interface IDragonBoatProcessor
{
	void OnGameData(GamePlayer player, GSPacketIn packet);
}
