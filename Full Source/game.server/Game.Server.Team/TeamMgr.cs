using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using Bussiness;
using Bussiness.Managers;
using EntityDatabase.PlayerModels;
using EntityDatabase.ServerModels;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Team;

public class TeamMgr
{
	public static List<BattleTeamSegmentList> battleTeamSegmentList;

	public static List<BattleTeamLevelList> battleTeamLevelList;

	public static List<BattleTeamActiveTemplateList> ActiveList;

	public static List<BattleTeamShopItemList> Shops;

	public static TeamPacketsOut Out = new TeamPacketsOut();

	public static bool Init()
	{
		ServerData serverData = new ServerData();
		try
		{
			battleTeamSegmentList = serverData.BattleTeamSegmentList.OrderBy((BattleTeamSegmentList p) => p.SegmentID).ToList();
			battleTeamLevelList = serverData.BattleTeamLevelList.OrderBy((BattleTeamLevelList p) => p.NeedActive).ToList();
			ActiveList = serverData.BattleTeamActiveTemplateList.OrderBy((BattleTeamActiveTemplateList p) => p.Type).ToList();
			Shops = serverData.BattleTeamShopItemList.OrderBy((BattleTeamShopItemList p) => p.ID).ToList();
			return true;
		}
		catch
		{
			return false;
		}
	}

	public static int MaxLv()
	{
		return battleTeamLevelList.ToArray().Last().Level;
	}

	public static int MaxDivision()
	{
		return battleTeamSegmentList.ToArray().Last().SegmentID;
	}

	public static void AddNewPlayerToTeam(PlayerData db, PlayerInfo PlayerCharacter, int teamID, bool IsCaption, Sys_Users_Team Team = null)
	{
		if (!IsCaption)
		{
			PlayerCharacter.Team = Team;
		}
		Sys_Users_Team_User sys_Users_Team_User = db.Sys_Users_Team_User.Where((Sys_Users_Team_User p) => p.UserID == PlayerCharacter.ID).FirstOrDefault();
		if (sys_Users_Team_User != null)
		{
			sys_Users_Team_User.teamID = teamID;
			sys_Users_Team_User.IsCaption = IsCaption;
			sys_Users_Team_User.CreateDate = DateTime.Now;
			db.Entry(sys_Users_Team_User).State = EntityState.Modified;
			PlayerCharacter.TeamUser = sys_Users_Team_User;
		}
		else
		{
			PlayerCharacter.TeamUser = new Sys_Users_Team_User();
			PlayerCharacter.TeamUser.UserID = PlayerCharacter.ID;
			PlayerCharacter.TeamUser.teamID = teamID;
			PlayerCharacter.TeamUser.BattleScore = 800;
			PlayerCharacter.TeamUser.IsCaption = IsCaption;
			PlayerCharacter.TeamUser.CreateDate = DateTime.Now;
			db.Sys_Users_Team_User.Add(PlayerCharacter.TeamUser);
		}
		Out.SendToAll(PlayerCharacter.Team.teamID);
	}

