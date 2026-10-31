using System;
using System.Collections.Generic;
using System.Reflection;

namespace Game.Server.HorseAmulet.Handle;

public class HorseAmuletHandleMgr
{
	private Dictionary<int, IHorseAmuletCommandHadler> handles = new Dictionary<int, IHorseAmuletCommandHadler>();

	public IHorseAmuletCommandHadler LoadCommandHandler(int code)
	{
		return handles[code];
	}

	public HorseAmuletHandleMgr()
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
			if (type.IsClass && !(type.GetInterface("Game.Server.HorseAmulet.Handle.IHorseAmuletCommandHadler") == null))
			{
				HorseAmuletHandleAttbute[] array = (HorseAmuletHandleAttbute[])type.GetCustomAttributes(typeof(HorseAmuletHandleAttbute), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as IHorseAmuletCommandHadler);
				}
			}
		}
		return num;
	}

	protected void RegisterCommandHandler(int code, IHorseAmuletCommandHadler handle)
	{
		handles.Add(code, handle);
	}
}
