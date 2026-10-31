using EntityDatabase.PlayerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Team.Handle;

[TeamHandle(10)]
public class TeamActive : ITeamCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		PlayerData db = new PlayerData();
		TeamMgr.Out.GetActiveList(db, player);
		return 0;
	}
}
