using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;

namespace Game.Server.ActiveSystem.Handle;

public class ActiveSystemHandleMgr
{
	private Dictionary<int, IActiveSystemCommandHadler> handles = new Dictionary<int, IActiveSystemCommandHadler>();

	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public IActiveSystemCommandHadler LoadCommandHandler(int code)
	{
		if (handles.ContainsKey(code))
		{
			return handles[code];
		}
		log.Error("LoadCommandHandler code002: " + code);
		return null;
	}

	public ActiveSystemHandleMgr()
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
			if (type.IsClass && !(type.GetInterface("Game.Server.ActiveSystem.Handle.IActiveSystemCommandHadler") == null))
			{
				ActiveSystemHandleAttbute[] array = (ActiveSystemHandleAttbute[])type.GetCustomAttributes(typeof(ActiveSystemHandleAttbute), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as IActiveSystemCommandHadler);
				}
			}
		}
		return num;
	}

	protected void RegisterCommandHandler(int code, IActiveSystemCommandHadler handle)
	{
		handles.Add(code, handle);
	}
}
