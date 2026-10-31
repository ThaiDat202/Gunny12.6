using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public static class ShopMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, ShopItemInfo> m_shop = new Dictionary<int, ShopItemInfo>();

	private static Dictionary<int, ShopGoodsShowListInfo> m_shopShowLists = new Dictionary<int, ShopGoodsShowListInfo>();

	private static Dictionary<int, List<ShopGoodsShowListInfo>> m_shopGoodsShowLists = new Dictionary<int, List<ShopGoodsShowListInfo>>();

	private static Dictionary<int, MinigameShopTemplateInfo> m_miniShop = new Dictionary<int, MinigameShopTemplateInfo>();

	public static bool Init()
	{
		return ReLoad();
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, ShopItemInfo> dictionary = LoadFromDatabase();
			Dictionary<int, ShopGoodsShowListInfo> dictionary2 = LoadShowListFromDatabase();
			Dictionary<int, MinigameShopTemplateInfo> dictionary3 = LoadMinigameShopDatabase();
			if (dictionary.Count > 0)
			{
				Interlocked.Exchange(ref m_shop, dictionary);
			}
			if (dictionary2.Count > 0)
			{
				Interlocked.Exchange(ref m_shopShowLists, dictionary2);
			}
			if (dictionary3.Count > 0)
			{
				Interlocked.Exchange(ref m_miniShop, dictionary3);
			}
			ShopGoodsShowListInfo[] array = LoadShopGoodsShowListDb();
			Dictionary<int, List<ShopGoodsShowListInfo>> value = LoadShopGoodsShowLists(array);
			if (array.Length != 0)
			{
				Interlocked.Exchange(ref m_shopGoodsShowLists, value);
			}
			return true;
		}
		catch (Exception exception)
		{
			log.Error("ShopInfoMgr", exception);
		}
		return false;
	}

	public static ShopGoodsShowListInfo[] LoadShopGoodsShowListDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetAllShopGoodsShowList();
	}

	public static Dictionary<int, List<ShopGoodsShowListInfo>> LoadShopGoodsShowLists(ShopGoodsShowListInfo[] ShopGoodsShowList)
	{
		Dictionary<int, List<ShopGoodsShowListInfo>> dictionary = new Dictionary<int, List<ShopGoodsShowListInfo>>();
		foreach (ShopGoodsShowListInfo info in ShopGoodsShowList)
		{
			if (!dictionary.Keys.Contains(info.Type))
			{
				IEnumerable<ShopGoodsShowListInfo> source = ShopGoodsShowList.Where((ShopGoodsShowListInfo s) => s.Type == info.Type);
				dictionary.Add(info.Type, source.ToList());
			}
		}
		return dictionary;
	}

	public static MinigameShopTemplateInfo FindMinigameShop(int ID)
	{
		if (m_miniShop.ContainsKey(ID))
		{
			return m_miniShop[ID];
		}
		return null;
	}

	public static List<ShopGoodsShowListInfo> FindShopGoodsShowList(int type)
	{
		if (m_shopGoodsShowLists.ContainsKey(type))
		{
			return m_shopGoodsShowLists[type];
		}
		return null;
	}

	private static Dictionary<int, ShopItemInfo> LoadFromDatabase()
	{
		Dictionary<int, ShopItemInfo> dictionary = new Dictionary<int, ShopItemInfo>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			ShopItemInfo[] aLllShop = produceBussiness.GetALllShop();
			ShopItemInfo[] array = aLllShop;
			foreach (ShopItemInfo shopItemInfo in array)
			{
				if (!dictionary.ContainsKey(shopItemInfo.ID))
				{
					dictionary.Add(shopItemInfo.ID, shopItemInfo);
				}
			}
		}
		return dictionary;
	}

	private static Dictionary<int, MinigameShopTemplateInfo> LoadMinigameShopDatabase()
	{
		Dictionary<int, MinigameShopTemplateInfo> dictionary = new Dictionary<int, MinigameShopTemplateInfo>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			MinigameShopTemplateInfo[] allMinigameShop = produceBussiness.GetAllMinigameShop();
			MinigameShopTemplateInfo[] array = allMinigameShop;
			foreach (MinigameShopTemplateInfo minigameShopTemplateInfo in array)
			{
				if (!dictionary.ContainsKey(minigameShopTemplateInfo.ID))
				{
					dictionary.Add(minigameShopTemplateInfo.ID, minigameShopTemplateInfo);
				}
			}
		}
		return dictionary;
	}

	private static Dictionary<int, ShopGoodsShowListInfo> LoadShowListFromDatabase()
	{
		Dictionary<int, ShopGoodsShowListInfo> dictionary = new Dictionary<int, ShopGoodsShowListInfo>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			ShopGoodsShowListInfo[] allShopGoodsShowList = produceBussiness.GetAllShopGoodsShowList();
			ShopGoodsShowListInfo[] array = allShopGoodsShowList;
			foreach (ShopGoodsShowListInfo shopGoodsShowListInfo in array)
			{
				if (!dictionary.ContainsKey(shopGoodsShowListInfo.ShopId))
				{
					dictionary.Add(shopGoodsShowListInfo.ShopId, shopGoodsShowListInfo);
				}
			}
		}
		return dictionary;
	}

	public static bool IsOnShop(int Id)
	{
		return IsOnShop(Id, 0);
	}

	public static bool IsOnShop(int Id, int isLimited)
	{
		if (m_shopShowLists == null)
		{
			Init();
		}
		if (IsSpecialItem(Id))
		{
			return true;
		}
		if (m_shop.ContainsKey(Id) && isLimited == 2)
		{
			return m_shop[Id].EndDate > DateTime.Now && m_shop[Id].IsCheap;
		}
		if (m_shopShowLists.ContainsKey(Id))
		{
			return true;
		}
		return false;
	}

	public static bool IsSpecialItem(int Id)
	{
		switch (Id)
		{
		case 1061501:
		case 1061601:
		case 1100401:
		case 1100801:
		case 1101201:
		case 1101601:
		case 1116201:
		case 1116301:
		case 1116701:
		case 1116801:
		case 1117301:
		case 1117701:
		case 4000301:
		case 4000601:
		case 4000801:
			return true;
		default:
			return false;
		}
	}

	public static ShopItemInfo GetShopItemInfoById(int ID)
	{
		if (m_shop.ContainsKey(ID))
		{
			return m_shop[ID];
		}
		return null;
	}

	public static ShopItemInfo GetShopItemDisCountById(int ID)
	{
		if (m_shop.ContainsKey(ID))
		{
			return m_shop[ID];
		}
		return null;
	}

	public static bool CanNotBuy(ItemTemplateInfo item)
	{
		if (item == null)
		{
			return false;
		}
		int categoryID = item.CategoryID;
		int num = categoryID;
		if ((uint)(num - 27) <= 2u || num == 31)
		{
			return FindShopbyTemplateID(item.TemplateID) == null;
		}
		return false;
	}

	public static bool CanBuy(int shopID, int consortiaShopLevel, ref bool isBinds, int cousortiaID, int playerRiches)
	{
		bool result = false;
		ConsortiaEquipControlInfo consortiaEquipControlInfo = null;
		using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
		{
			switch (shopID)
			{
			case 1:
				result = true;
				isBinds = false;
				break;
			case 2:
			case 3:
			case 4:
				result = true;
				isBinds = true;
				break;
			case 11:
				consortiaEquipControlInfo = consortiaBussiness.GetConsortiaEuqipRiches(cousortiaID, 1, 1);
				if (consortiaShopLevel >= consortiaEquipControlInfo.Level && playerRiches >= consortiaEquipControlInfo.Riches)
				{
					result = true;
					isBinds = true;
				}
				break;
			case 12:
				consortiaEquipControlInfo = consortiaBussiness.GetConsortiaEuqipRiches(cousortiaID, 2, 1);
				if (consortiaShopLevel >= consortiaEquipControlInfo.Level && playerRiches >= consortiaEquipControlInfo.Riches)
				{
					result = true;
					isBinds = true;
				}
				break;
			case 13:
				consortiaEquipControlInfo = consortiaBussiness.GetConsortiaEuqipRiches(cousortiaID, 3, 1);
				if (consortiaShopLevel >= consortiaEquipControlInfo.Level && playerRiches >= consortiaEquipControlInfo.Riches)
				{
					result = true;
					isBinds = true;
				}
				break;
			case 14:
				consortiaEquipControlInfo = consortiaBussiness.GetConsortiaEuqipRiches(cousortiaID, 4, 1);
				if (consortiaShopLevel >= consortiaEquipControlInfo.Level && playerRiches >= consortiaEquipControlInfo.Riches)
				{
					result = true;
					isBinds = true;
				}
				break;
			case 15:
				consortiaEquipControlInfo = consortiaBussiness.GetConsortiaEuqipRiches(cousortiaID, 5, 1);
				if (consortiaShopLevel >= consortiaEquipControlInfo.Level && playerRiches >= consortiaEquipControlInfo.Riches)
				{
					result = true;
					isBinds = true;
				}
				break;
			case 72:
			case 87:
			case 90:
			case 91:
			case 92:
			case 93:
			case 94:
			case 95:
			case 98:
			case 99:
			case 100:
			case 110:
			case 133:
				result = true;
				isBinds = true;
				break;
			default:
				Console.WriteLine("CanBuy: {0}", shopID);
				break;
			}
		}
		return result;
	}

	public static bool FindSpecialItemInfo(ItemInfo info, ref SpecialItemBoxInfo specialValue)
	{
		switch (info.TemplateID)
		{
		case -100:
			specialValue.Gold += info.Count;
			return true;
		case -200:
			specialValue.Money += info.Count;
			return true;
		case -1100:
			specialValue.DDTMoney += info.Count;
			return true;
		case -300:
			specialValue.Medal += info.Count;
			return true;
		case -800:
			specialValue.Honor += info.Count;
			return true;
		case -900:
			specialValue.HardCurrency += info.Count;
			return true;
		case -1000:
			specialValue.LeagueMoney += info.Count;
			return true;
		case -1200:
			specialValue.UseableScore += info.Count;
			return true;
		case -1300:
			specialValue.Prestge += info.Count;
			return true;
		case -1400:
			specialValue.MagicstoneScore += info.Count;
			return true;
		case -1500:
			specialValue.LoveScore += info.Count;
			return true;
		case -1600:
			specialValue.SummerScore += info.Count;
			return true;
		case -1700:
			specialValue.LaurelScore += info.Count;
			return true;
		case -1800:
			specialValue.EquestrianScore += info.Count;
			return true;
		default:
			return false;
		}
	}

	public static bool SetItemType(ShopItemInfo shop, int type, ref SpecialItemBoxInfo specialValue)
	{
		if (type == 1)
		{
			GetItemPrice(shop.APrice1, shop.AValue1, shop.Beat, ref specialValue);
			GetItemPrice(shop.APrice2, shop.AValue2, shop.Beat, ref specialValue);
			GetItemPrice(shop.APrice3, shop.AValue3, shop.Beat, ref specialValue);
		}
		if (type == 2)
		{
			GetItemPrice(shop.BPrice1, shop.BValue1, shop.Beat, ref specialValue);
			GetItemPrice(shop.BPrice2, shop.BValue2, shop.Beat, ref specialValue);
			GetItemPrice(shop.BPrice3, shop.BValue3, shop.Beat, ref specialValue);
		}
		if (type == 3)
		{
			GetItemPrice(shop.CPrice1, shop.CValue1, shop.Beat, ref specialValue);
			GetItemPrice(shop.CPrice2, shop.CValue2, shop.Beat, ref specialValue);
			GetItemPrice(shop.CPrice3, shop.CValue3, shop.Beat, ref specialValue);
		}
		return true;
	}

	public static bool SetItemType(ShopItemInfo shop, int type, ref int damageScore, ref int petScore, ref int iTemplateID, ref int iCount, ref int gold, ref int money, ref int offer, ref int gifttoken, ref int medal, ref int hardCurrency, ref int LeagueMoney, ref int useableScore, ref int scoreCatchInsect, ref int fishScore)
	{
		if (type == 1)
		{
			GetItemPrice(shop.APrice1, shop.AValue1, shop.Beat, ref damageScore, ref petScore, ref iTemplateID, ref iCount, ref gold, ref money, ref offer, ref gifttoken, ref medal, ref hardCurrency, ref LeagueMoney, ref useableScore, ref scoreCatchInsect, ref fishScore);
			GetItemPrice(shop.APrice2, shop.AValue2, shop.Beat, ref damageScore, ref petScore, ref iTemplateID, ref iCount, ref gold, ref money, ref offer, ref gifttoken, ref medal, ref hardCurrency, ref LeagueMoney, ref useableScore, ref scoreCatchInsect, ref fishScore);
			GetItemPrice(shop.APrice3, shop.AValue3, shop.Beat, ref damageScore, ref petScore, ref iTemplateID, ref iCount, ref gold, ref money, ref offer, ref gifttoken, ref medal, ref hardCurrency, ref LeagueMoney, ref useableScore, ref scoreCatchInsect, ref fishScore);
		}
		if (type == 2)
		{
			GetItemPrice(shop.BPrice1, shop.BValue1, shop.Beat, ref damageScore, ref petScore, ref iTemplateID, ref iCount, ref gold, ref money, ref offer, ref gifttoken, ref medal, ref hardCurrency, ref LeagueMoney, ref useableScore, ref scoreCatchInsect, ref fishScore);
			GetItemPrice(shop.BPrice2, shop.BValue2, shop.Beat, ref damageScore, ref petScore, ref iTemplateID, ref iCount, ref gold, ref money, ref offer, ref gifttoken, ref medal, ref hardCurrency, ref LeagueMoney, ref useableScore, ref scoreCatchInsect, ref fishScore);
			GetItemPrice(shop.BPrice3, shop.BValue3, shop.Beat, ref damageScore, ref petScore, ref iTemplateID, ref iCount, ref gold, ref money, ref offer, ref gifttoken, ref medal, ref hardCurrency, ref LeagueMoney, ref useableScore, ref scoreCatchInsect, ref fishScore);
		}
		if (type == 3)
		{
			GetItemPrice(shop.CPrice1, shop.CValue1, shop.Beat, ref damageScore, ref petScore, ref iTemplateID, ref iCount, ref gold, ref money, ref offer, ref gifttoken, ref medal, ref hardCurrency, ref LeagueMoney, ref useableScore, ref scoreCatchInsect, ref fishScore);
			GetItemPrice(shop.CPrice2, shop.CValue2, shop.Beat, ref damageScore, ref petScore, ref iTemplateID, ref iCount, ref gold, ref money, ref offer, ref gifttoken, ref medal, ref hardCurrency, ref LeagueMoney, ref useableScore, ref scoreCatchInsect, ref fishScore);
			GetItemPrice(shop.CPrice3, shop.CValue3, shop.Beat, ref damageScore, ref petScore, ref iTemplateID, ref iCount, ref gold, ref money, ref offer, ref gifttoken, ref medal, ref hardCurrency, ref LeagueMoney, ref useableScore, ref scoreCatchInsect, ref fishScore);
		}
		return true;
	}

	public static void GetItemPrice(int Prices, int Values, decimal beat, ref int damageScore, ref int petScore, ref int iTemplateID, ref int iCount, ref int gold, ref int money, ref int offer, ref int gifttoken, ref int medal, ref int hardCurrency, ref int LeagueMoney, ref int useableScore, ref int scoreCatchInsect, ref int fishScore)
	{
		if (Prices <= -1000)
		{
			switch (Prices)
			{
			case -2000:
				fishScore += (int)((decimal)Values * beat);
				return;
			case -1600:
				scoreCatchInsect += (int)((decimal)Values * beat);
				return;
			case -1200:
				useableScore += (int)((decimal)Values * beat);
				return;
			case -1000:
				LeagueMoney += (int)((decimal)Values * beat);
				return;
			}
		}
		else if (Prices <= -800)
		{
			switch (Prices)
			{
			case -900:
				hardCurrency += (int)((decimal)Values * beat);
				return;
			case -800:
				medal += (int)((decimal)Values * beat);
				return;
			}
		}
		else
		{
			if (Prices == -300)
			{
				medal += (int)((decimal)Values * beat);
				return;
			}
			switch (Prices)
			{
			case -8:
				petScore += (int)((decimal)Values * beat);
				return;
			case -7:
				damageScore += (int)((decimal)Values * beat);
				return;
			case -6:
				medal += (int)((decimal)Values * beat);
				return;
			case -4:
				offer += (int)((decimal)Values * beat);
				return;
			case -3:
				gold += (int)((decimal)Values * beat);
				return;
			case -2:
				gifttoken += (int)((decimal)Values * beat);
				return;
			case -1:
				money += (int)((decimal)Values * beat);
				return;
			}
		}
		if (Prices > 0)
		{
			iTemplateID = Prices;
			iCount = Values;
		}
	}

	public static void GetItemPrice(int Prices, int Values, decimal beat, ref SpecialItemBoxInfo specialValue)
	{
		switch (Prices)
		{
		case -1:
			specialValue.Money += (int)((decimal)Values * beat);
			return;
		case -2:
			specialValue.DDTMoney += (int)((decimal)Values * beat);
			return;
		case -3:
			specialValue.Gold += (int)((decimal)Values * beat);
			return;
		case -4:
			specialValue.Offer += (int)((decimal)Values * beat);
			return;
		case -6:
			return;
		case -7:
			specialValue.DamageScore += (int)((decimal)Values * beat);
			return;
		case -8:
			specialValue.PetScore += (int)((decimal)Values * beat);
			return;
		case -300:
			specialValue.Medal += (int)((decimal)Values * beat);
			return;
		case -800:
			specialValue.Honor += (int)((decimal)Values * beat);
			return;
		case -900:
			specialValue.HardCurrency += (int)((decimal)Values * beat);
			return;
		case -1000:
			specialValue.LeagueMoney += (int)((decimal)Values * beat);
			return;
		case -1200:
			specialValue.UseableScore += (int)((decimal)Values * beat);
			return;
		case -1300:
			specialValue.Prestge += (int)((decimal)Values * beat);
			return;
		case -1400:
			specialValue.MagicstoneScore += (int)((decimal)Values * beat);
			return;
		case -1500:
			specialValue.LoveScore += (int)((decimal)Values * beat);
			return;
		case -1600:
			specialValue.SummerScore += (int)((decimal)Values * beat);
			return;
		case -1700:
			specialValue.LaurelScore += (int)((decimal)Values * beat);
			return;
		case -1800:
			specialValue.EquestrianScore += (int)((decimal)Values * beat);
			return;
		case -2000:
			specialValue.FishScore += (int)((decimal)Values * beat);
			return;
		case -3900:
			specialValue.ElfCoin += (int)((decimal)Values * beat);
			return;
		}
		if (Prices > 0)
		{
			specialValue.iTemplateID = Prices;
			specialValue.iCount = Values;
		}
	}

	public static int FindItemTemplateID(int id)
	{
		if (m_shop.ContainsKey(id))
		{
			return m_shop[id].TemplateID;
		}
		return 0;
	}

	public static List<ShopItemInfo> FindShopbyTemplatID(int TemplatID)
	{
		List<ShopItemInfo> list = new List<ShopItemInfo>();
		foreach (ShopItemInfo value in m_shop.Values)
		{
			if (value.TemplateID == TemplatID)
			{
				list.Add(value);
			}
		}
		return list;
	}

	public static ShopItemInfo FindShopbyTemplateID(int TemplatID)
	{
		foreach (ShopItemInfo value in m_shop.Values)
		{
			if (value.TemplateID == TemplatID)
			{
				return value;
			}
		}
		return null;
	}

	public static List<ShopItemInfo> FindShopbyShopID(int ShopID)
	{
		List<ShopItemInfo> list = new List<ShopItemInfo>();
		foreach (ShopItemInfo value in m_shop.Values)
		{
			if (value.ShopID == ShopID)
			{
				list.Add(value);
			}
		}
		return list;
	}
}
