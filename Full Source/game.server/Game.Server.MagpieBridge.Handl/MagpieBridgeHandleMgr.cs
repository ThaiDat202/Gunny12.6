using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;

namespace Game.Server.MagpieBridge.Handle;

public class MagpieBridgeHandleMgr
{
	private Dictionary<int, IMagpieBridgeCommandHadler> handles = new Dictionary<int, IMagpieBridgeCommandHadler>();

	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public IMagpieBridgeCommandHadler LoadCommandHandler(int code)
	{
		if (handles.ContainsKey(code))
		{
			return handles[code];
		}
		log.Error("LoadCommandHandler code021: " + code);
		return null;
	}

	public MagpieBridgeHandleMgr()
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
			if (type.IsClass && !(type.GetInterface("Game.Server.MagpieBridge.Handle.IMagpieBridgeCommandHadler") == null))
			{
				MagpieBridgeHandleAttbute[] array = (MagpieBridgeHandleAttbute[])type.GetCustomAttributes(typeof(MagpieBridgeHandleAttbute), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as IMagpieBridgeCommandHadler);
				}
			}
		}
		return num;
	}

	protected void RegisterCommandHandler(int code, IMagpieBridgeCommandHadler handle)
	{
		handles.Add(code, handle);
	}
}
