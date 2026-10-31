using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HomeFight.Handle;

public interface IHomeFightCommandHadler
{
	bool CommandHandler(GamePlayer Player, GSPacketIn packet);
}
