using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;

namespace Game.Server.NewHall.Handle;

public class NewHallHandleMgr
{
	private Dictionary<int, INewHallCommandHadler> handles = new Dictionary<int, INewHallCommandHadler>();

	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public INewHallCommandHadler LoadCommandHandler(int code)
	{
		if (handles.ContainsKey(code))
		{
			return handles[code];
		}
		log.Error("LoadCommandHandler code022: " + code);
		return null;
	}

	public NewHallHandleMgr()
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
			if (type.IsClass && !(type.GetInterface("Game.Server.NewHall.Handle.INewHallCommandHadler") == null))
			{
				NewHallHandleAttbute[] array = (NewHallHandleAttbute[])type.GetCustomAttributes(typeof(NewHallHandleAttbute), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as INewHallCommandHadler);
				}
			}
		}
		return num;
	}

	protected void RegisterCommandHandler(int code, INewHallCommandHadler handle)
	{
		handles.Add(code, handle);
	}
}
