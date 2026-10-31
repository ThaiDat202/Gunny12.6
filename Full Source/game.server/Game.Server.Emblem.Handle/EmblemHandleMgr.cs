using System;
using System.Collections.Generic;
using System.Reflection;

namespace Game.Server.Emblem.Handle;

public class EmblemHandleMgr
{
	private Dictionary<int, IEmblemCommandHandler> handles = new Dictionary<int, IEmblemCommandHandler>();

	public IEmblemCommandHandler LoadCommandHandler(int code)
	{
		return handles[code];
	}

	public EmblemHandleMgr()
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
			if (type.IsClass && !(type.GetInterface("Game.Server.Emblem.Handle.IEmblemCommandHandler") == null))
			{
				EmblemHandleAttribute[] array = (EmblemHandleAttribute[])type.GetCustomAttributes(typeof(EmblemHandleAttribute), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as IEmblemCommandHandler);
				}
			}
		}
		return num;
	}

	protected void RegisterCommandHandler(int code, IEmblemCommandHandler handle)
	{
		handles.Add(code, handle);
	}
}
