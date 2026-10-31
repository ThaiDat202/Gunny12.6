using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.EquipAmulet;

public interface IEquipAmuletProcessor
{
	void OnGameData(GamePlayer player, GSPacketIn packet);
}
