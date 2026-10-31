using System;
using System.Collections.Generic;
using System.Reflection;

namespace Game.Server.EquipAmulet.Handle;

public class EquipAmuletHandleMgr
{
	private Dictionary<int, IEquipAmuletCommandHadler> handles = new Dictionary<int, IEquipAmuletCommandHadler>();

	public IEquipAmuletCommandHadler LoadCommandHandler(int code)
	{
		return handles[code];
	}

	public EquipAmuletHandleMgr()
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
			if (type.IsClass && !(type.GetInterface("Game.Server.EquipAmulet.Handle.IEquipAmuletCommandHadler") == null))
			{
				EquipAmuletHandleAttbute[] array = (EquipAmuletHandleAttbute[])type.GetCustomAttributes(typeof(EquipAmuletHandleAttbute), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as IEquipAmuletCommandHadler);
				}
			}
		}
		return num;
	}

	protected void RegisterCommandHandler(int code, IEquipAmuletCommandHadler handle)
	{
		handles.Add(code, handle);
	}
}
