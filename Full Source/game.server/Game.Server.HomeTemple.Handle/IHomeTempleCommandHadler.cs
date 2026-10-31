using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HomeTemple.Handle;

public interface IHomeTempleCommandHadler
{
	int CommandHandler(GamePlayer Player, GSPacketIn packet);
}
