using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.CardNewSystem.Handle;

public interface ICardNewSystemCommandHandler
{
	int CommandHandler(GamePlayer Player, GSPacketIn packet);
}
