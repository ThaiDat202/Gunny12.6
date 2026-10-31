using System.Collections.Generic;
using System.Linq;
using EntityDatabase.PlayerModels;
using EntityDatabase.ServerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Team;

public class TeamPacketsOut
{
	public void SendUseRoomBorden(GamePlayer player, ItemInfo item)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(390);
		gSPacketIn.WriteByte(21);
		gSPacketIn.WriteInt(item.ItemID);
		gSPacketIn.WriteBoolean(item.IsUsed);
		gSPacketIn.WriteDateTime(item.BeginDate);
		player.SendTCP(gSPacketIn);
	}

	public GSPacketIn SendRoomBordenChange(GamePlayer player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(390, player.PlayerId);
		gSPacketIn.WriteByte(25);
		gSPacketIn.WriteInt(player.CurrentRoomIndex);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(player.Extra.Info.curcentRoomBordenTemplateId);
		player.SendTCP(gSPacketIn);
		return gSPacketIn;
	}

	public void SendToAll(int teamId)
	{
		GamePlayer[] allPlayersByTeamId = WorldMgr.GetAllPlayersByTeamId(teamId);
		Sys_Users_Team team = TeamMgr.UpdateTeam(teamId);
		GamePlayer[] array = allPlayersByTeamId;
		foreach (GamePlayer gamePlayer in array)
		{
			BattleTeamLevelList nextLvExp = TeamMgr.GetNextLvExp(gamePlayer.PlayerCharacter.Team.teamGrade + 1);
			BattleTeamLevelList nextLvExp2 = TeamMgr.GetNextLvExp(gamePlayer.PlayerCharacter.Team.teamGrade);
			gamePlayer.PlayerCharacter.Team = team;
			gamePlayer.UpdateProperties();
			GSPacketIn gSPacketIn = new GSPacketIn(390, gamePlayer.PlayerId);
			gSPacketIn.WriteByte(1);
			gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Team.teamDivision);
			gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Team.teamID);
			gSPacketIn.WriteString(gamePlayer.PlayerCharacter.Team.teamName);
			gSPacketIn.WriteString(gamePlayer.PlayerCharacter.Team.teamTag);
			gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Team.teamGrade);
			gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Team.totalActive);
			gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Team.active);
			gSPacketIn.WriteInt(nextLvExp?.NeedActive ?? 0);
			gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Team.teamWinTime);
			gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Team.teamTotalTime);
			gSPacketIn.WriteDateTime(gamePlayer.PlayerCharacter.Team.createDate);
			gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Team.teamScore);
			gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Team.totalMember);
			gSPacketIn.WriteInt(nextLvExp2?.MaxPlayerNum ?? 0);
			gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Team.season);
			gSPacketIn.WriteString("");
			gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.TeamUser.IsCaption ? 1 : 0);
			gSPacketIn.WriteDateTime(gamePlayer.PlayerCharacter.Team.teamLoginDate);
			gSPacketIn.WriteInt(0);
			gamePlayer.Out.SendTCP(gSPacketIn);
			UpdateTeamPropertise(gamePlayer);
		}
	}

	public void UpdateTeamPropertise(GamePlayer player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(390, player.PlayerId);
		gSPacketIn.WriteByte(19);
		player.SendTCP(gSPacketIn);
	}

	public void SendToSingle(GamePlayer player)
	{
		player.PlayerCharacter.Team = TeamMgr.UpdateTeam(player.PlayerCharacter.Team.teamID);
		player.UpdateProperties();
		BattleTeamLevelList nextLvExp = TeamMgr.GetNextLvExp(player.PlayerCharacter.Team.teamGrade + 1);
		BattleTeamLevelList nextLvExp2 = TeamMgr.GetNextLvExp(player.PlayerCharacter.Team.teamGrade);
		GSPacketIn gSPacketIn = new GSPacketIn(390, player.PlayerId);
		gSPacketIn.WriteByte(1);
		gSPacketIn.WriteInt(player.PlayerCharacter.Team.teamDivision);
		gSPacketIn.WriteInt(player.PlayerCharacter.Team.teamID);
		gSPacketIn.WriteString(player.PlayerCharacter.Team.teamName);
		gSPacketIn.WriteString(player.PlayerCharacter.Team.teamTag);
		gSPacketIn.WriteInt(player.PlayerCharacter.Team.teamGrade);
		gSPacketIn.WriteInt(player.PlayerCharacter.Team.totalActive);
		gSPacketIn.WriteInt(player.PlayerCharacter.Team.active);
		gSPacketIn.WriteInt(nextLvExp?.NeedActive ?? 0);
		gSPacketIn.WriteInt(player.PlayerCharacter.Team.teamWinTime);
		gSPacketIn.WriteInt(player.PlayerCharacter.Team.teamTotalTime);
		gSPacketIn.WriteDateTime(player.PlayerCharacter.Team.createDate);
		gSPacketIn.WriteInt(player.PlayerCharacter.Team.teamScore);
		gSPacketIn.WriteInt(player.PlayerCharacter.Team.totalMember);
		gSPacketIn.WriteInt(nextLvExp2?.MaxPlayerNum ?? 0);
		gSPacketIn.WriteInt(player.PlayerCharacter.Team.season);
		gSPacketIn.WriteString("");
		gSPacketIn.WriteInt(player.PlayerCharacter.TeamUser.IsCaption ? 1 : 0);
		gSPacketIn.WriteDateTime(player.PlayerCharacter.Team.teamLoginDate);
		gSPacketIn.WriteInt(0);
		player.Out.SendTCP(gSPacketIn);
		UpdateTeamPropertise(player);
	}

	public void GetInviteList(PlayerData db, GamePlayer Player)
	{
		List<Sys_Users_TeamInviteList> list = db.Sys_Users_TeamInviteList.Where((Sys_Users_TeamInviteList p) => p.teamID == Player.PlayerCharacter.Team.teamID).ToList();
		GSPacketIn gSPacketIn = new GSPacketIn(390, Player.PlayerId);
		gSPacketIn.WriteByte(15);
		gSPacketIn.WriteInt(list.Count);
		foreach (Sys_Users_TeamInviteList item in list)
		{
			gSPacketIn.WriteInt(item.ID);
			gSPacketIn.WriteString(item.name);
			gSPacketIn.WriteDateTime(item.date);
			gSPacketIn.WriteInt(item.VipLv);
		}
		Player.SendTCP(gSPacketIn);
	}

	public void GetActiveList(PlayerData db, GamePlayer Player)
	{
		List<Sys_Users_TeamActive> list = db.Sys_Users_TeamActive.Where((Sys_Users_TeamActive p) => p.teamID == Player.PlayerCharacter.Team.teamID).ToList();
		GSPacketIn gSPacketIn = new GSPacketIn(390, Player.PlayerId);
		gSPacketIn.WriteByte(10);
		gSPacketIn.WriteInt(list.Count);
		foreach (Sys_Users_TeamActive item in list)
		{
			gSPacketIn.WriteString(item.NickName);
			gSPacketIn.WriteInt(item.UserActive);
			gSPacketIn.WriteInt(item.ActiveType);
		}
		Player.SendTCP(gSPacketIn);
	}

	public void SendUpdateDayActive(GamePlayer player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(390, player.PlayerId);
		gSPacketIn.WriteByte(12);
		gSPacketIn.WriteInt(player.PlayerCharacter.TeamDayActive.selfScore);
		gSPacketIn.WriteInt(player.PlayerCharacter.TeamDayActive.selfActive);
		gSPacketIn.WriteInt(player.PlayerCharacter.TeamDayActive.selfAllActive);
		player.Out.SendTCP(gSPacketIn);
	}

	public void SendOpenClose(GamePlayer player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(390, player.PlayerId);
		gSPacketIn.WriteByte(14);
		gSPacketIn.WriteBoolean(val: true);
		player.Out.SendTCP(gSPacketIn);
	}
}
