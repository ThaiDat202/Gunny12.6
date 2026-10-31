using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HorseAmulet.Handle;

public interface IHorseAmuletCommandHadler
{
	int CommandHandler(GamePlayer Player, GSPacketIn packet);
}