	public static void ExitTeam(PlayerData db, GamePlayer Player, bool IsCaption)
	{
		Player.SendMessage(LanguageMgr.GetTranslation("TeamHandler.ExitTeam.Success"));
		Sys_Users_Team teaminDb = Player.PlayerCharacter.Team;
		ClearTeam(db, Player);
		if (teaminDb.totalMember > 1 && IsCaption)
		{
			Sys_Users_Team_User sys_Users_Team_User = (from p in db.Sys_Users_Team_User
				where p.teamID == teaminDb.teamID && p.UserID != Player.PlayerId && !p.IsCaption
				orderby p.CreateDate
				select p).FirstOrDefault();
			if (sys_Users_Team_User != null)
			{
				sys_Users_Team_User.IsCaption = true;
				GamePlayer playerById = WorldMgr.GetPlayerById(sys_Users_Team_User.UserID);
				if (playerById != null)
				{
					playerById.PlayerCharacter.TeamUser = sys_Users_Team_User;
					playerById.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("TeamHandler.ExitTeam.TeamOwner.ExitTeam"));
				}
				db.Entry(sys_Users_Team_User).State = EntityState.Modified;
				db.SaveChanges();
			}
		}
		teaminDb.totalMember--;
		SaveTeam(db, teaminDb);
		Out.SendToSingle(Player);
		Out.SendToAll(teaminDb.teamID);
		GamePlayer[] allPlayersByTeamId = WorldMgr.GetAllPlayersByTeamId(teaminDb.teamID);
		GamePlayer[] array = allPlayersByTeamId;
		foreach (GamePlayer gamePlayer in array)
		{
			gamePlayer.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("TeamHandler.ExitTeam.Tip", Player.PlayerCharacter.NickName));
		}
	}

	public static void SaveTeam(PlayerData db, Sys_Users_Team team)
	{
		db.Entry(team).State = EntityState.Modified;
		db.SaveChanges();
	}

	public static void SaveUserTeam(PlayerData db, Sys_Users_Team_User userteam)
	{
		db.Entry(userteam).State = EntityState.Modified;
		db.SaveChanges();
	}

	public static void ClearTeam(PlayerData db, GamePlayer Player)
	{
		Player.PlayerCharacter.Team = new Sys_Users_Team();
		Sys_Users_Team_User sys_Users_Team_User = db.Sys_Users_Team_User.Find(Player.PlayerCharacter.TeamUser.ID);
		sys_Users_Team_User.teamID = 0;
		sys_Users_Team_User.IsCaption = false;
		Player.PlayerCharacter.TeamUser = sys_Users_Team_User;
		db.Entry(sys_Users_Team_User).State = EntityState.Modified;
		db.SaveChanges();
	}

	public static int GetTotalMemberAvable(int TeamId)
	{
		PlayerData playerData = new PlayerData();
		return (from p in playerData.Sys_Users_Team_User
			where p.teamID == TeamId && SqlFunctions.DateDiff("hour", p.CreateDate, DateTime.Now) >= (int?)24
			orderby p.BattleScore descending
			select p).Count();
	}

	public static int GetTotalMembers(int TeamId)
	{
		PlayerData playerData = new PlayerData();
		return playerData.Sys_Users_Team_User.Where((Sys_Users_Team_User p) => p.teamID == TeamId).Count();
	}

	public static int GetCurrentDivision(int Score)
	{
		return battleTeamSegmentList.Where((BattleTeamSegmentList p) => p.NeedScore >= Score).FirstOrDefault().SegmentID;
	}

	public static int GetTeamScore(int TeamId)
	{
		PlayerData playerData = new PlayerData();
		int num = 0;
		List<Sys_Users_Team_User> list = (from p in playerData.Sys_Users_Team_User
			where p.teamID == TeamId && SqlFunctions.DateDiff("hour", p.CreateDate, DateTime.Now) >= (int?)24
			orderby p.BattleScore descending
			select p).ToList();
		if (list.Count > 0)
		{
			Sys_Users_Team_User sys_Users_Team_User = list.FirstOrDefault();
			if (sys_Users_Team_User.BattleScore > 0)
			{
				int num2 = 0;
				foreach (Sys_Users_Team_User item in list)
				{
					int num3 = item.BattleScore / sys_Users_Team_User.BattleScore;
					num2 += Convert.ToInt32(Math.Pow(16.0, num3));
				}
				foreach (Sys_Users_Team_User item2 in list)
				{
					int num4 = item2.BattleScore / sys_Users_Team_User.BattleScore;
					num += item2.BattleScore * Convert.ToInt32(Math.Pow(16.0, num4)) / num2;
				}
				double num5 = 0.0;
				switch (list.Count)
				{
				case 2:
					num5 = 1.0;
					break;
				case 3:
					num5 = 1.05;
					break;
				case 4:
					num5 = 1.06;
					break;
				case 5:
					num5 = 1.07;
					break;
				case 6:
					num5 = 1.08;
					break;
				case 7:
					num5 = 1.09;
					break;
				case 8:
					num5 = 1.1;
					break;
				case 9:
					num5 = 1.11;
					break;
				}
				num = Convert.ToInt16((double)num * num5);
			}
		}
		return num;
	}

	public static BattleTeamLevelList GetNextLvExp(int nextlv)
	{
		return battleTeamLevelList.Where((BattleTeamLevelList p) => p.Level == nextlv).FirstOrDefault();
	}

	public static void ReduceScore(GamePlayer Player, int Score)
	{
		Player.PlayerCharacter.TeamDayActive.selfActive -= Score;
		Out.SendUpdateDayActive(Player);
		Out.SendToAll(Player.PlayerCharacter.Team.teamID);
	}

	public static void AddScore(PlayerData db, GamePlayer Player, int Score, int Type, string Mes, ref int AddMoney)
	{
		if (Player.PlayerCharacter.Team.teamGrade == MaxLv())
		{
			Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("TeamHandler.AddScore.MaxLv"));
			return;
		}
		Sys_Users_Team_User sys_Users_Team_User = db.Sys_Users_Team_User.Find(Player.PlayerCharacter.TeamUser.ID);
		Sys_Users_Team sys_Users_Team = db.Sys_Users_Team.Find(Player.PlayerCharacter.Team.teamID);
		if (sys_Users_Team_User == null || sys_Users_Team == null)
		{
			return;
		}
		sys_Users_Team_User.ActiveTotalScore += Score;
		sys_Users_Team_User.ActiveWeekScore += Score;
		sys_Users_Team_User.ActiveSeasonScore += Score;
		Sys_Users_TeamActive sys_Users_TeamActive = new Sys_Users_TeamActive();
		sys_Users_TeamActive.ActiveType = Type;
		sys_Users_TeamActive.NickName = Player.PlayerCharacter.NickName;
		sys_Users_TeamActive.teamID = sys_Users_Team.teamID;
		sys_Users_TeamActive.UserActive = Score;
		sys_Users_Team.active += Score;
		sys_Users_Team.totalActive += Score;
		int ResidualActive = 0;
		int active = sys_Users_Team.active;
		if (CanUpLv(sys_Users_Team.teamGrade, sys_Users_Team.active, ref ResidualActive))
		{
			sys_Users_Team.teamGrade++;
			sys_Users_Team.active = 0;
			if (ResidualActive > 0)
			{
				sys_Users_Team.totalActive -= ResidualActive;
				sys_Users_Team_User.ActiveTotalScore -= ResidualActive;
				sys_Users_Team_User.ActiveWeekScore -= ResidualActive;
				sys_Users_Team_User.ActiveSeasonScore -= ResidualActive;
				sys_Users_TeamActive.UserActive -= ResidualActive;
				AddMoney = GameProperties.MoneyRichesOffer * ResidualActive;
			}
		}
		AddSelfActive(Player, Score - ResidualActive, Type);
		db.Sys_Users_TeamActive.Add(sys_Users_TeamActive);
		db.Entry(sys_Users_Team).State = EntityState.Modified;
		db.Entry(sys_Users_Team_User).State = EntityState.Modified;
		Player.PlayerCharacter.Team = sys_Users_Team;
		Player.PlayerCharacter.TeamUser = sys_Users_Team_User;
		Player.Out.SendMessage(eMessageType.Normal, Mes);
		db.SaveChanges();
		Out.SendToAll(sys_Users_Team.teamID);
	}

	public static void AddBattleScore(PlayerData db, GamePlayer Player, int Score)
	{
		Sys_Users_Team_User sys_Users_Team_User = db.Sys_Users_Team_User.Find(Player.PlayerCharacter.TeamUser.ID);
		sys_Users_Team_User.BattleScore += Score;
		if (sys_Users_Team_User.BattleScore < 800)
		{
			sys_Users_Team_User.BattleScore = 800;
		}
		db.Entry(sys_Users_Team_User).State = EntityState.Modified;
		Player.PlayerCharacter.TeamUser = sys_Users_Team_User;
		Player.PlayerCharacter.TeamDayActive.selfScore += Score;
		db.SaveChanges();
		Out.SendToAll(sys_Users_Team_User.teamID);
		Out.SendUpdateDayActive(Player);
	}

	public static void AddSelfActive(GamePlayer Player, int Score, int Type)
	{
		if (CanAddScore(Player, Type))
		{
			Sys_Users_TeamSelfActive sys_Users_TeamSelfActive = Player.PlayerCharacter.TeamSelfActive.Where((Sys_Users_TeamSelfActive p) => p.Type == Type).FirstOrDefault();
			if (sys_Users_TeamSelfActive != null)
			{
				Sys_Users_TeamSelfActive sys_Users_TeamSelfActive2 = sys_Users_TeamSelfActive;
				sys_Users_TeamSelfActive2.Times++;
				sys_Users_TeamSelfActive2.haveScore += Score;
				sys_Users_TeamSelfActive = sys_Users_TeamSelfActive2;
			}
			else
			{
				Sys_Users_TeamSelfActive sys_Users_TeamSelfActive2 = new Sys_Users_TeamSelfActive();
				sys_Users_TeamSelfActive2.UserID = Player.PlayerId;
				sys_Users_TeamSelfActive2.Type = Type;
				sys_Users_TeamSelfActive2.Times = 1;
				sys_Users_TeamSelfActive2.haveScore = Score;
				sys_Users_TeamSelfActive2.LastDate = DateTime.Now;
				Player.PlayerCharacter.TeamSelfActive.Add(sys_Users_TeamSelfActive2);
			}
			Player.PlayerCharacter.TeamDayActive.selfActive += Score;
			Player.PlayerCharacter.TeamDayActive.selfAllActive += Score;
			Out.SendUpdateDayActive(Player);
		}
	}

	public static bool CanAddScore(GamePlayer Player, int Type)
	{
		BattleTeamActiveTemplateList battleTeamActiveTemplateList = ActiveList.Where((BattleTeamActiveTemplateList p) => p.Type == Type).FirstOrDefault();
		Sys_Users_TeamSelfActive sys_Users_TeamSelfActive = Player.PlayerCharacter.TeamSelfActive.Where((Sys_Users_TeamSelfActive p) => p.Type == Type).FirstOrDefault();
		if (sys_Users_TeamSelfActive != null && battleTeamActiveTemplateList.MaxLimit <= sys_Users_TeamSelfActive.haveScore)
		{
			return false;
		}
		return true;
	}

	public static bool CanUpLv(int currentLv, int currentExp, ref int ResidualActive)
	{
		int nextLevel = currentLv + 1;
		BattleTeamLevelList battleTeamLevelList = TeamMgr.battleTeamLevelList.Where((BattleTeamLevelList p) => p.Level == nextLevel).FirstOrDefault();
		if (currentExp < battleTeamLevelList.NeedActive)
		{
			return false;
		}
		ResidualActive = currentExp - battleTeamLevelList.NeedActive;
		return true;
	}

	public static bool CanUpDivision(int currentDivision, int currentScore)
	{
		int nextDivision = currentDivision + 1;
		BattleTeamSegmentList battleTeamSegmentList = TeamMgr.battleTeamSegmentList.Where((BattleTeamSegmentList p) => p.SegmentID == nextDivision).FirstOrDefault();
		if (currentDivision < battleTeamSegmentList.NeedScore)
		{
			return false;
		}
		return true;
	}

	public static int CheckCreateTeam(string name, bool type, PlayerData db)
	{
		int result = 0;
		int num = 0;
		if (name.Length > 12 || name.Length < 2)
		{
			result = 3;
		}
		num = ((!type) ? db.Sys_Users_Team.Where((Sys_Users_Team p) => p.teamTag == name).Count() : db.Sys_Users_Team.Where((Sys_Users_Team p) => p.teamName == name).Count());
		if (num > 0)
		{
			result = 2;
		}
		return result;
	}

	public static bool CheckBeforAddNewUsers(GamePlayer Player)
	{
		return (GetTotalMembers(Player.PlayerCharacter.Team.teamID) != battleTeamLevelList.Where((BattleTeamLevelList p) => p.Level == Player.PlayerCharacter.Team.teamGrade).FirstOrDefault().MaxPlayerNum) ? true : false;
	}

	public static Sys_Users_Team UpdateTeam(int teamId)
	{
		PlayerData playerData = new PlayerData();
		if (teamId == 0)
		{
			return new Sys_Users_Team();
		}
		Sys_Users_Team sys_Users_Team = playerData.Sys_Users_Team.Find(teamId);
		int teamScore = GetTeamScore(teamId);
		int currentDivision = GetCurrentDivision(teamScore);
		int totalMembers = GetTotalMembers(teamId);
		sys_Users_Team.teamDivision = currentDivision;
		sys_Users_Team.teamScore = teamScore;
		sys_Users_Team.totalMember = GetTotalMembers(teamId);
		if (sys_Users_Team.AreaName == "")
		{
			sys_Users_Team.AreaName = GameServer.Instance.Configuration.ServerName;
		}
		playerData.Entry(sys_Users_Team).State = EntityState.Modified;
		playerData.SaveChanges();
		return sys_Users_Team;
	}

	public static void SendTeamWeekReward(ref string Mes)
	{
		PlayerData playerData = new PlayerData();
		ServerData serverData = new ServerData();
		List<Sys_Users_Team> list = playerData.Sys_Users_Team.OrderByDescending((Sys_Users_Team p) => p.teamScore).Take(10).ToList();
		List<BattleTeamSegmentList> list2 = serverData.BattleTeamSegmentList.OrderByDescending((BattleTeamSegmentList p) => p.SegmentID).ToList();
		int num = 0;
		try
		{
			foreach (Sys_Users_Team item in list)
			{
				List<Sys_Users_Team_User> list3 = playerData.Sys_Users_Team_User.Where((Sys_Users_Team_User p) => p.teamID == item.teamID).ToList();
				foreach (Sys_Users_Team_User item2 in list3)
				{
					using PlayerBussiness playerBussiness = new PlayerBussiness();
					PlayerInfo userSingleByUserID = playerBussiness.GetUserSingleByUserID(item2.UserID);
					int giftBagId = list2[num].GiftBagId;
					ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(giftBagId), 1, 105);
					itemInfo.IsBinds = true;
					itemInfo.UserID = 0;
					playerBussiness.AddGoods(itemInfo);
					MailInfo mailInfo = new MailInfo();
					mailInfo.Annex1 = itemInfo.ItemID.ToString();
					mailInfo.Title = "本周战队排名奖励";
					mailInfo.Gold = 0;
					mailInfo.Money = 0;
					mailInfo.Receiver = userSingleByUserID.NickName;
					mailInfo.ReceiverID = userSingleByUserID.ID;
					mailInfo.Sender = mailInfo.Receiver;
					mailInfo.SenderID = mailInfo.ReceiverID;
					mailInfo.Content = "本周战队排名:" + (num + 1) + " 名";
					mailInfo.Type = 12;
					playerBussiness.SendMail(mailInfo);
					GamePlayer playerById = WorldMgr.GetPlayerById(userSingleByUserID.ID);
					playerById?.Out.SendMailResponse(playerById.PlayerCharacter.ID, eMailRespose.Receiver);
				}
				num++;
			}
			Mes = "发送战队周奖励成功";
		}
		catch (Exception ex)
		{
			Mes = "发送战队周奖励失败: " + ex.ToString();
		}
	}

	public static void SendTeamSeasonReward(ref string Mes)
	{
		try
		{
			PlayerData playerData = new PlayerData();
			ServerData serverData = new ServerData();
			List<Sys_Users_Team> list = playerData.Sys_Users_Team.OrderByDescending((Sys_Users_Team p) => p.teamScore).Take(3).ToList();
			string[] array = serverData.BattleTeamSeasonList.FirstOrDefault().RankGift.Split(',');
			int num = 0;
			foreach (Sys_Users_Team item2 in list)
			{
				List<Sys_Users_Team_User> list2 = playerData.Sys_Users_Team_User.Where((Sys_Users_Team_User p) => p.teamID == item2.teamID).ToList();
				foreach (Sys_Users_Team_User item3 in list2)
				{
					using PlayerBussiness playerBussiness = new PlayerBussiness();
					PlayerInfo userSingleByUserID = playerBussiness.GetUserSingleByUserID(item3.UserID);
					int templateId = Convert.ToInt32(array[num]);
					ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(templateId), 1, 105);
					itemInfo.IsBinds = true;
					itemInfo.UserID = 0;
					playerBussiness.AddGoods(itemInfo);
					MailInfo mailInfo = new MailInfo();
					mailInfo.Annex1 = itemInfo.ItemID.ToString();
					mailInfo.Title = "战队本赛季奖励";
					mailInfo.Gold = 0;
					mailInfo.Money = 0;
					mailInfo.Receiver = userSingleByUserID.NickName;
					mailInfo.ReceiverID = userSingleByUserID.ID;
					mailInfo.Sender = mailInfo.Receiver;
					mailInfo.SenderID = mailInfo.ReceiverID;
					mailInfo.Content = "本赛季战队排名:" + num + "名";
					mailInfo.Type = 12;
					playerBussiness.SendMail(mailInfo);
					GamePlayer playerById = WorldMgr.GetPlayerById(userSingleByUserID.ID);
					playerById?.Out.SendMailResponse(playerById.PlayerCharacter.ID, eMailRespose.Receiver);
				}
				num++;
			}
			List<Sys_Users_Team> list3 = playerData.Sys_Users_Team.ToList();
			foreach (Sys_Users_Team item in list3)
			{
				List<Sys_Users_Team_User> list4 = playerData.Sys_Users_Team_User.Where((Sys_Users_Team_User p) => p.teamID == item.teamID && p.teamID != 0).ToList();
				foreach (Sys_Users_Team_User item4 in list4)
				{
					item4.ActiveSeasonScore = 0;
					item4.BattleScore = 800;
					item4.ActiveWeekScore = 0;
					item4.ActiveTotalScore = 0;
					GamePlayer playerById2 = WorldMgr.GetPlayerById(item4.UserID);
					if (playerById2 != null)
					{
						playerById2.PlayerCharacter.TeamUser = item4;
						playerData.Entry(item4).State = EntityState.Modified;
					}
				}
				item.teamGrade = 1;
				item.teamWinTime = 0;
				item.teamTotalTime = 0;
				item.teamDivision = 0;
				item.teamScore = 0;
				item.teamDuty = 0;
				item.teamPersonalScore = 0;
				item.freeInvitedUsedCnt = 0;
				item.season = 0;
				item.active = 0;
				playerData.Entry(item).State = EntityState.Modified;
				playerData.SaveChanges();
				Out.SendToAll(item.teamID);
			}
			Mes = "发送战队赛季奖励成功";
		}
		catch (Exception ex)
		{
			Mes = "发送战队赛季奖励失败:" + ex;
		}
	}
}
