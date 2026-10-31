using System;
using System.Linq;
using Bussiness;
using EntityDatabase.PlayerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;

namespace Game.Server.Team.Handle;

[TeamHandle(5)]
public class InviteResult : ITeamCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		PlayerData playerData = new PlayerData();
		int userId = packet.ReadInt();
		int teamID = packet.ReadInt();
		player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("RoomBordenHandler.Translate8"));
		Sys_Users_TeamInviteList sys_Users_TeamInviteList = new Sys_Users_TeamInviteList();
		sys_Users_TeamInviteList.UserId = userId;
		sys_Users_TeamInviteList.teamID = teamID;
		sys_Users_TeamInviteList.name = player.PlayerCharacter.NickName;
		sys_Users_TeamInviteList.VipLv = player.PlayerCharacter.VIPLevel;
		sys_Users_TeamInviteList.date = DateTime.Now;
		playerData.Sys_Users_TeamInviteList.Add(sys_Users_TeamInviteList);
		playerData.SaveChanges();
		Sys_Users_Team_User sys_Users_Team_User = playerData.Sys_Users_Team_User.Where((Sys_Users_Team_User p) => p.teamID == teamID && p.IsCaption).FirstOrDefault();
		if (sys_Users_Team_User != null)
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(sys_Users_Team_User.UserID);
			if (playerById != null)
			{
				TeamMgr.Out.GetInviteList(playerData, playerById);
			}
		}
		return 0;
	}
}
