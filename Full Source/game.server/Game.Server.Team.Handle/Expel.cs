using System.Data.Entity;
using System.Linq;
using Bussiness;
using EntityDatabase.PlayerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;

namespace Game.Server.Team.Handle;

[TeamHandle(8)]
public class Expel : ITeamCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		PlayerData playerData = new PlayerData();
		int Id = packet.ReadInt();
		Sys_Users_Team_User sys_Users_Team_User = playerData.Sys_Users_Team_User.Where((Sys_Users_Team_User p) => p.UserID == Id).FirstOrDefault();
		if (sys_Users_Team_User != null)
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(sys_Users_Team_User.UserID);
			if (playerById != null)
			{
				playerById.PlayerCharacter.Team = new Sys_Users_Team();
				playerById.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("TeamHandler.Expel.Tip"));
				TeamMgr.ClearTeam(playerData, playerById);
				TeamMgr.Out.SendToSingle(playerById);
			}
			else
			{
				sys_Users_Team_User.teamID = 0;
				sys_Users_Team_User.IsCaption = false;
				playerData.Entry(sys_Users_Team_User).State = EntityState.Modified;
				playerData.SaveChanges();
			}
			player.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("TeamHandler.Expel.Success"));
			TeamMgr.Out.SendToAll(player.PlayerCharacter.Team.teamID);
		}
		else
		{
			player.SendMessage(LanguageMgr.GetTranslation("TeamHandler.Expel.Fail"));
		}
		return 0;
	}
}
