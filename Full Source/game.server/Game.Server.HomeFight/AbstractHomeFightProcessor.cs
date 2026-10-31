using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HomeFight;

public abstract class AbstractHomeFightProcessor : IHomeFightProcessor
{
	public virtual void OnGameData(GamePlayer player, GSPacketIn packet)
	{
	}
}
