using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Team.Handle;

[TeamHandle(2)]
public class RequestInfo : ITeamCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		packet.ReadInt();
		TeamMgr.Out.SendToSingle(player);
		return 0;
	}
}
