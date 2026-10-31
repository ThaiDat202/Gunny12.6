using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.MagicStone;

public interface IMagicStoneProcessor
{
	void OnGameData(GamePlayer player, GSPacketIn packet);
}
