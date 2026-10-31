using System.Collections.Generic;
using System.Linq;
using EntityDatabase.PlayerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Team.Handle;

[TeamHandle(9)]
public class TeamRecord : ITeamCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		PlayerData playerData = new PlayerData();
		GSPacketIn gSPacketIn = new GSPacketIn(390, player.PlayerId);
		gSPacketIn.WriteByte(9);
		List<Sys_Users_TeamRecord> list = playerData.Sys_Users_TeamRecord.Where((Sys_Users_TeamRecord p) => p.teamID == player.PlayerCharacter.Team.teamID).ToList();
		gSPacketIn.WriteInt(player.PlayerCharacter.Team.teamID);
		gSPacketIn.WriteInt(list.Count);
		foreach (Sys_Users_TeamRecord item in list)
		{
			gSPacketIn.WriteBoolean(item.isWin);
			gSPacketIn.WriteInt(item.active);
			gSPacketIn.WriteDateTime(item.date);
			gSPacketIn.WriteString(item.teamName);
			gSPacketIn.WriteString(item.teamZone);
			gSPacketIn.WriteInt(item.teamKill);
			gSPacketIn.WriteInt(item.teamSurvival);
			gSPacketIn.WriteString(item.teamMemberInfo);
			gSPacketIn.WriteString(item.enemyName);
			gSPacketIn.WriteString(item.enemyZone);
			gSPacketIn.WriteInt(item.enemyKill);
			gSPacketIn.WriteInt(item.enemySurvival);
			gSPacketIn.WriteString(item.enemyMemberInfo);
		}
		player.SendTCP(gSPacketIn);
		return 0;
	}
}
