using System;
using System.Reflection;
using System.Threading;
using log4net;

namespace Game.Server.AnotherDimension;

public class AnotherDimensionMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Random rand;

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

	public static void SendAnotherDimensionAward()
	{
	}

	public static bool Init()
	{
		try
		{
			rand = new Random();
			return true;
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("AnotherDimensionMgr =>Int", exception);
			}
			return false;
		}
		finally
		{
			BeginTimer();
		}
	}
}
