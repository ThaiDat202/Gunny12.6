using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class WorldEventMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, LuckyStartToptenAwardInfo> m_luckyStartToptenAward;

	private static Dictionary<int, LuckyStartToptenAwardInfo> m_lanternriddlesToptenAward;

	private static Random random = new Random();

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, LuckyStartToptenAwardInfo> dictionary = new Dictionary<int, LuckyStartToptenAwardInfo>();
			Dictionary<int, LuckyStartToptenAwardInfo> dictionary2 = new Dictionary<int, LuckyStartToptenAwardInfo>();
			if (LoadData(dictionary, dictionary2))
			{
				try
				{
					m_luckyStartToptenAward = dictionary;
					m_lanternriddlesToptenAward = dictionary2;
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
				log.Error("ReLoad", exception);
			}
		}
		return false;
	}

	public static bool Init()
	{
		try
		{
			m_luckyStartToptenAward = new Dictionary<int, LuckyStartToptenAwardInfo>();
			m_lanternriddlesToptenAward = new Dictionary<int, LuckyStartToptenAwardInfo>();
			return LoadData(m_luckyStartToptenAward, m_lanternriddlesToptenAward);
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("Init", exception);
			}
			return false;
		}
	}

	public static bool LoadData(Dictionary<int, LuckyStartToptenAwardInfo> luckyStarts, Dictionary<int, LuckyStartToptenAwardInfo> lanternriddles)
	{
		using (ActiveBussiness activeBussiness = new ActiveBussiness())
		{
			LuckyStartToptenAwardInfo[] allLuckyStartToptenAward = activeBussiness.GetAllLuckyStartToptenAward();
			LuckyStartToptenAwardInfo[] array = allLuckyStartToptenAward;
			foreach (LuckyStartToptenAwardInfo luckyStartToptenAwardInfo in array)
			{
				if (!luckyStarts.Keys.Contains(luckyStartToptenAwardInfo.ID))
				{
					luckyStarts.Add(luckyStartToptenAwardInfo.ID, luckyStartToptenAwardInfo);
				}
			}
			LuckyStartToptenAwardInfo[] allLanternriddlesTopTenAward = activeBussiness.GetAllLanternriddlesTopTenAward();
			LuckyStartToptenAwardInfo[] array2 = allLanternriddlesTopTenAward;
			foreach (LuckyStartToptenAwardInfo luckyStartToptenAwardInfo2 in array2)
			{
				if (!lanternriddles.Keys.Contains(luckyStartToptenAwardInfo2.ID))
				{
					lanternriddles.Add(luckyStartToptenAwardInfo2.ID, luckyStartToptenAwardInfo2);
				}
			}
		}
		return true;
	}

	public static List<LuckyStartToptenAwardInfo> GetLuckyStartToptenAward()
	{
		List<LuckyStartToptenAwardInfo> list = new List<LuckyStartToptenAwardInfo>();
		foreach (LuckyStartToptenAwardInfo value in m_luckyStartToptenAward.Values)
		{
			list.Add(value);
		}
		return list;
	}

	public static List<LuckyStartToptenAwardInfo> GetLuckyStartAwardByRank(int rank)
	{
		int num = 0;
		switch (rank)
		{
		case 1:
			num = 11;
			break;
		case 2:
			num = 12;
			break;
		case 3:
			num = 13;
			break;
		case 4:
		case 5:
			num = 14;
			break;
		case 6:
		case 7:
			num = 15;
			break;
		case 8:
		case 9:
		case 10:
			num = 16;
			break;
		}
		List<LuckyStartToptenAwardInfo> list = new List<LuckyStartToptenAwardInfo>();
		foreach (LuckyStartToptenAwardInfo value in m_luckyStartToptenAward.Values)
		{
			if (value.Type == num)
			{
				list.Add(value);
			}
		}
		return list;
	}

	public static List<LuckyStartToptenAwardInfo> GetLanternriddlesAwardByRank(int rank)
	{
		List<LuckyStartToptenAwardInfo> list = new List<LuckyStartToptenAwardInfo>();
		foreach (LuckyStartToptenAwardInfo value in m_lanternriddlesToptenAward.Values)
		{
			if (value.Type == rank)
			{
				list.Add(value);
			}
		}
		return list;
	}

	public static bool SendMailToUser(int userId, string NickName, int ZoneID, string title, string content)
	{
		using (PlayerBussiness playerBussiness = new PlayerBussiness())
		{
			MailInfo mail = new MailInfo
			{
				Content = content,
				Title = title,
				Gold = 0,
				IsExist = true,
				Money = 0,
				GiftToken = 0,
				Receiver = NickName,
				ReceiverID = userId,
				Sender = "Game Master",
				SenderID = 0,
				Type = 0,
				Annex1 = "",
				Annex1Name = ""
			};
			if (playerBussiness.SendMail(mail, ZoneID))
			{
				return true;
			}
		}
		return false;
	}

	public static bool SendItemsToMail(int[] TemplateIDs, string userInfo, string title, int zoneId)
	{
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		PlayerInfo userSingleByUserName = playerBussiness.GetUserSingleByUserName(userInfo);
		if (userSingleByUserName == null)
		{
			return false;
		}
		List<ItemInfo> list = new List<ItemInfo>();
		foreach (int templateId in TemplateIDs)
		{
			ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(templateId);
			if (itemTemplateInfo != null)
			{
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 102);
				itemInfo.Count = itemTemplateInfo.MaxCount;
				itemInfo.ValidDate = 0;
				itemInfo.IsBinds = true;
				list.Add(itemInfo);
			}
		}
		return SendItemsToMail(list, userSingleByUserName.ID, userSingleByUserName.NickName, zoneId, title, 83, null, null);
	}

	public static bool SendItemsToMail(int[] TemplateIDs, string userInfo, string title, int type, int zoneId)
	{
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		PlayerInfo userSingleByUserName = playerBussiness.GetUserSingleByUserName(userInfo);
		if (userSingleByUserName == null)
		{
			return false;
		}
		List<ItemInfo> list = new List<ItemInfo>();
		foreach (int templateId in TemplateIDs)
		{
			ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(templateId);
			if (itemTemplateInfo != null)
			{
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 102);
				itemInfo.Count = itemTemplateInfo.MaxCount;
				itemInfo.ValidDate = 0;
				itemInfo.IsBinds = true;
				list.Add(itemInfo);
			}
		}
		return SendItemsToMail(list, userSingleByUserName.ID, userSingleByUserName.NickName, zoneId, title, type, null, null);
	}

	public static bool SendItemToMail(ItemInfo info, int PlayerId, string Nickname, int zoneId, string title)
	{
		List<ItemInfo> list = new List<ItemInfo>();
		list.Add(info);
		return SendItemsToMail(list, PlayerId, Nickname, zoneId, title, 83, null, null);
	}

	public static bool SendItemsToMails(List<ItemInfo> infos, int PlayerId, string Nickname, int zoneId, string title)
	{
		return SendItemsToMail(infos, PlayerId, Nickname, zoneId, title, 83, null, null);
	}

	public static bool SendItemsToMails(List<ItemInfo> infos, int PlayerId, string Nickname, int zoneId, string title, string content)
	{
		return SendItemsToMail(infos, PlayerId, Nickname, zoneId, title, 83, null, content);
	}

	public static bool SendItemToMail(ItemInfo info, int PlayerId, string Nickname, int zoneId, string title, string sender)
	{
		List<ItemInfo> list = new List<ItemInfo>();
		list.Add(info);
		return SendItemsToMail(list, PlayerId, Nickname, zoneId, title, 83, sender, null);
	}

	public static bool SendItemsToMail(List<ItemInfo> infos, int PlayerId, string Nickname, int zoneId, string title, int type, string sender)
	{
		return SendItemsToMail(infos, PlayerId, Nickname, zoneId, title, 83, sender, null);
	}

	public static bool SendItemsToMail(List<ItemInfo> infos, int PlayerId, string Nickname, int zoneId, string title, string content)
	{
		return SendItemsToMail(infos, PlayerId, Nickname, zoneId, title, 83, null, content);
	}

	public static bool SendItemsToMail(List<ItemInfo> infos, int PlayerId, string Nickname, int zoneId, string title, int type, string sender, string content)
	{
		bool result = false;
		using (PlayerBussiness playerBussiness = new PlayerBussiness())
		{
			List<ItemInfo> list = new List<ItemInfo>();
			foreach (ItemInfo info in infos)
			{
				if (info.Template.MaxCount == 1 && info.Count > info.Template.MaxCount)
				{
					for (int i = 0; i < info.Count; i++)
					{
						ItemInfo itemInfo = ItemInfo.CloneFromTemplate(info.Template, info);
						itemInfo.Count = 1;
						list.Add(itemInfo);
					}
				}
				else
				{
					list.Add(info);
				}
			}
			for (int j = 0; j < list.Count; j += 5)
			{
				MailInfo mailInfo = new MailInfo
				{
					Title = title,
					Gold = 0,
					IsExist = true,
					Money = 0,
					Receiver = Nickname,
					ReceiverID = PlayerId,
					Sender = (string.IsNullOrEmpty(sender) ? LanguageMgr.GetTranslation("SystermSender.Msg") : sender),
					SenderID = 0,
					Type = type,
					GiftToken = 0
				};
				StringBuilder stringBuilder = new StringBuilder();
				StringBuilder stringBuilder2 = new StringBuilder();
				stringBuilder.Append(LanguageMgr.GetTranslation("Game.Server.GameUtils.CommonBag.AnnexRemark"));
				int num = j;
				if (list.Count > num)
				{
					ItemInfo itemInfo2 = list[num];
					if (itemInfo2.ItemID == 0)
					{
						playerBussiness.AddGoods(itemInfo2);
					}
					mailInfo.Annex1 = itemInfo2.ItemID.ToString();
					mailInfo.Annex1Name = itemInfo2.Template.Name;
					stringBuilder.Append("1、" + mailInfo.Annex1Name + "x" + itemInfo2.Count + ";");
					stringBuilder2.Append("1、" + mailInfo.Annex1Name + "x" + itemInfo2.Count + ";");
				}
				num = j + 1;
				if (list.Count > num)
				{
					ItemInfo itemInfo2 = list[num];
					if (itemInfo2.ItemID == 0)
					{
						playerBussiness.AddGoods(itemInfo2);
					}
					mailInfo.Annex2 = itemInfo2.ItemID.ToString();
					mailInfo.Annex2Name = itemInfo2.Template.Name;
					stringBuilder.Append("2、" + mailInfo.Annex2Name + "x" + itemInfo2.Count + ";");
					stringBuilder2.Append("2、" + mailInfo.Annex2Name + "x" + itemInfo2.Count + ";");
				}
				num = j + 2;
				if (list.Count > num)
				{
					ItemInfo itemInfo2 = list[num];
					if (itemInfo2.ItemID == 0)
					{
						playerBussiness.AddGoods(itemInfo2);
					}
					mailInfo.Annex3 = itemInfo2.ItemID.ToString();
					mailInfo.Annex3Name = itemInfo2.Template.Name;
					stringBuilder.Append("3、" + mailInfo.Annex3Name + "x" + itemInfo2.Count + ";");
					stringBuilder2.Append("3、" + mailInfo.Annex3Name + "x" + itemInfo2.Count + ";");
				}
				num = j + 3;
				if (list.Count > num)
				{
					ItemInfo itemInfo2 = list[num];
					if (itemInfo2.ItemID == 0)
					{
						playerBussiness.AddGoods(itemInfo2);
					}
					mailInfo.Annex4 = itemInfo2.ItemID.ToString();
					mailInfo.Annex4Name = itemInfo2.Template.Name;
					stringBuilder.Append("4、" + mailInfo.Annex4Name + "x" + itemInfo2.Count + ";");
					stringBuilder2.Append("4、" + mailInfo.Annex4Name + "x" + itemInfo2.Count + ";");
				}
				num = j + 4;
				if (list.Count > num)
				{
					ItemInfo itemInfo2 = list[num];
					if (itemInfo2.ItemID == 0)
					{
						playerBussiness.AddGoods(itemInfo2);
					}
					mailInfo.Annex5 = itemInfo2.ItemID.ToString();
					mailInfo.Annex5Name = itemInfo2.Template.Name;
					stringBuilder.Append("5、" + mailInfo.Annex5Name + "x" + itemInfo2.Count + ";");
					stringBuilder2.Append("5、" + mailInfo.Annex5Name + "x" + itemInfo2.Count + ";");
				}
				mailInfo.AnnexRemark = stringBuilder.ToString();
				mailInfo.Content = ((content == null) ? stringBuilder2.ToString() : content);
				result = playerBussiness.SendMail(mailInfo, zoneId);
			}
		}
		return result;
	}

	public static bool SendMoneyMailToUser(int UserID, string NickName, int ZoneId, string title, string content, int money)
	{
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		MailInfo mail = new MailInfo
		{
			Content = content,
			Title = title,
			Gold = 0,
			IsExist = true,
			Money = money,
			GiftToken = 0,
			Receiver = NickName,
			ReceiverID = UserID,
			Sender = LanguageMgr.GetTranslation("SystermSender.Msg"),
			SenderID = 0,
			Type = 0,
			Annex1 = "",
			Annex1Name = ""
		};
		return playerBussiness.SendMail(mail, ZoneId);
	}

	public static bool SendItemsToMailCrosszone(PlayerBussiness pb, List<ItemInfo> infos, int PlayerId, string Nickname, int zoneId, string title)
	{
		return SendItemsToMailCrosszone(pb, infos, PlayerId, Nickname, zoneId, title, 83, null, null);
	}

	public static bool SendItemsToMailCrosszone(PlayerBussiness pb, List<ItemInfo> infos, int PlayerId, string Nickname, int zoneId, string title, string content)
	{
		return SendItemsToMailCrosszone(pb, infos, PlayerId, Nickname, zoneId, title, 83, null, content);
	}

	public static bool SendItemsToMailCrosszone(PlayerBussiness pb, List<ItemInfo> infos, int PlayerId, string Nickname, int zoneId, string title, int type, string sender, string content)
	{
		bool result = false;
		List<ItemInfo> list = new List<ItemInfo>();
		foreach (ItemInfo info in infos)
		{
			if (info.Template.MaxCount == 1)
			{
				for (int i = 0; i < info.Count; i++)
				{
					ItemInfo itemInfo = ItemInfo.CloneFromTemplate(info.Template, info);
					itemInfo.Count = 1;
					list.Add(itemInfo);
				}
			}
			else
			{
				list.Add(info);
			}
		}
		for (int j = 0; j < list.Count; j += 5)
		{
			MailInfo mailInfo = new MailInfo
			{
				Title = title,
				Gold = 0,
				IsExist = true,
				Money = 0,
				Receiver = Nickname,
				ReceiverID = PlayerId,
				Sender = (string.IsNullOrEmpty(sender) ? LanguageMgr.GetTranslation("SystermSender.Msg") : sender),
				SenderID = 0,
				Type = type,
				GiftToken = 0
			};
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder.Append(LanguageMgr.GetTranslation("Game.Server.GameUtils.CommonBag.AnnexRemark"));
			int num = j;
			if (list.Count > num)
			{
				ItemInfo itemInfo2 = list[num];
				if (itemInfo2.ItemID == 0)
				{
					pb.AddGoods(itemInfo2);
				}
				mailInfo.Annex1 = itemInfo2.ItemID.ToString();
				mailInfo.Annex1Name = itemInfo2.Template.Name;
				stringBuilder.Append("1、" + mailInfo.Annex1Name + "x" + itemInfo2.Count + ";");
				stringBuilder2.Append("1、" + mailInfo.Annex1Name + "x" + itemInfo2.Count + ";");
			}
			num = j + 1;
			if (list.Count > num)
			{
				ItemInfo itemInfo2 = list[num];
				if (itemInfo2.ItemID == 0)
				{
					pb.AddGoods(itemInfo2);
				}
				mailInfo.Annex2 = itemInfo2.ItemID.ToString();
				mailInfo.Annex2Name = itemInfo2.Template.Name;
				stringBuilder.Append("2、" + mailInfo.Annex2Name + "x" + itemInfo2.Count + ";");
				stringBuilder2.Append("2、" + mailInfo.Annex2Name + "x" + itemInfo2.Count + ";");
			}
			num = j + 2;
			if (list.Count > num)
			{
				ItemInfo itemInfo2 = list[num];
				if (itemInfo2.ItemID == 0)
				{
					pb.AddGoods(itemInfo2);
				}
				mailInfo.Annex3 = itemInfo2.ItemID.ToString();
				mailInfo.Annex3Name = itemInfo2.Template.Name;
				stringBuilder.Append("3、" + mailInfo.Annex3Name + "x" + itemInfo2.Count + ";");
				stringBuilder2.Append("3、" + mailInfo.Annex3Name + "x" + itemInfo2.Count + ";");
			}
			num = j + 3;
			if (list.Count > num)
			{
				ItemInfo itemInfo2 = list[num];
				if (itemInfo2.ItemID == 0)
				{
					pb.AddGoods(itemInfo2);
				}
				mailInfo.Annex4 = itemInfo2.ItemID.ToString();
				mailInfo.Annex4Name = itemInfo2.Template.Name;
				stringBuilder.Append("4、" + mailInfo.Annex4Name + "x" + itemInfo2.Count + ";");
				stringBuilder2.Append("4、" + mailInfo.Annex4Name + "x" + itemInfo2.Count + ";");
			}
			num = j + 4;
			if (list.Count > num)
			{
				ItemInfo itemInfo2 = list[num];
				if (itemInfo2.ItemID == 0)
				{
					pb.AddGoods(itemInfo2);
				}
				mailInfo.Annex5 = itemInfo2.ItemID.ToString();
				mailInfo.Annex5Name = itemInfo2.Template.Name;
				stringBuilder.Append("5、" + mailInfo.Annex5Name + "x" + itemInfo2.Count + ";");
				stringBuilder2.Append("5、" + mailInfo.Annex5Name + "x" + itemInfo2.Count + ";");
			}
			mailInfo.AnnexRemark = stringBuilder.ToString();
			mailInfo.Content = ((content == null) ? stringBuilder2.ToString() : content);
			result = pb.SendMail(mailInfo, zoneId, useServiceClient: false);
		}
		return result;
	}
}
