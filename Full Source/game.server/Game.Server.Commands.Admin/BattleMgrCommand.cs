using Game.Base;
using Game.Server.Battle;

namespace Game.Server.Commands.Admin;

[Cmd("&fs", ePrivLevel.Player, "   战斗服务器相关操作", new string[] { "       /fs -list                       : 列举所有FightServer信息", "       /fs -connect id ip port key     : 连接战斗服务器", "       /fs -disconnect ip port         : 断开战斗服务器连接", "       /fs -remove id                  : 移除战斗服务器" })]
public class BattleMgrCommand : AbstractCommandHandler, ICommandHandler
{
	public bool OnCommand(BaseClient client, string[] args)
	{
		bool flag;
		if (args.Length > 1)
		{
			string text = args[1];
			string text2 = text;
			if (text2 != null)
			{
				switch (text2)
				{
				case "-list":
					foreach (BattleServer allBattle in BattleMgr.GetAllBattles())
					{
						DisplayMessage(client, allBattle.ToString());
					}
					DisplayMessage(client, "---------------------------");
					DisplayMessage(client, $"TotalFightServerCount : {BattleMgr.GetAllBattles().Count}");
					flag = true;
					return true;
				default:
					if (text == "-remove" && args.Length == 3)
					{
						BattleServer server = BattleMgr.GetServer(int.Parse(args[2]));
						if (server.IsActive)
						{
							DisplayMessage(client, "Server is active,please disconnect it first!");
						}
						else
						{
							BattleMgr.RemoveServer(server);
						}
						flag = true;
						return true;
					}
					break;
				case "-disconnect":
					if (args.Length == 3)
					{
						BattleMgr.Disconnet(int.Parse(args[2]));
						DisplayMessage(client, "server disconnected!");
						flag = true;
						return true;
					}
					break;
				case "-connect":
					if (args.Length == 6)
					{
						BattleMgr.ConnectTo(int.Parse(args[2]), args[3], int.Parse(args[4]), args[5]);
						DisplayMessage(client, "connect completed!");
						flag = true;
						return true;
					}
					break;
				}
			}
		}
		DisplaySyntax(client);
		flag = true;
		return true;
	}
}
