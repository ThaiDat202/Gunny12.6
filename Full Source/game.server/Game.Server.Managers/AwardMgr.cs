using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using Game.Server.Buffer;
using Game.Server.GameObjects;
using Game.Server.Packets;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Managers;

public class AwardMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, DailyAwardInfo> _dailyAward;

	private static Dictionary<int, CampWarItemsInfo> _campAward;

	private static Dictionary<int, ThreeClearPointAwardInfo> _threeClearPointAwards;

	private static bool _dailyAwardState;

	private static Dictionary<int, SearchGoodsTempInfo> _searchGoodsTemp;

	private static ReaderWriterLock m_lock;

	public static bool DailyAwardState
	{
		get
		{
			return _dailyAwardState;
		}
		set
		{
			_dailyAwardState = value;
		}
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, DailyAwardInfo> dictionary = new Dictionary<int, DailyAwardInfo>();
			Dictionary<int, CampWarItemsInfo> dictionary2 = new Dictionary<int, CampWarItemsInfo>();
			Dictionary<int, ThreeClearPointAwardInfo> dictionary3 = new Dictionary<int, ThreeClearPointAwardInfo>();
			if (LoadDailyAward(dictionary, dictionary2, dictionary3))
			{
				try
				{
					_dailyAward = dictionary;
					_campAward = dictionary2;
					_threeClearPointAwards = dictionary3;
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
				log.Error("AwardMgr", exception);
			}
		}
		return false;
	}

	public static bool Init()
	{
		try
		{
			_dailyAward = new Dictionary<int, DailyAwardInfo>();
			_campAward = new Dictionary<int, CampWarItemsInfo>();
			_threeClearPointAwards = new Dictionary<int, ThreeClearPointAwardInfo>();
			_dailyAwardState = false;
			return LoadDailyAward(_dailyAward, _campAward, _threeClearPointAwards);
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("AwardMgr", exception);
			}
			return false;
		}
	}

	public static int MaxStar()
	{
		return _searchGoodsTemp.Count;
	}

	public static SearchGoodsTempInfo GetSearchGoodsTempInfo(int starId)
	{
		m_lock.AcquireReaderLock(10000);
		try
		{
			if (_searchGoodsTemp.ContainsKey(starId))
			{
				return _searchGoodsTemp[starId];
			}
		}
		catch
		{
		}
		finally
		{
			m_lock.ReleaseReaderLock();
		}
		SearchGoodsTempInfo searchGoodsTempInfo = null;
		return null;
	}

	public static List<CampWarItemsInfo> GetCampBattleAward(int rank, bool isCamp)
	{
		if (rank > 60)
		{
			rank = 60;
		}
		return _campAward.Values.Where((CampWarItemsInfo a) => a.MinRank >= rank && a.MaxRank <= rank && a.IsCamp == isCamp).ToList();
	}

	private static bool LoadDailyAward(Dictionary<int, DailyAwardInfo> awards, Dictionary<int, CampWarItemsInfo> camps, Dictionary<int, ThreeClearPointAwardInfo> threeClearAwards)
	{
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			DailyAwardInfo[] allDailyAward = produceBussiness.GetAllDailyAward();
			DailyAwardInfo[] array = allDailyAward;
			foreach (DailyAwardInfo dailyAwardInfo in array)
			{
				if (!awards.ContainsKey(dailyAwardInfo.ID))
				{
					awards.Add(dailyAwardInfo.ID, dailyAwardInfo);
				}
			}
			CampWarItemsInfo[] campWarItems = produceBussiness.GetCampWarItems();
			CampWarItemsInfo[] array2 = campWarItems;
			foreach (CampWarItemsInfo campWarItemsInfo in array2)
			{
				if (!camps.ContainsKey(campWarItemsInfo.ID))
				{
					camps.Add(campWarItemsInfo.ID, campWarItemsInfo);
				}
			}
			ThreeClearPointAwardInfo[] threeCleanPointAwards = produceBussiness.GetThreeCleanPointAwards();
			ThreeClearPointAwardInfo[] array3 = threeCleanPointAwards;
			foreach (ThreeClearPointAwardInfo threeClearPointAwardInfo in array3)
			{
				if (!threeClearAwards.ContainsKey(threeClearPointAwardInfo.ID))
				{
					threeClearAwards.Add(threeClearPointAwardInfo.ID, threeClearPointAwardInfo);
				}
			}
		}
		return true;
	}

	public static DailyAwardInfo[] GetAllAwardInfo()
	{
		DailyAwardInfo[] array = null;
		array = _dailyAward.Values.ToArray();
		return (array == null) ? new DailyAwardInfo[0] : array;
	}

	public static bool AddDailyAward(GamePlayer player)
	{
		if (DateTime.Now.Date == player.PlayerCharacter.LastAward.Date)
		{
			return false;
		}
		player.PlayerCharacter.DayLoginCount++;
		player.PlayerCharacter.LastAward = DateTime.Now;
		DailyAwardInfo[] allAwardInfo = GetAllAwardInfo();
		DailyAwardInfo[] array = allAwardInfo;
		foreach (DailyAwardInfo dailyAwardInfo in array)
		{
			if (dailyAwardInfo.Type == 0)
			{
				ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(dailyAwardInfo.TemplateID);
				if (itemTemplateInfo != null)
				{
					AbstractBuffer abstractBuffer = BufferList.CreateBufferHour(itemTemplateInfo, dailyAwardInfo.ValidDate);
					abstractBuffer.Start(player);
					return true;
				}
			}
		}
		return false;
	}

	public static bool AddSignAwards(GamePlayer player, int DailyLog)
	{
		DailyAwardInfo[] allAwardInfo = GetAllAwardInfo();
		StringBuilder stringBuilder = new StringBuilder();
		string value = string.Empty;
		bool flag = false;
		int templateId = 0;
		int num = 1;
		int validDate = 0;
		bool isBinds = true;
		bool result = false;
		DailyAwardInfo[] array = allAwardInfo;
		foreach (DailyAwardInfo dailyAwardInfo in array)
		{
			flag = true;
			switch (DailyLog)
			{
			case 3:
				if (dailyAwardInfo.Type == DailyLog)
				{
					num = dailyAwardInfo.Count;
					player.AddGiftToken(num);
					result = true;
				}
				break;
			case 9:
				if (dailyAwardInfo.Type == DailyLog)
				{
					templateId = dailyAwardInfo.TemplateID;
					num = dailyAwardInfo.Count;
					validDate = dailyAwardInfo.ValidDate;
					isBinds = dailyAwardInfo.IsBinds;
					result = true;
				}
				break;
			case 17:
				if (dailyAwardInfo.Type == DailyLog)
				{
					templateId = dailyAwardInfo.TemplateID;
					num = dailyAwardInfo.Count;
					validDate = dailyAwardInfo.ValidDate;
					isBinds = dailyAwardInfo.IsBinds;
					result = true;
				}
				break;
			case 26:
				if (dailyAwardInfo.Type == DailyLog)
				{
					templateId = dailyAwardInfo.TemplateID;
					num = dailyAwardInfo.Count;
					validDate = dailyAwardInfo.ValidDate;
					isBinds = dailyAwardInfo.IsBinds;
					result = true;
				}
				break;
			case 293:
				if (dailyAwardInfo.Type == DailyLog)
				{
					templateId = dailyAwardInfo.TemplateID;
					num = dailyAwardInfo.Count;
					validDate = dailyAwardInfo.ValidDate;
					isBinds = dailyAwardInfo.IsBinds;
					result = true;
				}
				break;
			}
		}
		ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(templateId);
		if (itemTemplateInfo != null)
		{
			int num2 = num;
			for (int j = 0; j < num2; j += itemTemplateInfo.MaxCount)
			{
				int count = ((j + itemTemplateInfo.MaxCount > num2) ? (num2 - j) : itemTemplateInfo.MaxCount);
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, count, 113);
				itemInfo.ValidDate = validDate;
				itemInfo.IsBinds = isBinds;
				if (!player.AddTemplate(itemInfo, itemInfo.Template.BagType, itemInfo.Count, eGameView.CaddyTypeGet))
				{
					flag = true;
					using PlayerBussiness playerBussiness = new PlayerBussiness();
					itemInfo.UserID = 0;
					playerBussiness.AddGoods(itemInfo);
					MailInfo mailInfo = new MailInfo();
					mailInfo.Annex1 = itemInfo.ItemID.ToString();
					mailInfo.Content = LanguageMgr.GetTranslation("AwardMgr.AddDailyAward.Content", itemInfo.Template.Name);
					mailInfo.Gold = 0;
					mailInfo.Money = 0;
					mailInfo.Receiver = player.PlayerCharacter.NickName;
					mailInfo.ReceiverID = player.PlayerCharacter.ID;
					mailInfo.Sender = mailInfo.Receiver;
					mailInfo.SenderID = mailInfo.ReceiverID;
					mailInfo.Title = LanguageMgr.GetTranslation("AwardMgr.AddDailyAward.Title", itemInfo.Template.Name);
					mailInfo.Type = 15;
					playerBussiness.SendMail(mailInfo, player.ZoneId);
					value = LanguageMgr.GetTranslation("AwardMgr.AddDailyAward.Mail");
				}
			}
		}
		if (flag && !string.IsNullOrEmpty(value))
		{
			player.Out.SendMailResponse(player.PlayerCharacter.ID, eMailRespose.Receiver);
		}
		return result;
	}

	public static ThreeClearPointAwardInfo FindThreeClearPointAward(int ID)
	{
		if (_threeClearPointAwards.ContainsKey(ID))
		{
			return _threeClearPointAwards[ID];
		}
		return null;
	}
}
