using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;

namespace Game.Server.HomeAdorn.Handle;

public class HomeAdornHandleMgr
{
	private Dictionary<int, IHomeAdornCommandHadler> handles = new Dictionary<int, IHomeAdornCommandHadler>();

	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public IHomeAdornCommandHadler LoadCommandHandler(int code)
	{
		if (handles.ContainsKey(code))
		{
			return handles[code];
		}
		log.Error("LoadCommandHandler code012: " + code);
		return null;
	}

	public HomeAdornHandleMgr()
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
			if (type.IsClass && !(type.GetInterface("Game.Server.HomeAdorn.Handle.IHomeAdornCommandHadler") == null))
			{
				HomeAdornHandleAttbute[] array = (HomeAdornHandleAttbute[])type.GetCustomAttributes(typeof(HomeAdornHandleAttbute), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as IHomeAdornCommandHadler);
				}
			}
		}
		return num;
	}

	protected void RegisterCommandHandler(int code, IHomeAdornCommandHadler handle)
	{
		handles.Add(code, handle);
	}
}
