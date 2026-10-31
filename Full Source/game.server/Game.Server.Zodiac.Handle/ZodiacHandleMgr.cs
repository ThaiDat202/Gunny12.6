using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;

namespace Game.Server.Zodiac.Handle;

public class ZodiacHandleMgr
{
	private Dictionary<int, IZodiacCommandHadler> handles = new Dictionary<int, IZodiacCommandHadler>();

	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public IZodiacCommandHadler LoadCommandHandler(int code)
	{
		if (handles.ContainsKey(code))
		{
			return handles[code];
		}
		log.Error("LoadCommandHandler code029: " + code);
		return null;
	}

	public ZodiacHandleMgr()
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
			if (type.IsClass && !(type.GetInterface("Game.Server.Zodiac.Handle.IZodiacCommandHadler") == null))
			{
				ZodiacHandleAttbute[] array = (ZodiacHandleAttbute[])type.GetCustomAttributes(typeof(ZodiacHandleAttbute), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as IZodiacCommandHadler);
				}
			}
		}
		return num;
	}

	protected void RegisterCommandHandler(int code, IZodiacCommandHadler handle)
	{
		handles.Add(code, handle);
	}
}
