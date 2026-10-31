using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using log4net;
using SqlDataProvider.Data;

namespace Center.Server.Managers
{
	public class BombKingMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private static Timer m_BombKing;

		private static int _battleStage;

		private static int _turn;

		private static int _status;

		private static bool IsOpen = false;

		private static bool _IsBombKingOpen;

		private static int _Week;

		private static List<BombKingPlayerInfo> PlayerInfo_;

		private static List<BombKingRewardInfo> BombKingReward_;

		private static DateTime _BombKingBeginTime;

		private static DateTime _BombKingEndTime;

		public static int battleStage
		{
			get
			{
				return _battleStage;
			}
			set
			{
				_battleStage = value;
				CenterServer.Instance.SendSynBombKingAll();
			}
		}

		public static int turn
		{
			get
			{
				return _turn;
			}
			set
			{
				_turn = value;
				CenterServer.Instance.SendSynBombKingAll();
			}
		}

		public static int status
		{
			get
			{
				return _status;
			}
			set
			{
				_status = value;
				CenterServer.Instance.SendSynBombKingAll();
			}
		}

		public static bool IsBombKingOpen
		{
			get
			{
				return _IsBombKingOpen;
			}
			set
			{
				_IsBombKingOpen = value;
				CenterServer.Instance.SendSynBombKingAll();
			}
		}

		public static int Week
		{
			get
			{
				return _Week;
			}
			set
			{
				_Week = value;
				CenterServer.Instance.SendSynBombKingAll();
			}
		}

		public static DateTime BombKingBeginTime
		{
			get
			{
				return _BombKingBeginTime;
			}
			set
			{
				_BombKingBeginTime = value;
				CenterServer.Instance.SendSynBombKingAll();
			}
		}

		public static DateTime BombKingEndTime
		{
			get
			{
				return _BombKingEndTime;
			}
			set
			{
				_BombKingEndTime = value;
				CenterServer.Instance.SendSynBombKingAll();
			}
		}

		public static bool LoadPlayer()
		{
			using (ProduceBussiness produceBussiness = new ProduceBussiness())
			{
				PlayerInfo_ = produceBussiness.GetBombKingPlayer().ToList();
				BombKingReward_ = produceBussiness.GetBombKingReward().ToList();
			}
			return true;
		}

		public static bool Init()
		{
			bool result = false;
			try
			{
				PlayerInfo_ = new List<BombKingPlayerInfo>();
				battleStage = 0;
				status = 1;
				turn = 0;
				IsOpen = false;
				IsBombKingOpen = false;
				if (GameProperties.EventBombKingWeek)
				{
					if (DateTime.Now.DayOfWeek > DayOfWeek.Tuesday && DateTime.Now.DayOfWeek < DayOfWeek.Saturday)
					{
						Week = 1;
					}
					else if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
					{
						Week = 2;
					}
					else
					{
						Week = ((DateTime.Now.DayOfWeek == DayOfWeek.Sunday) ? 3 : 0);
					}
				}
				else
				{
					Week = GetWeekNumber(DateTime.Now);
				}
				BombKingBeginTime = new DateTime(2016, 1, 1);
				BombKingEndTime = new DateTime(2016, 2, 1);
				LoadPlayer();
				InitGlobalTimers();
				result = true;
			}
			catch (Exception exception)
			{
				log.Error("BombKingMgr Init", exception);
			}
			return result;
		}

		public static int GetWeekNumber(DateTime date)
		{
			bool flag = false;
			int dayOfYear = date.DayOfYear;
			int num = (int)new DateTime(date.Year, 1, 1).DayOfWeek;
			int num2 = (int)new DateTime(date.Year, 12, 31).DayOfWeek;
			if (num == 0)
			{
				num = 7;
			}
			if (num2 == 0)
			{
				num2 = 7;
			}
			int num3 = 8 - num;
			if (num == 4 || num2 == 4)
			{
				flag = true;
			}
			int num4 = (int)Math.Ceiling((double)(dayOfYear - num3) / 7.0);
			if (num3 >= 4)
			{
				num4++;
			}
			if (num4 > 52 && !flag)
			{
				num4 = 1;
			}
			if (num4 == 0)
			{
				num4 = GetWeekNumber(new DateTime(date.Year - 1, 12, 31));
			}
			return num4 % 4;
		}

