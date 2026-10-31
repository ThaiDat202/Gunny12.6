using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;

namespace Game.Server.GypsyShop.Handle;

public class GypsyShopHandleMgr
{
	private Dictionary<int, IGypsyShopCommandHadler> handles = new Dictionary<int, IGypsyShopCommandHadler>();

	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public IGypsyShopCommandHadler LoadCommandHandler(int code)
	{
		if (handles.ContainsKey(code))
		{
			return handles[code];
		}
		log.Error("LoadCommandHandler code010: " + code);
		return null;
	}

	public GypsyShopHandleMgr()
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
			if (type.IsClass && !(type.GetInterface("Game.Server.GypsyShop.Handle.IGypsyShopCommandHadler") == null))
			{
				GypsyShopHandleAttbute[] array = (GypsyShopHandleAttbute[])type.GetCustomAttributes(typeof(GypsyShopHandleAttbute), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as IGypsyShopCommandHadler);
				}
			}
		}
		return num;
	}

	protected void RegisterCommandHandler(int code, IGypsyShopCommandHadler handle)
	{
		handles.Add(code, handle);
	}
}
