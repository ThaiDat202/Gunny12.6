using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HorseAmulet;

public interface IHorseAmuletProcessor
{
	void OnGameData(GamePlayer player, GSPacketIn packet);
}
