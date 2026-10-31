using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Zodiac.Handle;

public interface IZodiacCommandHadler
{
	bool CommandHandler(GamePlayer Player, GSPacketIn packet);
}
