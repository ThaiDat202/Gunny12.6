using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.ConsortiaTask;

public class AbstractConsortiaTaskProcessor : IConsortiaTaskProcessor
{
	public virtual void OnGameData(GamePlayer player, GSPacketIn packet)
	{
	}
}
