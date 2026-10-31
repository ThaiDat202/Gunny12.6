using System;
using System.Collections.Generic;
using System.Reflection;

namespace Game.Server.SanXiao.Handle;

public class SanXiaoHandleMgr
{
	private Dictionary<int, ISanXiaoCommandHadler> handles = new Dictionary<int, ISanXiaoCommandHadler>();

	public ISanXiaoCommandHadler LoadCommandHandler(int code)
	{
		return handles[code];
	}

	public SanXiaoHandleMgr()
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
			if (type.IsClass && !(type.GetInterface("Game.Server.SanXiao.Handle.ISanXiaoCommandHadler") == null))
			{
				SanXiaoHandleAttbute[] array = (SanXiaoHandleAttbute[])type.GetCustomAttributes(typeof(SanXiaoHandleAttbute), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as ISanXiaoCommandHadler);
				}
			}
		}
		return num;
	}

	protected void RegisterCommandHandler(int code, ISanXiaoCommandHadler handle)
	{
		handles.Add(code, handle);
	}
}
