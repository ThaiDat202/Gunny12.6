using System;
using System.Collections.Generic;
using System.Linq;
using Bussiness;
using Bussiness.Managers;
using EntityDatabase.ServerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.DevilTurn.Handle;

[DevilTurnHandle(11)]
public class Sacrifice : IDevilTurnCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int num = 1;
		bool flag = packet.ReadBoolean();
		byte b = packet.ReadByte();
		packet.ReadBoolean();
		if (b == 2)
		{
			num = 10;
		}
		if (flag)
		{
			if (player.PlayerCharacter.DevilTurn.freeTimes < num)
			{
				player.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("DevilTurnHandle.Sacrifice.Fail.freeTimes"));
				return 0;
			}
			player.PlayerCharacter.DevilTurn.freeTimes -= num;
			player.PlayerCharacter.DevilTurn.freeDate = DateTime.Now.AddMinutes(10.0);
		}
		else
		{
			int num2 = GameProperties.DevilTreasureOneCost;
			if (num == 10)
			{
				num2 = GameProperties.DevilTreasureTenCost;
			}
			if (player.PlayerCharacter.Money < num2)
			{
				player.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("DevilTurnHandle.Sacrifice.Fail.Money"));
				return 0;
			}
			player.RemoveMoney(num2);
		}
		EventTurn(player, b);
		DevilTurnMgr.Out.SendUpdateInfo(player);
		return 0;
	}

	private void EventTurn(GamePlayer player, byte openType)
	{
		Random random = new Random();
		int num = 1;
		double num2 = 0.0;
		List<DevilTreasItemList> list = new List<DevilTreasItemList>();
		List<ItemInfo> list2 = new List<ItemInfo>();
		if (openType == 2)
		{
			num = 10;
		}
		int num3 = 0;
		int num4 = 0;
		GSPacketIn gSPacketIn = new GSPacketIn(608, player.PlayerId);
		gSPacketIn.WriteByte(11);
		gSPacketIn.WriteByte((byte)num);
		for (int i = 0; i < num; i++)
		{
			player.PlayerCharacter.DevilTurn.lotteryCount++;
			num3++;
			DevilTreasItemList item = DevilTurnMgr.GetItem();
			int num5 = item.Type;
			switch (num5)
			{
			case 3:
				num5 = random.Next(1, 6);
				AddBead(player, num5, item.Value);
				break;
			case 2:
			{
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(item.TemplateID), item.Value, 101);
				itemInfo.IsBinds = true;
				list2.Add(itemInfo);
				break;
			}
			default:
				num2 += (double)item.Value;
				break;
			}
			num4 = Convert.ToInt32(num2 / 100.0 * (double)DevilTurnMgr.JackBox);
			if (num4 > 0)
			{
				ItemInfo itemInfo2 = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(GameProperties.DevilTreasureTemplateID), num4, 101);
				itemInfo2.IsBinds = true;
				list2.Add(itemInfo2);
			}
			gSPacketIn.WriteByte((byte)item.ID);
			gSPacketIn.WriteByte((byte)num5);
			list.Add(item);
		}
		if (list.Count > 0)
		{
			player.AddTemplate(list2);
			player.PlayerCharacter.DevilTurn.myRankScore += list.Sum((DevilTreasItemList p) => p.Value);
			player.PlayerCharacter.DevilTurnRank.Score = player.PlayerCharacter.DevilTurn.myRankScore;
		}
		DevilTurnMgr.AddDevilTurnValue(player.PlayerId, player.PlayerCharacter.DevilTurn.myRankScore, player.PlayerCharacter.NickName, num3 - num4, GameServer.Instance.Configuration.AreaId, GameServer.Instance.Configuration.ServerName);
		player.Out.SendTCP(gSPacketIn);
		DevilTurnMgr.GetDevilTurnData();
	}

	private void AddBead(GamePlayer player, int type, int value)
	{
		switch (type)
		{
		case 1:
			player.PlayerCharacter.DevilTurn.beadCount1 += value;
			break;
		case 2:
			player.PlayerCharacter.DevilTurn.beadCount2 += value;
			break;
		case 3:
			player.PlayerCharacter.DevilTurn.beadCount3 += value;
			break;
		case 4:
			player.PlayerCharacter.DevilTurn.beadCount4 += value;
			break;
		case 5:
			player.PlayerCharacter.DevilTurn.beadCount5 += value;
			break;
		}
	}
}
