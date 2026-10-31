using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;

namespace Game.Server.BombKing.Handle;

public class BombKingHandleMgr
{
	private Dictionary<int, IBombKingCommandHadler> handles = new Dictionary<int, IBombKingCommandHadler>();

	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public IBombKingCommandHadler LoadCommandHandler(int code)
	{
		if (handles.ContainsKey(code))
		{
			return handles[code];
		}
		log.Error("LoadCommandHandler code005: " + code);
		return null;
	}

	public BombKingHandleMgr()
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
			if (type.IsClass && !(type.GetInterface("Game.Server.BombKing.Handle.IBombKingCommandHadler") == null))
			{
				BombKingHandleAttbute[] array = (BombKingHandleAttbute[])type.GetCustomAttributes(typeof(BombKingHandleAttbute), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as IBombKingCommandHadler);
				}
			}
		}
		return num;
	}

	protected void RegisterCommandHandler(int code, IBombKingCommandHadler handle)
	{
		handles.Add(code, handle);
	}
}
