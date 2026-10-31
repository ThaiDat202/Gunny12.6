using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;

namespace Game.Server.HomeFight.Handle;

public class HomeFightHandleMgr
{
	private Dictionary<int, IHomeFightCommandHadler> handles = new Dictionary<int, IHomeFightCommandHadler>();

	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public IHomeFightCommandHadler LoadCommandHandler(int code)
	{
		if (handles.ContainsKey(code))
		{
			return handles[code];
		}
		log.Error("LoadCommandHandler code014: " + code);
		return null;
	}

	public HomeFightHandleMgr()
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
			if (type.IsClass && !(type.GetInterface("Game.Server.HomeFight.Handle.IHomeFightCommandHadler") == null))
			{
				HomeFightHandleAttbute[] array = (HomeFightHandleAttbute[])type.GetCustomAttributes(typeof(HomeFightHandleAttbute), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as IHomeFightCommandHadler);
				}
			}
		}
		return num;
	}

	protected void RegisterCommandHandler(int code, IHomeFightCommandHadler handle)
	{
		handles.Add(code, handle);
	}
}
