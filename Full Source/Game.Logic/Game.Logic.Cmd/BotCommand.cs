using System;
using System.Collections.Generic;
using System.Linq;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic.Cmd;

[GameCommand(143, "机器人指令")]
public class BotCommand : ICommandHandler
{
	public enum BotBuffType
	{
		PlusTwoAttack = 10001,
		ThreeInOne = 10002,
		PlusOneAttack = 10003,
		Power50 = 10004,
		Power40 = 10005,
		Power30 = 10006,
		Power20 = 10007,
		Power10 = 10008,
		HealTeam = 10009,
		Invisibility = 10010,
		InvisibilityTeam = 10011,
		Heal = 10012,
		ChangeWind = 10013,
		Fly = 10016,
		AutoAim = 10017,
		Fury = 10018
	}

	public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
	{
		if (!(game is PVPGame))
		{
			return;
		}
		Player[] allPlayers = game.GetAllPlayers();
		List<Player> list = allPlayers.Where((Player child) => child.Team != player.Team).ToList();
		int index = game.Random.Next(0, list.Count);
		Player player2 = list.ElementAt(index);
		if (player2.X > player.X)
		{
			player.ChangeDirection(1, 500);
		}
		else
		{
			player.ChangeDirection(-1, 500);
		}
		int num = game.Random.Next(0, 3);
		int num2 = 10001;
		int num3 = 10004;
		int num4 = 10004;
		int num5 = 0;
		int num6 = 0;
		int num7 = 1;
		float time = 1f;
		int bombCount = 1;
		int num8 = 1000;
		ItemTemplateInfo itemTemplateInfo = null;
		if (Math.Abs(player.X - player2.X) > 60)
		{
			if (num == 0)
			{
				num2 = 10002;
				num5 = player2.X;
				num6 = player2.Y;
			}
			else if (player2.X < player.X && player.X - player2.X > 200 && player.X - player2.X < 800)
			{
				num2 = 10001;
				num3 = 10003;
				num5 = player2.X + 20;
				num6 = player2.Y;
			}
			else if (Math.Abs(player.X - player2.X) > 1200 && player.flyCount <= 0)
			{
				num5 = game.Random.Next(100, game.Map.Bound.Width - 100);
				num2 = 0;
				num3 = 10010;
				num4 = 10016;
				num6 = player.X - 200;
			}
			else
			{
				num2 = 10001;
				num5 = player2.X;
				num6 = player2.Y;
			}
			if (num2 != 0)
			{
				itemTemplateInfo = ItemMgr.FindItemTemplate(num2);
				if (itemTemplateInfo != null && CanUseItem(player, itemTemplateInfo))
				{
					num7 = ((itemTemplateInfo.TemplateID == 10002) ? 2 : 3);
					player.UseItem(itemTemplateInfo);
				}
			}
			if (num3 != 0)
			{
				itemTemplateInfo = ItemMgr.FindItemTemplate(num3);
				if (itemTemplateInfo != null && CanUseItem(player, itemTemplateInfo))
				{
					bombCount = ((itemTemplateInfo.TemplateID != 10003) ? 1 : 3);
					num8 += 1000;
					player.UseItem(itemTemplateInfo);
				}
			}
			itemTemplateInfo = ItemMgr.FindItemTemplate(num4);
			if (num4 == 10004)
			{
				if (itemTemplateInfo != null && CanUseItem(player, itemTemplateInfo))
				{
					int num9 = player.Energy / itemTemplateInfo.Property4;
					for (int i = 0; i < num9; i++)
					{
						num8 += 1000;
						player.UseItem(itemTemplateInfo);
					}
				}
			}
			else if (itemTemplateInfo != null && CanUseItem(player, itemTemplateInfo))
			{
				num8 += 1000;
				if (num4 == 10016)
				{
					player.SetBall(3);
				}
				player.UseItem(itemTemplateInfo);
			}
			time = ((Math.Abs(player.X - player2.X) < 200) ? 1f : ((Math.Abs(player.X - player2.X) < 400) ? 1.5f : ((Math.Abs(player.X - player2.X) < 700) ? 2f : ((Math.Abs(player.X - player2.X) < 1000) ? 2.5f : ((Math.Abs(player.X - player2.X) >= 1100) ? 3.5f : 3f)))));
		}
		else if (num != 0 && player.flyCount <= 0)
		{
			num8 = 1000;
			bombCount = 1;
			num3 = 10010;
			num4 = 10016;
			num7 = 1;
			time = 4f;
			num5 = game.Random.Next(100, game.Map.Bound.Width - 100);
			num6 = player.Y;
			itemTemplateInfo = ItemMgr.FindItemTemplate(num3);
			if (itemTemplateInfo != null && CanUseItem(player, itemTemplateInfo))
			{
				player.UseItem(itemTemplateInfo);
			}
			itemTemplateInfo = ItemMgr.FindItemTemplate(num4);
			if (itemTemplateInfo != null && CanUseItem(player, itemTemplateInfo))
			{
				num8 += 1000;
				if (num4 == 10016)
				{
					player.SetBall(3);
				}
				player.UseItem(itemTemplateInfo);
			}
		}
		else
		{
			bombCount = 1;
			num2 = 10001;
			num3 = 10004;
			num4 = 10004;
			num5 = player2.X;
			num6 = player2.Y;
			num7 = 1;
			num8 = 1000;
			itemTemplateInfo = ItemMgr.FindItemTemplate(num2);
			if (itemTemplateInfo != null && CanUseItem(player, itemTemplateInfo))
			{
				num7 = ((itemTemplateInfo.TemplateID != 10002) ? 1 : 3);
				player.UseItem(itemTemplateInfo);
			}
			itemTemplateInfo = ItemMgr.FindItemTemplate(num3);
			if (itemTemplateInfo != null && CanUseItem(player, itemTemplateInfo))
			{
				num8 += 1000;
				player.UseItem(itemTemplateInfo);
			}
			itemTemplateInfo = ItemMgr.FindItemTemplate(num4);
			int num10 = player.Energy / itemTemplateInfo.Property4;
			for (int j = 0; j < num10; j++)
			{
				if (itemTemplateInfo != null && CanUseItem(player, itemTemplateInfo))
				{
					num8 += 1000;
					player.UseItem(itemTemplateInfo);
				}
			}
			if (player2.PlayerDetail.PlayerCharacter.Grade > 30)
			{
				itemTemplateInfo = ItemMgr.FindItemTemplate(10017);
				num8 += 1000;
				player.UseItem(itemTemplateInfo);
			}
		}
		for (int k = 0; k < num7; k++)
		{
			num8 += 1000;
			game.AddAction(new BotShootAction(player, num5, num6, 0, 0, bombCount, 1001, 10001, time, num8));
		}
		if (player.IsAttacking)
		{
			player.StopAttacking();
		}
	}

	private bool CanUseItem(Player player, ItemTemplateInfo item)
	{
		if (player.Energy >= item.Property4)
		{
			player.Energy -= item.Property4;
			return true;
		}
		return false;
	}
}
