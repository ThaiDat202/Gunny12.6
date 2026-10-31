using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.CardNewSystem;

public abstract class AbstractCardNewSystemProcessor : ICardNewSystemProcessor
{
	public virtual void OnGameData(GamePlayer player, GSPacketIn packet)
	{
	}
}
