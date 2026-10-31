using System;
using System.Collections.Generic;
using System.Reflection;

namespace Game.Server.DDTQiYuan.Handle;

public class QiYuanHandleMgr
{
	private Dictionary<int, IQiYuanCommandHadler> handles = new Dictionary<int, IQiYuanCommandHadler>();

	public IQiYuanCommandHadler LoadCommandHandler(int code)
	{
		return handles[code];
	}

	public QiYuanHandleMgr()
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
			if (type.IsClass && !(type.GetInterface("Game.Server.DDTQiYuan.Handle.IQiYuanCommandHadler") == null))
			{
				QiYuanHandleAttbute[] array = (QiYuanHandleAttbute[])type.GetCustomAttributes(typeof(QiYuanHandleAttbute), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as IQiYuanCommandHadler);
				}
			}
		}
		return num;
	}

	protected void RegisterCommandHandler(int code, IQiYuanCommandHadler handle)
	{
		handles.Add(code, handle);
	}
}
