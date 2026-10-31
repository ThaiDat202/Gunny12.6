using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Emblem;

public class AbstractEmblemProcessor : IEmblemProcessor
{
	public virtual void OnGameData(GamePlayer player, GSPacketIn packet)
	{
	}
}
