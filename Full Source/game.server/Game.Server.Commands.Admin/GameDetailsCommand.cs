using System;
using System.Collections.Generic;
using Game.Base;
using Game.Logic;
using Game.Server.Games;

namespace Game.Server.Commands.Admin;

[Cmd("&game", ePrivLevel.Player, "   Game对象相关查询.", new string[] { "       /game -list           : 列举所有game对象", "       /game -gameid [gameid]: 根据gameid列举该对象所有信息" })]
public class GameDetailsCommand : AbstractCommandHandler, ICommandHandler
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
			List<BaseGame> allGame = GameMgr.GetAllGame();
			string text = args[1];
			string text2 = text;
			if (text2 != null)
			{
				if (!(text2 == "-list"))
				{
					if (text == "-gameid")
					{
						if (args.Length <= 2)
						{
							DisplaySyntax(client);
						}
						else
						{
							int num;
							try
							{
								num = int.Parse(args[2]);
							}
							catch (Exception)
							{
								DisplayMessage(client, "You enter id type error!");
								flag = true;
								return true;
							}
							string name = allGame[num].Map.Info.Name;
							string text3 = allGame[num].PlayerCount.ToString();
							DisplayMessage(client, "-------------------------------------------------------");
							DisplayMessage(client, "Id:{0},PlayerCount:{1}", num, text3);
							if (allGame[num] is PVEGame)
							{
								PVEGame pVEGame = allGame[num] as PVEGame;
								DisplayMessage(client, ToString(pVEGame));
							}
							else
							{
								DisplayMessage(client, allGame[num].ToString());
							}
							DisplayMessage(client, "-------------------------------------------------------");
						}
					}
				}
				else
				{
					int num2 = 0;
					int num3 = 0;
					DisplayMessage(client, "game list:");
					DisplayMessage(client, "-------------------------------");
					foreach (BaseGame item in allGame)
					{
						DisplayMessage(client, item.ToString());
						if (item is PVEGame)
						{
							num2++;
						}
						else
						{
							num3++;
						}
					}
					DisplayMessage(client, "-------------------------------");
					DisplayMessage(client, "total:{0}", allGame.Count);
					DisplayMessage(client, "total:{0}", allGame.Count);
					DisplayMessage(client, "pvecount:{0}", num2);
					DisplayMessage(client, "pvpcount:{0}", num3);
				}
			}
		}
		flag = true;
		return true;
	}

	private string ToString(PVEGame pveGame)
	{
		return $"Type:PVE ,Id:{pveGame.MissionInfo.Id}, Name:{pveGame.MissionInfo.Name},IncrementDelay:{pveGame.MissionInfo.IncrementDelay},Delay:{pveGame.MissionInfo.Delay},Script:{pveGame.MissionInfo.Script}";
	}
}
