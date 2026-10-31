using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.MemoryGame.Handle;

public interface IMemoryGameCommandHadler
{
	int CommandHandler(GamePlayer Player, GSPacketIn packet);
}
