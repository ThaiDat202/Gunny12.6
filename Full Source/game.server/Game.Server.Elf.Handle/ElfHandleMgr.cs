using System;
using System.Collections.Generic;
using System.Reflection;

namespace Game.Server.Elf.Handle;

public class ElfHandleMgr
{
	private Dictionary<int, IElfCommandHandler> handles = new Dictionary<int, IElfCommandHandler>();

	public IElfCommandHandler LoadCommandHandler(int code)
	{
		return handles[code];
	}

	public ElfHandleMgr()
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
			if (type.IsClass && !(type.GetInterface("Game.Server.Elf.Handle.IElfCommandHandler") == null))
			{
				ElfHandleAttribute[] array = (ElfHandleAttribute[])type.GetCustomAttributes(typeof(ElfHandleAttribute), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as IElfCommandHandler);
				}
			}
		}
		return num;
	}

	protected void RegisterCommandHandler(int code, IElfCommandHandler handle)
	{
		handles.Add(code, handle);
	}
}
