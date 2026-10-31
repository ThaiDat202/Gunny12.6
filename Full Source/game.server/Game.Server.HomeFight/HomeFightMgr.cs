using System;
using System.Reflection;
using System.Threading;
using Bussiness.Managers;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.HomeFight;

public class HomeFightMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Random rand;

	private static CommunalActiveInfo m_communalActives;

	protected static Timer _dbTimer;

	protected static Timer _expTimer;

	public static void BeginTimer()
	{
		int num = 1800000;
		if (_dbTimer == null)
		{
			_dbTimer = new Timer(dbTimeCheck, null, num, num);
		}
		else
		{
			_dbTimer.Change(num, num);
		}
		num = 60000;
		if (_expTimer == null)
		{
			_expTimer = new Timer(BoatExpCheck, null, num, num);
		}
		else
		{
			_expTimer.Change(num, num);
		}
	}

	protected static void dbTimeCheck(object sender)
	{
		try
		{
			int tickCount = Environment.TickCount;
			ThreadPriority priority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			Thread.CurrentThread.Priority = priority;
			tickCount = Environment.TickCount - tickCount;
		}
		catch (Exception ex)
		{
			Console.WriteLine("Communal Active: " + ex);
		}
	}

	protected static void BoatExpCheck(object sender)
	{
		try
		{
			int tickCount = Environment.TickCount;
			ThreadPriority priority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			Thread.CurrentThread.Priority = priority;
			tickCount = Environment.TickCount - tickCount;
		}
		catch (Exception ex)
		{
			Console.WriteLine("Communal Active: " + ex);
		}
	}

	public static void StopAllTimer()
	{
		if (_dbTimer != null)
		{
			_dbTimer.Dispose();
			_dbTimer = null;
		}
		if (_expTimer != null)
		{
			_expTimer.Dispose();
			_expTimer = null;
		}
	}

	public static void SendHomeFightAward()
	{
	}

	public static bool Init()
	{
		try
		{
			rand = new Random();
			CommunalActiveInfo[] allCommunalActives = CommunalActiveMgr.GetAllCommunalActives();
			CommunalActiveInfo[] array = allCommunalActives;
			foreach (CommunalActiveInfo communalActiveInfo in array)
			{
				if (communalActiveInfo.IsOpen)
				{
					m_communalActives = communalActiveInfo;
				}
			}
			return LoadTotalScore();
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("HomeFightMgr =>Int", exception);
			}
			return false;
		}
		finally
		{
			BeginTimer();
		}
	}

	private static bool LoadTotalScore()
	{
		bool result = false;
		try
		{
			result = true;
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("HomeFightMgr =>LoadLocalTotalScore", exception);
			}
		}
		return result;
	}
}
