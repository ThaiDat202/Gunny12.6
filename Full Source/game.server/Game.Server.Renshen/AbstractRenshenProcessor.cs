using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Renshen;

public abstract class AbstractRenshenProcessor : IRenshenProcessor
{
	public virtual void OnGameData(GamePlayer player, GSPacketIn packet)
	{
	}
}
