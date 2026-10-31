using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using Bussiness;
using EntityDatabase.ServerModels;
using Game.Server.ConsortiaTask.Conditions;
using Game.Server.ConsortiaTask.Data;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using log4net;

namespace Game.Server.ConsortiaTask;

public static class ConsortiaTaskMgr
{
	public static ConsortiaTaskPacketsOut Out = new ConsortiaTaskPacketsOut();

	public static ConsortiaTaskData Data = new ConsortiaTaskData();

	private static ServerData ServerData = new ServerData();

	private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public static List<Consortia> Consortiums = new List<Consortia>();

	private static List<Mission> Missions = new List<Mission>();

	public static object m_lock = new object();

	private static Timer m_scanTimer;

	public static bool Init()
	{
		m_scanTimer = new Timer(Scan, null, 30000, 60000);
		Log.InfoFormat("Total loaded tasks: {0}", ServerData.Consortia_Task_Conditions.ToList().Count);
		ServerData.Consortia_Task_Conditions.ToList().ForEach(delegate(Consortia_Task_Conditions t)
		{
			Data.AddTask(t);
		});
		return true;
	}

	public static void AddPlayer(GamePlayer player)
	{
		try
		{
			lock (m_lock)
			{
				Consortia consortia = Consortiums.Find((Consortia c) => c.ID == player.PlayerCharacter.ConsortiaID);
				if (consortia == null)
				{
					return;
				}
				consortia.Players.Add(player);
				foreach (ConsortiaTaskConditionInfo condition2 in consortia.Task.Conditions)
				{
					Condition condition = null;
					switch (condition2.CondictionType)
					{
					case 1:
						condition = new PvPBattleCondition(player, consortia);
						consortia.Conditions.Add(condition);
						break;
					case 2:
						condition = new GuildBattleCondition(player, consortia);
						consortia.Conditions.Add(condition);
						break;
					case 3:
						condition = new UseItemCondition(player, consortia, condition2.TemplateID);
						consortia.Conditions.Add(condition);
						break;
					case 4:
						condition = new MissionCompleteCondition(player, consortia);
						consortia.Conditions.Add(condition);
						break;
					case 5:
						condition = new DonateRichesCondition(player, consortia);
						consortia.Conditions.Add(condition);
						break;
					}
					condition?.AddTrigger();
				}
			}
		}
		catch (Exception exception)
		{
			if (Log.IsErrorEnabled)
			{
				Log.Error("AddPlayer ConsortiaTask error", exception);
			}
		}
	}

	public static void RemovePlayer(GamePlayer player)
	{
		try
		{
			lock (m_lock)
			{
				Consortia consortia = Consortiums.Find((Consortia c) => c.ID == player.PlayerCharacter.ConsortiaID);
				if (consortia == null)
				{
					return;
				}
				consortia.Players.Remove(player);
				foreach (Condition item in consortia.Conditions.FindAll((Condition c) => c.Player == player))
				{
					item.RemoveTrigger();
				}
			}
		}
		catch (Exception exception)
		{
			if (Log.IsErrorEnabled)
			{
				Log.Error("Remove ConsortiaTask error", exception);
			}
		}
	}

