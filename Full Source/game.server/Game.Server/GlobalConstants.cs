using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Bussiness;
using Game.Server.GameObjects;
using Game.Server.Managers;
using log4net;

namespace Game.Server;

public class GlobalConstants
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<eActive, ActiveStatusInfo> _activeStatus = new Dictionary<eActive, ActiveStatusInfo>();

	private static bool _subSysUserActice;

	private static eActive[] _activeArray;

	protected static Timer _globalTimer;

	public static bool SubSysUserActice
	{
		get
		{
			return _subSysUserActice;
		}
		set
		{
			_subSysUserActice = value;
		}
	}

	public static bool Start()
	{
		try
		{
			Array values = Enum.GetValues(typeof(eActive));
			List<eActive> list = new List<eActive>();
			for (int i = 0; i < values.Length; i++)
			{
				eActive item = (eActive)values.GetValue(i);
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}
			_activeArray = list.ToArray();
			LoadActice();
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				_subSysUserActice = playerBussiness.GetInformationAchema("Sys_Users_Detail_2");
			}
			return InitGlobalTimer();
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("GlobalConstants", exception);
			}
			return false;
		}
	}

	private static bool SurvivalTime()
	{
		if (GameProperties.AlwaysOpen1vs5)
		{
			return true;
		}
		return DateTime.Now.Hour >= 10 && DateTime.Now.Hour <= 23;
	}

	private static bool HorseRaceTime()
	{
		DateTime dateTime = Convert.ToDateTime(GameProperties.HorseGameEndTime);
		return DateTime.Now.Date < dateTime.Date;
	}

	private static bool SanXiaoTime()
	{
		DateTime dateTime = Convert.ToDateTime(GameProperties.SanXiaoEndTime);
		return DateTime.Now.Date < dateTime.Date;
	}

	private static bool MemoryGameTime()
	{
		DateTime dateTime = Convert.ToDateTime(GameProperties.MemoryGameEndTime);
		return DateTime.Now.Date < dateTime.Date;
	}

	private static bool QiYuanGameTime()
	{
		DateTime dateTime = Convert.ToDateTime(GameProperties.QiYuanEndTime);
		return DateTime.Now.Date < dateTime.Date;
	}

	public static void LoadActice()
	{
		try
		{
			eActive[] activeArray = _activeArray;
			foreach (eActive eActive2 in activeArray)
			{
				ActiveStatusInfo activeStatusInfo = new ActiveStatusInfo
				{
					Id = eActive2
				};
				activeStatusInfo.OnClose();
				switch (eActive2)
				{
				case eActive.SurvivalMode:
					activeStatusInfo.State = (SurvivalTime() ? 1 : 0);
					break;
				case eActive.HorseRace:
					activeStatusInfo.State = (HorseRaceTime() ? 1 : 0);
					break;
				case eActive.GodCardRaise:
					activeStatusInfo.State = (SanXiaoTime() ? 1 : 0);
					break;
				case eActive.SanXiao:
					activeStatusInfo.State = (SanXiaoTime() ? 1 : 0);
					break;
				case eActive.MemoryGame:
					activeStatusInfo.State = (MemoryGameTime() ? 1 : 0);
					break;
				case eActive.QiYuan:
					activeStatusInfo.State = (QiYuanGameTime() ? 1 : 0);
					break;
				}
				lock (_activeStatus)
				{
					if (!_activeStatus.ContainsKey(eActive2))
					{
						_activeStatus.Add(eActive2, activeStatusInfo);
					}
				}
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("Load active", exception);
			}
		}
	}

	public static void UpdateActice()
	{
		lock (_activeStatus)
		{
			foreach (eActive key in _activeStatus.Keys)
			{
				ActiveStatusInfo activeStatusInfo = _activeStatus[key];
				switch (activeStatusInfo.Id)
				{
				case eActive.SurvivalMode:
					activeStatusInfo.State = (SurvivalTime() ? 1 : 0);
					break;
				case eActive.HorseRace:
					activeStatusInfo.State = (HorseRaceTime() ? 1 : 0);
					break;
				case eActive.GodCardRaise:
					activeStatusInfo.State = (SanXiaoTime() ? 1 : 0);
					break;
				case eActive.SanXiao:
					activeStatusInfo.State = (SanXiaoTime() ? 1 : 0);
					break;
				case eActive.MemoryGame:
					activeStatusInfo.State = (MemoryGameTime() ? 1 : 0);
					break;
				case eActive.QiYuan:
					activeStatusInfo.State = (QiYuanGameTime() ? 1 : 0);
					break;
				}
			}
		}
	}

	public static void OpenOrCloseAll(bool isOpen)
	{
		lock (_activeStatus)
		{
			foreach (eActive key in _activeStatus.Keys)
			{
				if (isOpen)
				{
					_activeStatus[key].OnOpen();
				}
				else
				{
					_activeStatus[key].OnClose();
				}
			}
		}
	}

	public static void OpenOrCloseById(eActive type, bool isOpen)
	{
		lock (_activeStatus)
		{
			if (_activeStatus.ContainsKey(type))
			{
				if (isOpen)
				{
					_activeStatus[type].OnOpen();
				}
				else
				{
					_activeStatus[type].OnClose();
				}
			}
		}
	}

	public static bool IsOpenActive(eActive type)
	{
		return GetActiveStatus(type).State == 1;
	}

	public static ActiveStatusInfo GetActiveStatus(eActive type)
	{
		lock (_activeStatus)
		{
			if (_activeStatus.ContainsKey(type))
			{
				return _activeStatus[type];
			}
		}
		return new ActiveStatusInfo();
	}

	public static bool InitGlobalTimer()
	{
		int num = 60000;
		if (_globalTimer == null)
		{
			_globalTimer = new Timer(GlobalTimerProc, null, num, num);
		}
		else
		{
			_globalTimer.Change(num, num);
		}
		return true;
	}

	public static void StopGlobalTimer()
	{
		if (_globalTimer != null)
		{
			_globalTimer.Dispose();
			_globalTimer = null;
		}
	}

	protected static void GlobalTimerProc(object sender)
	{
		try
		{
			int tickCount = Environment.TickCount;
			if (log.IsInfoEnabled)
			{
				log.Info("Global Proccess...");
				log.Debug("Global ThreadId=" + Thread.CurrentThread.ManagedThreadId);
			}
			ThreadPriority priority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			UpdateActice();
			lock (_activeStatus)
			{
				foreach (ActiveStatusInfo value in _activeStatus.Values)
				{
					bool flag = value.State == 1;
					switch (value.Id)
					{
					case eActive.SurvivalMode:
					{
						if (!value.OnChange())
						{
							break;
						}
						GamePlayer[] allPlayers2 = WorldMgr.GetAllPlayers();
						GamePlayer[] array2 = allPlayers2;
						foreach (GamePlayer gamePlayer2 in array2)
						{
							if (!flag)
							{
								gamePlayer2.ResetRoom(isWin: false, "SurvivalModeClose");
							}
						}
						value.OnSuccess(value.State);
						break;
					}
					case eActive.HorseRace:
						if (value.OnChange())
						{
							GamePlayer[] allPlayers3 = WorldMgr.GetAllPlayers();
							GamePlayer[] array3 = allPlayers3;
							foreach (GamePlayer gamePlayer3 in array3)
							{
								gamePlayer3.Actives.SendHorseRaceOpenClose();
							}
							value.OnSuccess(value.State);
						}
						break;
					case eActive.GodCardRaise:
						if (value.OnChange())
						{
							GamePlayer[] allPlayers6 = WorldMgr.GetAllPlayers();
							GamePlayer[] array6 = allPlayers6;
							foreach (GamePlayer gamePlayer6 in array6)
							{
								gamePlayer6.Actives.SendGodCardRaiseOpenClose();
							}
							value.OnSuccess(value.State);
						}
						break;
					case eActive.SanXiao:
						if (value.OnChange())
						{
							GamePlayer[] allPlayers4 = WorldMgr.GetAllPlayers();
							GamePlayer[] array4 = allPlayers4;
							foreach (GamePlayer gamePlayer4 in array4)
							{
								gamePlayer4.Actives.SendSanXiaoOpenClose();
							}
							value.OnSuccess(value.State);
						}
						break;
					case eActive.MemoryGame:
						if (value.OnChange())
						{
							GamePlayer[] allPlayers5 = WorldMgr.GetAllPlayers();
							GamePlayer[] array5 = allPlayers5;
							foreach (GamePlayer gamePlayer5 in array5)
							{
								gamePlayer5.Actives.SendMemoryGameOpenClose();
							}
							value.OnSuccess(value.State);
						}
						break;
					case eActive.QiYuan:
						if (value.OnChange())
						{
							GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
							GamePlayer[] array = allPlayers;
							foreach (GamePlayer gamePlayer in array)
							{
								gamePlayer.Actives.SendDDTQiYuanOpenClose();
							}
							value.OnSuccess(value.State);
						}
						break;
					}
				}
			}
			Thread.CurrentThread.Priority = priority;
			if (log.IsInfoEnabled)
			{
				log.Info("Global scan complete!");
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error(" GlobalTimerProc", exception);
			}
		}
	}
}
