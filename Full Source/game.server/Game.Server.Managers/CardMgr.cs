using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Bussiness;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Managers;

public class CardMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static CardGrooveUpdateInfo[] m_grooveUpdate;

	private static Dictionary<int, List<CardGrooveUpdateInfo>> m_grooveUpdates;

	private static Dictionary<int, List<CardBuffInfo>> m_cardBuffs;

	private static Dictionary<int, CardInfo> m_cardInfo;

	private static CardTemplateInfo[] m_cardTemplate;

	private static Dictionary<int, List<CardTemplateInfo>> m_cardTemplates;

	private static Random random = new Random();

	private static ReaderWriterLock m_clientLocker = new ReaderWriterLock();

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, CardInfo> dictionary = LoadCardInfoDb();
			Dictionary<int, List<CardBuffInfo>> value = LoadCardBuffs(dictionary);
			if (dictionary.Count > 0)
			{
				Interlocked.Exchange(ref m_cardInfo, dictionary);
				Interlocked.Exchange(ref m_cardBuffs, value);
			}
			CardGrooveUpdateInfo[] array = LoadGrooveUpdateDb();
			Dictionary<int, List<CardGrooveUpdateInfo>> value2 = LoadGrooveUpdates(array);
			if (array.Length != 0)
			{
				Interlocked.Exchange(ref m_grooveUpdate, array);
				Interlocked.Exchange(ref m_grooveUpdates, value2);
			}
			CardTemplateInfo[] array2 = LoadCardBoxDb();
			Dictionary<int, List<CardTemplateInfo>> value3 = LoadCardBoxs(array2);
			if (array2.Length != 0)
			{
				Interlocked.Exchange(ref m_cardTemplate, array2);
				Interlocked.Exchange(ref m_cardTemplates, value3);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ReLoad CardMgr", exception);
			}
			return false;
		}
		return true;
	}

	public static bool Init()
	{
		return ReLoad();
	}

	public static CardInfo GetSingleCard(int id)
	{
		if (m_cardInfo.Count == 0)
		{
			Init();
		}
		if (m_cardInfo.ContainsKey(id))
		{
			return m_cardInfo[id];
		}
		return null;
	}

	public static Dictionary<int, CardInfo> LoadCardInfoDb()
	{
		Dictionary<int, CardInfo> dictionary = new Dictionary<int, CardInfo>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			CardInfo[] allCard = produceBussiness.GetAllCard();
			CardInfo[] array = allCard;
			foreach (CardInfo cardInfo in array)
			{
				if (!dictionary.ContainsKey(cardInfo.ID))
				{
					dictionary.Add(cardInfo.ID, cardInfo);
				}
			}
		}
		return dictionary;
	}

	public static CardBuffInfo[] LoadCardBuffDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetAllCardBuff();
	}

	public static Dictionary<int, List<CardBuffInfo>> LoadCardBuffs(Dictionary<int, CardInfo> Cards)
	{
		Dictionary<int, List<CardBuffInfo>> dictionary = new Dictionary<int, List<CardBuffInfo>>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			CardBuffInfo[] allCardBuff = produceBussiness.GetAllCardBuff();
			foreach (CardInfo Card in Cards.Values)
			{
				IEnumerable<CardBuffInfo> source = allCardBuff.Where((CardBuffInfo s) => s.CardID == Card.ID);
				dictionary.Add(Card.ID, source.ToList());
			}
		}
		return dictionary;
	}

	public static List<CardBuffInfo> FindCardBuff(int ID)
	{
		if (m_cardBuffs.ContainsKey(ID))
		{
			return m_cardBuffs[ID];
		}
		return null;
	}

	public static CardGrooveUpdateInfo[] LoadGrooveUpdateDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetAllCardGrooveUpdate();
	}

	public static Dictionary<int, List<CardGrooveUpdateInfo>> LoadGrooveUpdates(CardGrooveUpdateInfo[] GrooveUpdates)
	{
		Dictionary<int, List<CardGrooveUpdateInfo>> dictionary = new Dictionary<int, List<CardGrooveUpdateInfo>>();
		foreach (CardGrooveUpdateInfo info in GrooveUpdates)
		{
			if (!dictionary.Keys.Contains(info.Type))
			{
				IEnumerable<CardGrooveUpdateInfo> source = GrooveUpdates.Where((CardGrooveUpdateInfo s) => s.Type == info.Type);
				dictionary.Add(info.Type, source.ToList());
			}
		}
		return dictionary;
	}

	public static CardTemplateInfo[] LoadCardBoxDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetAllCardTemplate();
	}

	public static Dictionary<int, List<CardTemplateInfo>> LoadCardBoxs(CardTemplateInfo[] CardBoxs)
	{
		Dictionary<int, List<CardTemplateInfo>> dictionary = new Dictionary<int, List<CardTemplateInfo>>();
		foreach (CardTemplateInfo info in CardBoxs)
		{
			if (!dictionary.Keys.Contains(info.CardID))
			{
				IEnumerable<CardTemplateInfo> source = CardBoxs.Where((CardTemplateInfo s) => s.CardID == info.CardID);
				dictionary.Add(info.CardID, source.ToList());
			}
		}
		return dictionary;
	}

	public static CardTemplateInfo GetCard(int cardId)
	{
		CardTemplateInfo cardTemplateInfo = new CardTemplateInfo();
		List<CardTemplateInfo> list = FindCardBox(cardId);
		if (list == null)
		{
			return null;
		}
		int maxRound = ThreadSafeRandom.NextStatic(list.Select((CardTemplateInfo s) => s.probability).Max());
		List<CardTemplateInfo> list2 = list.Where((CardTemplateInfo s) => s.probability >= maxRound).ToList();
		if (list2.Count > 0)
		{
			cardTemplateInfo = list2[random.Next(0, list2.Count)];
		}
		if (cardTemplateInfo.CardType <= 0)
		{
			return null;
		}
		return cardTemplateInfo;
	}

	public static int[] GetRandomUnrepeatArray(int minValue, int maxValue, int count)
	{
		int[] array = new int[count];
		for (int i = 0; i < count; i++)
		{
			int num = random.Next(minValue, maxValue + 1);
			int num2 = 0;
			for (int j = 0; j < i; j++)
			{
				if (array[j] == num)
				{
					num2++;
				}
			}
			if (num2 == 0)
			{
				array[i] = num;
			}
			else
			{
				i--;
			}
		}
		return array;
	}

	public static List<CardTemplateInfo> FindCardBox(int cardId)
	{
		m_clientLocker.AcquireWriterLock(-1);
		try
		{
			if (m_cardTemplates.ContainsKey(cardId))
			{
				return m_cardTemplates[cardId];
			}
		}
		finally
		{
			m_clientLocker.ReleaseWriterLock();
		}
		return null;
	}

	public static CardTemplateInfo FindCardTemplate(int cardId, int type)
	{
		List<CardTemplateInfo> list = FindCardBox(cardId);
		if (list == null)
		{
			return null;
		}
		foreach (CardTemplateInfo item in list)
		{
			if (type == item.CardType)
			{
				return item;
			}
		}
		return null;
	}

	public static List<CardGrooveUpdateInfo> FindCardGrooveUpdate(int type)
	{
		if (m_grooveUpdates == null)
		{
			Init();
		}
		if (m_grooveUpdates.ContainsKey(type))
		{
			return m_grooveUpdates[type];
		}
		return null;
	}

	public static int CardCount()
	{
		return m_cardTemplate.Count();
	}

	public static int MaxLv(int type)
	{
		return FindCardGrooveUpdate(type).Count - 1;
	}

	public static int GetLevel(int GP, int type)
	{
		if (GP >= FindCardGrooveUpdate(type)[MaxLv(type)].Exp)
		{
			return FindCardGrooveUpdate(type)[MaxLv(type)].Level;
		}
		for (int i = 1; i <= MaxLv(type); i++)
		{
			if (GP < FindCardGrooveUpdate(type)[i].Exp)
			{
				int index = ((i - 1 != -1) ? (i - 1) : 0);
				return FindCardGrooveUpdate(type)[index].Level;
			}
		}
		return 0;
	}

	public static int GetProp(UsersCardInfo slot, int type)
	{
		int num = 0;
		for (int i = 0; i < slot.Level; i++)
		{
			num += GetGrooveSlot(slot.Type, i, type);
		}
		if (slot.CardID != 0)
		{
			num += GetPropCard(slot.CardType, slot.CardID, type);
		}
		return num;
	}

	public static int GetGrooveSlot(int type, int lv, int typeProp)
	{
		CardGrooveUpdateInfo[] grooveUpdate = m_grooveUpdate;
		foreach (CardGrooveUpdateInfo cardGrooveUpdateInfo in grooveUpdate)
		{
			if (cardGrooveUpdateInfo.Type == type && cardGrooveUpdateInfo.Level == lv)
			{
				switch (typeProp)
				{
				case 0:
					return cardGrooveUpdateInfo.Attack;
				case 1:
					return cardGrooveUpdateInfo.Defend;
				case 2:
					return cardGrooveUpdateInfo.Agility;
				case 3:
					return cardGrooveUpdateInfo.Lucky;
				case 4:
					return cardGrooveUpdateInfo.Damage;
				case 5:
					return cardGrooveUpdateInfo.Guard;
				}
			}
		}
		return 0;
	}

	public static int GetPropCard(int cardtype, int cardID, int type)
	{
		CardTemplateInfo[] cardTemplate = m_cardTemplate;
		foreach (CardTemplateInfo cardTemplateInfo in cardTemplate)
		{
			if (cardTemplateInfo.CardType == cardtype && cardTemplateInfo.CardID == cardID)
			{
				switch (type)
				{
				case 0:
					return cardTemplateInfo.AddAttack;
				case 1:
					return cardTemplateInfo.AddDefend;
				case 2:
					return cardTemplateInfo.AddAgility;
				case 3:
					return cardTemplateInfo.AddLucky;
				case 4:
					return cardTemplateInfo.AddDamage;
				case 5:
					return cardTemplateInfo.AddGuard;
				}
			}
		}
		return 0;
	}

	public static int GetGP(int level, int type)
	{
		for (int i = 1; i <= MaxLv(type); i++)
		{
			if (level == FindCardGrooveUpdate(type)[i].Level)
			{
				return FindCardGrooveUpdate(type)[i].Exp;
			}
		}
		return 0;
	}
}
