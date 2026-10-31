using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;

namespace Game.Server.HotSpringRooms.Handle;

public class HotSpringRoomsHandleMgr
{
	private Dictionary<int, IHotSpringRoomsCommandHadler> handles = new Dictionary<int, IHotSpringRoomsCommandHadler>();

	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public IHotSpringRoomsCommandHadler LoadCommandHandler(int code)
	{
		if (handles.ContainsKey(code))
		{
			return handles[code];
		}
		log.Error("LoadCommandHandler code018: " + code);
		return null;
	}

	public HotSpringRoomsHandleMgr()
	{
		handles.Clear();
		SearchCommandHandlers(Assembly.GetAssembly(typeof(GameServer)));
	}

	protected int SearchCommandHandlers(Assembly assembly)
	{
		int num = 0;
		Type[] types = assembly.GetTypes();
		foreach (Type type in types)
		{
			if (type.IsClass && !(type.GetInterface("Game.Server.HotSpringRooms.Handle.IHotSpringRoomsCommandHadler") == null))
			{
				HotSpringRoomsHandleAttbute[] array = (HotSpringRoomsHandleAttbute[])type.GetCustomAttributes(typeof(HotSpringRoomsHandleAttbute), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as IHotSpringRoomsCommandHadler);
				}
			}
		}
		return num;
	}

	protected void RegisterCommandHandler(int code, IHotSpringRoomsCommandHadler handle)
	{
		handles.Add(code, handle);
	}
}
