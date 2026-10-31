using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;

namespace Game.Server.WorshipTheMoon.Handle;

public class WorshipTheMoonHandleMgr
{
	private Dictionary<int, IWorshipTheMoonCommandHadler> handles = new Dictionary<int, IWorshipTheMoonCommandHadler>();

	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public IWorshipTheMoonCommandHadler LoadCommandHandler(int code)
	{
		if (handles.ContainsKey(code))
		{
			return handles[code];
		}
		log.Error("LoadCommandHandler code028: " + code);
		return null;
	}

	public WorshipTheMoonHandleMgr()
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
			if (type.IsClass && !(type.GetInterface("Game.Server.WorshipTheMoon.Handle.IWorshipTheMoonCommandHadler") == null))
			{
				WorshipTheMoonHandleAttbute[] array = (WorshipTheMoonHandleAttbute[])type.GetCustomAttributes(typeof(WorshipTheMoonHandleAttbute), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as IWorshipTheMoonCommandHadler);
				}
			}
		}
		return num;
	}

	protected void RegisterCommandHandler(int code, IWorshipTheMoonCommandHadler handle)
	{
		handles.Add(code, handle);
	}
}
