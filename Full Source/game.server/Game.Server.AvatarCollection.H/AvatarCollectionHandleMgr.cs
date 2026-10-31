using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;

namespace Game.Server.AvatarCollection.Handle;

public class AvatarCollectionHandleMgr
{
	private Dictionary<int, IAvatarCollectionCommandHadler> handles = new Dictionary<int, IAvatarCollectionCommandHadler>();

	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public IAvatarCollectionCommandHadler LoadCommandHandler(int code)
	{
		if (handles.ContainsKey(code))
		{
			return handles[code];
		}
		log.Error("LoadCommandHandler code004: " + code);
		return null;
	}

	public AvatarCollectionHandleMgr()
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
			if (type.IsClass && !(type.GetInterface("Game.Server.AvatarCollection.Handle.IAvatarCollectionCommandHadler") == null))
			{
				AvatarCollectionHandleAttbute[] array = (AvatarCollectionHandleAttbute[])type.GetCustomAttributes(typeof(AvatarCollectionHandleAttbute), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as IAvatarCollectionCommandHadler);
				}
			}
		}
		return num;
	}

	protected void RegisterCommandHandler(int code, IAvatarCollectionCommandHadler handle)
	{
		handles.Add(code, handle);
	}
}
