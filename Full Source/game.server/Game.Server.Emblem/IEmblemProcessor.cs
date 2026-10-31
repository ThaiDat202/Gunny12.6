using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Emblem;

public interface IEmblemProcessor
{
	void OnGameData(GamePlayer player, GSPacketIn packet);
}
