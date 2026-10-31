using System;
using System.Collections.Generic;
using System.Reflection;

namespace Game.Server.ConsortiaTask.Handle;

public class ConsortiaTaskHandleMgr
{
	private Dictionary<int, IConsortiaTaskCommandHandler> handles = new Dictionary<int, IConsortiaTaskCommandHandler>();

	public IConsortiaTaskCommandHandler LoadCommandHandler(int code)
	{
		return handles[code];
	}

	public ConsortiaTaskHandleMgr()
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
			if (type.IsClass && !(type.GetInterface("Game.Server.ConsortiaTask.Handle.IConsortiaTaskCommandHandler") == null))
			{
				ConsortiaTaskHandleAttribute[] array = (ConsortiaTaskHandleAttribute[])type.GetCustomAttributes(typeof(ConsortiaTaskHandleAttribute), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as IConsortiaTaskCommandHandler);
				}
			}
		}
		return num;
	}

	protected void RegisterCommandHandler(int code, IConsortiaTaskCommandHandler handle)
	{
		handles.Add(code, handle);
	}
}
