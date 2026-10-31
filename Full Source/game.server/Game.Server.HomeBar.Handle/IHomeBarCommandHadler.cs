using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HomeBar.Handle;

public interface IHomeBarCommandHadler
{
	bool CommandHandler(GamePlayer Player, GSPacketIn packet);
}
