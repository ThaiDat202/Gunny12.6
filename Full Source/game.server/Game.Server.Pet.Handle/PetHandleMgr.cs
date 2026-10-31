using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;

namespace Game.Server.Pet.Handle;

public class PetHandleMgr
{
	private Dictionary<int, IPetCommandHadler> handles = new Dictionary<int, IPetCommandHadler>();

	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public IPetCommandHadler LoadCommandHandler(int code)
	{
		if (handles.ContainsKey(code))
		{
			return handles[code];
		}
		log.Error("LoadCommandHandler code024: " + code);
		return null;
	}

	public PetHandleMgr()
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
			if (type.IsClass && !(type.GetInterface("Game.Server.Pet.Handle.IPetCommandHadler") == null))
			{
				PetHandleAttbute[] array = (PetHandleAttbute[])type.GetCustomAttributes(typeof(PetHandleAttbute), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as IPetCommandHadler);
				}
			}
		}
		return num;
	}

	protected void RegisterCommandHandler(int code, IPetCommandHadler handle)
	{
		handles.Add(code, handle);
	}
}
