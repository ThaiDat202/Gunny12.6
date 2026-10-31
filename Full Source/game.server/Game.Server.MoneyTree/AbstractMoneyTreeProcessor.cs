using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.MoneyTree;

public abstract class AbstractMoneyTreeProcessor : IMoneyTreeProcessor
{
	public virtual void OnGameData(GamePlayer player, GSPacketIn packet)
	{
	}
}
