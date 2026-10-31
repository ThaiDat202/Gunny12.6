using System;
using Bussiness;
using EntityDatabase.PlayerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.Team.Handle;

[TeamHandle(1)]
public class UpdateTeamInfo : ITeamCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		string text = packet.ReadString();
		string text2 = packet.ReadString();
		PlayerData playerData = new PlayerData();
		int num = TeamMgr.CheckCreateTeam(text, type: true, playerData);
		int num2 = TeamMgr.CheckCreateTeam(text2, type: false, playerData);
		if (num == 0 && num2 == 0)
		{
			int createTeamMoney = GameProperties.CreateTeamMoney;
			if (player.PlayerCharacter.Money < createTeamMoney)
			{
				player.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("TeamHandler.UpdateTeamInfo.Success"));
				return 0;
			}
			player.RemoveMoney(createTeamMoney);
			player.PlayerCharacter.Team = new Sys_Users_Team();
			player.PlayerCharacter.Team.teamName = text;
			player.PlayerCharacter.Team.teamTag = text2;
			player.PlayerCharacter.Team.teamGrade = 1;
			player.PlayerCharacter.Team.totalMember = 1;
			player.PlayerCharacter.Team.createDate = DateTime.Now;
			player.PlayerCharacter.Team.teamLoginDate = DateTime.Now;
			player.PlayerCharacter.Team.AreaName = GameServer.Instance.Configuration.ServerName;
			playerData.Sys_Users_Team.Add(player.PlayerCharacter.Team);
			playerData.SaveChanges();
			TeamMgr.AddNewPlayerToTeam(playerData, player.PlayerCharacter, player.PlayerCharacter.Team.teamID, IsCaption: true);
			playerData.SaveChanges();
		}
		else
		{
			player.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("TeamHandler.UpdateTeamInfo.Fail"));
		}
		return 0;
	}
}
