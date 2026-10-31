using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using Game.Base;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.LotteryTicket;

public class LotteryTicketMgr
{
	private readonly ILog log;

	private readonly int int_0;

	private readonly TimeSpan timeSpan_0;

	private readonly TimeSpan timeSpan_1;

	private readonly int int_1;

	private Timer timer_0;

	public LotteryTicketInfo Data;

	private Random random_0;

	public LotteryTicketMgr()
	{
		log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		int_0 = GameProperties.LotteryPoolCountDefault;
		timeSpan_0 = DateTime.Parse(DateTime.Now.ToShortDateString() + " " + GameProperties.LotteryTicketCloseTicketTime).TimeOfDay;
		timeSpan_1 = DateTime.Parse(DateTime.Now.ToShortDateString() + " " + GameProperties.LotteryTicketShowPoolTime).TimeOfDay;
		int_1 = 16;
		Data = Marshal.LoadDataFile<LotteryTicketInfo>("lottery", isEncrypt: true);
		if (Data == null)
		{
			Data = new LotteryTicketInfo
			{
				Players = new Dictionary<int, UserLotteryTicketInfo>(),
				Status = LotteryTicketStatus.CLOSE,
				PoolCountCurrent = int_0
			};
		}
		random_0 = new Random();
	}

	public void AddPool(GamePlayer p, string pool)
	{
		lock (Data.Players)
		{
			if (!Data.Players.ContainsKey(p.PlayerId))
			{
				UserLotteryTicketInfo userLotteryTicketInfo = new UserLotteryTicketInfo
				{
					UserID = p.PlayerId,
					NickName = p.PlayerCharacter.NickName,
					Grade = p.PlayerCharacter.Grade,
					PoolList = new List<string> { pool }
				};
				Data.Players.Add(userLotteryTicketInfo.UserID, userLotteryTicketInfo);
			}
			else
			{
				Data.Players[p.PlayerId].PoolList.Add(pool);
			}
		}
	}

	public void BeginTimer()
	{
		int num = 60000;
		if (timer_0 != null)
		{
			timer_0.Change(num, num);
		}
		else
		{
			timer_0 = new Timer(method_7, null, num, num);
		}
	}

	public void CheckRunning()
	{
		DateTime now = DateTime.Now;
		if (DateTime.Parse(GameProperties.LotteryTicketEndTime) > now)
		{
			if (Data.Status == LotteryTicketStatus.CLOSE)
			{
				Data.Status = LotteryTicketStatus.OPEN;
				method_0();
			}
			BeginTimer();
		}
	}

	public UserLotteryTicketInfo FindPlayer(int userId)
	{
		lock (Data.Players)
		{
			if (!Data.Players.ContainsKey(userId))
			{
				return null;
			}
			return Data.Players[userId];
		}
	}

	private void method_0()
	{
		Data.Players = new Dictionary<int, UserLotteryTicketInfo>();
		Data.PoolWinner = "";
		Data.PlayerLastAward = "";
		Data.LastTimeSendAward = DateTime.MinValue;
		Data.CanBuyPool = true;
		Data.FirstCount = 0;
		Data.FirstMoney = 0;
		Data.SecondCount = 0;
		Data.SecondMoney = 0;
		Data.ThirdCount = 0;
		Data.ThirdMoney = 0;
		Data.FourCount = 0;
		Data.FourMoney = 0;
	}

	private void method_1()
	{
		DateTime now = DateTime.Now;
		if (now.TimeOfDay >= timeSpan_0 && Data.Status == LotteryTicketStatus.OPEN)
		{
			Data.Status = LotteryTicketStatus.WAITING_POOL;
			method_2();
			Console.WriteLine("LotteryTicket Waiting Winner Result");
		}
		else if (Data.Status != LotteryTicketStatus.WAITING_POOL || !(now.TimeOfDay >= timeSpan_1))
		{
			if (Data.Status == LotteryTicketStatus.SHOW_POOL && now.TimeOfDay < timeSpan_0)
			{
				Data.Status = LotteryTicketStatus.OPEN;
				method_3();
				method_2();
				Console.WriteLine("LotteryTicket Send Award & Reset");
			}
		}
		else
		{
			Data.Status = LotteryTicketStatus.SHOW_POOL;
			Data.PoolWinner = method_6();
			method_4();
			GSPacketIn packet = WorldMgr.SendSysNotice(LanguageMgr.GetTranslation("LotteryTicket.Event.PoolWiner.Notice"));
			GameServer.Instance.LoginServer.SendPacket(packet);
			Console.WriteLine("LotteryTicket Show Pool State");
		}
	}

	private void method_2()
	{
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		foreach (GamePlayer gamePlayer in allPlayers)
		{
			gamePlayer.Out.SendLotteryTicketInfo(gamePlayer.PlayerId, this);
		}
	}

