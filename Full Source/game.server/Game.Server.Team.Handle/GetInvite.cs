using EntityDatabase.PlayerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Team.Handle;

[TeamHandle(15)]
public class GetInvite : ITeamCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		PlayerData db = new PlayerData();
		TeamMgr.Out.GetInviteList(db, player);
		return 0;
	}
}
