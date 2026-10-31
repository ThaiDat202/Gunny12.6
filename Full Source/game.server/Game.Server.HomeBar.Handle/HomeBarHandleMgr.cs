using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;

namespace Game.Server.HomeBar.Handle;

public class HomeBarHandleMgr
{
	private Dictionary<int, IHomeBarCommandHadler> handles = new Dictionary<int, IHomeBarCommandHadler>();

	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public IHomeBarCommandHadler LoadCommandHandler(int code)
	{
		if (handles.ContainsKey(code))
		{
			return handles[code];
		}
		log.Error("LoadCommandHandler code013: " + code);
		return null;
	}

	public HomeBarHandleMgr()
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
			if (type.IsClass && !(type.GetInterface("Game.Server.HomeBar.Handle.IHomeBarCommandHadler") == null))
			{
				HomeBarHandleAttbute[] array = (HomeBarHandleAttbute[])type.GetCustomAttributes(typeof(HomeBarHandleAttbute), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as IHomeBarCommandHadler);
				}
			}
		}
		return num;
	}

	protected void RegisterCommandHandler(int code, IHomeBarCommandHadler handle)
	{
		handles.Add(code, handle);
	}
}
