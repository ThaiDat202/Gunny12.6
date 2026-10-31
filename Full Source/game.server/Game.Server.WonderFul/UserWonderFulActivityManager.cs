using System;
using System.Collections.Generic;
using System.Linq;
using EntityDatabase.PlayerModels;
using EntityDatabase.ServerModels;
using Game.Server.GameObjects;
using Game.Server.Managers;

namespace Game.Server.WonderFul;

public class UserWonderFulActivityManager
{
	private GamePlayer Player;

	private Dictionary<string, GM_Active_Condition> StrengthenActives = new Dictionary<string, GM_Active_Condition>();

	private Dictionary<string, GM_Active_Condition> PayActives = new Dictionary<string, GM_Active_Condition>();

	private Dictionary<string, GM_Active_Condition> GodTempActives = new Dictionary<string, GM_Active_Condition>();

	private Dictionary<string, GM_Active_Condition> BuildUpgradeActives = new Dictionary<string, GM_Active_Condition>();

	private Dictionary<string, GM_Active_Condition> MountMasterActives = new Dictionary<string, GM_Active_Condition>();

	public UserWonderFulActivityManager(GamePlayer player)
	{
		Player = player;
		Init();
	}

	public void Init()
	{
		foreach (GM_Activity item in GmActivityMgr.GmActivityInfos.FindAll((GM_Activity a) => a.endTime > DateTime.Now && a.beginTime <= DateTime.Now))
		{
			switch (item.activityType)
			{
			case 0:
				foreach (GM_Gift item2 in GmActivityMgr.FindGmGift(item.activityId))
				{
					PayActives.Add(item2.giftbagId, GmActivityMgr.FindGmActiveCondition(item2.giftbagId).First());
				}
				break;
			case 8:
				foreach (GM_Gift item3 in GmActivityMgr.FindGmGift(item.activityId))
				{
					StrengthenActives.Add(item3.giftbagId, GmActivityMgr.FindGmActiveCondition(item3.giftbagId).First());
				}
				break;
			case 20:
				foreach (GM_Gift item4 in GmActivityMgr.FindGmGift(item.activityId))
				{
					GodTempActives.Add(item4.giftbagId, GmActivityMgr.FindGmActiveCondition(item4.giftbagId).First());
				}
				break;
			case 25:
				foreach (GM_Gift item5 in GmActivityMgr.FindGmGift(item.activityId))
				{
					BuildUpgradeActives.Add(item5.giftbagId, GmActivityMgr.FindGmActiveCondition(item5.giftbagId).First());
				}
				break;
			case 14:
				foreach (GM_Gift item6 in GmActivityMgr.FindGmGift(item.activityId))
				{
					MountMasterActives.Add(item6.giftbagId, GmActivityMgr.FindGmActiveCondition(item6.giftbagId).First());
				}
				break;
			}
		}
		Player.AddFund += Player_AddMedal;
		Player.ItemStrengthen += Player_ItemStrengthen;
		Player.GodTempleImmolation += Player_GodTemple;
		Player.BuildGpgrade += Player_BuildGpgrade;
		Player.MountMaster += Player_MountMaster;
	}

	private void Player_MountMaster(int level)
	{
		foreach (KeyValuePair<string, GM_Active_Condition> act in MountMasterActives)
		{
			User_GmActivityCondition user_GmActivityCondition = Player.GmActivityConditions.Find((User_GmActivityCondition t) => t.GiftBagID == act.Key);
			if (user_GmActivityCondition.StatusID <= 0 || user_GmActivityCondition.StatusValue != 0)
			{
				user_GmActivityCondition.StatusValue = level;
				user_GmActivityCondition.StatusID = 1;
			}
		}
	}

	private void Player_GodTemple(int star, int level)
	{
		foreach (KeyValuePair<string, GM_Active_Condition> act in GodTempActives)
		{
			User_GmActivityCondition user_GmActivityCondition = Player.GmActivityConditions.Find((User_GmActivityCondition t) => t.GiftBagID == act.Key);
			if (user_GmActivityCondition.StatusID <= 0 || user_GmActivityCondition.StatusValue != 0)
			{
				user_GmActivityCondition.StatusValue = level;
				user_GmActivityCondition.StatusID = act.Value.remain1;
			}
		}
	}

	private void Player_BuildGpgrade(int level)
	{
		foreach (KeyValuePair<string, GM_Active_Condition> act in BuildUpgradeActives)
		{
			User_GmActivityCondition user_GmActivityCondition = Player.GmActivityConditions.Find((User_GmActivityCondition t) => t.GiftBagID == act.Key);
			if (user_GmActivityCondition.StatusID <= 0 || user_GmActivityCondition.StatusValue != 0)
			{
				user_GmActivityCondition.StatusValue = level;
				user_GmActivityCondition.StatusID = 1;
			}
		}
	}

	private void Player_ItemStrengthen(int categoryID, int level)
	{
		foreach (KeyValuePair<string, GM_Active_Condition> act in StrengthenActives)
		{
			User_GmActivityCondition user_GmActivityCondition = Player.GmActivityConditions.Find((User_GmActivityCondition t) => t.GiftBagID == act.Key);
			if ((user_GmActivityCondition.StatusID <= 0 || user_GmActivityCondition.StatusValue != 0) && act.Value.conditionValue == level)
			{
				user_GmActivityCondition.StatusValue = level;
				user_GmActivityCondition.StatusID = act.Value.conditionValue;
			}
		}
	}

	private void Player_AddMedal(int count, int type)
	{
		if (type != 4)
		{
			return;
		}
		foreach (KeyValuePair<string, GM_Active_Condition> act in PayActives)
		{
			User_GmActivityCondition user_GmActivityCondition = Player.GmActivityConditions.Find((User_GmActivityCondition t) => t.GiftBagID == act.Key);
			if (user_GmActivityCondition.StatusID <= 0 || user_GmActivityCondition.StatusValue != 0)
			{
				user_GmActivityCondition.StatusValue += count;
				user_GmActivityCondition.StatusID = act.Value.conditionValue;
			}
		}
	}
}
