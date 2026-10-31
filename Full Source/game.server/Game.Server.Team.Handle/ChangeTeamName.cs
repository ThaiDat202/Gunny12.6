using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Team.Handle;

[TeamHandle(32)]
public class ChangeTeamName : ITeamCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		return 0;
	}
}
