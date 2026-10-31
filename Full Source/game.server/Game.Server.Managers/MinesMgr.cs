using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml;
using Bussiness;
using Bussiness.Managers;
using Game.Base;
using Game.Base.Packets;
using Game.Server.GameObjects;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Managers;

public class MinesMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, OnlineArmLevelInfo> _armLevelList;

	private static Dictionary<int, OnlineArmDropInfo> _armDropList;

	private static Dictionary<int, ItemTemplateInfo> _itemDigList;

	private static ThreadSafeRandom m_random;

	private static Timer m_timer;

	private static MinesManageDataInfo m_data;

	protected static object m_lock = new object();

	public static MinesManageDataInfo Data => m_data;

	public static bool Init()
	{
		_armLevelList = new Dictionary<int, OnlineArmLevelInfo>();
		_armDropList = new Dictionary<int, OnlineArmDropInfo>();
		_itemDigList = new Dictionary<int, ItemTemplateInfo>();
		m_random = new ThreadSafeRandom();
		ReLoad();
		LoadData();
		StartTimer();
		return true;
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, OnlineArmLevelInfo> dictionary = new Dictionary<int, OnlineArmLevelInfo>();
			Dictionary<int, OnlineArmDropInfo> dictionary2 = new Dictionary<int, OnlineArmDropInfo>();
			if (LoadOnlineArmLevel(dictionary) && LoadOnlineArmDrop(dictionary2) && ItemMgr.LoadMineItemDig(_itemDigList))
			{
				try
				{
					_armLevelList = dictionary;
					_armDropList = dictionary2;
					return true;
				}
				catch
				{
				}
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("MinesMgr", exception);
			}
		}
		return false;
	}

	public static void StartTimer()
	{
		int num = 60000;
		if (m_timer == null)
		{
			m_timer = new Timer(MinesScan, null, num, num);
		}
		else
		{
			m_timer.Change(num, num);
		}
	}

	public static void StopTimer()
	{
		if (m_timer != null)
		{
			m_timer.Dispose();
			m_timer = null;
		}
		SaveData();
	}

	protected static void MinesScan(object sender)
	{
		try
		{
			log.Info("Begin Scan Mines ....");
			int tickCount = Environment.TickCount;
			ThreadPriority priority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			ScanWorking();
			Thread.CurrentThread.Priority = priority;
			tickCount = Environment.TickCount - tickCount;
			log.Info("End Mines Scan....");
		}
		catch (Exception exception)
		{
			log.Error("Mines Scan ", exception);
		}
	}

	private static void ScanWorking()
	{
		if (m_data.LastTimeChange.Date != DateTime.Now.Date)
		{
			m_data.LastTimeChange = DateTime.Now;
			ResetDropAndExchangeData();
			GamePlayer singlePlayerNoGame = WorldMgr.GetSinglePlayerNoGame();
			if (singlePlayerNoGame != null)
			{
				GSPacketIn pkg = singlePlayerNoGame.Out.SendMinesShopInfo();
				WorldMgr.SendToNoGame(pkg, singlePlayerNoGame);
			}
		}
	}

	private static void ResetDropAndExchangeData()
	{
		lock (m_data)
		{
			m_data.ListShopDrop.Clear();
			m_data.ListShopExchange.Clear();
		}
		List<ItemTemplateInfo> allItemDig = GetAllItemDig();
		for (int i = 0; i < 4; i++)
		{
			int index = m_random.Next(allItemDig.Count);
			MineManageItemInfo mineManageItemInfo = new MineManageItemInfo();
			mineManageItemInfo.TemplateID = allItemDig[index].TemplateID;
			mineManageItemInfo.MaxCount = allItemDig[index].Property6;
			mineManageItemInfo.CurrentCount = allItemDig[index].Property6;
			mineManageItemInfo.Price = allItemDig[index].Property5;
			lock (m_data)
			{
				m_data.ListShopDrop.Add(mineManageItemInfo);
			}
			allItemDig.RemoveAt(index);
		}
		List<ActivitySystemItemInfo> list = ActiveMgr.FindMinesRandomExchange(4);
		int num = 0;
		foreach (ActivitySystemItemInfo item in list)
		{
			ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(item.TemplateID);
			MineManageExchangeInfo mineManageExchangeInfo = new MineManageExchangeInfo();
			mineManageExchangeInfo.ID = item.ID;
			mineManageExchangeInfo.activityType = item.ActivityType;
			mineManageExchangeInfo.Quality = item.Quality;
			mineManageExchangeInfo.TemplateID = item.TemplateID;
			mineManageExchangeInfo.Name = itemTemplateInfo.Name;
			mineManageExchangeInfo.Price = item.Probability;
			mineManageExchangeInfo.strengthLevel = item.StrengthLevel;
			mineManageExchangeInfo.attackCompose = item.AttackCompose;
			mineManageExchangeInfo.defendCompose = item.DefendCompose;
			mineManageExchangeInfo.luckCompose = item.LuckCompose;
			mineManageExchangeInfo.isBind = item.IsBind;
			mineManageExchangeInfo.validDate = item.ValidDate;
			mineManageExchangeInfo.Count = item.Count;
			mineManageExchangeInfo.Position = num;
			mineManageExchangeInfo.LimitCount = item.Quality;
			lock (m_data)
			{
				m_data.ListShopExchange.Add(mineManageExchangeInfo);
			}
			num++;
		}
	}

	private static bool LoadOnlineArmLevel(Dictionary<int, OnlineArmLevelInfo> datas)
	{
		try
		{
			XmlDocument xmlDocument = Marshal.LoadXMLData("onlinearmlevelinfo", isEncrypt: true);
			XmlNodeList xmlNodeList = xmlDocument.SelectNodes("Result/child::node()");
			foreach (XmlNode item in xmlNodeList)
			{
				int num = int.Parse(item.Attributes["LevelCount"].Value);
				if (!datas.ContainsKey(num))
				{
					OnlineArmLevelInfo onlineArmLevelInfo = new OnlineArmLevelInfo();
					onlineArmLevelInfo.LevelCount = num;
					onlineArmLevelInfo.PickAxeExp = int.Parse(item.Attributes["PickAxeExp"].Value);
					onlineArmLevelInfo.HeadExp = int.Parse(item.Attributes["HeadExp"].Value);
					onlineArmLevelInfo.ClothExp = int.Parse(item.Attributes["ClothExp"].Value);
					onlineArmLevelInfo.SwordExp = int.Parse(item.Attributes["SwordExp"].Value);
					onlineArmLevelInfo.ShieldExp = int.Parse(item.Attributes["ShieldExp"].Value);
					onlineArmLevelInfo.Attack = int.Parse(item.Attributes["Attack"].Value);
					onlineArmLevelInfo.Defence = int.Parse(item.Attributes["Defence"].Value);
					onlineArmLevelInfo.Agility = int.Parse(item.Attributes["Agility"].Value);
					onlineArmLevelInfo.Lucky = int.Parse(item.Attributes["Lucky"].Value);
					datas.Add(num, onlineArmLevelInfo);
				}
			}
			return true;
		}
		catch (Exception message)
		{
			log.Error(message);
		}
		return false;
	}

	private static bool LoadOnlineArmDrop(Dictionary<int, OnlineArmDropInfo> datas)
	{
		try
		{
			XmlDocument xmlDocument = Marshal.LoadXMLData("onlinearmdropitem", isEncrypt: true);
			XmlNodeList xmlNodeList = xmlDocument.SelectNodes("Result/child::node()");
			foreach (XmlNode item in xmlNodeList)
			{
				int num = int.Parse(item.Attributes["ID"].Value);
				if (!datas.ContainsKey(num))
				{
					OnlineArmDropInfo onlineArmDropInfo = new OnlineArmDropInfo();
					onlineArmDropInfo.ID = num;
					onlineArmDropInfo.FloorCount = int.Parse(item.Attributes["FloorCount"].Value);
					onlineArmDropInfo.PickAxeLevel = int.Parse(item.Attributes["PickAxeLevel"].Value);
					onlineArmDropInfo.Rate1 = int.Parse(item.Attributes["Rate1"].Value);
					onlineArmDropInfo.Rate2 = int.Parse(item.Attributes["Rate2"].Value);
					onlineArmDropInfo.Rate3 = int.Parse(item.Attributes["Rate3"].Value);
					onlineArmDropInfo.Rate4 = int.Parse(item.Attributes["Rate4"].Value);
					onlineArmDropInfo.Rate5 = int.Parse(item.Attributes["Rate5"].Value);
					onlineArmDropInfo.Rate6 = int.Parse(item.Attributes["Rate6"].Value);
					onlineArmDropInfo.Rate7 = int.Parse(item.Attributes["Rate7"].Value);
					onlineArmDropInfo.Rate8 = int.Parse(item.Attributes["Rate8"].Value);
					onlineArmDropInfo.Rate9 = int.Parse(item.Attributes["Rate9"].Value);
					onlineArmDropInfo.Rate10 = int.Parse(item.Attributes["Rate10"].Value);
					onlineArmDropInfo.Rate11 = int.Parse(item.Attributes["Rate11"].Value);
					datas.Add(num, onlineArmDropInfo);
				}
			}
			return true;
		}
		catch (Exception message)
		{
			log.Error(message);
		}
		return false;
	}

	private static bool LoadData()
	{
		m_data = Marshal.LoadDataFile<MinesManageDataInfo>("mines", isEncrypt: true);
		if (m_data == null)
		{
			m_data = new MinesManageDataInfo();
			m_data.LastTimeChange = DateTime.Now;
			m_data.ListShopDrop = new List<MineManageItemInfo>();
			m_data.ListShopExchange = new List<MineManageExchangeInfo>();
			ResetDropAndExchangeData();
		}
		return true;
	}

	private static bool SaveData()
	{
		return Marshal.SaveDataFile(m_data, "mines", isEncrypt: true);
	}

	public static List<ItemTemplateInfo> GetAllItemDig()
	{
		List<ItemTemplateInfo> list = new List<ItemTemplateInfo>();
		foreach (ItemTemplateInfo value in _itemDigList.Values)
		{
			list.Add(value);
		}
		return list;
	}

	public static OnlineArmDropInfo GetArmDropRate(int floor, int pickAxeLevel)
	{
		foreach (OnlineArmDropInfo value in _armDropList.Values)
		{
			if (value.FloorCount == floor && value.PickAxeLevel == pickAxeLevel)
			{
				return value;
			}
		}
		return null;
	}

	public static OnlineArmLevelInfo GetLevelArmWithExp(int exp, string type)
	{
		List<OnlineArmLevelInfo> list = _armLevelList.Values.OrderBy((OnlineArmLevelInfo a) => a.LevelCount).ToList();
		OnlineArmLevelInfo result = null;
		foreach (OnlineArmLevelInfo item in list)
		{
			switch (type)
			{
			case "pickaxe":
				if (item.PickAxeExp <= exp && (item.LevelCount <= 1 || item.PickAxeExp != 0))
				{
					result = item;
				}
				break;
			case "head":
				if (item.HeadExp <= exp)
				{
					result = item;
				}
				break;
			case "cloth":
				if (item.ClothExp <= exp)
				{
					result = item;
				}
				break;
			case "sword":
				if (item.SwordExp <= exp)
				{
					result = item;
				}
				break;
			case "shield":
				if (item.ShieldExp <= exp)
				{
					result = item;
				}
				break;
			}
		}
		return result;
	}

	public static ItemTemplateInfo GetItemDig(OnlineArmDropInfo rateDig, int rand)
	{
		ItemTemplateInfo result = null;
		if (rateDig.Rate11 > 0 && rand <= rateDig.Rate11)
		{
			result = _itemDigList[11];
		}
		else if (rateDig.Rate10 > 0 && rand <= rateDig.Rate10)
		{
			result = _itemDigList[10];
		}
		else if (rateDig.Rate9 > 0 && rand <= rateDig.Rate9)
		{
			result = _itemDigList[9];
		}
		else if (rateDig.Rate8 > 0 && rand <= rateDig.Rate8)
		{
			result = _itemDigList[8];
		}
		else if (rateDig.Rate7 > 0 && rand <= rateDig.Rate7)
		{
			result = _itemDigList[7];
		}
		else if (rateDig.Rate6 > 0 && rand <= rateDig.Rate6)
		{
			result = _itemDigList[6];
		}
		else if (rateDig.Rate5 > 0 && rand <= rateDig.Rate5)
		{
			result = _itemDigList[5];
		}
		else if (rateDig.Rate4 > 0 && rand <= rateDig.Rate4)
		{
			result = _itemDigList[4];
		}
		else if (rateDig.Rate3 > 0 && rand <= rateDig.Rate3)
		{
			result = _itemDigList[3];
		}
		else if (rateDig.Rate2 > 0 && rand <= rateDig.Rate2)
		{
			result = _itemDigList[2];
		}
		else if (rateDig.Rate1 > 0 && rand <= rateDig.Rate1)
		{
			result = _itemDigList[1];
		}
		return result;
	}

	public static List<MineManageItemInfo> GetListDrop()
	{
		List<MineManageItemInfo> list = new List<MineManageItemInfo>();
		lock (m_data)
		{
			foreach (MineManageItemInfo item in m_data.ListShopDrop)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public static List<MineManageExchangeInfo> GetListExchange()
	{
		List<MineManageExchangeInfo> list = new List<MineManageExchangeInfo>();
		lock (m_data)
		{
			foreach (MineManageExchangeInfo item in m_data.ListShopExchange)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public static MineManageItemInfo GetSingleDrop(int template)
	{
		lock (m_data)
		{
			return m_data.ListShopDrop.SingleOrDefault((MineManageItemInfo a) => a.TemplateID == template);
		}
	}

	public static MineManageExchangeInfo GetSingleExchange(int id)
	{
		lock (m_data)
		{
			return m_data.ListShopExchange.SingleOrDefault((MineManageExchangeInfo a) => a.ID == id);
		}
	}

	public static bool TrySell(int templateId, int count)
	{
		bool result = false;
		lock (m_lock)
		{
			MineManageItemInfo singleDrop = GetSingleDrop(templateId);
			if (singleDrop != null && singleDrop.CurrentCount >= count)
			{
				singleDrop.CurrentCount -= count;
				result = true;
			}
		}
		return result;
	}

	public static bool TryExchange(int id, int count)
	{
		bool result = false;
		lock (m_lock)
		{
			MineManageExchangeInfo singleExchange = GetSingleExchange(id);
			if (singleExchange != null && singleExchange.LimitCount >= count)
			{
				singleExchange.LimitCount -= count;
				result = true;
			}
		}
		return result;
	}
}
