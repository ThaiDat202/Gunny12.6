using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HomeAdorn;

public interface IHomeAdornProcessor
{
	void OnGameData(GamePlayer player, GSPacketIn packet);
}
