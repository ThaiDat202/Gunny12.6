using System;
using System.Reflection;
using System.Threading;
using log4net;

namespace Game.Server.Renshen;

public class RenshenMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Random rand;

	protected static Timer _dbTimer;

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

	public static void StopAllTimer()
	{
		if (_dbTimer != null)
		{
			_dbTimer.Dispose();
			_dbTimer = null;
		}
	}

	public static void SendRenshenAward()
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
				log.Error("RenshenMgr =>Int", exception);
			}
			return false;
		}
		finally
		{
			BeginTimer();
		}
	}
}
