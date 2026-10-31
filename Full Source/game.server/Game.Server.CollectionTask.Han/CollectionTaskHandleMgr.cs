using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;

namespace Game.Server.CollectionTask.Handle;

public class CollectionTaskHandleMgr
{
	private Dictionary<int, ICollectionTaskCommandHadler> handles = new Dictionary<int, ICollectionTaskCommandHadler>();

	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public ICollectionTaskCommandHadler LoadCommandHandler(int code)
	{
		if (handles.ContainsKey(code))
		{
			return handles[code];
		}
		log.Error("LoadCommandHandler code006: " + code);
		return null;
	}

	public CollectionTaskHandleMgr()
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
			if (type.IsClass && !(type.GetInterface("Game.Server.CollectionTask.Handle.ICollectionTaskCommandHadler") == null))
			{
				CollectionTaskHandleAttbute[] array = (CollectionTaskHandleAttbute[])type.GetCustomAttributes(typeof(CollectionTaskHandleAttbute), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as ICollectionTaskCommandHadler);
				}
			}
		}
		return num;
	}

	protected void RegisterCommandHandler(int code, ICollectionTaskCommandHadler handle)
	{
		handles.Add(code, handle);
	}
}
