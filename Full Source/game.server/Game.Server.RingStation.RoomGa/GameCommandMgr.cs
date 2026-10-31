using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;

namespace Game.Server.RingStation.RoomGamePkg.TankHandle;

public class GameCommandMgr
{
	private Dictionary<int, IGameCommandHandler> handles = new Dictionary<int, IGameCommandHandler>();

	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public GameCommandMgr()
	{
		handles.Clear();
		SearchCommandHandlers(Assembly.GetAssembly(typeof(GameServer)));
	}

	public IGameCommandHandler LoadCommandHandler(int code)
	{
		if (handles.ContainsKey(code))
		{
			return handles[code];
		}
		log.Error("LoadCommandHandler code026: " + code);
		return null;
	}

	protected void RegisterCommandHandler(int code, IGameCommandHandler handle)
	{
		handles.Add(code, handle);
	}

	protected int SearchCommandHandlers(Assembly assembly)
	{
		int num = 0;
		Type[] types = assembly.GetTypes();
		foreach (Type type in types)
		{
			if (type.IsClass && type.GetInterface("Game.Server.RingStation.RoomGamePkg.TankHandle.IGameCommandHandler") != null)
			{
				GameCommandAttbute[] array = (GameCommandAttbute[])type.GetCustomAttributes(typeof(GameCommandAttbute), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as IGameCommandHandler);
				}
			}
		}
		return num;
	}
}
