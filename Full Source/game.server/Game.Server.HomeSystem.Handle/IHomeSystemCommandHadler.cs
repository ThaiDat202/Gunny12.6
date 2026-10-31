using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HomeSystem.Handle;

public interface IHomeSystemCommandHadler
{
	bool CommandHandler(GamePlayer Player, GSPacketIn packet);
}
