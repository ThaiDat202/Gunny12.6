using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;

namespace Game.Server.HomeSystem.Handle;

public class HomeSystemHandleMgr
{
	private Dictionary<int, IHomeSystemCommandHadler> handles = new Dictionary<int, IHomeSystemCommandHadler>();

	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public IHomeSystemCommandHadler LoadCommandHandler(int code)
	{
		if (handles.ContainsKey(code))
		{
			return handles[code];
		}
		log.Error("LoadCommandHandler code011: " + code);
		return null;
	}

	public HomeSystemHandleMgr()
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
			if (type.IsClass && !(type.GetInterface("Game.Server.HomeSystem.Handle.IHomeSystemCommandHadler") == null))
			{
				HomeSystemHandleAttbute[] array = (HomeSystemHandleAttbute[])type.GetCustomAttributes(typeof(HomeSystemHandleAttbute), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as IHomeSystemCommandHadler);
				}
			}
		}
		return num;
	}

	protected void RegisterCommandHandler(int code, IHomeSystemCommandHadler handle)
	{
		handles.Add(code, handle);
	}
}