	private static void Scan(object state)
	{
		try
		{
			lock (m_lock)
			{
				foreach (Consortia consortia2 in Consortiums.Where((Consortia consortia) => consortia != null))
				{
					if (consortia2.Task.BeginTime.AddMinutes(consortia2.Task.Time) < DateTime.Now)
					{
						consortia2.Players.ForEach(delegate(GamePlayer t)
						{
							Out.SendTaskComplete(t);
						});
						RemoveConsortiaForm(consortia2);
					}
					else if (consortia2.Task.Completed)
					{
						consortia2.Players.ForEach(delegate(GamePlayer t)
						{
							int num = consortia2.Task.Experience * consortia2.RankTable[t.PlayerId] / consortia2.Task.Points;
							int num2 = consortia2.Task.Offer * consortia2.RankTable[t.PlayerId] / consortia2.Task.Points;
							int value = consortia2.Task.Contribution * consortia2.RankTable[t.PlayerId] / consortia2.Task.Points;
							t.AddGP(num);
							t.AddOffer(num2);
							t.AddRichesOffer(value);
							t.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("GameServer.ConsortiaTask.msg3", consortia2.Task.Points, num, num2));
							string content = "公会使命已完成 \n" + $"经验: {num}" + $"贡献: {num2}";
							t.SendMailToUser(new PlayerBussiness(), content, "公会使命奖励", eMailType.ConsortionEmail);
							Out.SendTaskComplete(t);
						});
						RemoveConsortiaForm(consortia2);
					}
				}
			}
		}
		catch (Exception exception)
		{
			if (Log.IsErrorEnabled)
			{
				Log.Error("Scan ConsortiaTask error", exception);
			}
		}
	}

	private static void RemoveConsortiaForm(Consortia consortia)
	{
		Consortiums.Remove(consortia);
		foreach (Condition condition in consortia.Conditions)
		{
			condition.RemoveTrigger();
		}
		consortia.Players.Clear();
		consortia.Players = null;
		consortia.Conditions = null;
		consortia.Task = null;
	}

	public static bool CreateTask(GamePlayer player, int level, bool reset = false)
	{
		try
		{
			lock (m_lock)
			{
				int consortiaId = player.PlayerCharacter.ConsortiaID;
				Consortia consortia = Consortiums.Find((Consortia c) => c.ID == consortiaId);
				if (consortia != null)
				{
					return true;
				}
				consortia = new Consortia
				{
					ID = player.PlayerCharacter.ConsortiaID,
					Players = WorldMgr.GetAllConsortiaPlayers(consortiaId).ToList(),
					Conditions = new List<Condition>(),
					RankTable = new Dictionary<int, int>()
				};
				ConsortiaTaskInfo task = new ConsortiaTaskInfo
				{
					ID = 0,
					BeginTime = DateTime.Now,
					Contribution = level * 5000,
					Experience = level * 10000,
					Offer = level * 1000,
					BuffID = -1,
					Level = level,
					Riches = level * 15000,
					Time = 120,
					Conditions = CreateTaskConditions(level, 0, player)
				};
				consortia.Task = task;
				consortia.Task.Conditions.ForEach(delegate(ConsortiaTaskConditionInfo t)
				{
					consortia.Task.Points += t.TargetCount;
				});
				CreateTaskEvents(consortia);
				Consortiums.Add(consortia);
				if (reset)
				{
					return true;
				}
				return true;
			}
		}
		catch (Exception exception)
		{
			if (Log.IsErrorEnabled)
			{
				Log.Error("CreateTask error", exception);
			}
		}
		return false;
	}

	public static void ResetTask(GamePlayer player)
	{
		lock (m_lock)
		{
			try
			{
				int consortiaId = player.PlayerCharacter.ConsortiaID;
				Consortia consortia = Consortiums.Find((Consortia c) => c.ID == consortiaId);
				if (consortia != null)
				{
					Consortiums.Remove(consortia);
					int level = consortia.Task.Level;
					foreach (Condition condition in consortia.Conditions)
					{
						condition.RemoveTrigger();
					}
					consortia.Conditions = null;
					consortia.Task = null;
					consortia = null;
					CreateTask(player, level, reset: true);
				}
				else
				{
					CreateTask(player, 1, reset: true);
				}
			}
			catch (Exception exception)
			{
				if (Log.IsErrorEnabled)
				{
					Log.Error("ResetTask error", exception);
				}
			}
		}
	}

	private static void CreateTaskEvents(Consortia consortia)
	{
		try
		{
			foreach (GamePlayer player in consortia.Players)
			{
				foreach (ConsortiaTaskConditionInfo condition2 in consortia.Task.Conditions)
				{
					Condition condition = null;
					switch (condition2.CondictionType)
					{
					case 1:
						condition = new PvPBattleCondition(player, consortia);
						consortia.Conditions.Add(condition);
						break;
					case 2:
						condition = new GuildBattleCondition(player, consortia);
						consortia.Conditions.Add(condition);
						break;
					case 3:
						condition = new UseItemCondition(player, consortia, condition2.TemplateID);
						consortia.Conditions.Add(condition);
						break;
					case 4:
						condition = new MissionCompleteCondition(player, consortia);
						consortia.Conditions.Add(condition);
						break;
					case 5:
						condition = new DonateRichesCondition(player, consortia);
						consortia.Conditions.Add(condition);
						break;
					}
					condition?.AddTrigger();
				}
			}
		}
		catch (Exception exception)
		{
			if (Log.IsErrorEnabled)
			{
				Log.Error("CreateTaskEvent error", exception);
			}
		}
	}

	private static List<ConsortiaTaskConditionInfo> CreateTaskConditions(int level, int taskId, GamePlayer player)
	{
		List<ConsortiaTaskConditionInfo> list = new List<ConsortiaTaskConditionInfo>();
		List<int> list2 = new List<int>();
		try
		{
			while (list2.Count < 3)
			{
				int num = ThreadSafeRandom.NextStatic(1, 40);
				int item = ((num < 10) ? 1 : ((num >= 10 && num < 20) ? 2 : ((num >= 20 && num < 30) ? 3 : ((num < 30 || num >= 40) ? 5 : 4))));
				if (!list2.Contains(item))
				{
					list2.Add(item);
				}
			}
			for (int i = 0; i < list2.Count; i++)
			{
				Consortia_Task_Conditions taskConditionDataInfo = Data.GetTaskConditionDataInfo(list2[i], level);
				ConsortiaTaskConditionInfo consortiaTaskConditionInfo = new ConsortiaTaskConditionInfo
				{
					ID = i,
					TaskID = taskId,
					CondictionTitle = string.Format(taskConditionDataInfo.CondictionTitle, taskConditionDataInfo.TargetCount),
					CondictionType = list2[i],
					Value = 0,
					TargetCount = taskConditionDataInfo.TargetCount,
					Finish = 0,
					TemplateID = taskConditionDataInfo.Target,
					MustWin = Convert.ToBoolean(taskConditionDataInfo.Para2),
					MissionID = taskConditionDataInfo.Target
				};
				consortiaTaskConditionInfo.Complete += ConditionCompleted;
				list.Add(consortiaTaskConditionInfo);
			}
		}
		catch (Exception exception)
		{
			if (Log.IsErrorEnabled)
			{
				Log.Error("CreateTaskCondition error", exception);
			}
		}
		return list;
	}

	private static void ConditionCompleted(Consortia consortia, ConsortiaTaskConditionInfo condition)
	{
		lock (m_lock)
		{
			foreach (GamePlayer player in consortia.Players)
			{
				Out.SendTaskConditionCompleted(player, condition.CondictionTitle);
			}
			bool allCompleted = true;
			consortia.Task.Conditions.ForEach(delegate(ConsortiaTaskConditionInfo t)
			{
				if (!t.Completed)
				{
					allCompleted = false;
				}
			});
			if (allCompleted)
			{
				consortia.Task.Completed = true;
			}
		}
	}

	public static void Shuffle<T>(this IList<T> list)
	{
		RNGCryptoServiceProvider rNGCryptoServiceProvider = new RNGCryptoServiceProvider();
		int num = list.Count;
		while (num > 1)
		{
			byte[] array = new byte[1];
			do
			{
				rNGCryptoServiceProvider.GetBytes(array);
			}
			while (array[0] >= num * (255 / num));
			int index = (int)array[0] % num;
			num--;
			T value = list[index];
			list[index] = list[num];
			list[num] = value;
		}
	}
}
