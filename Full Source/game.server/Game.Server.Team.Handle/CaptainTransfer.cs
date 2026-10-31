using System.Linq;
using Bussiness;
using EntityDatabase.PlayerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.Team.Handle;

[TeamHandle(34)]
public class CaptainTransfer : ITeamCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		PlayerData playerData = new PlayerData();
		int userID = packet.ReadInt();
		if (player.PlayerCharacter.TeamUser.IsCaption)
		{
			Sys_Users_Team_User sys_Users_Team_User = playerData.Sys_Users_Team_User.Where((Sys_Users_Team_User p) => p.UserID == userID).FirstOrDefault();
			if (sys_Users_Team_User == null && sys_Users_Team_User.teamID != player.PlayerCharacter.Team.teamID)
			{
				player.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("TeamHandler.CaptainTransfer.Fail"));
			}
			else
			{
				sys_Users_Team_User.IsCaption = true;
				player.PlayerCharacter.TeamUser.IsCaption = false;
				TeamMgr.SaveUserTeam(playerData, sys_Users_Team_User);
				TeamMgr.SaveUserTeam(playerData, player.PlayerCharacter.TeamUser);
				TeamMgr.Out.SendToAll(player.PlayerCharacter.Team.teamID);
				player.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("TeamHandler.CaptainTransfer.Success"));
			}
		}
		return 0;
	}
}
