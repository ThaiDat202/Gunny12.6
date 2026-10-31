using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;

namespace Game.Server.SceneMarryRooms.TankHandle;

public class MarryCommandMgr
{
	private Dictionary<int, IMarryCommandHandler> handles = new Dictionary<int, IMarryCommandHandler>();

	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public IMarryCommandHandler LoadCommandHandler(int code)
	{
		if (handles.ContainsKey(code))
		{
			return handles[code];
		}
		log.Error("LoadCommandHandler code027: " + code);
		return null;
	}

	public MarryCommandMgr()
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
			if (type.IsClass && !(type.GetInterface("Game.Server.SceneMarryRooms.TankHandle.IMarryCommandHandler") == null))
			{
				MarryCommandAttbute[] array = (MarryCommandAttbute[])type.GetCustomAttributes(typeof(MarryCommandAttbute), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as IMarryCommandHandler);
				}
			}
		}
		return num;
	}

	protected void RegisterCommandHandler(int code, IMarryCommandHandler handle)
	{
		handles.Add(code, handle);
	}
}
