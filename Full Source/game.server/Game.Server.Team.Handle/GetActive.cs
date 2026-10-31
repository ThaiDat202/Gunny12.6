using System.Linq;
using EntityDatabase.PlayerModels;
using EntityDatabase.ServerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Team.Handle;

[TeamHandle(16)]
public class GetActive : ITeamCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(390, player.PlayerId);
		gSPacketIn.WriteByte(16);
		gSPacketIn.WriteInt(TeamMgr.ActiveList.Count);
		foreach (BattleTeamActiveTemplateList item in TeamMgr.ActiveList)
		{
			Sys_Users_TeamSelfActive sys_Users_TeamSelfActive = player.PlayerCharacter.TeamSelfActive.Where((Sys_Users_TeamSelfActive p) => p.Type == item.Type).FirstOrDefault();
			gSPacketIn.WriteInt(item.Type);
			gSPacketIn.WriteInt(sys_Users_TeamSelfActive?.haveScore ?? 0);
		}
		player.SendTCP(gSPacketIn);
		return 0;
	}
}
