using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HomeBar;

public interface IHomeBarProcessor
{
	void OnGameData(GamePlayer player, GSPacketIn packet);
}
