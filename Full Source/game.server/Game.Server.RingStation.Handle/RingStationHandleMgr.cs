using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;

namespace Game.Server.RingStation.Handle;

public class RingStationHandleMgr
{
	private Dictionary<int, IRingStationCommandHadler> handles = new Dictionary<int, IRingStationCommandHadler>();

	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public IRingStationCommandHadler LoadCommandHandler(int code)
	{
		if (handles.ContainsKey(code))
		{
			return handles[code];
		}
		log.Error("LoadCommandHandler code025: " + code);
		return null;
	}

	public RingStationHandleMgr()
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
			if (type.IsClass && !(type.GetInterface("Game.Server.RingStation.Handle.IRingStationCommandHadler") == null))
			{
				RingStationHandleAttbute[] array = (RingStationHandleAttbute[])type.GetCustomAttributes(typeof(RingStationHandleAttbute), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as IRingStationCommandHadler);
				}
			}
		}
		return num;
	}

	protected void RegisterCommandHandler(int code, IRingStationCommandHadler handle)
	{
		handles.Add(code, handle);
	}
}
