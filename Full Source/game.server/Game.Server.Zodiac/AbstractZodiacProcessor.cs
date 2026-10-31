using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Zodiac;

public abstract class AbstractZodiacProcessor : IZodiacProcessor
{
	public virtual void OnGameData(GamePlayer player, GSPacketIn packet)
	{
	}
}
