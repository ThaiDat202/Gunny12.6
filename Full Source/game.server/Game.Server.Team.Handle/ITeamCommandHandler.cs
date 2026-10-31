using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Team.Handle;

public interface ITeamCommandHandler
{
	int CommandHandler(GamePlayer Player, GSPacketIn packet);
}
