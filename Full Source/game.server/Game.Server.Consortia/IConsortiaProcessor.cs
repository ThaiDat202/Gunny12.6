using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Consortia;

public interface IConsortiaProcessor
{
	void OnGameData(GamePlayer player, GSPacketIn packet);
}
