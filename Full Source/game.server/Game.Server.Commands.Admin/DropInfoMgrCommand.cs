using Game.Base;
using Game.Logic;

namespace Game.Server.Commands.Admin;

[Cmd("&drop", ePrivLevel.Player, "   物品掉落相关查询", new string[] { "       /drop -list             :   列举宏观掉落信息.", "       /drop -dropid [dropid]  :   根据ID查询宏观掉落详细信息." })]
public class DropInfoMgrCommand : AbstractCommandHandler, ICommandHandler
{
	public bool OnCommand(BaseClient client, string[] args)
	{
		bool flag;
		if (args.Length <= 1)
		{
			DisplaySyntax(client);
		}
		else
		{
			string text = args[1];
			if (text != null)
			{
				if (text == "-list")
				{
					if (DropInfoMgr.DropInfo.Count == 0)
					{
						DisplayMessage(client, "There is no data!");
					}
					else
					{
						foreach (int key2 in DropInfoMgr.DropInfo.Keys)
						{
							DisplayMessage(client, " " + DropInfoMgr.DropInfo[key2]);
						}
					}
					flag = true;
					return true;
				}
				if (text == "-dropid")
				{
					if (args.Length <= 2)
					{
						DisplaySyntax(client);
					}
					else
					{
						int key;
						try
						{
							key = int.Parse(args[2]);
						}
						catch
						{
							DisplayMessage(client, " You Enter DropId Type Error,Please Enter Int Type！");
							flag = true;
							return true;
						}
						if (!DropInfoMgr.DropInfo.ContainsKey(key))
						{
							DisplayMessage(client, " You Enter DropId Not Exist！");
						}
						else
						{
							DisplayMessage(client, " " + DropInfoMgr.DropInfo[key]);
						}
					}
					flag = true;
					return true;
				}
			}
			DisplaySyntax(client);
		}
		flag = true;
		return true;
	}
}
