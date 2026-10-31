using System;
using System.Collections.Generic;
using System.Reflection;

namespace Game.Server.Labyrinth.Handle;

public class LabyrinthHandleMgr
{
	private Dictionary<int, ILabyrinthCommandHandler> handles = new Dictionary<int, ILabyrinthCommandHandler>();

	public ILabyrinthCommandHandler LoadCommandHandler(int code)
	{
		return handles[code];
	}

	public LabyrinthHandleMgr()
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
			if (type.IsClass && !(type.GetInterface("Game.Server.Labyrinth.Handle.ILabyrinthCommandHandler") == null))
			{
				LabyrinthHandleAttribute[] array = (LabyrinthHandleAttribute[])type.GetCustomAttributes(typeof(LabyrinthHandleAttribute), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as ILabyrinthCommandHandler);
				}
			}
		}
		return num;
	}

	protected void RegisterCommandHandler(int code, ILabyrinthCommandHandler handle)
	{
		handles.Add(code, handle);
	}
}
