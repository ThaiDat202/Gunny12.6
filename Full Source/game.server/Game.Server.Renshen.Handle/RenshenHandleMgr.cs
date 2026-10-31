using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;

namespace Game.Server.Renshen.Handle;

public class RenshenHandleMgr
{
	private Dictionary<int, IRenshenCommandHadler> handles = new Dictionary<int, IRenshenCommandHadler>();

	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public IRenshenCommandHadler LoadCommandHandler(int code)
	{
		if (handles.ContainsKey(code))
		{
			return handles[code];
		}
		log.Error("LoadCommandHandler code016: " + code);
		return null;
	}

	public RenshenHandleMgr()
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
			if (type.IsClass && !(type.GetInterface("Game.Server.Renshen.Handle.IRenshenCommandHadler") == null))
			{
				RenshenHandleAttbute[] array = (RenshenHandleAttbute[])type.GetCustomAttributes(typeof(RenshenHandleAttbute), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as IRenshenCommandHadler);
				}
			}
		}
		return num;
	}

	protected void RegisterCommandHandler(int code, IRenshenCommandHadler handle)
	{
		handles.Add(code, handle);
	}
}
