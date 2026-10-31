using System;
using System.Collections.Generic;
using System.Reflection;

namespace Game.Server.Team.Handle;

public class TeamHandleMgr
{
	private Dictionary<int, ITeamCommandHandler> handles = new Dictionary<int, ITeamCommandHandler>();

	public ITeamCommandHandler LoadCommandHandler(int code)
	{
		return handles[code];
	}

	public TeamHandleMgr()
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
			if (type.IsClass && !(type.GetInterface("Game.Server.Team.Handle.ITeamCommandHandler") == null))
			{
				TeamHandleAttribute[] array = (TeamHandleAttribute[])type.GetCustomAttributes(typeof(TeamHandleAttribute), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as ITeamCommandHandler);
				}
			}
		}
		return num;
	}

	protected void RegisterCommandHandler(int code, ITeamCommandHandler handle)
	{
		handles.Add(code, handle);
	}
}
