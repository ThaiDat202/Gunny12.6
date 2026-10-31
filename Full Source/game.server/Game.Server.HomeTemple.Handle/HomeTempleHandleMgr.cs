using System;
using System.Collections.Generic;
using System.Reflection;

namespace Game.Server.HomeTemple.Handle;

public class HomeTempleHandleMgr
{
	private Dictionary<int, IHomeTempleCommandHadler> handles = new Dictionary<int, IHomeTempleCommandHadler>();

	public IHomeTempleCommandHadler LoadCommandHandler(int code)
	{
		return handles[code];
	}

	public HomeTempleHandleMgr()
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
			if (type.IsClass && !(type.GetInterface("Game.Server.HomeTemple.Handle.IHomeTempleCommandHadler") == null))
			{
				HomeTempleHandleAttbute[] array = (HomeTempleHandleAttbute[])type.GetCustomAttributes(typeof(HomeTempleHandleAttbute), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as IHomeTempleCommandHadler);
				}
			}
		}
		return num;
	}

	protected void RegisterCommandHandler(int code, IHomeTempleCommandHadler handle)
	{
		handles.Add(code, handle);
	}
}
