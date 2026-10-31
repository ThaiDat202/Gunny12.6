using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;

namespace Game.Server.MagicHouse.Handle;

public class MagicHouseHandleMgr
{
	private Dictionary<int, IMagicHouseCommandHadler> handles = new Dictionary<int, IMagicHouseCommandHadler>();

	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public IMagicHouseCommandHadler LoadCommandHandler(int code)
	{
		if (handles.ContainsKey(code))
		{
			return handles[code];
		}
		log.Error("LoadCommandHandler code019: " + code);
		return null;
	}

	public MagicHouseHandleMgr()
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
			if (type.IsClass && !(type.GetInterface("Game.Server.MagicHouse.Handle.IMagicHouseCommandHadler") == null))
			{
				MagicHouseHandleAttbute[] array = (MagicHouseHandleAttbute[])type.GetCustomAttributes(typeof(MagicHouseHandleAttbute), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as IMagicHouseCommandHadler);
				}
			}
		}
		return num;
	}

	protected void RegisterCommandHandler(int code, IMagicHouseCommandHadler handle)
	{
		handles.Add(code, handle);
	}
}
