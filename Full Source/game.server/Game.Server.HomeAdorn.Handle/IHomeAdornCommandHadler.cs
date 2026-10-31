using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HomeAdorn.Handle;

public interface IHomeAdornCommandHadler
{
	bool CommandHandler(GamePlayer Player, GSPacketIn packet);
}