	private void method_3()
	{
		if (!(Data.LastTimeSendAward < DateTime.Now.Date))
		{
			return;
		}
		Data.LastTimeSendAward = DateTime.Now.Date;
		int num = Data.PoolCountCurrent - (Data.FirstMoney + Data.SecondMoney + Data.ThirdMoney + Data.FourMoney);
		Data.PoolCountCurrent = int_0 + num;
		lock (Data.Players)
		{
			foreach (UserLotteryTicketInfo value in Data.Players.Values)
			{
				List<ActivitySystemItemInfo> list = new List<ActivitySystemItemInfo>();
				if (value.TotalPoolFirstWin > 0)
				{
					UserLotteryTicketInfo userLotteryTicketInfo = value;
					userLotteryTicketInfo.TotalMoneyEat += Data.FirstMoney * value.TotalPoolFirstWin;
					list.AddRange(ActiveMgr.FindLotteryTicketPackage(1));
				}
				if (value.TotalPoolSecondWin > 0)
				{
					UserLotteryTicketInfo userLotteryTicketInfo2 = value;
					userLotteryTicketInfo2.TotalMoneyEat += Data.SecondMoney * value.TotalPoolSecondWin;
					list.AddRange(ActiveMgr.FindLotteryTicketPackage(2));
				}
				if (value.TotalPoolThreeWin > 0)
				{
					UserLotteryTicketInfo userLotteryTicketInfo3 = value;
					userLotteryTicketInfo3.TotalMoneyEat += Data.ThirdMoney * value.TotalPoolThreeWin;
					list.AddRange(ActiveMgr.FindLotteryTicketPackage(3));
				}
				if (value.TotalPoolFourWin > 0)
				{
					UserLotteryTicketInfo userLotteryTicketInfo4 = value;
					userLotteryTicketInfo4.TotalMoneyEat += Data.FourMoney * value.TotalPoolFourWin;
				}
				List<ItemInfo> list2 = new List<ItemInfo>();
				foreach (ActivitySystemItemInfo item in list)
				{
					ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(item.TemplateID);
					if (itemTemplateInfo != null)
					{
						ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, item.Count, 102);
						itemInfo.Count = item.Count * value.TotalPoolThreeWin;
						itemInfo.IsBinds = item.IsBind;
						itemInfo.ValidDate = item.ValidDate;
						itemInfo.StrengthenLevel = item.StrengthLevel;
						itemInfo.AttackCompose = item.AttackCompose;
						itemInfo.DefendCompose = item.DefendCompose;
						itemInfo.AgilityCompose = item.AgilityCompose;
						itemInfo.LuckCompose = item.LuckCompose;
						list2.Add(itemInfo);
					}
				}
				if (value.TotalMoneyEat > 0)
				{
					WorldEventMgr.SendMoneyMailToUser(value.UserID, value.NickName, GameServer.Instance.Configuration.AreaId, LanguageMgr.GetTranslation("LotteryTicket.MailAward.Title"), LanguageMgr.GetTranslation("LotteryTicket.MailAward.Content", value.TotalMoneyEat, value.TotalPoolFirstWin, value.TotalPoolSecondWin, value.TotalPoolThreeWin, value.TotalPoolFourWin), value.TotalMoneyEat);
				}
				if (list2.Count > 0)
				{
					WorldEventMgr.SendItemsToMail(list2, value.UserID, value.NickName, GameServer.Instance.Configuration.AreaId, LanguageMgr.GetTranslation("LotteryTicket.MailAwardItems.Title"), LanguageMgr.GetTranslation("LotteryTicket.MailAwardItems.Content"));
				}
			}
		}
		method_0();
	}

	private void method_4()
	{
		lock (Data.Players)
		{
			List<string> list = new List<string>();
			foreach (UserLotteryTicketInfo value in Data.Players.Values)
			{
				foreach (string pool in value.PoolList)
				{
					switch (method_5(pool.ToLower()))
					{
					case 1:
						Data.FourCount++;
						value.TotalPoolFourWin++;
						break;
					case 2:
						Data.ThirdCount++;
						value.TotalPoolThreeWin++;
						break;
					case 3:
						Data.SecondCount++;
						value.TotalPoolSecondWin++;
						break;
					case 4:
						Data.FirstCount++;
						value.TotalPoolFirstWin++;
						break;
					}
				}
				if (value.TotalPoolFirstWin > 0)
				{
					list.Add($"{GameServer.Instance.Configuration.AreaName}-{value.NickName}-{value.Grade}");
				}
			}
			if (Data.FirstCount > 0)
			{
				Data.FirstMoney = Data.PoolCountCurrent / 100 * 20 / Data.FirstCount;
			}
			if (Data.SecondCount > 0)
			{
				Data.SecondMoney = Data.PoolCountCurrent / 100 * 20 / Data.SecondCount;
			}
			if (Data.ThirdCount > 0)
			{
				Data.ThirdMoney = Data.PoolCountCurrent / 100 * 5 / Data.ThirdCount;
			}
			if (Data.FourCount > 0)
			{
				Data.FourMoney = Data.PoolCountCurrent / 100 * 5 / Data.FourCount;
			}
			if (list.Count > 0)
			{
				Data.PlayerLastAward = string.Format("|", list);
			}
		}
	}

	private int method_5(string string_0)
	{
		int num = 0;
		for (int i = 0; i < 4; i++)
		{
			if (string_0[i] == Data.PoolWinner[i])
			{
				num++;
			}
		}
		return num;
	}

	private string method_6()
	{
		string text = "";
		List<int> list = new List<int>();
		for (int i = 0; i < int_1; i++)
		{
			list.Add(i);
		}
		for (int j = 0; j < 4; j++)
		{
			int index = random_0.Next(list.Count);
			text += list[index].ToString("X");
			list.RemoveAt(index);
		}
		return text.ToLower();
	}

	private void method_7(object object_0)
	{
		try
		{
			int tickCount = Environment.TickCount;
			ThreadPriority priority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			method_1();
			Thread.CurrentThread.Priority = priority;
			tickCount = Environment.TickCount - tickCount;
		}
		catch (Exception ex)
		{
			Console.WriteLine("CheckStatus: " + ex);
		}
	}

	public void Save()
	{
		Marshal.SaveDataFile(Data, "lottery", isEncrypt: true);
	}

	public void StopTimer()
	{
		if (timer_0 != null)
		{
			timer_0.Dispose();
			timer_0 = null;
		}
		Save();
	}
}
