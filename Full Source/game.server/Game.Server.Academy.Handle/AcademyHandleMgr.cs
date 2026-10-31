using System;
using System.Collections.Generic;
using System.Reflection;

namespace Game.Server.Academy.Handle;

public class AcademyHandleMgr
{
	private Dictionary<int, IAcademyCommandHandler> handles = new Dictionary<int, IAcademyCommandHandler>();

	public IAcademyCommandHandler LoadCommandHandler(int code)
	{
		return handles[code];
	}

	public AcademyHandleMgr()
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
			if (type.IsClass && !(type.GetInterface("Game.Server.Academy.Handle.IAcademyCommandHandler") == null))
			{
				AcademyHandleAttribute[] array = (AcademyHandleAttribute[])type.GetCustomAttributes(typeof(AcademyHandleAttribute), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as IAcademyCommandHandler);
				}
			}
		}
		return num;
	}

	protected void RegisterCommandHandler(int code, IAcademyCommandHandler handle)
	{
		handles.Add(code, handle);
	}
}
