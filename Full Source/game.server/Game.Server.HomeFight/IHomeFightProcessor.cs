using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HomeFight;

public interface IHomeFightProcessor
{
	void OnGameData(GamePlayer player, GSPacketIn packet);
}
