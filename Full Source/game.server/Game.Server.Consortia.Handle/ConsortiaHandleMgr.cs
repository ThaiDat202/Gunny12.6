using System;
using System.Collections.Generic;
using System.Reflection;

namespace Game.Server.Consortia.Handle;

public class ConsortiaHandleMgr
{
	private Dictionary<int, IConsortiaCommandHadler> handles = new Dictionary<int, IConsortiaCommandHadler>();

	public IConsortiaCommandHadler LoadCommandHandler(int code)
	{
		return handles[code];
	}

	public ConsortiaHandleMgr()
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
			if (type.IsClass && !(type.GetInterface("Game.Server.Consortia.Handle.IConsortiaCommandHadler") == null))
			{
				ConsortiaHandleAttbute[] array = (ConsortiaHandleAttbute[])type.GetCustomAttributes(typeof(ConsortiaHandleAttbute), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as IConsortiaCommandHadler);
				}
			}
		}
		return num;
	}

	protected void RegisterCommandHandler(int code, IConsortiaCommandHadler handle)
	{
		handles.Add(code, handle);
	}
}
