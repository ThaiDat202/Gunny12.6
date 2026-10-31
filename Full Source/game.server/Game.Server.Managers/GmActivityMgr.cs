using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bussiness;
using EntityDatabase.ServerModels;
using log4net;

namespace Game.Server.Managers;

public class GmActivityMgr
{
	private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public static List<GM_Activity> GmActivityInfos;

	public static List<GM_Gift> GmGiftInfos;

	public static List<GM_Active_Condition> GmActiveConditionInfos;

	public static List<GM_Active_Reward> GmActiveRewardInfos;

	public static List<GM_Active_Condition> FindGmActiveCondition(string giftId)
	{
		return GmActiveConditionInfos.FindAll((GM_Active_Condition q) => q.giftbagId == giftId);
	}

	public static List<GM_Active_Reward> FindGmActiveReward(string giftId)
	{
		return GmActiveRewardInfos.FindAll((GM_Active_Reward q) => q.giftId == giftId);
	}

	public static GM_Activity FindGmActivity(string activityId)
	{
		return GmActivityInfos.Find((GM_Activity q) => q.activityId == activityId);
	}

	public static List<GM_Gift> FindGmGift(string activeId)
	{
		return GmGiftInfos.FindAll((GM_Gift q) => q.activityId == activeId);
	}

	public static bool Init()
	{
		try
		{
			ProduceBussiness produceBussiness = new ProduceBussiness();
			GmActivityInfos = produceBussiness.GetAllGmActivity().ToList();
			GmGiftInfos = produceBussiness.GetAllGmGift().ToList();
			GmActiveConditionInfos = produceBussiness.GetAllGmActiveCondition().ToList();
			GmActiveRewardInfos = produceBussiness.GetAllGmActiveReward().ToList();
			return true;
		}
		catch (Exception exception)
		{
			if (Log.IsErrorEnabled)
			{
				Log.Error("GmActivityMgr Init Error:", exception);
			}
			return false;
		}
	}
}
