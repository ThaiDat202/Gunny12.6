using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.EquipAmulet;

public abstract class AbstractEquipAmuletProcessor : IEquipAmuletProcessor
{
	public virtual void OnGameData(GamePlayer player, GSPacketIn packet)
	{
	}
}
