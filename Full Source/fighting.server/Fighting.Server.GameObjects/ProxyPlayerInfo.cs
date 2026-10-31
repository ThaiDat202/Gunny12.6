using System;
using Bussiness.Managers;
using SqlDataProvider.Data;

namespace Fighting.Server.GameObjects;

public class ProxyPlayerInfo
{
	public int ServerId { get; set; }

	public int ZoneId { get; set; }

	public string ZoneName { get; set; }

	public double BaseAttack { get; set; }

	public double BaseDefence { get; set; }

	public double BaseAgility { get; set; }

	public double BaseBlood { get; set; }

	public int TemplateId { get; set; }

	public bool CanUserProp { get; set; }

	public int DragonBoatAddExpPlus { get; set; }

	public bool DragonBoatOpen { get; set; }

	public string TcpEndPoint { get; set; }

	public int SecondWeapon { get; set; }

	public int Weapon3D { get; set; }

	public int StrengthLevel { get; set; }

	public int CurExp { get; set; }

	public int Healstone { get; set; }

	public int HealstoneCount { get; set; }

	public double GPAddPlus { get; set; }

	public float GMExperienceRate { get; set; }

	public float AuncherExperienceRate { get; set; }

	public double OfferAddPlus { get; set; }

	public float GMOfferRate { get; set; }

	public float AuncherOfferRate { get; set; }

	public float GMRichesRate { get; set; }

	public float AuncherRichesRate { get; set; }

	public double AntiAddictionRate { get; set; }

	public string RedFootballStyle { get; set; }

	public string BlueFootballStyle { get; set; }

	public int GoldTemplateId { get; set; }

	public int WeaponStrengthLevel { get; set; }

	public DateTime goldBeginTime { get; set; }

	public int goldValidDate { get; set; }

	public ItemInfo Amulet { get; set; }

	public ItemInfo GetItemTemplateInfo()
	{
		ItemInfo itemInfo = null;
		if (TemplateId != 0)
		{
			itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(TemplateId), 1, 1);
			itemInfo.StrengthenLevel = WeaponStrengthLevel;
		}
		if (GoldTemplateId != 0)
		{
			itemInfo.GoldEquip = ItemMgr.FindItemTemplate(GoldTemplateId);
			itemInfo.goldBeginTime = goldBeginTime;
			itemInfo.goldValidDate = goldValidDate;
		}
		return itemInfo;
	}

	public ItemInfo GetSecondWeaponItemInfo()
	{
		ItemInfo itemInfo = null;
		if (SecondWeapon != 0)
		{
			itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(SecondWeapon), 1, 1);
			itemInfo.StrengthenLevel = StrengthLevel;
			itemInfo.curExp = CurExp;
		}
		return itemInfo;
	}

	public ItemInfo GetItem3DInfo()
	{
		ItemInfo result = null;
		if (Weapon3D != 0)
		{
			result = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(Weapon3D), 1, 1);
		}
		return result;
	}

	public ItemInfo GetHealstone()
	{
		ItemInfo itemInfo = null;
		if (Healstone != 0)
		{
			itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(Healstone), 1, 1);
			itemInfo.Count = HealstoneCount;
		}
		return itemInfo;
	}
}
