using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Bussiness;
using Game.Logic;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Statics;

public class LogMgr
{
	public static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static object _syncStop;

	private static int _gameType;

	private static int _serverId;

	private static int _areaId;

	public static DataTable m_LogItem;

	public static DataTable m_LogMoney;

	public static DataTable m_LogFight;

	public static DataTable m_LogDropItem;

	private static Dictionary<int, long> m_player = new Dictionary<int, long>();

	public static int GetMoney(int id)
	{
		lock (m_player)
		{
			if (m_player.ContainsKey(id))
			{
				return (int)m_player[id];
			}
		}
		return 0;
	}

	public static bool IsExitLogMoney(int id, int money)
	{
		lock (m_player)
		{
			if (!m_player.ContainsKey(id))
			{
				m_player.Add(id, money);
				return false;
			}
		}
		return true;
	}

	public static int AddMoney(int id, int money)
	{
		lock (m_player)
		{
			if (m_player.ContainsKey(id) && money > 0)
			{
				m_player[id] += money;
				return money;
			}
		}
		return 0;
	}

	public static int RemoveMoney(int id, int money)
	{
		lock (m_player)
		{
			if (m_player.ContainsKey(id) && m_player[id] >= money)
			{
				m_player[id] -= money;
				return money;
			}
		}
		return 0;
	}

	public static bool Setup(int gametype, int serverid, int areaid)
	{
		_gameType = gametype;
		_serverId = serverid;
		_areaId = areaid;
		_syncStop = new object();
		m_player = new Dictionary<int, long>();
		m_LogItem = new DataTable("Log_Item");
		m_LogItem.Columns.Add("ApplicationId", Type.GetType("System.Int32"));
		m_LogItem.Columns.Add("SubId", typeof(int));
		m_LogItem.Columns.Add("LineId", typeof(int));
		m_LogItem.Columns.Add("EnterTime", Type.GetType("System.DateTime"));
		m_LogItem.Columns.Add("UserId", typeof(int));
		m_LogItem.Columns.Add("Operation", typeof(int));
		m_LogItem.Columns.Add("ItemName", typeof(string));
		m_LogItem.Columns.Add("ItemID", typeof(int));
		m_LogItem.Columns.Add("AddItem", typeof(string));
		m_LogItem.Columns.Add("BeginProperty", typeof(string));
		m_LogItem.Columns.Add("EndProperty", typeof(string));
		m_LogItem.Columns.Add("Result", typeof(int));
		m_LogMoney = new DataTable("Log_Money");
		m_LogMoney.Columns.Add("ApplicationId", typeof(int));
		m_LogMoney.Columns.Add("SubId", typeof(int));
		m_LogMoney.Columns.Add("LineId", typeof(int));
		m_LogMoney.Columns.Add("MastType", typeof(int));
		m_LogMoney.Columns.Add("SonType", typeof(int));
		m_LogMoney.Columns.Add("UserId", typeof(int));
		m_LogMoney.Columns.Add("EnterTime", Type.GetType("System.DateTime"));
		m_LogMoney.Columns.Add("Moneys", typeof(int));
		m_LogMoney.Columns.Add("Gold", typeof(int));
		m_LogMoney.Columns.Add("GiftToken", typeof(int));
		m_LogMoney.Columns.Add("Offer", typeof(int));
		m_LogMoney.Columns.Add("OtherPay", typeof(string));
		m_LogMoney.Columns.Add("GoodId", typeof(int));
		m_LogMoney.Columns.Add("ShopId", typeof(int));
		m_LogMoney.Columns.Add("Datas", typeof(int));
		m_LogFight = new DataTable("Log_Fight");
		m_LogFight.Columns.Add("ApplicationId", typeof(int));
		m_LogFight.Columns.Add("SubId", typeof(int));
		m_LogFight.Columns.Add("LineId", typeof(int));
		m_LogFight.Columns.Add("RoomId", typeof(int));
		m_LogFight.Columns.Add("RoomType", typeof(int));
		m_LogFight.Columns.Add("FightType", typeof(int));
		m_LogFight.Columns.Add("ChangeTeam", typeof(int));
		m_LogFight.Columns.Add("PlayBegin", Type.GetType("System.DateTime"));
		m_LogFight.Columns.Add("PlayEnd", Type.GetType("System.DateTime"));
		m_LogFight.Columns.Add("UserCount", typeof(int));
		m_LogFight.Columns.Add("MapId", typeof(int));
		m_LogFight.Columns.Add("TeamA", typeof(string));
		m_LogFight.Columns.Add("TeamB", typeof(string));
		m_LogFight.Columns.Add("PlayResult", typeof(string));
		m_LogFight.Columns.Add("WinTeam", typeof(int));
		m_LogFight.Columns.Add("Detail", typeof(string));
		m_LogDropItem = new DataTable("Log_DropItem");
		m_LogDropItem.Columns.Add("ApplicationId", typeof(int));
		m_LogDropItem.Columns.Add("SubId", typeof(int));
		m_LogDropItem.Columns.Add("LineId", typeof(int));
		m_LogDropItem.Columns.Add("UserId", typeof(int));
		m_LogDropItem.Columns.Add("ItemId", typeof(int));
		m_LogDropItem.Columns.Add("TemplateID", typeof(int));
		m_LogDropItem.Columns.Add("DropId", typeof(int));
		m_LogDropItem.Columns.Add("DropData", typeof(int));
		m_LogDropItem.Columns.Add("EnterTime", Type.GetType("System.DateTime"));
		return true;
	}

