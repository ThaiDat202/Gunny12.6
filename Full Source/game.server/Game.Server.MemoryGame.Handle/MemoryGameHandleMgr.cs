using System;
using System.Collections.Generic;
using System.Reflection;

namespace Game.Server.MemoryGame.Handle;

public class MemoryGameHandleMgr
{
	private Dictionary<int, IMemoryGameCommandHadler> handles = new Dictionary<int, IMemoryGameCommandHadler>();

	public IMemoryGameCommandHadler LoadCommandHandler(int code)
	{
		return handles[code];
	}

	public MemoryGameHandleMgr()
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
			if (type.IsClass && !(type.GetInterface("Game.Server.MemoryGame.Handle.IMemoryGameCommandHadler") == null))
			{
				MemoryGameHandleAttbute[] array = (MemoryGameHandleAttbute[])type.GetCustomAttributes(typeof(MemoryGameHandleAttbute), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as IMemoryGameCommandHadler);
				}
			}
		}
		return num;
	}

	protected void RegisterCommandHandler(int code, IMemoryGameCommandHadler handle)
	{
		handles.Add(code, handle);
	}
}
