using System;
using System.Collections.Generic;
using System.Reflection;

namespace Game.Server.CardNewSystem.Handle;

public class CardNewSystemHandleMgr
{
	private Dictionary<int, ICardNewSystemCommandHandler> handles = new Dictionary<int, ICardNewSystemCommandHandler>();

	public ICardNewSystemCommandHandler LoadCommandHandler(int code)
	{
		return handles[code];
	}

	public CardNewSystemHandleMgr()
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
			if (type.IsClass && !(type.GetInterface("Game.Server.CardNewSystem.Handle.ICardNewSystemCommandHandler") == null))
			{
				CardNewSystemHandle[] array = (CardNewSystemHandle[])type.GetCustomAttributes(typeof(CardNewSystemHandle), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as ICardNewSystemCommandHandler);
				}
			}
		}
		return num;
	}

	protected void RegisterCommandHandler(int code, ICardNewSystemCommandHandler handle)
	{
		handles.Add(code, handle);
	}
}