	public static void Reset()
	{
		lock (m_LogItem)
		{
			m_LogItem.Clear();
		}
		lock (m_LogMoney)
		{
			m_LogMoney.Clear();
		}
		lock (m_LogFight)
		{
			m_LogFight.Clear();
		}
	}

	public static void Save()
	{
		if (_syncStop == null)
		{
			return;
		}
		lock (_syncStop)
		{
			using (new ItemRecordBussiness())
			{
			}
		}
	}

	public static void SaveLogItem(ItemRecordBussiness db)
	{
		lock (m_LogItem)
		{
			db.LogItemDb(m_LogItem);
		}
	}

	public static void SaveLogMoney(ItemRecordBussiness db)
	{
		lock (m_LogMoney)
		{
			db.LogMoneyDb(m_LogMoney);
		}
	}

	public static void SaveLogFight(ItemRecordBussiness db)
	{
		lock (m_LogFight)
		{
			db.LogFightDb(m_LogFight);
		}
	}

	public static void SaveLogDropItem(ItemRecordBussiness db)
	{
		lock (m_LogDropItem)
		{
			db.LogDropItemDb(m_LogDropItem);
		}
	}

	public static void LogItemAdd(int userId, LogItemType itemType, string beginProperty, ItemInfo item, string AddItem, int result)
	{
		try
		{
			string text = "";
			if (item != null)
			{
				text = $"{item.StrengthenLevel},{item.Attack},{item.Defence},{item.Agility},{item.Luck},{item.AttackCompose},{item.DefendCompose},{item.AgilityCompose},{item.LuckCompose}";
			}
			object[] values = new object[12]
			{
				_gameType,
				_serverId,
				_areaId,
				DateTime.Now,
				userId,
				(int)itemType,
				(item == null) ? "" : item.Template.Name,
				item?.ItemID ?? 0,
				AddItem,
				beginProperty,
				text,
				result
			};
			lock (m_LogItem)
			{
				m_LogItem.Rows.Add(values);
			}
		}
		catch (Exception ex)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("LogMgr Error：ItemAdd @ " + ex);
			}
		}
	}

	public static void LogMoneyAdd(LogMoneyType masterType, LogMoneyType sonType, int userId, int moneys, int SpareMoney, int gold, int giftToken, int offer, int medal, string otherPay, int goodId, int shopId)
	{
		try
		{
			if (moneys != 0)
			{
				switch (sonType)
				{
				case LogMoneyType.Auction_Update:
				case LogMoneyType.Mail_Pay:
				case LogMoneyType.Mail_Send:
				case LogMoneyType.Shop_Buy:
				case LogMoneyType.Shop_Continue:
				case LogMoneyType.Shop_Card:
				case LogMoneyType.Shop_Present:
				case LogMoneyType.Marry_Spark:
				case LogMoneyType.Marry_Gift:
				case LogMoneyType.Marry_Unmarry:
				case LogMoneyType.Marry_Room:
				case LogMoneyType.Marry_RoomAdd:
				case LogMoneyType.Marry_Flower:
				case LogMoneyType.Marry_Hymeneal:
				case LogMoneyType.Consortia_Rich:
				case LogMoneyType.Item_Move:
				case LogMoneyType.Item_Color:
				case LogMoneyType.Game_Boos:
				case LogMoneyType.Game_PaymentTakeCard:
				case LogMoneyType.Game_TryAgain:
				case LogMoneyType.Mail_Hack:
				case LogMoneyType.Mail_Paied:
					moneys *= -1;
					break;
				}
				object[] array = new object[15]
				{
					_gameType,
					_serverId,
					_areaId,
					masterType,
					sonType,
					userId,
					DateTime.Now,
					moneys,
					gold,
					giftToken,
					offer,
					otherPay,
					goodId,
					shopId,
					SpareMoney
				};
				lock (m_LogMoney)
				{
					return;
				}
			}
		}
		catch (Exception ex)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("LogMgr Error：LogMoney @ " + ex);
			}
		}
	}

	public static void LogFightAdd(int roomId, eRoomType roomType, eGameType fightType, int changeTeam, DateTime playBegin, DateTime playEnd, int userCount, int mapId, string teamA, string teamB, string playResult, int winTeam, string BossWar)
	{
		try
		{
			object[] values = new object[16]
			{
				_gameType,
				_serverId,
				_areaId,
				roomId,
				(int)roomType,
				(int)fightType,
				changeTeam,
				playBegin,
				playEnd,
				userCount,
				mapId,
				teamA,
				teamB,
				playResult,
				winTeam,
				BossWar
			};
			lock (m_LogFight)
			{
				m_LogFight.Rows.Add(values);
			}
		}
		catch (Exception ex)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("LogMgr Error：Fight @ " + ex);
			}
		}
	}

	public static void LogDropItemAdd(int userId, int itemId, int templateId, int dropId, int dropData)
	{
		try
		{
			object[] values = new object[9]
			{
				_gameType,
				_areaId,
				_serverId,
				userId,
				itemId,
				templateId,
				dropId,
				dropData,
				DateTime.Now
			};
			lock (m_LogDropItem)
			{
				m_LogDropItem.Rows.Add(values);
			}
		}
		catch (Exception ex)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("LogMgr Error：LogDropItem @ " + ex);
			}
		}
	}
}