		public static bool InitGlobalTimers()
		{
			int num = 30000;
			if (m_BombKing == null)
			{
				m_BombKing = new Timer(ScanBombKingProc, null, num, num);
			}
			else
			{
				m_BombKing.Change(num, num);
			}
			return true;
		}

		protected static void ScanBombKingProc(object sender)
		{
			try
			{
				int tickCount = Environment.TickCount;
				if (log.IsInfoEnabled)
				{
					log.Info("Scan BombKing ...");
					log.Debug("Scan ThreadId=" + Thread.CurrentThread.ManagedThreadId);
				}
				ThreadPriority priority = Thread.CurrentThread.Priority;
				Thread.CurrentThread.Priority = ThreadPriority.Lowest;
				SendUpdateBombKing();
				Thread.CurrentThread.Priority = priority;
				int num = Environment.TickCount - tickCount;
				if (log.IsInfoEnabled)
				{
					log.Info("Scan BombKing complete!");
				}
			}
			catch (Exception exception)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("Scan BombKing Proc", exception);
				}
			}
		}

		public static void SendUpdateBombKing()
		{
			DateTime now = DateTime.Now;
			if ((now.Hour == 20 && now.Minute >= 30) || (now.Hour == 21 && now.Minute < 40))
			{
				if (GameProperties.EventBombKingWeek)
				{
					GetWeekNow(now);
					EventBombKing(now);
				}
				else if (now.DayOfWeek == DayOfWeek.Sunday)
				{
					Week = GetWeekNumber(now);
					EventBombKing(now);
				}
				return;
			}
			IsOpen = false;
			if (now.Hour >= 20)
			{
				return;
			}
			GetWeekNow(now);
			if (Week != 1)
			{
				return;
			}
			if (!GameProperties.EventBombKingWeek)
			{
				PlayerInfo_.Clear();
				CenterServer.Instance.SendBombKingClearPlayer();
				using ProduceBussiness produceBussiness = new ProduceBussiness();
				produceBussiness.DeleteBombKingPlayer();
				return;
			}
			if (now.DayOfWeek != DayOfWeek.Wednesday)
			{
				return;
			}
			PlayerInfo_.Clear();
			CenterServer.Instance.SendBombKingClearPlayer();
			using ProduceBussiness produceBussiness2 = new ProduceBussiness();
			produceBussiness2.DeleteBombKingPlayer();
		}

		private static void GetWeekNow(DateTime TimeNow)
		{
			if (TimeNow.DayOfWeek > DayOfWeek.Tuesday && TimeNow.DayOfWeek < DayOfWeek.Saturday)
			{
				Week = 1;
			}
			else if (TimeNow.DayOfWeek == DayOfWeek.Saturday)
			{
				Week = 2;
			}
			else if (TimeNow.DayOfWeek == DayOfWeek.Sunday)
			{
				Week = 3;
			}
			else
			{
				Week = 0;
			}
		}

		private static void EventBombKing(DateTime TimeNow)
		{
			if (!IsOpen)
			{
				IsOpen = true;
				if (Week == 0)
				{
					return;
				}
				if (Week > 1 && PlayerInfo_.Count == 0)
				{
					CenterServer.Instance.SendGMNotice(LanguageMgr.GetTranslation("BombKing.Msg.IsCancel"));
					battleStage = 0;
					status = 1;
					turn = 0;
					return;
				}
				CenterServer.Instance.SendBombKingOpenClose(value: true);
				IsBombKingOpen = true;
				CenterServer.Instance.SendGMNotice(LanguageMgr.GetTranslation("BombKing.Msg.IsStarted"));
				BombKingBeginTime = DateTime.Now.AddMinutes(1.0);
				BombKingEndTime = DateTime.Now.AddMinutes(6.0);
				turn = 0;
				switch (Week)
				{
				case 1:
					battleStage = 1;
					status = 0;
					turn = 0;
					CenterServer.Instance.SendGMNotice(LanguageMgr.GetTranslation("BombKing.Msg.Step1Notice", turn + 1));
					CenterServer.Instance.SendBombKingUpdateRequest();
					break;
				case 2:
					battleStage = 2;
					status = 0;
					turn = 0;
					CenterServer.Instance.SendGMNotice(LanguageMgr.GetTranslation("BombKing.Msg.Step2Notice", turn + 1));
					CenterServer.Instance.SendBombKingUpdateRequest();
					break;
				case 3:
					battleStage = 3;
					status = 0;
					turn = 0;
					CenterServer.Instance.SendGMNotice(LanguageMgr.GetTranslation("BombKing.Msg.Step3Notice"));
					CenterServer.Instance.SendBombKingUpdateRequest();
					break;
				}
			}
			else
			{
				if (!IsBombKingOpen)
				{
					return;
				}
				switch (Week)
				{
				case 1:
					if (DateTime.Now >= BombKingBeginTime)
					{
						status = 1;
						CenterServer.Instance.SendBombKingUpdateRequest();
					}
					if (!(DateTime.Now >= BombKingEndTime))
					{
						break;
					}
					if (turn < 9)
					{
						CenterServer.Instance.SendBombKingPlayerUpdateStep1();
						turn++;
						status = 0;
						CenterServer.Instance.SendGMNotice(LanguageMgr.GetTranslation("BombKing.Msg.Step1Notice2", turn, turn + 1));
						BombKingBeginTime = DateTime.Now.AddMinutes(1.0);
						BombKingEndTime = DateTime.Now.AddMinutes(6.0);
						CenterServer.Instance.SendBombKingUpdateRequest();
						break;
					}
					CenterServer.Instance.SendBombKingPlayerUpdateStep1();
					IsBombKingOpen = false;
					status = 1;
					battleStage = 0;
					turn = 0;
					if (!GameProperties.EventBombKingWeek)
					{
						CenterServer.Instance.SendGMNotice(LanguageMgr.GetTranslation("BombKing.Msg.Step1Complted"));
					}
					else if (TimeNow.DayOfWeek != DayOfWeek.Friday)
					{
						CenterServer.Instance.SendGMNotice("BombKing.msg.Step1Completed2");
					}
					else
					{
						CenterServer.Instance.SendGMNotice("BombKing.Msg.Step1Completed3");
					}
					CenterServer.Instance.SendBombKingUpdateRequest();
					if (GameProperties.EventBombKingWeek)
					{
						if (TimeNow.DayOfWeek == DayOfWeek.Friday)
						{
							SendItemReward();
						}
					}
					else
					{
						SendItemReward();
					}
					break;
				case 2:
					if (DateTime.Now >= BombKingBeginTime)
					{
						status = 1;
						CenterServer.Instance.SendBombKingUpdateRequest();
					}
					if (!(DateTime.Now >= BombKingEndTime))
					{
						break;
					}
					if (turn < 9)
					{
						CenterServer.Instance.SendBombKingPlayerUpdateStep1();
						turn++;
						status = 0;
						CenterServer.Instance.SendGMNotice(LanguageMgr.GetTranslation("BombKing.Msg.Step2Completed", turn, turn + 1));
						BombKingBeginTime = DateTime.Now.AddMinutes(1.0);
						BombKingEndTime = DateTime.Now.AddMinutes(6.0);
						CenterServer.Instance.SendBombKingUpdateRequest();
						break;
					}
					CenterServer.Instance.SendBombKingPlayerUpdateStep1();
					IsBombKingOpen = false;
					status = 1;
					battleStage = 0;
					turn = 0;
					if (!GameProperties.EventBombKingWeek)
					{
						CenterServer.Instance.SendGMNotice(LanguageMgr.GetTranslation("BombKing.Msg.Step2Completed2"));
					}
					else
					{
						CenterServer.Instance.SendGMNotice(LanguageMgr.GetTranslation("BombKing.Msg.Step2Completed3"));
					}
					CenterServer.Instance.SendBombKingUpdateRequest();
					SendItemReward();
					break;
				case 3:
					if (DateTime.Now >= BombKingBeginTime)
					{
						status = 1;
						CenterServer.Instance.SendBombKingUpdateRequest();
					}
					if (!(DateTime.Now >= BombKingEndTime))
					{
						break;
					}
					if (battleStage < 6)
					{
						CenterServer.Instance.SendBombKingPlayerUpdateStep2();
						battleStage++;
						status = 0;
						switch (battleStage)
						{
						case 4:
							CenterServer.Instance.SendGMNotice(LanguageMgr.GetTranslation("BombKing.Msg.Step3Completed"));
							UpdateRankAll(2);
							UpdateMyRank();
							break;
						case 5:
							CenterServer.Instance.SendGMNotice(LanguageMgr.GetTranslation("BombKing.Msg.Step3Completed2"));
							UpdateRankAll(3);
							UpdateMyRank();
							break;
						case 6:
							CenterServer.Instance.SendGMNotice(LanguageMgr.GetTranslation("BombKing.Msg.Step3Completed3"));
							UpdateRankAll(4);
							UpdateMyRank();
							break;
						}
						BombKingBeginTime = DateTime.Now.AddMinutes(1.0);
						BombKingEndTime = DateTime.Now.AddMinutes(6.0);
						CenterServer.Instance.SendBombKingUpdateRequest();
					}
					else
					{
						CenterServer.Instance.SendBombKingPlayerUpdateStep2();
						IsBombKingOpen = false;
						CenterServer.Instance.SendGMNotice(LanguageMgr.GetTranslation("BombKing.Msg.CompletedAll"));
						UpdateRankAll(5);
						UpdateMyRank();
						SendItemReward();
						status = 1;
						battleStage = 0;
						turn = 0;
						CenterServer.Instance.SendBombKingUpdateRequest();
					}
					break;
				}
			}
		}

		public static void AddScore(BombKingPlayerInfo temp, int score, bool win)
		{
			using ProduceBussiness produceBussiness = new ProduceBussiness();
			BombKingPlayerInfo bombKingPlayerInfo = null;
			switch (Week)
			{
			case 1:
			{
				PlayerInfo_ = PlayerInfo_.OrderByDescending((BombKingPlayerInfo s) => s.myScore).ToList();
				bool flag = false;
				bombKingPlayerInfo = PlayerInfo_.SingleOrDefault((BombKingPlayerInfo s) => s.UserID == temp.UserID && s.AreaID == temp.AreaID);
				if (bombKingPlayerInfo != null)
				{
					PlayerInfo_.Remove(bombKingPlayerInfo);
					bombKingPlayerInfo.myScore += score;
					PlayerInfo_.Add(bombKingPlayerInfo);
					produceBussiness.UpdateBombKingPlayer(bombKingPlayerInfo);
					flag = true;
				}
				if (!flag)
				{
					BombKingPlayerInfo bombKingPlayerInfo2 = new BombKingPlayerInfo();
					bombKingPlayerInfo2.UserID = temp.UserID;
					bombKingPlayerInfo2.NickName = temp.NickName;
					bombKingPlayerInfo2.AreaID = temp.AreaID;
					bombKingPlayerInfo2.AreaName = temp.AreaName;
					bombKingPlayerInfo2.myRank = 0;
					bombKingPlayerInfo2.Place = 0;
					bombKingPlayerInfo2.vipType = temp.vipType;
					bombKingPlayerInfo2.vipLevel = temp.vipLevel;
					bombKingPlayerInfo2.myScore = score;
					bombKingPlayerInfo2.Score = 0;
					bombKingPlayerInfo2.RankType = 2;
					bombKingPlayerInfo2.status = 0;
					bombKingPlayerInfo2.style = temp.style;
					bombKingPlayerInfo2.color = temp.color;
					bombKingPlayerInfo2.sex = temp.sex;
					PlayerInfo_.Add(bombKingPlayerInfo2);
					produceBussiness.AddBombKingPlayer(bombKingPlayerInfo2);
				}
				break;
			}
			case 2:
				bombKingPlayerInfo = PlayerInfo_.OrderByDescending((BombKingPlayerInfo s) => s.myScore).Take(100).SingleOrDefault((BombKingPlayerInfo s) => s.UserID == temp.UserID && s.AreaID == temp.AreaID);
				if (bombKingPlayerInfo != null)
				{
					PlayerInfo_.Remove(bombKingPlayerInfo);
					bombKingPlayerInfo.Score += score;
					bombKingPlayerInfo.NickName = temp.NickName;
					bombKingPlayerInfo.vipType = temp.vipType;
					bombKingPlayerInfo.vipLevel = temp.vipLevel;
					bombKingPlayerInfo.style = temp.style;
					bombKingPlayerInfo.color = temp.color;
					bombKingPlayerInfo.sex = temp.sex;
					bombKingPlayerInfo.RankType = 2;
					PlayerInfo_.Add(bombKingPlayerInfo);
					produceBussiness.UpdateBombKingPlayer(bombKingPlayerInfo);
				}
				break;
			case 3:
				bombKingPlayerInfo = PlayerInfo_.OrderByDescending((BombKingPlayerInfo s) => s.Score).Take(16).SingleOrDefault((BombKingPlayerInfo s) => s.UserID == temp.UserID && s.AreaID == temp.AreaID);
				if (bombKingPlayerInfo != null)
				{
					PlayerInfo_.Remove(bombKingPlayerInfo);
					bombKingPlayerInfo.NickName = temp.NickName;
					bombKingPlayerInfo.vipType = temp.vipType;
					bombKingPlayerInfo.vipLevel = temp.vipLevel;
					bombKingPlayerInfo.style = temp.style;
					bombKingPlayerInfo.color = temp.color;
					bombKingPlayerInfo.sex = temp.sex;
					bombKingPlayerInfo.Place += score;
					if (win)
					{
						bombKingPlayerInfo.RankType++;
					}
					PlayerInfo_.Add(bombKingPlayerInfo);
					produceBussiness.UpdateBombKingPlayer(bombKingPlayerInfo);
				}
				break;
			}
			if (bombKingPlayerInfo != null)
			{
				CenterServer.Instance.SendBombKingUpdatePlayer(bombKingPlayerInfo);
			}
		}

		public static void UpdateMyRank()
		{
			using ProduceBussiness produceBussiness = new ProduceBussiness();
			List<BombKingPlayerInfo> list = (from s in GetListTop16BombKingPlayerInfo()
				orderby s.Place descending
				select s).ToList();
			foreach (BombKingPlayerInfo item in list)
			{
				int rank = GetRank(item.UserID, item.AreaID, item.RankType);
				PlayerInfo_.Remove(item);
				item.myRank = rank;
				PlayerInfo_.Add(item);
				produceBussiness.UpdateBombKingPlayer(item);
			}
			CenterServer.Instance.SendBombKingUpdatePlayer(list);
		}

		private static int GetRank(int UserId, int araeId, int RankType)
		{
			int num = 0;
			switch (RankType)
			{
			case 3:
				num = 8;
				break;
			case 4:
				num = 12;
				break;
			case 5:
				num = 14;
				break;
			case 6:
				num = 15;
				break;
			}
			List<BombKingPlayerInfo> list = (from s in GetListTop16BombKingPlayerInfo()
				where s.RankType == RankType
				orderby s.Place
				select s).ToList();
			for (int i = 0; i < list.Count; i++)
			{
				if (UserId == list[i].UserID && araeId == list[i].AreaID)
				{
					return 16 - (num + i);
				}
			}
			return 16;
		}

		public static void UpdateRankAll(int RankType)
		{
			using ProduceBussiness produceBussiness = new ProduceBussiness();
			List<BombKingPlayerInfo> list = (from s in GetListTop16BombKingPlayerInfo()
				where s.RankType >= RankType
				select s).ToList();
			for (int i = 0; i < list.Count; i += 2)
			{
				if (i + 1 < list.Count)
				{
					if (list[i].RankType == list[i + 1].RankType)
					{
						PlayerInfo_.Remove(list[i]);
						list[i].RankType++;
						PlayerInfo_.Add(list[i]);
						produceBussiness.UpdateBombKingPlayer(list[i]);
					}
				}
				else if (list[i].RankType == RankType)
				{
					PlayerInfo_.Remove(list[i]);
					list[i].RankType++;
					PlayerInfo_.Add(list[i]);
					produceBussiness.UpdateBombKingPlayer(list[i]);
				}
			}
			CenterServer.Instance.SendBombKingUpdatePlayer(list);
		}

		public static List<ItemInfo> CreateReward(int type)
		{
			List<BombKingRewardInfo> list = BombKingReward_.Where((BombKingRewardInfo s) => s.Type == type && s.Week == Week).ToList();
			List<ItemInfo> list2 = new List<ItemInfo>();
			foreach (BombKingRewardInfo item in list)
			{
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(item.ItemId), item.Count, 101);
				if (itemInfo != null)
				{
					itemInfo.Count = item.Count;
					itemInfo.IsBinds = true;
					itemInfo.ValidDate = item.Valid;
					itemInfo.StrengthenLevel = item.Strengthen;
					itemInfo.AttackCompose = item.Attack;
					itemInfo.DefendCompose = item.Defend;
					itemInfo.AgilityCompose = item.Agility;
					itemInfo.LuckCompose = item.Luck;
					if (list2 == null)
					{
						list2 = new List<ItemInfo>();
					}
					list2.Add(itemInfo);
				}
			}
			return list2;
		}

		public static List<BombKingPlayerInfo> GetListTop100BombKingPlayerInfo()
		{
			return PlayerInfo_.OrderByDescending((BombKingPlayerInfo s) => s.myScore).Take(100).ToList();
		}

		public static List<BombKingPlayerInfo> GetListTop16BombKingPlayerInfo()
		{
			return (from s in PlayerInfo_
				where s.Score > 0
				orderby s.Score descending
				select s).Take(16).ToList();
		}

		public static void AddNewRank(int ID, int valid, int UserId, int ZoneId)
		{
			Player player = ServerMgr.GetPlayer(UserId, ZoneId);
			if (player != null)
			{
				player.CurrentServer.SendBombKingAddRank(ID, valid, UserId);
				return;
			}
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			List<UserRankInfo> singleUserRank = playerBussiness.GetSingleUserRank(UserId);
			NewTitleInfo newTitleInfo = NewTitleMgr.FindNewTitle(ID);
			foreach (UserRankInfo item2 in singleUserRank)
			{
				if (!item2.IsValidRank())
				{
					item2.IsExit = false;
					playerBussiness.UpdateUserRank(item2);
				}
			}
			if (newTitleInfo != null)
			{
				UserRankInfo item = new UserRankInfo
				{
					Info = newTitleInfo,
					Name = newTitleInfo.Name,
					NewTitleID = newTitleInfo.ID,
					UserID = UserId,
					BeginDate = DateTime.Now,
					EndDate = DateTime.Now.AddDays(valid),
					Validate = valid,
					IsExit = true
				};
				playerBussiness.AddUserRank(item);
			}
		}

		public static void SendItemReward()
		{
			List<BombKingPlayerInfo> list = new List<BombKingPlayerInfo>();
			List<ItemInfo> infos = new List<ItemInfo>();
			using (new PlayerBussiness())
			{
				switch (Week)
				{
				case 1:
				{
					List<BombKingPlayerInfo> list4 = PlayerInfo_.OrderByDescending((BombKingPlayerInfo s) => s.myScore).ToList();
					for (int k = 0; k < list4.Count; k++)
					{
						switch (k)
						{
						case 0:
							infos = CreateReward(1);
							break;
						case 100:
							infos = CreateReward(2);
							break;
						case 200:
							infos = CreateReward(3);
							break;
						case 300:
							infos = CreateReward(4);
							break;
						case 500:
							infos = CreateReward(5);
							break;
						case 600:
							infos = CreateReward(6);
							break;
						case 800:
							infos = CreateReward(7);
							break;
						case 1000:
							infos = CreateReward(8);
							break;
						}
						WorldEventMgr.SendItemsToMails(infos, list4[k].UserID, list4[k].NickName, list4[k].AreaID, LanguageMgr.GetTranslation("BombKing.Msg.Mail.Title.Award.Step1", k + 1));
						if (k < 100)
						{
							if (!GameProperties.EventBombKingWeek)
							{
								int userID3 = list4[k].UserID;
								string nickName3 = list4[k].NickName;
								int areaID3 = list4[k].AreaID;
								string translation5 = LanguageMgr.GetTranslation("BombKing.Msg.Mail.Title.Notice");
								string translation6 = LanguageMgr.GetTranslation("BombKing.Msg.Mail.Content.Notice", DateTime.Now.AddDays(7.0).Date.ToString("dd/MM/yyyy"));
								WorldEventMgr.SendMailToUser(userID3, nickName3, areaID3, translation5, translation6);
							}
							else
							{
								int userID4 = list4[k].UserID;
								string nickName4 = list4[k].NickName;
								int areaID4 = list4[k].AreaID;
								string translation7 = LanguageMgr.GetTranslation("BombKing.Msg.Mail.Title.Notice");
								string translation8 = LanguageMgr.GetTranslation("BombKing.Msg.Mail.Content.Notice", DateTime.Now.AddDays(1.0).Date.ToString("dd/MM/yyyy"));
								WorldEventMgr.SendMailToUser(userID4, nickName4, areaID4, translation7, translation8);
							}
						}
					}
					break;
				}
				case 2:
				{
					List<BombKingPlayerInfo> list3 = (from s in GetListTop100BombKingPlayerInfo()
						orderby s.Score descending
						select s).ToList();
					for (int j = 0; j < list3.Count; j++)
					{
						switch (j)
						{
						case 0:
							infos = CreateReward(1);
							break;
						case 16:
							infos = CreateReward(2);
							break;
						case 30:
							infos = CreateReward(3);
							break;
						case 60:
							infos = CreateReward(4);
							break;
						}
						WorldEventMgr.SendItemsToMails(infos, list3[j].UserID, list3[j].NickName, list3[j].AreaID, LanguageMgr.GetTranslation("BombKing.Msg.Mail.Title.Award.Step2", j + 1));
						if (j < 16)
						{
							if (!GameProperties.EventBombKingWeek)
							{
								int userID = list3[j].UserID;
								string nickName = list3[j].NickName;
								int areaID = list3[j].AreaID;
								string translation = LanguageMgr.GetTranslation("BombKing.Msg.Mail.Title.Notice");
								string translation2 = LanguageMgr.GetTranslation("BombKing.Msg.Mail.Content.Notice2", DateTime.Now.AddDays(7.0).Date.ToString("dd/MM/yyyy"));
								WorldEventMgr.SendMailToUser(userID, nickName, areaID, translation, translation2);
							}
							else
							{
								int userID2 = list3[j].UserID;
								string nickName2 = list3[j].NickName;
								int areaID2 = list3[j].AreaID;
								string translation3 = LanguageMgr.GetTranslation("BombKing.Msg.Mail.Title.Notice");
								string translation4 = LanguageMgr.GetTranslation("BombKing.Msg.Mail.Content.Notice2", DateTime.Now.AddDays(1.0).Date.ToString("dd/MM/yyyy"));
								WorldEventMgr.SendMailToUser(userID2, nickName2, areaID2, translation3, translation4);
							}
						}
					}
					break;
				}
				case 3:
				{
					int valid = (GameProperties.EventBombKingWeek ? 2 : 13);
					List<BombKingPlayerInfo> list2 = (from s in GetListTop16BombKingPlayerInfo()
						orderby s.myRank
						select s).ToList();
					for (int i = 0; i < list2.Count; i++)
					{
						int myRank = list2[i].myRank;
						List<ItemInfo> infos2;
						switch (myRank)
						{
						case 1:
							infos2 = CreateReward(1);
							AddNewRank(611, valid, list2[i].UserID, list2[i].AreaID);
							break;
						case 2:
							infos2 = CreateReward(2);
							AddNewRank(612, valid, list2[i].UserID, list2[i].AreaID);
							break;
						case 3:
							infos2 = CreateReward(3);
							AddNewRank(613, valid, list2[i].UserID, list2[i].AreaID);
							break;
						case 4:
							infos2 = CreateReward(4);
							break;
						case 5:
							infos2 = CreateReward(5);
							break;
						default:
							infos2 = CreateReward(6);
							break;
						}
						if (myRank > 3 || myRank < 9)
						{
							AddNewRank(614, valid, list2[i].UserID, list2[i].AreaID);
						}
						WorldEventMgr.SendItemsToMails(infos2, list2[i].UserID, list2[i].NickName, list2[i].AreaID, LanguageMgr.GetTranslation("BombKing.Msg.Mail.Title.Award.Step3", myRank));
					}
					break;
				}
				}
			}
		}
	}
}
