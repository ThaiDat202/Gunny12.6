using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;

namespace Game.Server.Horse.Handle;

public class HorseHandleMgr
{
	private Dictionary<int, IHorseCommandHadler> handles = new Dictionary<int, IHorseCommandHadler>();

	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public IHorseCommandHadler LoadCommandHandler(int code)
	{
		if (handles.ContainsKey(code))
		{
			return handles[code];
		}
		log.Error("LoadCommandHandler code017: " + code);
		return null;
	}

	public HorseHandleMgr()
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
			if (type.IsClass && !(type.GetInterface("Game.Server.Horse.Handle.IHorseCommandHadler") == null))
			{
				HorseHandleAttbute[] array = (HorseHandleAttbute[])type.GetCustomAttributes(typeof(HorseHandleAttbute), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as IHorseCommandHadler);
				}
			}
		}
		return num;
	}

	protected void RegisterCommandHandler(int code, IHorseCommandHadler handle)
	{
		handles.Add(code, handle);
	}
}
