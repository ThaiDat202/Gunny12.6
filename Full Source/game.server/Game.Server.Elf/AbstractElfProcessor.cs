using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Elf;

public class AbstractElfProcessor : IElfProcessor
{
	public virtual void OnGameData(GamePlayer player, GSPacketIn packet)
	{
	}
}
