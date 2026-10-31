using Bussiness;
using EntityDatabase.PlayerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Team.Handle;

[TeamHandle(6)]
public class Approval : ITeamCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		PlayerData playerData = new PlayerData();
		if (!TeamMgr.CheckBeforAddNewUsers(player))
		{
			player.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("TeamHandler.Approval.CheckFail"));
			return 0;
		}
		int num = packet.ReadInt();
		Sys_Users_TeamInviteList sys_Users_TeamInviteList = playerData.Sys_Users_TeamInviteList.Find(num);
		if (sys_Users_TeamInviteList != null)
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(sys_Users_TeamInviteList.UserId);
			GSPacketIn gSPacketIn = new GSPacketIn(390, player.PlayerId);
			gSPacketIn.WriteByte(4);
			gSPacketIn.WriteInt(num);
			player.SendTCP(gSPacketIn);
			player.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("TeamHandler.Approval.Success"));
			PlayerInfo playerCharacter;
			if (playerById != null)
			{
				playerCharacter = playerById.PlayerCharacter;
				playerById.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("TeamHandler.Approval.Success"));
			}
			else
			{
				PlayerBussiness playerBussiness = new PlayerBussiness();
				playerCharacter = playerBussiness.GetUserSingleByUserID(sys_Users_TeamInviteList.UserId);
			}
			TeamMgr.AddNewPlayerToTeam(playerData, playerCharacter, sys_Users_TeamInviteList.teamID, IsCaption: false, player.PlayerCharacter.Team);
			playerData.Sys_Users_TeamInviteList.Remove(sys_Users_TeamInviteList);
			Sys_Users_Team team = player.PlayerCharacter.Team;
			team.totalMember++;
			TeamMgr.SaveTeam(playerData, team);
			playerData.SaveChanges();
		}
		TeamMgr.Out.SendToAll(sys_Users_TeamInviteList.teamID);
		return 0;
	}
}
