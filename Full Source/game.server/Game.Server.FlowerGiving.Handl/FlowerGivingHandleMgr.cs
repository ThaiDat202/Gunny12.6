using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;

namespace Game.Server.FlowerGiving.Handle;

public class FlowerGivingHandleMgr
{
	private Dictionary<int, IFlowerGivingCommandHadler> handles = new Dictionary<int, IFlowerGivingCommandHadler>();

	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public IFlowerGivingCommandHadler LoadCommandHandler(int code)
	{
		if (handles.ContainsKey(code))
		{
			return handles[code];
		}
		log.Error("LoadCommandHandler code008: " + code);
		return null;
	}

	public FlowerGivingHandleMgr()
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
			if (type.IsClass && !(type.GetInterface("Game.Server.FlowerGiving.Handle.IFlowerGivingCommandHadler") == null))
			{
				FlowerGivingHandleAttbute[] array = (FlowerGivingHandleAttbute[])type.GetCustomAttributes(typeof(FlowerGivingHandleAttbute), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as IFlowerGivingCommandHadler);
				}
			}
		}
		return num;
	}

	protected void RegisterCommandHandler(int code, IFlowerGivingCommandHadler handle)
	{
		handles.Add(code, handle);
	}
}
