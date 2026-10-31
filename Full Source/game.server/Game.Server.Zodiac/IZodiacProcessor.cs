using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Zodiac;

public interface IZodiacProcessor
{
	void OnGameData(GamePlayer player, GSPacketIn packet);
}
