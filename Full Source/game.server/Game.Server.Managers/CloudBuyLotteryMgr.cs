using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Managers;

public class CloudBuyLotteryMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, CloudBuyLotteryInfo> m_CloudBuyLotterys = new Dictionary<int, CloudBuyLotteryInfo>();

	private static Random rand = new Random();

	protected static Timer m_cloudBuyLotteryScanTimer;

	public static bool ReLoad()
	{
		try
		{
			CloudBuyLotteryInfo[] array = LoadCloudBuyLotteryDb();
			Dictionary<int, CloudBuyLotteryInfo> value = LoadCloudBuyLotterys(array);
			if (array.Length != 0)
			{
				Interlocked.Exchange(ref m_CloudBuyLotterys, value);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ReLoad CloudBuyLottery", exception);
			}
			return false;
		}
		finally
		{
			BeginTimer();
		}
		return true;
	}

	public static bool Init()
	{
		return ReLoad();
	}

	public static CloudBuyLotteryInfo[] LoadCloudBuyLotteryDb()
	{
		using ActiveBussiness activeBussiness = new ActiveBussiness();
		return activeBussiness.GetAllCloudBuyLottery();
	}

	public static Dictionary<int, CloudBuyLotteryInfo> LoadCloudBuyLotterys(CloudBuyLotteryInfo[] CloudBuyLottery)
	{
		Dictionary<int, CloudBuyLotteryInfo> dictionary = new Dictionary<int, CloudBuyLotteryInfo>();
		foreach (CloudBuyLotteryInfo cloudBuyLotteryInfo in CloudBuyLottery)
		{
			if (!dictionary.Keys.Contains(cloudBuyLotteryInfo.ID))
			{
				dictionary.Add(cloudBuyLotteryInfo.ID, cloudBuyLotteryInfo);
			}
		}
		return dictionary;
	}

	public static CloudBuyLotteryInfo FindCloudBuyLottery(int ID)
	{
		if (m_CloudBuyLotterys.ContainsKey(ID))
		{
			return m_CloudBuyLotterys[ID];
		}
		return null;
	}

	public static CloudBuyLotteryInfo GetCloudBuyLottery()
	{
		foreach (CloudBuyLotteryInfo value in m_CloudBuyLotterys.Values)
		{
			if (value.currentNum > 0)
			{
				return value;
			}
		}
		return null;
	}

	public static void UpdateCloudBuyLottery(int count, ref bool isGetAward)
	{
		CloudBuyLotteryInfo cloudBuyLottery = GetCloudBuyLottery();
		if (cloudBuyLottery == null)
		{
			return;
		}
		cloudBuyLottery.currentNum -= count;
		lock (m_CloudBuyLotterys)
		{
			if (m_CloudBuyLotterys.ContainsKey(cloudBuyLottery.ID))
			{
				m_CloudBuyLotterys[cloudBuyLottery.ID] = cloudBuyLottery;
			}
		}
		if (cloudBuyLottery.currentNum <= 0)
		{
			SendCloudBuyLotteryRandomReward(cloudBuyLottery);
			isGetAward = true;
		}
	}

	public static void SendCloudBuyLotteryRandomReward(CloudBuyLotteryInfo info)
	{
		if (info == null)
		{
			return;
		}
		ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(info.templateId);
		if (itemTemplateInfo != null)
		{
			ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 133);
			itemInfo.IsBinds = true;
			itemInfo.Count = info.templatedIdCount;
			itemInfo.ValidDate = info.validDate;
			string[] array = info.property.Split(',');
			if (array.Length == 5)
			{
				itemInfo.StrengthenLevel = int.Parse(array[0]);
				itemInfo.AttackCompose = int.Parse(array[1]);
				itemInfo.DefendCompose = int.Parse(array[2]);
				itemInfo.AgilityCompose = int.Parse(array[3]);
				itemInfo.LuckCompose = int.Parse(array[4]);
			}
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			ActiveSystemInfo[] allActiveSystemData = playerBussiness.GetAllActiveSystemData();
			List<ActiveSystemInfo> list = new List<ActiveSystemInfo>();
			ActiveSystemInfo[] array2 = allActiveSystemData;
			foreach (ActiveSystemInfo activeSystemInfo in array2)
			{
				if (activeSystemInfo.luckCount > 0)
				{
					list.Add(activeSystemInfo);
				}
			}
			if (list.Count <= 0)
			{
				return;
			}
			ActiveSystemInfo activeSystemInfo2 = list[rand.Next(list.Count)];
			WorldEventMgr.SendItemToMail(itemInfo, activeSystemInfo2.UserID, activeSystemInfo2.NickName, activeSystemInfo2.ZoneID, LanguageMgr.GetTranslation("SendCloudBuyLotteryRandomReward.Success"));
			string translation = LanguageMgr.GetTranslation("SendCloudBuyLotteryRandomReward.Notice", activeSystemInfo2.NickName, itemInfo.TemplateID);
			GSPacketIn packet = WorldMgr.SendSysNotice(eMessageType.ChatNormal, translation, itemInfo.ItemID, itemInfo.TemplateID, GameServer.Instance.Configuration.AreaId, null);
			GameServer.Instance.LoginServer.SendPacket(packet);
			CloudBuyLogInfo cloudBuyLogInfo = new CloudBuyLogInfo();
			cloudBuyLogInfo.UserID = activeSystemInfo2.UserID;
			cloudBuyLogInfo.nickName = activeSystemInfo2.NickName;
			cloudBuyLogInfo.templateId = info.templateId;
			cloudBuyLogInfo.validate = info.validDate;
			cloudBuyLogInfo.count = info.templatedIdCount;
			cloudBuyLogInfo.property = info.property;
			bool flag = playerBussiness.AddCloudBuyLog(cloudBuyLogInfo);
			CloudBuyLotteryInfo cloudBuyLottery = GetCloudBuyLottery();
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			GamePlayer[] array3 = allPlayers;
			foreach (GamePlayer gamePlayer in array3)
			{
				if (gamePlayer != null)
				{
					if (info != null)
					{
						gamePlayer.Actives.SendCloudBuyLotteryUpdateInfos(cloudBuyLottery, isGame: true);
					}
					else
					{
						gamePlayer.Actives.SendCloudBuyLotteryUpdateInfos(info, isGame: false);
					}
					gamePlayer.Actives.ResetLuckCount();
				}
			}
			flag = playerBussiness.ResetLuckCount();
			return;
		}
		if (log.IsErrorEnabled)
		{
			log.Error("CloudBuyLottery not found ItemTemplateID: " + info.templateId);
		}
	}

	public static void SaveCloudBuyLottery()
	{
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		foreach (CloudBuyLotteryInfo value in m_CloudBuyLotterys.Values)
		{
			playerBussiness.UpdateCloudBuyLottery(value);
		}
	}

	public static void BeginTimer()
	{
		int num = 300000;
		if (m_cloudBuyLotteryScanTimer == null)
		{
			m_cloudBuyLotteryScanTimer = new Timer(CloudBuyLotteryScan, null, num, num);
		}
		else
		{
			m_cloudBuyLotteryScanTimer.Change(num, num);
		}
	}

	protected static void CloudBuyLotteryScan(object sender)
	{
		try
		{
			log.Info("Begin Save CloudBuyLottery to DB ....");
			int tickCount = Environment.TickCount;
			ThreadPriority priority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			SaveCloudBuyLottery();
			Thread.CurrentThread.Priority = priority;
			tickCount = Environment.TickCount - tickCount;
			log.Info("End CloudBuyLottery Scan....");
		}
		catch (Exception exception)
		{
			log.Error("CloudBuyLottery Scan ", exception);
		}
	}

	public static void StopAllTimer()
	{
		if (m_cloudBuyLotteryScanTimer != null)
		{
			m_cloudBuyLotteryScanTimer.Dispose();
			m_cloudBuyLotteryScanTimer = null;
		}
	}
}
