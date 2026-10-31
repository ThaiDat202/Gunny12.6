using System;
using System.Collections.Generic;
using System.Reflection;

namespace Game.Server.DevilTurn.Handle;

public class DevilTurnHandleMgr
{
	private Dictionary<int, IDevilTurnCommandHandler> handles = new Dictionary<int, IDevilTurnCommandHandler>();

	public IDevilTurnCommandHandler LoadCommandHandler(int code)
	{
		return handles[code];
	}

	public DevilTurnHandleMgr()
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
			if (type.IsClass && !(type.GetInterface("Game.Server.DevilTurn.Handle.IDevilTurnCommandHandler") == null))
			{
				DevilTurnHandleAttribute[] array = (DevilTurnHandleAttribute[])type.GetCustomAttributes(typeof(DevilTurnHandleAttribute), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as IDevilTurnCommandHandler);
				}
			}
		}
		return num;
	}

	protected void RegisterCommandHandler(int code, IDevilTurnCommandHandler handle)
	{
		handles.Add(code, handle);
	}
}
