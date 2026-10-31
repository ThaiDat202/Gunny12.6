using EntityDatabase.PlayerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Team.Handle;

[TeamHandle(7)]
public class Exit : ITeamCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		PlayerData db = new PlayerData();
		TeamMgr.ExitTeam(db, player, player.PlayerCharacter.TeamUser.IsCaption);
		return 0;
	}
}
