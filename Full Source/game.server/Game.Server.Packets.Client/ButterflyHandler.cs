using System;
using System.Collections.Generic;
using System.Linq;
using Bussiness.Managers;
using EntityDatabase.ServerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(670, "蝶妖森林")]
public class ButterflyHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		switch (packet.ReadByte())
		{
		case 1:
			ButterflyInfo(client.Player);
			break;
		case 2:
			TouchButterflyHead(client.Player);
			break;
		case 3:
			FeedButterfly(client.Player);
			break;
		case 4:
			ButterflyGetReward(client.Player);
			break;
		case 5:
		{
			int taskId3 = packet.ReadInt();
			ButterflyAcceptTask(client.Player, taskId3);
			break;
		}
		case 6:
		{
			int taskId2 = packet.ReadInt();
			ButterflySpeedTask(client.Player, taskId2);
			break;
		}
		case 7:
		{
			int taskId = packet.ReadInt();
			ButterflyGetTaskReward(client.Player, taskId);
			break;
		}
		}
		return 1;
	}

	private void ButterflyGetTaskReward(GamePlayer player, int TaskId)
	{
		ServerData serverData = new ServerData();
		TS_ButterflyTask tS_ButterflyTask = serverData.TS_ButterflyTask.FirstOrDefault((TS_ButterflyTask i) => i.TaskId == TaskId);
		if ((DateTime.Now - player.PlayerCharacter.ButterflyInfo.taskDate).TotalMinutes >= (double)tS_ButterflyTask.NeedTime)
		{
			AddExp(player, tS_ButterflyTask.RewardGp);
			ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(tS_ButterflyTask.RewardItemId), 1, 101);
			itemInfo.IsBinds = true;
			player.SendItemToMail(itemInfo, "蝶妖森林:探索巡逻奖励", "蝶妖森林:探索巡逻奖励", eMailType.Active);
			player.PlayerCharacter.ButterflyInfo.curTaskID = 0;
			player.PlayerCharacter.ButterflyInfo.taskState = 0;
			player.PlayerCharacter.ButterflyInfo.taskDate = DateTime.Now;
		}
		else
		{
			player.SendMessage("巡逻还没结束哦,请再耐心等待一会儿...");
		}
		ButterflyInfo(player);
	}

	private void ButterflySpeedTask(GamePlayer player, int TaskId)
	{
		player.PlayerCharacter.ButterflyInfo.curTaskID = 0;
		player.PlayerCharacter.ButterflyInfo.taskState = 0;
		player.PlayerCharacter.ButterflyInfo.taskDate = DateTime.Now;
		ButterflyInfo(player);
	}

	private void ButterflyAcceptTask(GamePlayer player, int TaskId)
	{
		ServerData serverData = new ServerData();
		TS_ButterflyTask tS_ButterflyTask = serverData.TS_ButterflyTask.FirstOrDefault((TS_ButterflyTask i) => i.TaskId == TaskId);
		if (player.PlayerCharacter.ButterflyInfo.MoonAll >= tS_ButterflyTask.NeedFeeling)
		{
			player.PlayerCharacter.ButterflyInfo.MoonAll -= tS_ButterflyTask.NeedFeeling;
			player.PlayerCharacter.ButterflyInfo.curTaskID = TaskId;
			player.PlayerCharacter.ButterflyInfo.taskState = 0;
			player.PlayerCharacter.ButterflyInfo.taskDate = DateTime.Now.AddMinutes(tS_ButterflyTask.NeedTime);
		}
		else
		{
			player.SendMessage("心情值不足,无法进行巡逻...");
		}
		ButterflyInfo(player);
	}

	private void AddExp(GamePlayer player, int value)
	{
		player.PlayerCharacter.ButterflyInfo.butterflyExp += value;
		if (player.PlayerCharacter.ButterflyInfo.level != GetLevel(player.PlayerCharacter.ButterflyInfo.butterflyExp))
		{
			player.PlayerCharacter.ButterflyInfo.level = GetLevel(player.PlayerCharacter.ButterflyInfo.butterflyExp);
			player.PlayerCharacter.ButterflyInfo.isGetLevelReward = true;
		}
		ButterflyInfo(player);
	}

	private int GetLevel(int butterflyExp)
	{
		if (butterflyExp >= FindLevel(70).Gp)
		{
			return 70;
		}
		for (int i = 1; i <= 70; i++)
		{
			if (butterflyExp < FindLevel(i).Gp)
			{
				return (i - 1 == 0) ? 1 : (i - 1);
			}
		}
		return 1;
	}

	public static TS_Butterfly FindLevel(int Grade)
	{
		Dictionary<int, TS_Butterfly> dictionary = new Dictionary<int, TS_Butterfly>();
		ServerData serverData = new ServerData();
		TS_Butterfly[] array = serverData.TS_Butterfly.ToArray();
		TS_Butterfly[] array2 = array;
		foreach (TS_Butterfly tS_Butterfly in array2)
		{
			if (!dictionary.ContainsKey(tS_Butterfly.Grades))
			{
				dictionary.Add(tS_Butterfly.Grades, tS_Butterfly);
			}
		}
		if (dictionary.ContainsKey(Grade))
		{
			return dictionary[Grade];
		}
		return null;
	}

	private void ButterflyGetReward(GamePlayer player)
	{
		ServerData serverData = new ServerData();
		player.PlayerCharacter.ButterflyInfo.level = GetLevel(player.PlayerCharacter.ButterflyInfo.butterflyExp);
		int gradeRewardItemId = serverData.TS_Butterfly.FirstOrDefault((TS_Butterfly i) => i.Grades == player.PlayerCharacter.ButterflyInfo.level).GradeRewardItemId;
		if (player.PlayerCharacter.ButterflyInfo.isGetLevelReward)
		{
			player.PlayerCharacter.ButterflyInfo.isGetLevelReward = false;
			ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(gradeRewardItemId), 1, 101);
			itemInfo.IsBinds = true;
			player.SendItemToMail(itemInfo, "蝶妖森林:等级奖励", "蝶妖森林:等级奖励", eMailType.Active);
			player.SendMessage("领取成功,已发送到邮件..");
		}
		ButterflyInfo(player);
	}

	private void FeedButterfly(GamePlayer player)
	{
		player.SendMessage("喂养功能已被禁用...");
		ButterflyInfo(player);
	}

	private void TouchButterflyHead(GamePlayer player)
	{
		ServerData serverData = new ServerData();
		player.PlayerCharacter.ButterflyInfo.level = GetLevel(player.PlayerCharacter.ButterflyInfo.butterflyExp);
		int dayFondMax = serverData.TS_Butterfly.FirstOrDefault((TS_Butterfly i) => i.Grades == player.PlayerCharacter.ButterflyInfo.level).DayFondMax;
		if (player.PlayerCharacter.ButterflyInfo.touchCount < dayFondMax)
		{
			player.PlayerCharacter.ButterflyInfo.touchCount++;
			player.PlayerCharacter.ButterflyInfo.MoonAll += 10;
		}
		else
		{
			player.SendMessage("今日已达上限,明天再来摸摸头吧~~");
		}
		ButterflyInfo(player);
	}

	private void ButterflyInfo(GamePlayer player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(670);
		gSPacketIn.WriteByte(1);
		gSPacketIn.WriteInt(player.PlayerCharacter.ButterflyInfo.isHasButterfly);
		gSPacketIn.WriteInt(player.PlayerCharacter.ButterflyInfo.butterflyExp);
		gSPacketIn.WriteBoolean(player.PlayerCharacter.ButterflyInfo.isGetLevelReward);
		gSPacketIn.WriteInt(player.PlayerCharacter.ButterflyInfo.MoonAll);
		gSPacketIn.WriteInt(player.PlayerCharacter.ButterflyInfo.feedCount);
		gSPacketIn.WriteInt(player.PlayerCharacter.ButterflyInfo.touchCount);
		gSPacketIn.WriteInt(player.PlayerCharacter.ButterflyInfo.curTaskID);
		gSPacketIn.WriteInt(player.PlayerCharacter.ButterflyInfo.taskState);
		gSPacketIn.WriteDateTime(player.PlayerCharacter.ButterflyInfo.taskDate);
		player.SendTCP(gSPacketIn);
	}
}
