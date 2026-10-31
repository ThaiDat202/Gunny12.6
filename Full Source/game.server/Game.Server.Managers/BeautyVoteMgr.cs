using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Managers;

public class BeautyVoteMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, UserBeautyVoteInfo> _beautyUsers;

	public static BeautyStatus Status;

	private static bool IsSendAward;

	protected static Timer _beautyTimer;

	private static DateTime LastTimeSave = DateTime.Now;

	public static bool Init()
	{
		try
		{
			_beautyUsers = new Dictionary<int, UserBeautyVoteInfo>();
			Status = BeautyStatus.CLOSED;
			IsSendAward = false;
			return LoadData(_beautyUsers);
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("BeautyVoteMgr", exception);
			}
			return false;
		}
	}

	private static bool LoadData(Dictionary<int, UserBeautyVoteInfo> data)
	{
		using (PlayerBussiness playerBussiness = new PlayerBussiness())
		{
			List<UserBeautyVoteInfo> allUserBeautyVote = playerBussiness.GetAllUserBeautyVote();
			foreach (UserBeautyVoteInfo item in allUserBeautyVote)
			{
				if (!_beautyUsers.ContainsKey(item.UserID))
				{
					_beautyUsers.Add(item.UserID, item);
					if (!IsSendAward && item.IsSendAward)
					{
						IsSendAward = true;
					}
				}
			}
		}
		SynData();
		BeginTimer();
		return true;
	}

	private static void SaveData()
	{
		lock (_beautyUsers)
		{
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			foreach (UserBeautyVoteInfo value in _beautyUsers.Values)
			{
				if (value.IsDirty)
				{
					playerBussiness.AddBeautyVote(value);
					value.IsDirty = false;
				}
			}
		}
	}

	private static void SaveSingleData(int userId)
	{
		lock (_beautyUsers)
		{
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			if (_beautyUsers.ContainsKey(userId))
			{
				playerBussiness.AddBeautyVote(_beautyUsers[userId]);
				_beautyUsers[userId].IsDirty = false;
			}
		}
	}

	public static bool CanJoin(int userId)
	{
		UserBeautyVoteInfo player = GetPlayer(userId);
		if (Status == BeautyStatus.OPENING && player == null)
		{
			return true;
		}
		return false;
	}

	public static bool VotePlayer(int userId, int count)
	{
		bool result = false;
		UserBeautyVoteInfo player = GetPlayer(userId);
		if (player != null)
		{
			player.Count += count;
			SaveSingleData(player.UserID);
			result = true;
		}
		return result;
	}

	private static void ClearData()
	{
		lock (_beautyUsers)
		{
			_beautyUsers.Clear();
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			playerBussiness.RemoveAllUserBeautyVote();
		}
	}

	private static void SendAward()
	{
		List<UserBeautyVoteInfo> allPlayers = GetAllPlayers();
		int num = 1;
		List<ItemInfo> list = null;
		string translation = LanguageMgr.GetTranslation("BeautyVote.AwardTop.Title.Msg");
		string text = "";
		foreach (UserBeautyVoteInfo item in allPlayers.OrderByDescending((UserBeautyVoteInfo a) => a.Count))
		{
			switch (num)
			{
			case 1:
				list = EventAwardMgr.GetEventAwardByType(eEventType.BEAUTY_TOP_1);
				text = LanguageMgr.GetTranslation("BeautyVote.AwardTop.Content.Msg", num, item.TimeCreate.Month, item.TimeCreate.GetWeekOfMonth());
				break;
			case 2:
				list = EventAwardMgr.GetEventAwardByType(eEventType.BEAUTY_TOP_2);
				text = LanguageMgr.GetTranslation("BeautyVote.AwardTop.Content.Msg", num, item.TimeCreate.Month, item.TimeCreate.GetWeekOfMonth());
				break;
			case 3:
				list = EventAwardMgr.GetEventAwardByType(eEventType.BEAUTY_TOP_3);
				text = LanguageMgr.GetTranslation("BeautyVote.AwardTop.Content.Msg", num, item.TimeCreate.Month, item.TimeCreate.GetWeekOfMonth());
				break;
			default:
				list = EventAwardMgr.GetEventAwardByType(eEventType.BEAUTY_AWARD);
				text = LanguageMgr.GetTranslation("BeautyVote.Award.Content.Msg", item.TimeCreate.Month, item.TimeCreate.GetWeekOfMonth());
				break;
			}
			if (list != null && list.Count > 0)
			{
				WorldEventMgr.SendItemsToMails(list, item.UserID, item.NickName, GameServer.Instance.Configuration.AreaId, translation, text);
				item.IsSendAward = true;
			}
			num++;
		}
		SaveData();
	}

	public static void AddPlayer(UserBeautyVoteInfo user)
	{
		lock (_beautyUsers)
		{
			if (!_beautyUsers.ContainsKey(user.UserID))
			{
				_beautyUsers.Add(user.UserID, user);
			}
		}
		SaveSingleData(user.UserID);
	}

	public static UserBeautyVoteInfo GetPlayer(int UserID)
	{
		lock (_beautyUsers)
		{
			if (_beautyUsers.ContainsKey(UserID))
			{
				return _beautyUsers[UserID];
			}
			return null;
		}
	}

	public static List<UserBeautyVoteInfo> GetAllPlayers()
	{
		lock (_beautyUsers)
		{
			return _beautyUsers.Values.ToList();
		}
	}

	private static void SynData()
	{
		DateTime date = DateTime.Now.Date;
		if (date.DayOfWeek == DayOfWeek.Monday && Status != BeautyStatus.WAITING)
		{
			Status = BeautyStatus.WAITING;
			if (!IsSendAward && _beautyUsers.Count > 0)
			{
				IsSendAward = true;
				SendAward();
				log.Info("--- BeautyVote is sent award ---");
			}
		}
		else if ((Status == BeautyStatus.WAITING || Status == BeautyStatus.CLOSED) && date.DayOfWeek == DayOfWeek.Tuesday)
		{
			log.Info("--- BeautyVote is opened ---");
			Status = BeautyStatus.OPENING;
			IsSendAward = false;
			if (_beautyUsers.Count > 0 && _beautyUsers.Values.First().TimeCreate.GetWeekOfMonth() != date.GetWeekOfMonth())
			{
				ClearData();
			}
		}
		else if (Status == BeautyStatus.CLOSED)
		{
			Status = BeautyStatus.OPENING;
		}
		if (date >= LastTimeSave.AddHours(1.0) && _beautyUsers.Count > 0)
		{
			SaveData();
			LastTimeSave = DateTime.Now;
			log.Info("--- BeautyVote is auto save db ---");
		}
	}

	private static void BeginTimer()
	{
		int num = 60000;
		if (_beautyTimer == null)
		{
			_beautyTimer = new Timer(CheckStatus, null, num, num);
		}
		else
		{
			_beautyTimer.Change(num, num);
		}
	}

	protected static void CheckStatus(object sender)
	{
		try
		{
			int tickCount = Environment.TickCount;
			ThreadPriority priority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			SynData();
			Thread.CurrentThread.Priority = priority;
			tickCount = Environment.TickCount - tickCount;
		}
		catch (Exception ex)
		{
			Console.WriteLine("CheckStatus: " + ex);
		}
	}

	public static void StopTimer()
	{
		if (_beautyTimer != null)
		{
			_beautyTimer.Dispose();
			_beautyTimer = null;
		}
		SaveData();
	}
}
