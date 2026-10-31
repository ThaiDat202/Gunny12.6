using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using log4net;

namespace Game.Server.HotSpringRooms;

public class HotSpringRoomsMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Random rand;

	protected static Timer _expTimer;

	private static HotSpringRoom[] _rooms;

	public static HotSpringRoom[] Rooms => _rooms;

	public static void BeginTimer()
	{
		int num = 120000;
		if (_expTimer == null)
		{
			_expTimer = new Timer(ExpCheck, null, num, num);
		}
		else
		{
			_expTimer.Change(num, num);
		}
	}

	protected static void ExpCheck(object sender)
	{
		try
		{
			int tickCount = Environment.TickCount;
			ThreadPriority priority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			List<HotSpringRoom> allRooms = GetAllRooms();
			foreach (HotSpringRoom item in allRooms)
			{
				item.RoomTimeUpdate();
			}
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
		if (_expTimer != null)
		{
			_expTimer.Dispose();
			_expTimer = null;
		}
	}

	public static bool Init()
	{
		try
		{
			rand = new Random();
			int num = 30;
			_rooms = new HotSpringRoom[num];
			for (int i = 0; i < num; i++)
			{
				_rooms[i] = new HotSpringRoom(i + 1);
			}
			return true;
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("HotSpringRoomsMgr =>Int", exception);
			}
			return false;
		}
		finally
		{
			BeginTimer();
		}
	}

	public static List<HotSpringRoom> GetAllRooms()
	{
		List<HotSpringRoom> list = new List<HotSpringRoom>();
		lock (_rooms)
		{
			HotSpringRoom[] rooms = _rooms;
			foreach (HotSpringRoom item in rooms)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public static HotSpringRoom GetSingleRoom(int roomId)
	{
		lock (_rooms)
		{
			HotSpringRoom[] rooms = _rooms;
			foreach (HotSpringRoom hotSpringRoom in rooms)
			{
				if (hotSpringRoom.RoomId == roomId)
				{
					return hotSpringRoom;
				}
			}
		}
		return null;
	}

	public static HotSpringRoom GetRandomRoom()
	{
		lock (_rooms)
		{
			HotSpringRoom[] rooms = _rooms;
			foreach (HotSpringRoom hotSpringRoom in rooms)
			{
				if (hotSpringRoom.HasFreeLot())
				{
					return hotSpringRoom;
				}
			}
		}
		return null;
	}
}
