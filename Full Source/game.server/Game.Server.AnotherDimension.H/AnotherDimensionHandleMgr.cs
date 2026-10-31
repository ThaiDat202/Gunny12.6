using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;

namespace Game.Server.AnotherDimension.Handle;

public class AnotherDimensionHandleMgr
{
	private Dictionary<int, IAnotherDimensionCommandHadler> handles = new Dictionary<int, IAnotherDimensionCommandHadler>();

	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public IAnotherDimensionCommandHadler LoadCommandHandler(int code)
	{
		if (handles.ContainsKey(code))
		{
			return handles[code];
		}
		log.Error("LoadCommandHandler code003: " + code);
		return null;
	}

	public AnotherDimensionHandleMgr()
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
			if (type.IsClass && !(type.GetInterface("Game.Server.AnotherDimension.Handle.IAnotherDimensionCommandHadler") == null))
			{
				AnotherDimensionHandleAttbute[] array = (AnotherDimensionHandleAttbute[])type.GetCustomAttributes(typeof(AnotherDimensionHandleAttbute), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as IAnotherDimensionCommandHadler);
				}
			}
		}
		return num;
	}

	protected void RegisterCommandHandler(int code, IAnotherDimensionCommandHadler handle)
	{
		handles.Add(code, handle);
	}
}
