using System;
using System.Collections.Generic;
using System.Reflection;

namespace Game.Server.GuardCore.Handle;

public class GuardCoreHandleMgr
{
	private Dictionary<int, IGuardCoreCommandHadler> handles = new Dictionary<int, IGuardCoreCommandHadler>();

	public IGuardCoreCommandHadler LoadCommandHandler(int code)
	{
		return handles[code];
	}

	public GuardCoreHandleMgr()
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
			if (type.IsClass && !(type.GetInterface("Game.Server.GuardCore.Handle.IGuardCoreCommandHadler") == null))
			{
				GuardCoreHandleAttbute[] array = (GuardCoreHandleAttbute[])type.GetCustomAttributes(typeof(GuardCoreHandleAttbute), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as IGuardCoreCommandHadler);
				}
			}
		}
		return num;
	}

	protected void RegisterCommandHandler(int code, IGuardCoreCommandHadler handle)
	{
		handles.Add(code, handle);
	}
}
