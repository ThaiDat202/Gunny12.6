using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Elf;

public interface IElfProcessor
{
	void OnGameData(GamePlayer player, GSPacketIn packet);
}
