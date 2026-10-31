using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Consortia;

public abstract class AbstractConsortiaProcessor : IConsortiaProcessor
{
	public virtual void OnGameData(GamePlayer player, GSPacketIn packet)
	{
	}
}
