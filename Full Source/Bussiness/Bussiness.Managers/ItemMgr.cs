using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EntityDatabase.ServerModels;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class ItemMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, ItemTemplateInfo> _items;

	private static Dictionary<int, LoadUserBoxInfo> _timeBoxs;

	private static Dictionary<int, List<ItemTemplateInfo>> _magicStones;

	private static Dictionary<int, List<ItemTemplateInfo>> _runes;

	private static List<TS_UpgradeTemplate> _tsUpgrades;

	private static Dictionary<int, List<TS_UpgradeTemplate>> _totemUpgrades;

	private static Dictionary<int, TS_UpgradeTemplate> _sublimarPic;

	private static Dictionary<int, TS_UpgradeTemplate> _enchantSpiritDic;

	private static bool _isUpdate = true;

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, ItemTemplateInfo> dictionary = new Dictionary<int, ItemTemplateInfo>();
			Dictionary<int, LoadUserBoxInfo> dictionary2 = new Dictionary<int, LoadUserBoxInfo>();
			List<TS_UpgradeTemplate> list = new List<TS_UpgradeTemplate>();
			if (LoadItem(dictionary, dictionary2))
			{
				try
				{
					_items = dictionary;
					_timeBoxs = dictionary2;
					SyncTSUpgradeTemplate();
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

	public static int maxGradeSublimarImagemMontaria()
	{
		if (_sublimarPic.Count > 0)
		{
			int num = -1;
			foreach (int key in _sublimarPic.Keys)
			{
				if (key > num)
				{
					num = key;
				}
			}
			return num;
		}
		return -2;
	}

	public static TS_UpgradeTemplate getUpgradeSublimarImagemMontaria(int grade)
	{
		if (_sublimarPic.ContainsKey(grade))
		{
			return _sublimarPic[grade];
		}
		return null;
	}

	public static TS_UpgradeTemplate getUpgradeInjetarFusao(int grade)
	{
		if (_enchantSpiritDic.ContainsKey(grade))
		{
			return _enchantSpiritDic[grade];
		}
		return null;
	}

	public static bool Init(bool isUpdate)
	{
		try
		{
			_isUpdate = isUpdate;
			if (isUpdate)
			{
				LocalItemMgr.Init();
			}
			_items = new Dictionary<int, ItemTemplateInfo>();
			_timeBoxs = new Dictionary<int, LoadUserBoxInfo>();
			if (LoadItem(_items, _timeBoxs))
			{
				SyncTSUpgradeTemplate();
				return true;
			}
			return false;
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

	private static bool LoadTSUpgradeTemplate(List<TS_UpgradeTemplate> datas)
	{
		try
		{
			ServerData serverData = new ServerData();
			datas = serverData.TS_UpgradeTemplate.ToList();
			return true;
		}
		catch (Exception message)
		{
			log.Error(message);
		}
		return false;
	}

	private static void SyncTSUpgradeTemplate()
	{
		_totemUpgrades = new Dictionary<int, List<TS_UpgradeTemplate>>();
		_sublimarPic = new Dictionary<int, TS_UpgradeTemplate>();
		_enchantSpiritDic = new Dictionary<int, TS_UpgradeTemplate>();
		ServerData serverData = new ServerData();
		foreach (TS_UpgradeTemplate item in (IEnumerable<TS_UpgradeTemplate>)serverData.TS_UpgradeTemplate)
		{
			switch (item.Types)
			{
			case 101:
				if (!_totemUpgrades.ContainsKey(item.TemplateId))
				{
					_totemUpgrades.Add(item.TemplateId, new List<TS_UpgradeTemplate>());
				}
				_totemUpgrades[item.TemplateId].Add(item);
				break;
			case 104:
				if (!_enchantSpiritDic.ContainsKey(item.Grades))
				{
					_enchantSpiritDic.Add(item.Grades, item);
				}
				break;
			case 200:
				if (!_sublimarPic.ContainsKey(item.Grades))
				{
					_sublimarPic.Add(item.Grades, item);
				}
				break;
			}
		}
	}

	public static bool LoadMagicStones(List<ItemTemplateInfo> items)
	{
		_magicStones = new Dictionary<int, List<ItemTemplateInfo>>();
		foreach (ItemTemplateInfo info in items)
		{
			if (!_magicStones.Keys.Contains(info.Property2))
			{
				IEnumerable<ItemTemplateInfo> source = items.Where((ItemTemplateInfo s) => s.Property2 == info.Property2 && info.Property3 == 1);
				_magicStones.Add(info.Property2, source.ToList());
			}
		}
		return true;
	}

	public static bool LoadRunes(List<ItemTemplateInfo> items)
	{
		_runes = new Dictionary<int, List<ItemTemplateInfo>>();
		foreach (ItemTemplateInfo info in items)
		{
			if (!_runes.Keys.Contains(info.Property3))
			{
				IEnumerable<ItemTemplateInfo> source = items.Where((ItemTemplateInfo s) => s.Property3 == info.Property3 && info.Property1 == 31);
				_runes.Add(info.Property3, source.ToList());
			}
		}
		return true;
	}

	public static bool LoadItem(Dictionary<int, ItemTemplateInfo> infos, Dictionary<int, LoadUserBoxInfo> userBoxs)
	{
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			ItemTemplateInfo[] allGoods = produceBussiness.GetAllGoods();
			List<ItemTemplateInfo> list = new List<ItemTemplateInfo>();
			List<ItemTemplateInfo> list2 = new List<ItemTemplateInfo>();
			ItemTemplateInfo[] array = allGoods;
			foreach (ItemTemplateInfo itemTemplateInfo in array)
			{
				if (!infos.Keys.Contains(itemTemplateInfo.TemplateID))
				{
					infos.Add(itemTemplateInfo.TemplateID, itemTemplateInfo);
				}
				if (!LocalItemMgr.FindLocalItemTemplate(itemTemplateInfo.TemplateID))
				{
					LocalItemMgr.AddNewLocalItemTemplate(itemTemplateInfo);
				}
				if (Equip.isMagicStone(itemTemplateInfo.CategoryID))
				{
					list.Add(itemTemplateInfo);
				}
				if (itemTemplateInfo.Property1 == 31)
				{
					list2.Add(itemTemplateInfo);
				}
			}
			LoadMagicStones(list);
			LoadRunes(list2);
			LoadUserBoxInfo[] allTimeBoxAward = produceBussiness.GetAllTimeBoxAward();
			LoadUserBoxInfo[] array2 = allTimeBoxAward;
			foreach (LoadUserBoxInfo loadUserBoxInfo in array2)
			{
				if (!userBoxs.Keys.Contains(loadUserBoxInfo.ID))
				{
					userBoxs.Add(loadUserBoxInfo.ID, loadUserBoxInfo);
				}
			}
			LocalItemMgr.UpdateNewLocalItemToDb(_isUpdate);
		}
		return true;
	}

	public static List<ItemTemplateInfo> FindRuneTemplate(int Property3)
	{
		if (_runes == null)
		{
			Init(isUpdate: false);
		}
		if (_runes.Keys.Contains(Property3))
		{
			return _runes[Property3];
		}
		return null;
	}

	public static LoadUserBoxInfo FindItemBoxTypeAndLv(int type, int lv)
	{
		if (_timeBoxs == null)
		{
			Init(isUpdate: false);
		}
		foreach (LoadUserBoxInfo value in _timeBoxs.Values)
		{
			if (value.Type == type && value.Level == lv)
			{
				return value;
			}
		}
		return null;
	}

	public static List<ItemTemplateInfo> FindMagicStoneTemplate(int Group)
	{
		if (_magicStones == null)
		{
			Init(isUpdate: false);
		}
		if (_magicStones.Keys.Contains(Group))
		{
			return _magicStones[Group];
		}
		return null;
	}

	public static LoadUserBoxInfo FindItemBoxTemplate(int Id)
	{
		if (_timeBoxs == null)
		{
			Init(isUpdate: false);
		}
		if (_timeBoxs.Keys.Contains(Id))
		{
			return _timeBoxs[Id];
		}
		return null;
	}

	public static ItemTemplateInfo FindItemTemplate(int templateId)
	{
		if (_items == null)
		{
			Init(isUpdate: false);
		}
		if (_items.Keys.Contains(templateId))
		{
			return _items[templateId];
		}
		return null;
	}

	public static bool LoadMineItemDig(Dictionary<int, ItemTemplateInfo> datas)
	{
		if (_items == null)
		{
			Init(isUpdate: false);
		}
		foreach (ItemTemplateInfo value in _items.Values)
		{
			if (value.CategoryID == 81 && !datas.ContainsKey(value.Property1))
			{
				datas.Add(value.Property1, value);
			}
		}
		return true;
	}

	public static ItemTemplateInfo[] GetAllItemTemplate()
	{
		if (_items == null)
		{
			Init(isUpdate: false);
		}
		return _items.Values.ToArray();
	}

	public static ItemTemplateInfo FindGoldItemTemplate(int templateId, bool IsGold)
	{
		if (!IsGold)
		{
			return null;
		}
		GoldEquipTemplateInfo goldEquipTemplateInfo = GoldEquipMgr.FindGoldEquipByTemplate(templateId);
		if (goldEquipTemplateInfo == null)
		{
			return null;
		}
		if (_items == null)
		{
			Init(isUpdate: false);
		}
		if (_items.Keys.Contains(goldEquipTemplateInfo.NewTemplateId))
		{
			return _items[goldEquipTemplateInfo.NewTemplateId];
		}
		return null;
	}

	public static ItemTemplateInfo GetGoodsbyFusionTypeandQuality(int fusionType, int quality)
	{
		if (_items == null)
		{
			Init(isUpdate: false);
		}
		foreach (ItemTemplateInfo value in _items.Values)
		{
			if (value.FusionType == fusionType && value.Quality == quality)
			{
				return value;
			}
		}
		return null;
	}

	public static ItemTemplateInfo GetGoodsbyFusionTypeandLevel(int fusionType, int level)
	{
		if (_items == null)
		{
			Init(isUpdate: false);
		}
		foreach (ItemTemplateInfo value in _items.Values)
		{
			if (value.FusionType == fusionType && value.Level == level)
			{
				return value;
			}
		}
		return null;
	}

	public static List<ItemInfo> SpiltGoodsMaxCount(ItemInfo itemInfo)
	{
		List<ItemInfo> list = new List<ItemInfo>();
		for (int i = 0; i < itemInfo.Count; i += itemInfo.Template.MaxCount)
		{
			int count = ((itemInfo.Count < itemInfo.Template.MaxCount) ? itemInfo.Count : itemInfo.Template.MaxCount);
			ItemInfo itemInfo2 = itemInfo.Clone();
			itemInfo2.Count = count;
			list.Add(itemInfo2);
		}
		return list;
	}

	public static ItemTemplateInfo RandomMark(int minQ, int maxQ)
	{
		List<ItemTemplateInfo> markWithQuality = GetMarkWithQuality(minQ, maxQ);
		return markWithQuality.Random();
	}

	public static List<ItemTemplateInfo> GetMarkWithQuality(int minQuaity, int maxQuaity)
	{
		return _items.Values.Where((ItemTemplateInfo a) => a.CategoryID == 74 && a.Quality >= minQuaity && a.Quality <= maxQuaity && a.TemplateID <= 406465).ToList();
	}

	public static TS_UpgradeTemplate GetTotemUpGradeInfo(int templateId, int grade)
	{
		TS_UpgradeTemplate result = null;
		if (_totemUpgrades.ContainsKey(templateId))
		{
			return _totemUpgrades[templateId].SingleOrDefault((TS_UpgradeTemplate a) => a.Grades == grade);
		}
		return result;
	}
}
