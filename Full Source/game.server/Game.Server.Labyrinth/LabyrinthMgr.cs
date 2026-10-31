using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using EntityDatabase.PlayerModels;
using EntityDatabase.ServerModels;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Labyrinth;

public static class LabyrinthMgr
{
	private static readonly Dictionary<int, Timer> PlayerTimers = new Dictionary<int, Timer>();

	private static readonly object Locker = new object();

	private static readonly ServerData ServerData = new ServerData();

	public static void StartSweep(GamePlayer player)
	{
		int num = 1000;
		lock (Locker)
		{
			if (!PlayerTimers.ContainsKey(player.PlayerId))
			{
				Timer value = new Timer(delegate
				{
					LabyrinthCheck(player);
				}, null, num, num);
				PlayerTimers.Add(player.PlayerId, value);
			}
			else
			{
				PlayerTimers[player.PlayerId].Change(num, num);
			}
		}
	}

	public static string GetBuff(GamePlayer player)
	{
		return ServerData.Labyrinth_Rewards.FirstOrDefault((Labyrinth_Rewards r) => r.Level == player.PlayerCharacter.LabyrinthInfo.currentFinishLevel)?.Buffs;
	}

	public static void StopSweep(GamePlayer player)
	{
		lock (Locker)
		{
			if (PlayerTimers.ContainsKey(player.PlayerId))
			{
				PlayerTimers[player.PlayerId].Change(-1, -1);
				PlayerTimers[player.PlayerId].Dispose();
				PlayerTimers[player.PlayerId] = null;
				PlayerTimers.Remove(player.PlayerId);
			}
		}
	}

	private static void LabyrinthCheck(GamePlayer player)
	{
		try
		{
			int tickCount = Environment.TickCount;
			ThreadPriority priority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			UpdateLabyrinthTime(player);
			Thread.CurrentThread.Priority = priority;
			tickCount = Environment.TickCount - tickCount;
		}
		catch (Exception ex)
		{
			Console.WriteLine("LabyrinthCheck: " + ex);
		}
	}

	public static void UpdateLabyrinthTime(GamePlayer player)
	{
		Sys_Users_Labyrinth labyrinthInfo = player.PlayerCharacter.LabyrinthInfo;
		if (!labyrinthInfo.isSweep)
		{
			labyrinthInfo.isSweep = true;
			labyrinthInfo.isPlayGame = true;
			player.Out.SendLabyrinthStartSweep(player);
			player.Out.SendLabyrinthUpdataInfo(player);
		}
		if (labyrinthInfo.sweepTotalTimeRemaining > 0 && labyrinthInfo.sweepTotalTime > 0)
		{
			labyrinthInfo.sweepTotalTime--;
			labyrinthInfo.sweepTotalTimeRemaining--;
			player.LabyrinthTime--;
		}
		if (player.LabyrinthTime == 0)
		{
			CollectLabyrinthReward(player, isFirst: false, isSweep: true);
			labyrinthInfo.currentFinishLevel++;
			player.LabyrinthTime = 120;
			if (labyrinthInfo.currentFinishLevel > labyrinthInfo.maxWeekFinishLevel + 1)
			{
				labyrinthInfo.currentFinishLevel = labyrinthInfo.maxWeekFinishLevel + 1;
				labyrinthInfo.isPlayGame = false;
				labyrinthInfo.sweepTotalTimeRemaining = 0;
				labyrinthInfo.sweepTotalTime = 0;
				StopSweep(player);
			}
		}
	}

	public static void ForceComplete(GamePlayer player)
	{
		Sys_Users_Labyrinth labyrinthInfo = player.PlayerCharacter.LabyrinthInfo;
		labyrinthInfo.isPlayGame = false;
		labyrinthInfo.sweepTotalTimeRemaining = 0;
		labyrinthInfo.sweepTotalTime = 0;
		for (int i = labyrinthInfo.currentFinishLevel; i <= labyrinthInfo.maxWeekFinishLevel; i++)
		{
			CollectLabyrinthReward(player, isFirst: false, isSweep: true);
			labyrinthInfo.currentFinishLevel++;
		}
		player.Out.SendLabyrinthUpdataInfo(player);
		StopSweep(player);
	}

	public static void CollectLabyrinthReward(GamePlayer player, bool isFirst = false, bool isSweep = false)
	{
		List<string> list = ServerData.Labyrinth_Rewards.FirstOrDefault((Labyrinth_Rewards r) => r.Level == player.PlayerCharacter.LabyrinthInfo.currentFinishLevel && r.HardType == 0)?.Rewards.Split('|').ToList();
		if (isFirst)
		{
			list?.AddRange(ServerData.Labyrinth_Rewards.FirstOrDefault((Labyrinth_Rewards r) => r.Level == player.PlayerCharacter.LabyrinthInfo.currentFinishLevel && r.HardType == 0)?.FirstRewards.Split('|') ?? throw new InvalidOperationException());
		}
		if (list == null)
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		List<ItemInfo> list2 = new List<ItemInfo>();
		int num = 0;
		int num2 = 0;
		int num3 = ((!player.PlayerCharacter.LabyrinthInfo.isUseDoubleProp) ? 1 : 2);
		foreach (List<string> item2 in list.Select((string rewardItem) => rewardItem.Split(',').ToList()))
		{
			if (item2.Count != 2)
			{
				throw new ArrayTypeMismatchException("Labyrinth_Rewards table Error!");
			}
			int num4 = int.Parse(item2[0]);
			int num5 = int.Parse(item2[1]) * num3;
			switch (num4)
			{
			case 11107:
				num += num5;
				continue;
			case -900:
				num2 += num5;
				continue;
			}
			ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(num4);
			ItemInfo item = ItemInfo.CreateFromTemplate(itemTemplateInfo, num5, 101);
			stringBuilder.AppendLine($"{itemTemplateInfo.Name} x {num5}");
			list2.Add(item);
		}
		player.AddHardCurrency(num2);
		player.AddGP(num);
		if (isSweep)
		{
			player.Out.SendLabyrinthUpdataInfo(player);
			player.Out.SendLabyrinthSweepInfo(player, list2, num, num2);
		}
		player.SendItemsToMail(list2, LanguageMgr.GetTranslation("CollectLabyrinthReward.Content", num2, num, stringBuilder, player.PlayerCharacter.LabyrinthInfo.currentFinishLevel), LanguageMgr.GetTranslation("CollectLabyrinthReward.Title"), eMailType.Manage);
	}
}
