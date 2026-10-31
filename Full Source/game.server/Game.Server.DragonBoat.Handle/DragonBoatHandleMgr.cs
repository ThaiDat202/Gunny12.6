using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;

namespace Game.Server.DragonBoat.Handle;

public class DragonBoatHandleMgr
{
	private Dictionary<int, IDragonBoatCommandHadler> handles = new Dictionary<int, IDragonBoatCommandHadler>();

	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public IDragonBoatCommandHadler LoadCommandHandler(int code)
	{
		if (handles.ContainsKey(code))
		{
			return handles[code];
		}
		log.Error("LoadCommandHandler code007: " + code);
		return null;
	}

	public DragonBoatHandleMgr()
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
			if (type.IsClass && !(type.GetInterface("Game.Server.DragonBoat.Handle.IDragonBoatCommandHadler") == null))
			{
				DragonBoatHandleAttbute[] array = (DragonBoatHandleAttbute[])type.GetCustomAttributes(typeof(DragonBoatHandleAttbute), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as IDragonBoatCommandHadler);
				}
			}
		}
		return num;
	}

	protected void RegisterCommandHandler(int code, IDragonBoatCommandHadler handle)
	{
		handles.Add(code, handle);
	}
}
