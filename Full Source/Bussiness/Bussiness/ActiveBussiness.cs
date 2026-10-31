using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SqlDataProvider.Data;

namespace Bussiness;

public class ActiveBussiness : BaseCrossBussiness
{
	public CloudBuyLotteryInfo[] GetAllCloudBuyLottery()
	{
		List<CloudBuyLotteryInfo> list = new List<CloudBuyLotteryInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Cloud_Buy_Lottery_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitCloudBuyLotteryInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("InitCloudBuyLotteryInfo", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public CloudBuyLotteryInfo InitCloudBuyLotteryInfo(SqlDataReader dr)
	{
		CloudBuyLotteryInfo cloudBuyLotteryInfo = new CloudBuyLotteryInfo();
		cloudBuyLotteryInfo.ID = (int)dr["ID"];
		cloudBuyLotteryInfo.templateId = (int)dr["templateId"];
		cloudBuyLotteryInfo.templatedIdCount = (int)dr["templatedIdCount"];
		cloudBuyLotteryInfo.validDate = (int)dr["validDate"];
		cloudBuyLotteryInfo.property = ((dr["property"] == null) ? "" : dr["property"].ToString());
		cloudBuyLotteryInfo.buyItemsArr = ((dr["buyItemsArr"] == null) ? "" : dr["buyItemsArr"].ToString());
		cloudBuyLotteryInfo.buyMoney = (int)dr["buyMoney"];
		cloudBuyLotteryInfo.maxNum = (int)dr["maxNum"];
		cloudBuyLotteryInfo.currentNum = (int)dr["currentNum"];
		return cloudBuyLotteryInfo;
	}

	public SubActiveInfo[] GetAllSubActive()
	{
		List<SubActiveInfo> list = new List<SubActiveInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_SubActive_All");
			while (ResultDataReader.Read())
			{
				SubActiveInfo subActiveInfo = new SubActiveInfo();
				subActiveInfo.ID = (int)ResultDataReader["ID"];
				subActiveInfo.ActiveID = (int)ResultDataReader["ActiveID"];
				subActiveInfo.SubID = (int)ResultDataReader["SubID"];
				subActiveInfo.IsOpen = (bool)ResultDataReader["IsOpen"];
				subActiveInfo.StartDate = (DateTime)ResultDataReader["StartDate"];
				subActiveInfo.StartTime = (DateTime)ResultDataReader["StartTime"];
				subActiveInfo.EndDate = (DateTime)ResultDataReader["EndDate"];
				subActiveInfo.EndTime = (DateTime)ResultDataReader["EndTime"];
				subActiveInfo.IsContinued = (bool)ResultDataReader["IsContinued"];
				subActiveInfo.ActiveInfo = ((ResultDataReader["ActiveInfo"] == null) ? "" : ResultDataReader["ActiveInfo"].ToString());
				list.Add(subActiveInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("Init AllSubActive", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public SubActiveConditionInfo[] GetAllSubActiveCondition(int ActiveID)
	{
		List<SubActiveConditionInfo> list = new List<SubActiveConditionInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@ActiveID", ActiveID)
			};
			db.GetReader(ref ResultDataReader, "SP_SubActiveCondition_All", sqlParameters);
			while (ResultDataReader.Read())
			{
				SubActiveConditionInfo subActiveConditionInfo = new SubActiveConditionInfo();
				subActiveConditionInfo.ID = (int)ResultDataReader["ID"];
				subActiveConditionInfo.ActiveID = (int)ResultDataReader["ActiveID"];
				subActiveConditionInfo.SubID = (int)ResultDataReader["SubID"];
				subActiveConditionInfo.ConditionID = (int)ResultDataReader["ConditionID"];
				subActiveConditionInfo.Type = (int)ResultDataReader["Type"];
				subActiveConditionInfo.Value = ((ResultDataReader["Value"] == null) ? "" : ResultDataReader["Value"].ToString());
				subActiveConditionInfo.AwardType = (int)ResultDataReader["AwardType"];
				subActiveConditionInfo.AwardValue = ((ResultDataReader["AwardValue"] == null) ? "" : ResultDataReader["AwardValue"].ToString());
				subActiveConditionInfo.IsValid = (bool)ResultDataReader["IsValid"];
				list.Add(subActiveConditionInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("Init AllSubActive", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public LuckyStartToptenAwardInfo[] GetAllLuckyStartToptenAward()
	{
		List<LuckyStartToptenAwardInfo> list = new List<LuckyStartToptenAwardInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_LuckyStart_Topten_Award_All");
			while (ResultDataReader.Read())
			{
				LuckyStartToptenAwardInfo luckyStartToptenAwardInfo = new LuckyStartToptenAwardInfo();
				luckyStartToptenAwardInfo.ID = (int)ResultDataReader["ID"];
				luckyStartToptenAwardInfo.Type = (int)ResultDataReader["Type"];
				luckyStartToptenAwardInfo.TemplateID = (int)ResultDataReader["TemplateID"];
				luckyStartToptenAwardInfo.Validate = (int)ResultDataReader["Validate"];
				luckyStartToptenAwardInfo.Count = (int)ResultDataReader["Count"];
				luckyStartToptenAwardInfo.StrengthenLevel = (int)ResultDataReader["StrengthenLevel"];
				luckyStartToptenAwardInfo.AttackCompose = (int)ResultDataReader["AttackCompose"];
				luckyStartToptenAwardInfo.DefendCompose = (int)ResultDataReader["DefendCompose"];
				luckyStartToptenAwardInfo.AgilityCompose = (int)ResultDataReader["AgilityCompose"];
				luckyStartToptenAwardInfo.LuckCompose = (int)ResultDataReader["LuckCompose"];
				luckyStartToptenAwardInfo.IsBinds = (bool)ResultDataReader["IsBind"];
				list.Add(luckyStartToptenAwardInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetLuckyStart_Topten_Award_All", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public LuckyStartToptenAwardInfo[] GetAllLanternriddlesTopTenAward()
	{
		List<LuckyStartToptenAwardInfo> list = new List<LuckyStartToptenAwardInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_LanternriddlesTopTenAward_All");
			while (ResultDataReader.Read())
			{
				LuckyStartToptenAwardInfo luckyStartToptenAwardInfo = new LuckyStartToptenAwardInfo();
				luckyStartToptenAwardInfo.ID = (int)ResultDataReader["ID"];
				luckyStartToptenAwardInfo.Type = (int)ResultDataReader["Type"];
				luckyStartToptenAwardInfo.TemplateID = (int)ResultDataReader["TemplateID"];
				luckyStartToptenAwardInfo.Validate = (int)ResultDataReader["Validate"];
				luckyStartToptenAwardInfo.Count = (int)ResultDataReader["Count"];
				luckyStartToptenAwardInfo.StrengthenLevel = (int)ResultDataReader["StrengthenLevel"];
				luckyStartToptenAwardInfo.AttackCompose = (int)ResultDataReader["AttackCompose"];
				luckyStartToptenAwardInfo.DefendCompose = (int)ResultDataReader["DefendCompose"];
				luckyStartToptenAwardInfo.AgilityCompose = (int)ResultDataReader["AgilityCompose"];
				luckyStartToptenAwardInfo.LuckCompose = (int)ResultDataReader["LuckCompose"];
				luckyStartToptenAwardInfo.IsBinds = (bool)ResultDataReader["IsBind"];
				list.Add(luckyStartToptenAwardInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetLuckyStart_Topten_Award_All", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public LightriddleQuestInfo[] GetAllLightriddleQuestInfo()
	{
		List<LightriddleQuestInfo> list = new List<LightriddleQuestInfo>();
		SqlDataReader ResultDataReader = null;
		int num = 1;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Lightriddle_Quest_All");
			while (ResultDataReader.Read())
			{
				LightriddleQuestInfo lightriddleQuestInfo = new LightriddleQuestInfo();
				lightriddleQuestInfo.QuestionID = (int)ResultDataReader["QuestionID"];
				lightriddleQuestInfo.QuestionContent = (string)ResultDataReader["QuestionContent"];
				lightriddleQuestInfo.Option1 = (string)ResultDataReader["Option1"];
				lightriddleQuestInfo.Option2 = (string)ResultDataReader["Option2"];
				lightriddleQuestInfo.Option3 = (string)ResultDataReader["Option3"];
				lightriddleQuestInfo.Option4 = (string)ResultDataReader["Option4"];
				lightriddleQuestInfo.OptionTrue = (int)ResultDataReader["OptionTrue"];
				list.Add(lightriddleQuestInfo);
				num++;
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("SP_Lightriddle_Quest_All", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public ActivityQuestInfo[] GetAllActivityQuest()
	{
		List<ActivityQuestInfo> list = new List<ActivityQuestInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Activity_Quest_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitActivityQuestInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("InitActivityQuestInfo", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public AtivityQuestConditionInfo[] GetAllAtivityQuestCondition()
	{
		List<AtivityQuestConditionInfo> list = new List<AtivityQuestConditionInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Ativity_Quest_Condition_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitAtivityQuestConditionInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("InitAtivityQuestConditionInfo", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public AtivityQuestConditionInfo InitAtivityQuestConditionInfo(SqlDataReader dr)
	{
		AtivityQuestConditionInfo ativityQuestConditionInfo = new AtivityQuestConditionInfo();
		ativityQuestConditionInfo.QuestID = (int)dr["QuestID"];
		ativityQuestConditionInfo.CondictionID = (int)dr["CondictionID"];
		ativityQuestConditionInfo.CondictionTitle = (int)dr["CondictionTitle"];
		ativityQuestConditionInfo.CondictionType = (int)dr["CondictionType"];
		ativityQuestConditionInfo.Para1 = (int)dr["Para1"];
		ativityQuestConditionInfo.Para2 = (int)dr["Para2"];
		ativityQuestConditionInfo.IndexType = (int)dr["IndexType"];
		return ativityQuestConditionInfo;
	}

	public ActivityQuestInfo InitActivityQuestInfo(SqlDataReader dr)
	{
		ActivityQuestInfo activityQuestInfo = new ActivityQuestInfo();
		activityQuestInfo.ID = (int)dr["ID"];
		activityQuestInfo.QuestType = (int)dr["QuestType"];
		activityQuestInfo.Title = (string)dr["Title"];
		activityQuestInfo.Detail = (string)dr["Detail"];
		activityQuestInfo.Objective = (string)dr["Objective"];
		activityQuestInfo.NeedMinLevel = (int)dr["NeedMinLevel"];
		activityQuestInfo.NeedMaxLevel = (int)dr["NeedMaxLevel"];
		activityQuestInfo.Period = (int)dr["Period"];
		return activityQuestInfo;
	}

	public ActivityHalloweenItemsInfo[] GetAllActivityHalloweenItems()
	{
		List<ActivityHalloweenItemsInfo> list = new List<ActivityHalloweenItemsInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Activity_Halloween_Items_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitActivityHalloweenItemsInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("InitActivityHalloweenItemsInfo", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public ActivityHalloweenItemsInfo InitActivityHalloweenItemsInfo(SqlDataReader dr)
	{
		ActivityHalloweenItemsInfo activityHalloweenItemsInfo = new ActivityHalloweenItemsInfo();
		activityHalloweenItemsInfo.rewardLevel = (int)dr["rewardLevel"];
		activityHalloweenItemsInfo.templateId = (int)dr["templateId"];
		activityHalloweenItemsInfo.count = (int)dr["count"];
		activityHalloweenItemsInfo.validate = (int)dr["validate"];
		activityHalloweenItemsInfo.strenthLevel = (int)dr["strenthLevel"];
		activityHalloweenItemsInfo.attack = (int)dr["attack"];
		activityHalloweenItemsInfo.defend = (int)dr["defend"];
		activityHalloweenItemsInfo.luck = (int)dr["luck"];
		activityHalloweenItemsInfo.agility = (int)dr["agility"];
		activityHalloweenItemsInfo.isBind = (bool)dr["isBind"];
		return activityHalloweenItemsInfo;
	}

	public bool AddActiveNumber(string AwardID, int ActiveID)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[3]
			{
				new SqlParameter("@AwardID", AwardID),
				new SqlParameter("@ActiveID", ActiveID),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[2].Direction = ParameterDirection.ReturnValue;
			result = db.RunProcedure("SP_Active_Number_Add", array);
			result = (int)array[2].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public ActiveInfo[] GetAllActives()
	{
		List<ActiveInfo> list = new List<ActiveInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Active_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitActiveInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public ActiveInfo GetSingleActives(int activeID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@ID", SqlDbType.Int, 4)
			};
			array[0].Value = activeID;
			db.GetReader(ref ResultDataReader, "SP_Active_Single", array);
			if (ResultDataReader.Read())
			{
				return InitActiveInfo(ResultDataReader);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return null;
	}

	public ActiveInfo InitActiveInfo(SqlDataReader reader)
	{
		ActiveInfo activeInfo = new ActiveInfo();
		activeInfo.ActiveID = (int)reader["ActiveID"];
		activeInfo.Description = ((reader["Description"] == null) ? "" : reader["Description"].ToString());
		activeInfo.Content = ((reader["Content"] == null) ? "" : reader["Content"].ToString());
		activeInfo.AwardContent = ((reader["AwardContent"] == null) ? "" : reader["AwardContent"].ToString());
		activeInfo.HasKey = (int)reader["HasKey"];
		if (!string.IsNullOrEmpty(reader["EndDate"].ToString()))
		{
			activeInfo.EndDate = (DateTime)reader["EndDate"];
		}
		activeInfo.IsOnly = (int)reader["IsOnly"];
		activeInfo.StartDate = (DateTime)reader["StartDate"];
		activeInfo.Title = reader["Title"].ToString();
		activeInfo.Type = (int)reader["Type"];
		activeInfo.ActiveType = (int)reader["ActiveType"];
		activeInfo.ActionTimeContent = ((reader["ActionTimeContent"] == null) ? "" : reader["ActionTimeContent"].ToString());
		activeInfo.IsAdvance = (bool)reader["IsAdvance"];
		activeInfo.GoodsExchangeTypes = ((reader["GoodsExchangeTypes"] == null) ? "" : reader["GoodsExchangeTypes"].ToString());
		activeInfo.GoodsExchangeNum = ((reader["GoodsExchangeNum"] == null) ? "" : reader["GoodsExchangeNum"].ToString());
		activeInfo.limitType = ((reader["limitType"] == null) ? "" : reader["limitType"].ToString());
		activeInfo.limitValue = ((reader["limitValue"] == null) ? "" : reader["limitValue"].ToString());
		activeInfo.IsShow = (bool)reader["IsShow"];
		activeInfo.IconID = (int)reader["IconID"];
		return activeInfo;
	}

	public ActiveAwardInfo[] GetAllActiveAwardInfo()
	{
		List<ActiveAwardInfo> list = new List<ActiveAwardInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Active_Award_All");
			while (ResultDataReader.Read())
			{
				ActiveAwardInfo item = new ActiveAwardInfo
				{
					ID = (int)ResultDataReader["ID"],
					ActiveID = (int)ResultDataReader["ActiveID"],
					AgilityCompose = (int)ResultDataReader["AgilityCompose"],
					AttackCompose = (int)ResultDataReader["AttackCompose"],
					Count = (int)ResultDataReader["Count"],
					DefendCompose = (int)ResultDataReader["DefendCompose"],
					Gold = (int)ResultDataReader["Gold"],
					ItemID = (int)ResultDataReader["ItemID"],
					LuckCompose = (int)ResultDataReader["LuckCompose"],
					Mark = (int)ResultDataReader["Mark"],
					Money = (int)ResultDataReader["Money"],
					Sex = (int)ResultDataReader["Sex"],
					StrengthenLevel = (int)ResultDataReader["StrengthenLevel"],
					ValidDate = (int)ResultDataReader["ValidDate"]
				};
				list.Add(item);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllActiveAwardInfo", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public ActiveConvertItemInfo[] GetAllActiveConvertItem()
	{
		List<ActiveConvertItemInfo> list = new List<ActiveConvertItemInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Active_Convert_Item_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitActiveConvertItemInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("InitActiveConvertItemInfo", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public ActiveConvertItemInfo[] GetSingleActiveConvertItems(int activeID)
	{
		List<ActiveConvertItemInfo> list = new List<ActiveConvertItemInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@ID", SqlDbType.Int, 4)
			};
			array[0].Value = activeID;
			db.GetReader(ref ResultDataReader, "SP_Active_Convert_Item_Info_Single", array);
			while (ResultDataReader.Read())
			{
				list.Add(InitActiveConvertItemInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public ActiveConvertItemInfo InitActiveConvertItemInfo(SqlDataReader reader)
	{
		ActiveConvertItemInfo activeConvertItemInfo = new ActiveConvertItemInfo();
		activeConvertItemInfo.ActiveID = (int)reader["ActiveID"];
		activeConvertItemInfo.TemplateID = (int)reader["TemplateID"];
		activeConvertItemInfo.ItemType = (int)reader["ItemType"];
		activeConvertItemInfo.ItemCount = (int)reader["ItemCount"];
		activeConvertItemInfo.LimitValue = (int)reader["LimitValue"];
		activeConvertItemInfo.IsBind = (bool)reader["IsBind"];
		activeConvertItemInfo.ValidDate = (int)reader["ValidDate"];
		return activeConvertItemInfo;
	}
}
