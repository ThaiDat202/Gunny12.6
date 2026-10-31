using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;

namespace Game.Server.MoneyTree.Handle;

public class MoneyTreeHandleMgr
{
	private Dictionary<int, IMoneyTreeCommandHadler> handles = new Dictionary<int, IMoneyTreeCommandHadler>();

	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public IMoneyTreeCommandHadler LoadCommandHandler(int code)
	{
		if (handles.ContainsKey(code))
		{
			return handles[code];
		}
		log.Error("LoadCommandHandler code015: " + code);
		return null;
	}

	public MoneyTreeHandleMgr()
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
			if (type.IsClass && !(type.GetInterface("Game.Server.MoneyTree.Handle.IMoneyTreeCommandHadler") == null))
			{
				MoneyTreeHandleAttbute[] array = (MoneyTreeHandleAttbute[])type.GetCustomAttributes(typeof(MoneyTreeHandleAttbute), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as IMoneyTreeCommandHadler);
				}
			}
		}
		return num;
	}

	protected void RegisterCommandHandler(int code, IMoneyTreeCommandHadler handle)
	{
		handles.Add(code, handle);
	}
}
