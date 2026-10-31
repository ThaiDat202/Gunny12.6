using System;
using System.Linq;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic.Cmd;

[GameCommand(32, "使用道具")]
public class PropUseCommand : ICommandHandler
{
	public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
	{
		if (game.GameState != eGameState.Playing || player.GetSealState())
		{
			return;
		}
		int num = packet.ReadByte();
		int num2 = packet.ReadInt();
		int num3 = packet.ReadInt();
		if (player.Prop.Contains(10471) || (num3 == 10471 && player.Prop.Count != 0))
		{
			return;
		}
		ItemTemplateInfo item = ItemMgr.FindItemTemplate(num3);
		int[] source = new int[8] { 10001, 10002, 10003, 10004, 10005, 10006, 10007, 10008 };
		if (!player.CanUseItem(item))
		{
			return;
		}
		int num4 = num;
		int num5 = num4;
		if (num5 == 1)
		{
			if (source.Contains(num3))
			{
				if (num3 == 10001)
				{
					player.Prop1++;
				}
				if (num3 == 10002)
				{
					player.Prop2++;
				}
				if (num3 == 10003)
				{
					player.Prop3++;
				}
				if (num3 == 10004)
				{
					player.Prop4++;
				}
				player.UseItem(item);
				player.OnPropUseItem();
			}
			else
			{
				Console.WriteLine("eTankCmdType.PROP type:{0} {1} {2}, Player Hack buff", num, num2, num3);
				player.PlayerDetail.Disconnect();
			}
		}
		else
		{
			if (!player.PlayerDetail.UsePropItem(game, num, num2, num3, player.IsLiving))
			{
				return;
			}
			if (!player.UseItem(item))
			{
				Console.WriteLine("Using prop error fail!");
				return;
			}
			if (num3 == 10001)
			{
				player.Prop1++;
			}
			if (num3 == 10002)
			{
				player.Prop2++;
			}
			if (num3 == 10003)
			{
				player.Prop3++;
			}
			if (num3 == 10004)
			{
				player.Prop4++;
			}
			player.OnPropUseItem();
		}
	}
}
