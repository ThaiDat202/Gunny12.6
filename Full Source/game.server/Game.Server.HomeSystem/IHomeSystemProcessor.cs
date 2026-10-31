using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HomeSystem;

public interface IHomeSystemProcessor
{
	void OnGameData(GamePlayer player, GSPacketIn packet);
}
