using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Bussiness;
using EntityDatabase.PlayerModels;
using EntityDatabase.ServerModels;
using Game.Server.GameObjects;
using Game.Server.Managers;

namespace Game.Server.DevilTurn;

public class DevilTurnMgr
{
	public static List<DevilTreasItemList> ListDevilTreasItemList;

	public static List<DevilTreasPointsList> ListDevilTreasPointsList;

	public static List<DevilTreasRankRewardList> ListDevilTreasRankRewardlist;

	public static List<DevilTreasSarahToBoxList> ListDevilTreasSarahToBoxList;

	public static List<DevilTurnOpenBox> ListDevilTurnOpenBox;

	public static List<Sys_Users_DevilTurn_Rank> DevilTurn_Ranks = new List<Sys_Users_DevilTurn_Rank>();

	public static int JackBox = 0;

	public static DevilTurnPacketsOut Out = new DevilTurnPacketsOut();

	public static bool Init()
	{
		try
		{
			ServerData serverData = new ServerData();
			ListDevilTreasItemList = serverData.DevilTreasItemList.ToList();
			ListDevilTreasPointsList = serverData.DevilTreasPointsList.ToList();
			ListDevilTreasRankRewardlist = serverData.DevilTreasRankRewardList.ToList();
			ListDevilTreasSarahToBoxList = serverData.DevilTreasSarahToBoxList.ToList();
			ListDevilTurnOpenBox = serverData.DevilTurnOpenBox.ToList();
			GetDevilTurnData();
			return true;
		}
		catch
		{
			return false;
		}
	}

	public static int GetNeedMoneyToDice(int BoxId, int Index)
	{
		return ListDevilTurnOpenBox.Where((DevilTurnOpenBox p) => p.BoxId == BoxId && p.BoxIndex == Index).FirstOrDefault().NeedMoney;
	}

	public static void GetDevilTurnData()
	{
		List<Sys_Users_DevilTurn_Rank> list = DevilTurn_Ranks.OrderByDescending((Sys_Users_DevilTurn_Rank d) => d.Score).Take(10).ToList();
		for (int i = 0; i < list.Count; i++)
		{
			int UserID = list[i].UserID;
			int AreaID = list[i].ServerId;
			string serverName = list[i].ServerName;
			string nickName = list[i].NickName;
			int score = list[i].Score;
			Sys_Users_DevilTurn_Rank sys_Users_DevilTurn_Rank = DevilTurn_Ranks.Where((Sys_Users_DevilTurn_Rank dr) => dr.UserID == UserID && dr.ServerId == AreaID).FirstOrDefault();
			if (sys_Users_DevilTurn_Rank == null)
			{
				DevilTurn_Ranks.Add(new Sys_Users_DevilTurn_Rank
				{
					UserID = UserID,
					ServerId = AreaID,
					NickName = nickName,
					ServerName = serverName,
					Score = score
				});
			}
			else
			{
				sys_Users_DevilTurn_Rank.Score = score;
			}
		}
	}

	public static DevilTreasItemList GetItem()
	{
		Random random = new Random();
		int devilJack = ThreadSafeRandom.NextStatic(ListDevilTreasItemList.Select((DevilTreasItemList devilItem) => devilItem.Random).Max());
		List<DevilTreasItemList> list = ListDevilTreasItemList.Where((DevilTreasItemList devilItem) => devilItem.Random >= devilJack).ToList();
		Thread.Sleep(100);
		return list[random.Next(0, list.Count - 1)];
	}

	public static void CheckDiceResult(GamePlayer player, Sys_Users_ListBox box, int value)
	{
		Random random = new Random();
		int num = 0;
		int num2 = 0;
		box.currentIndex = value;
		switch (box.BoxId)
		{
		case 12711:
			num = 3;
			num2 = 3;
			break;
		case 12712:
			num = 4;
			num2 = 4;
			break;
		case 12713:
			num = 6;
			num2 = 5;
			break;
		}
		if (box.state == 2)
		{
			box.state = 1;
			box.subtractMin *= num;
			box.needmoney = GetNeedMoneyToDice(box.BoxId, box.boxIndex);
			box.min = random.Next(1, 10);
			box.min = random.Next(1, 8);
			while (box.max < box.min + 1 || box.max > box.min + 3)
			{
				box.max = random.Next(3, 12);
			}
		}
		else
		{
			if (value >= box.min && value <= box.max)
			{
				box.boxIndex++;
				box.state = 2;
				box.subtractMax *= num2 + box.boxIndex - 2;
			}
			else
			{
				box.state = 3;
			}
			Out.SendDiceResult(player, (byte)value, (byte)box.state);
		}
		Out.SendUpdateDiceBoxView(player, box);
	}

	public static void UpdateDevilTurnRank(int userID, int serverID)
	{
		int num = 0;
		num = DevilTurn_Ranks.OrderByDescending((Sys_Users_DevilTurn_Rank dv) => dv.Score).ToList().FindIndex((Sys_Users_DevilTurn_Rank dv) => dv.ServerId == serverID && dv.UserID == userID) + 1;
		GamePlayer playerById = WorldMgr.GetPlayerById(userID);
		if (playerById != null)
		{
			playerById.PlayerCharacter.DvRank = num;
		}
	}

	public static void AddDevilTurnValue(int userID, int score, string nickName, int value, int areaID, string areaName)
	{
		NewDevilTurnJackBow(value);
		Sys_Users_DevilTurn_Rank sys_Users_DevilTurn_Rank = DevilTurn_Ranks.Where((Sys_Users_DevilTurn_Rank d) => d.UserID == userID && d.ServerId == areaID).FirstOrDefault();
		if (sys_Users_DevilTurn_Rank == null)
		{
			sys_Users_DevilTurn_Rank = new Sys_Users_DevilTurn_Rank();
			sys_Users_DevilTurn_Rank.UserID = userID;
			sys_Users_DevilTurn_Rank.NickName = nickName;
			sys_Users_DevilTurn_Rank.Score = score;
			sys_Users_DevilTurn_Rank.ServerId = areaID;
			sys_Users_DevilTurn_Rank.ServerName = areaName;
			DevilTurn_Ranks.Add(sys_Users_DevilTurn_Rank);
		}
		else
		{
			sys_Users_DevilTurn_Rank.Score = score;
		}
	}

	public static void NewDevilTurnJackBow(int value)
	{
		JackBox += value;
		if (JackBox > GameProperties.DevilTreasurePrizePoolMax)
		{
			JackBox = GameProperties.DevilTreasurePrizePoolMax;
		}
		else if (JackBox < GameProperties.DevilTreasurePrizePool)
		{
			JackBox = GameProperties.DevilTreasurePrizePool;
		}
	}
}
