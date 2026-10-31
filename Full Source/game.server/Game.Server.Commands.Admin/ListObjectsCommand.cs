using System.Collections.Generic;
using Game.Base;
using Game.Logic;
using Game.Server.Battle;
using Game.Server.GameObjects;
using Game.Server.Games;
using Game.Server.Managers;
using Game.Server.Rooms;

namespace Game.Server.Commands.Admin;

[Cmd("&list", ePrivLevel.Player, "   服务器信息大全.", new string[] { "       /list -g :  列举所有game对象", "       /list -c :  列举所有客户端", "       /list -p :  列举所有玩家对象", "       /list -r :  列举所有房间", "       /list -b :  列举所有 battle server", "       /list -rate:查询所有倍率", "       /list -permission [playerId]: 根据playerid查询该player的关卡信息" })]
public class ListObjectsCommand : AbstractCommandHandler, ICommandHandler
{
	public bool OnCommand(BaseClient client, string[] args)
	{
		if (args.Length <= 1)
		{
			DisplaySyntax(client);
		}
		else
		{
			switch (args[1])
			{
			case "-c":
			{
				DisplayMessage(client, "client list:");
				DisplayMessage(client, "-------------------------------");
				GameClient[] allClients = GameServer.Instance.GetAllClients();
				GameClient[] array = allClients;
				foreach (GameClient gameClient in array)
				{
					DisplayMessage(client, gameClient.ToString());
				}
				DisplayMessage(client, "-------------------------------");
				DisplayMessage(client, "total:{0}", allClients.Length);
				break;
			}
			case "-p":
			{
				DisplayMessage(client, "player list:");
				DisplayMessage(client, "-------------------------------");
				GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
				GamePlayer[] array2 = allPlayers;
				foreach (GamePlayer gamePlayer in array2)
				{
					DisplayMessage(client, gamePlayer.ToString());
				}
				DisplayMessage(client, "-------------------------------");
				DisplayMessage(client, "total:{0}", allPlayers.Length);
				break;
			}
			case "-r":
			{
				DisplayMessage(client, "room list:");
				DisplayMessage(client, "-------------------------------");
				List<BaseRoom> allUsingRoom = RoomMgr.GetAllUsingRoom();
				foreach (BaseRoom item in allUsingRoom)
				{
					DisplayMessage(client, item.ToString());
				}
				DisplayMessage(client, "-------------------------------");
				DisplayMessage(client, "total:{0}", allUsingRoom.Count);
				break;
			}
			case "-g":
			{
				int num = 0;
				int num2 = 0;
				DisplayMessage(client, "game list:");
				DisplayMessage(client, "-------------------------------");
				List<BaseGame> allGame = GameMgr.GetAllGame();
				foreach (BaseGame item2 in allGame)
				{
					DisplayMessage(client, item2.ToString());
					if (item2 is PVEGame)
					{
						num++;
					}
					else
					{
						num2++;
					}
				}
				DisplayMessage(client, "-------------------------------");
				DisplayMessage(client, "total:{0}", allGame.Count);
				DisplayMessage(client, "pvecount:{0}", num);
				DisplayMessage(client, "pvpcount:{0}", num2);
				break;
			}
			case "-b":
			{
				DisplayMessage(client, "battle list:");
				DisplayMessage(client, "-------------------------------");
				List<BattleServer> allBattles = BattleMgr.GetAllBattles();
				foreach (BattleServer item3 in allBattles)
				{
					DisplayMessage(client, item3.ToString());
				}
				DisplayMessage(client, "-------------------------------");
				DisplayMessage(client, "total:{0}", allBattles.Count);
				break;
			}
			default:
				DisplaySyntax(client);
				break;
			}
		}
		return true;
	}
}
