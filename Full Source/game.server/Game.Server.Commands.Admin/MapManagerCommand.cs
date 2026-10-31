using System;
using Game.Base;

namespace Game.Server.Commands.Admin;

[Cmd("&map", ePrivLevel.Player, "   地图信息相关查询.", new string[] { "       /map -list            : 列举所有 map 对象池信息 ", "       /map -new             : 列举所有 新手玩家 使用地图", "       /map -old             : 列举所有 老手玩家 使用地图", "       /map -mapid [mapid]   : 根据 mapid 查询对应的对象池信息", "       /map -mapobj [mapid]  : 根据 mapid 查询该地图信息" })]
public class MapManagerCommand : AbstractCommandHandler, ICommandHandler
{
	private BaseClient client = null;

	private void ListAllMap()
	{
	}

	private void ListMapById(string mapId)
	{
	}

	private void ListMapObjects(string mapId)
	{
	}

	public bool OnCommand(BaseClient client, string[] args)
	{
		this.client = client;
		if (args.Length <= 1)
		{
			DisplaySyntax(client);
		}
		else
		{
			string text = args[1];
			if (text == null)
			{
				goto IL_00e4;
			}
			switch (text)
			{
			case "-list":
				break;
			case "-clear":
				goto IL_008c;
			case "-mapobj":
				goto IL_0098;
			case "-mapid":
				goto IL_00bf;
			default:
				goto IL_00e4;
			}
			ListAllMap();
		}
		goto IL_00ef;
		IL_00ef:
		return true;
		IL_0098:
		if (args.Length > 2)
		{
			ListMapObjects(args[2]);
		}
		else
		{
			DisplaySyntax(client);
		}
		goto IL_00ef;
		IL_00e4:
		DisplaySyntax(client);
		goto IL_00ef;
		IL_00bf:
		if (args.Length > 2)
		{
			ListMapById(args[2]);
		}
		else
		{
			DisplaySyntax(client);
		}
		goto IL_00ef;
		IL_008c:
		Console.Clear();
		goto IL_00ef;
	}
}
