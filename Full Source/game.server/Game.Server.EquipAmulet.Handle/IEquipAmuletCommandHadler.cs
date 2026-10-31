using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.EquipAmulet.Handle;

public interface IEquipAmuletCommandHadler
{
	int CommandHandler(GamePlayer Player, GSPacketIn packet);
}
