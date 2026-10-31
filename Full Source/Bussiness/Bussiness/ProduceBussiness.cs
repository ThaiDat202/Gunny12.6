using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using EntityDatabase.ServerModels;
using SqlDataProvider.BaseClass;
using SqlDataProvider.Data;

namespace Bussiness;

public class ProduceBussiness : BaseCrossBussiness
{
	private string AreaName = "Area 1";

	private int ZoneID = 1;

	public DailyLeagueAwardItems[] GetAllDailyLeagueAwardItems()
	{
		List<DailyLeagueAwardItems> list = new List<DailyLeagueAwardItems>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Get_DailyLeagueAwardItems");
			while (ResultDataReader.Read())
			{
				DailyLeagueAwardItems item = new DailyLeagueAwardItems
				{
					ID = (int)ResultDataReader["ID"],
					Class = (int)ResultDataReader["Class"],
					TemplateID = (int)ResultDataReader["TemplateID"],
					StrengthLevel = (int)ResultDataReader["StrengthLevel"],
					AttackCompose = (int)ResultDataReader["AttackCompose"],
					DefendCompose = (int)ResultDataReader["DefendCompose"],
					LuckCompose = (int)ResultDataReader["LuckCompose"],
					AgilityCompose = (int)ResultDataReader["AgilityCompose"],
					IsBind = (bool)ResultDataReader["IsBind"],
					ValidDate = (int)ResultDataReader["ValidDate"],
					Count = (int)ResultDataReader["Count"]
				};
				list.Add(item);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllDailyLeagueAwardItems", exception);
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

	public ShopItemInfo[] method_0()
	{
		List<ShopItemInfo> list = new List<ShopItemInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Shop_All");
			while (ResultDataReader.Read())
			{
				ShopItemInfo item = new ShopItemInfo
				{
					ID = int.Parse(ResultDataReader["ID"].ToString()),
					ShopID = int.Parse(ResultDataReader["ShopID"].ToString()),
					GroupID = int.Parse(ResultDataReader["GroupID"].ToString()),
					TemplateID = int.Parse(ResultDataReader["TemplateID"].ToString()),
					BuyType = int.Parse(ResultDataReader["BuyType"].ToString()),
					Label = int.Parse(ResultDataReader["Label"].ToString()),
					Beat = decimal.Parse(ResultDataReader["Beat"].ToString()),
					AUnit = int.Parse(ResultDataReader["AUnit"].ToString()),
					APrice1 = int.Parse(ResultDataReader["APrice1"].ToString()),
					AValue1 = int.Parse(ResultDataReader["AValue1"].ToString()),
					APrice2 = int.Parse(ResultDataReader["APrice2"].ToString()),
					AValue2 = int.Parse(ResultDataReader["AValue2"].ToString()),
					APrice3 = int.Parse(ResultDataReader["APrice3"].ToString()),
					AValue3 = int.Parse(ResultDataReader["AValue3"].ToString()),
					BUnit = int.Parse(ResultDataReader["BUnit"].ToString()),
					BPrice1 = int.Parse(ResultDataReader["BPrice1"].ToString()),
					BValue1 = int.Parse(ResultDataReader["BValue1"].ToString()),
					BPrice2 = int.Parse(ResultDataReader["BPrice2"].ToString()),
					BValue2 = int.Parse(ResultDataReader["BValue2"].ToString()),
					BPrice3 = int.Parse(ResultDataReader["BPrice3"].ToString()),
					BValue3 = int.Parse(ResultDataReader["BValue3"].ToString()),
					CUnit = int.Parse(ResultDataReader["CUnit"].ToString()),
					CPrice1 = int.Parse(ResultDataReader["CPrice1"].ToString()),
					CValue1 = int.Parse(ResultDataReader["CValue1"].ToString()),
					CPrice2 = int.Parse(ResultDataReader["CPrice2"].ToString()),
					CValue2 = int.Parse(ResultDataReader["CValue2"].ToString()),
					CPrice3 = int.Parse(ResultDataReader["CPrice3"].ToString()),
					CValue3 = int.Parse(ResultDataReader["CValue3"].ToString()),
					IsBind = int.Parse(ResultDataReader["IsBind"].ToString()),
					IsContinue = bool.Parse(ResultDataReader["IsContinue"].ToString()),
					IsCheap = bool.Parse(ResultDataReader["IsCheap"].ToString()),
					LimitCount = int.Parse(ResultDataReader["LimitCount"].ToString()),
					StartDate = DateTime.Parse(ResultDataReader["StartDate"].ToString()),
					EndDate = DateTime.Parse(ResultDataReader["EndDate"].ToString()),
					LimitGrade = int.Parse(ResultDataReader["LimitGrade"].ToString()),
					CanBuy = bool.Parse(ResultDataReader["CanBuy"].ToString()),
					LimitPersonalCount = int.Parse(ResultDataReader["LimitPersonalCount"].ToString())
				};
				list.Add(item);
			}
		}
		catch (Exception ex)
		{
			Exception exception = ex;
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

	public DailyLeagueAwardList[] GetAllDailyLeagueAwardList()
	{
		List<DailyLeagueAwardList> list = new List<DailyLeagueAwardList>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Get_DailyLeagueAwardList");
			while (ResultDataReader.Read())
			{
				DailyLeagueAwardList item = new DailyLeagueAwardList
				{
					Class = (int)ResultDataReader["Class"],
					Grade = (int)ResultDataReader["Grade"],
					Score = (int)ResultDataReader["Score"],
					Rank = (int)ResultDataReader["Rank"]
				};
				list.Add(item);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllDailyLeagueAwardList", exception);
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

	public ProduceBussiness()
	{
	}

	public ProduceBussiness(AreaConfigInfo config)
	{
		if (config == null)
		{
			db = new Sql_DbObject("AppConfig", "conString");
			return;
		}
		string text = "Data Source=.\\DDT;Initial Catalog=Db_Tank_All;Persist Security Info=True;User ID=sa;Password=123456;";
		AreaName = config.AreaName;
		ZoneID = config.AreaID;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("Data Source=");
		stringBuilder.Append(config.DataSource);
		stringBuilder.Append("; Initial Catalog=");
		stringBuilder.Append(config.Catalog);
		stringBuilder.Append("; Persist Security Info=True;User ID=");
		stringBuilder.Append(config.UserID);
		stringBuilder.Append("; Password=");
		stringBuilder.Append(config.Password);
		stringBuilder.Append(";");
		text = stringBuilder.ToString();
		db = new Sql_DbObject("AreaConfig", text);
	}

	public JampsUpgradeItemList[] Get_JampsUpgradeItemList()
	{
		List<JampsUpgradeItemList> list = new List<JampsUpgradeItemList>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_JampsUpgradeItemList");
			while (ResultDataReader.Read())
			{
				JampsUpgradeItemList jampsUpgradeItemList = new JampsUpgradeItemList();
				for (int i = 0; i < ResultDataReader.FieldCount; i++)
				{
					if (jampsUpgradeItemList.GetType().GetField(ResultDataReader.GetName(i)) == null)
					{
						BaseCrossBussiness.log.Info("Campo " + ResultDataReader.GetName(i) + " Faltando em " + jampsUpgradeItemList.GetType());
					}
					else
					{
						jampsUpgradeItemList.GetType().GetField(ResultDataReader.GetName(i)).SetValue(jampsUpgradeItemList, ResultDataReader[i]);
					}
				}
				list.Add(jampsUpgradeItemList);
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

	public JampsUpgradeItemList[] Get_JampsUpgradeItemList(int level)
	{
		List<JampsUpgradeItemList> list = new List<JampsUpgradeItemList>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@Level", level)
			};
			db.GetReader(ref ResultDataReader, "SP_JampsUpgradeItemList", sqlParameters);
			while (ResultDataReader.Read())
			{
				JampsUpgradeItemList jampsUpgradeItemList = new JampsUpgradeItemList();
				for (int i = 0; i < ResultDataReader.FieldCount; i++)
				{
					if (jampsUpgradeItemList.GetType().GetField(ResultDataReader.GetName(i)) == null)
					{
						BaseCrossBussiness.log.Info("Campo " + ResultDataReader.GetName(i) + " Faltando em " + jampsUpgradeItemList.GetType());
					}
					else
					{
						jampsUpgradeItemList.GetType().GetField(ResultDataReader.GetName(i)).SetValue(jampsUpgradeItemList, ResultDataReader[i]);
					}
				}
				list.Add(jampsUpgradeItemList);
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

	public JampsChapterItemList[] Get_JampsChapterItemList()
	{
		List<JampsChapterItemList> list = new List<JampsChapterItemList>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_JampsChapterItemList");
			while (ResultDataReader.Read())
			{
				JampsChapterItemList jampsChapterItemList = new JampsChapterItemList();
				for (int i = 0; i < ResultDataReader.FieldCount; i++)
				{
					if (jampsChapterItemList.GetType().GetField(ResultDataReader.GetName(i)) == null)
					{
						BaseCrossBussiness.log.Info("Campo " + ResultDataReader.GetName(i) + " Faltando em " + jampsChapterItemList.GetType());
					}
					else
					{
						jampsChapterItemList.GetType().GetField(ResultDataReader.GetName(i)).SetValue(jampsChapterItemList, ResultDataReader[i]);
					}
				}
				list.Add(jampsChapterItemList);
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

	public JampsDebrisItemList[] Get_JampsDebrisItemList()
	{
		List<JampsDebrisItemList> list = new List<JampsDebrisItemList>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_JampsDebrisItemList");
			while (ResultDataReader.Read())
			{
				JampsDebrisItemList jampsDebrisItemList = new JampsDebrisItemList();
				for (int i = 0; i < ResultDataReader.FieldCount; i++)
				{
					if (jampsDebrisItemList.GetType().GetField(ResultDataReader.GetName(i)) == null)
					{
						BaseCrossBussiness.log.Info("Campo " + ResultDataReader.GetName(i) + " Faltando em " + jampsDebrisItemList.GetType());
					}
					else
					{
						jampsDebrisItemList.GetType().GetField(ResultDataReader.GetName(i)).SetValue(jampsDebrisItemList, ResultDataReader[i]);
					}
				}
				list.Add(jampsDebrisItemList);
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

	public JampsDebrisItemList[] Get_JampsDebrisItemList(int pageID)
	{
		List<JampsDebrisItemList> list = new List<JampsDebrisItemList>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@pageID", pageID)
			};
			db.GetReader(ref ResultDataReader, "SP_JampsDebrisItemList", sqlParameters);
			while (ResultDataReader.Read())
			{
				JampsDebrisItemList jampsDebrisItemList = new JampsDebrisItemList();
				for (int i = 0; i < ResultDataReader.FieldCount; i++)
				{
					if (jampsDebrisItemList.GetType().GetField(ResultDataReader.GetName(i)) == null)
					{
						BaseCrossBussiness.log.Info("Campo " + ResultDataReader.GetName(i) + " Faltando em " + jampsDebrisItemList.GetType());
					}
					else
					{
						jampsDebrisItemList.GetType().GetField(ResultDataReader.GetName(i)).SetValue(jampsDebrisItemList, ResultDataReader[i]);
					}
				}
				list.Add(jampsDebrisItemList);
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

	public JampsPageItemList[] Get_JampsPageItemList(int ID)
	{
		List<JampsPageItemList> list = new List<JampsPageItemList>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@capitulo", ID)
			};
			db.GetReader(ref ResultDataReader, "SP_JampsPageItemList", sqlParameters);
			while (ResultDataReader.Read())
			{
				JampsPageItemList jampsPageItemList = new JampsPageItemList();
				for (int i = 0; i < ResultDataReader.FieldCount; i++)
				{
					if (jampsPageItemList.GetType().GetField(ResultDataReader.GetName(i)) == null)
					{
						BaseCrossBussiness.log.Info("Campo " + ResultDataReader.GetName(i) + " Faltando em " + jampsPageItemList.GetType());
					}
					else
					{
						jampsPageItemList.GetType().GetField(ResultDataReader.GetName(i)).SetValue(jampsPageItemList, ResultDataReader[i]);
					}
				}
				list.Add(jampsPageItemList);
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

	public JampsManualItemList[] Get_JampsManualItemList()
	{
		List<JampsManualItemList> list = new List<JampsManualItemList>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_JampsManualItemList");
			while (ResultDataReader.Read())
			{
				JampsManualItemList jampsManualItemList = new JampsManualItemList();
				for (int i = 0; i < ResultDataReader.FieldCount; i++)
				{
					if (jampsManualItemList.GetType().GetField(ResultDataReader.GetName(i)) == null)
					{
						BaseCrossBussiness.log.Info("Campo " + ResultDataReader.GetName(i) + " Faltando em " + jampsManualItemList.GetType());
					}
					else
					{
						jampsManualItemList.GetType().GetField(ResultDataReader.GetName(i)).SetValue(jampsManualItemList, ResultDataReader[i]);
					}
				}
				list.Add(jampsManualItemList);
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

	public GodCardGroupInfo[] GetAllGodCardGroup()
	{
		List<GodCardGroupInfo> list = new List<GodCardGroupInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_God_Card_Group_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitGodCardGroup(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllGodCardGroup", exception);
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

	public GodCardGroupInfo InitGodCardGroup(SqlDataReader reader)
	{
		GodCardGroupInfo godCardGroupInfo = new GodCardGroupInfo();
		godCardGroupInfo.GroupID = (int)reader["GroupID"];
		godCardGroupInfo.GroupName = ((reader["GroupName"] == null) ? "" : reader["GroupName"].ToString());
		godCardGroupInfo.ExchangeTimes = (int)reader["ExchangeTimes"];
		godCardGroupInfo.GiftID = (int)reader["GiftID"];
		return godCardGroupInfo;
	}

	public GodCardGroupDetailInfo[] GetAllGodCardGroupDetail()
	{
		List<GodCardGroupDetailInfo> list = new List<GodCardGroupDetailInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_God_Card_Group_Detail_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitGodCardGroupDetail(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllGodCardGroupDetail", exception);
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

	public GodCardGroupDetailInfo InitGodCardGroupDetail(SqlDataReader reader)
	{
		GodCardGroupDetailInfo godCardGroupDetailInfo = new GodCardGroupDetailInfo();
		godCardGroupDetailInfo.GroupID = (int)reader["GroupID"];
		godCardGroupDetailInfo.CardID = (int)reader["CardID"];
		godCardGroupDetailInfo.Number = (int)reader["Number"];
		return godCardGroupDetailInfo;
	}

	public GodCardInfo[] GetAllGodCard()
	{
		List<GodCardInfo> list = new List<GodCardInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_God_Card_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitGodCard(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllGodCard", exception);
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

	public GodCardInfo InitGodCard(SqlDataReader reader)
	{
		GodCardInfo godCardInfo = new GodCardInfo();
		godCardInfo.ID = (int)reader["ID"];
		godCardInfo.Name = ((reader["Name"] == null) ? "" : reader["Name"].ToString());
		godCardInfo.Pic = (int)reader["Pic"];
		godCardInfo.Composition = (int)reader["Composition"];
		godCardInfo.Decompose = (int)reader["Decompose"];
		godCardInfo.Level = (int)reader["Level"];
		return godCardInfo;
	}

	public GodCardPointRewardInfo[] GetAllGodCardPointReward()
	{
		List<GodCardPointRewardInfo> list = new List<GodCardPointRewardInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_God_Card_Point_Reward_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitGodCardPointReward(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllGodCardPointReward", exception);
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

	public GodCardPointRewardInfo InitGodCardPointReward(SqlDataReader reader)
	{
		GodCardPointRewardInfo godCardPointRewardInfo = new GodCardPointRewardInfo();
		godCardPointRewardInfo.ID = (int)reader["ID"];
		godCardPointRewardInfo.Count = (int)reader["Count"];
		godCardPointRewardInfo.IsBind = (bool)reader["IsBind"];
		godCardPointRewardInfo.ItemID = (int)reader["ItemID"];
		godCardPointRewardInfo.Point = (int)reader["Point"];
		godCardPointRewardInfo.Valid = (int)reader["Valid"];
		return godCardPointRewardInfo;
	}

	public MountTalismansInfo[] GetAllMountTalismans()
	{
		List<MountTalismansInfo> list = new List<MountTalismansInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Mount_Talismans_Info_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitMountTalismans(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllMountTalismans", exception);
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

	public MountTalismansInfo InitMountTalismans(SqlDataReader reader)
	{
		MountTalismansInfo mountTalismansInfo = new MountTalismansInfo();
		mountTalismansInfo.ID = (int)reader["ID"];
		mountTalismansInfo.AmuletName = ((reader["AmuletName"] == null) ? "" : reader["AmuletName"].ToString());
		mountTalismansInfo.BaseType1 = (int)reader["BaseType1"];
		mountTalismansInfo.BaseType1Value = (int)reader["BaseType1Value"];
		mountTalismansInfo.BaseType2 = (int)reader["BaseType2"];
		mountTalismansInfo.BaseType2Value = (int)reader["BaseType2Value"];
		mountTalismansInfo.Consume = (int)reader["Consume"];
		mountTalismansInfo.Desc = ((reader["Desc"] == null) ? "" : reader["Desc"].ToString());
		mountTalismansInfo.ExtendType1 = (int)reader["ExtendType1"];
		mountTalismansInfo.ExtendType1MaxValue = (int)reader["ExtendType1MaxValue"];
		mountTalismansInfo.ExtendType1MinValue = (int)reader["ExtendType1MinValue"];
		mountTalismansInfo.ExtendType2 = (int)reader["ExtendType2"];
		mountTalismansInfo.ExtendType2MaxValue = (int)reader["ExtendType2MaxValue"];
		mountTalismansInfo.ExtendType2MinValue = (int)reader["ExtendType2MinValue"];
		mountTalismansInfo.IsCanWash = (bool)reader["IsCanWash"];
		mountTalismansInfo.PicUrl = ((reader["PicUrl"] == null) ? "" : reader["PicUrl"].ToString());
		mountTalismansInfo.Quality = (int)reader["Quality"];
		mountTalismansInfo.Remark = ((reader["Remark"] == null) ? "" : reader["Remark"].ToString());
		mountTalismansInfo.Score = (int)reader["Score"];
		mountTalismansInfo.ShatterNum = (int)reader["ShatterNum"];
		return mountTalismansInfo;
	}

	public AmuletPhaseItemInfo[] GetAllAmuletPhaseItem()
	{
		List<AmuletPhaseItemInfo> list = new List<AmuletPhaseItemInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Amulet_Phase_Item_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitAmuletPhaseItem(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllAmuletPhaseItem", exception);
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

	public AmuletPhaseItemInfo InitAmuletPhaseItem(SqlDataReader reader)
	{
		AmuletPhaseItemInfo amuletPhaseItemInfo = new AmuletPhaseItemInfo();
		amuletPhaseItemInfo.AmuletLevel = (int)reader["AmuletLevel"];
		amuletPhaseItemInfo.AvoidInjury = (int)reader["AvoidInjury"];
		amuletPhaseItemInfo.Crit = (int)reader["Crit"];
		amuletPhaseItemInfo.Expend = (int)reader["Expend"];
		amuletPhaseItemInfo.Guard = (int)reader["Guard"];
		amuletPhaseItemInfo.Kill = (int)reader["Kill"];
		amuletPhaseItemInfo.LockPrice = (int)reader["LockPrice"];
		amuletPhaseItemInfo.Phase = (int)reader["Phase"];
		amuletPhaseItemInfo.Speed = (int)reader["Speed"];
		amuletPhaseItemInfo.SunderArmor = (int)reader["SunderArmor"];
		amuletPhaseItemInfo.Tenacity = (int)reader["Tenacity"];
		amuletPhaseItemInfo.ViolenceInjury = (int)reader["ViolenceInjury"];
		amuletPhaseItemInfo.WillFight = (int)reader["WillFight"];
		return amuletPhaseItemInfo;
	}

	public AmuletItemInfo[] GetAllAmuletItem()
	{
		List<AmuletItemInfo> list = new List<AmuletItemInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Amulet_Info_Item_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitAmuletItem(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllAmuletItem", exception);
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

	public AmuletItemInfo InitAmuletItem(SqlDataReader reader)
	{
		AmuletItemInfo amuletItemInfo = new AmuletItemInfo();
		amuletItemInfo.Level = (int)reader["Level"];
		amuletItemInfo.HP = (int)reader["HP"];
		amuletItemInfo.GuaranteeTimes = (int)reader["GuaranteeTimes"];
		amuletItemInfo.LvlupStep = (int)reader["LvlupStep"];
		amuletItemInfo.WashsStep = (int)reader["WashsStep"];
		amuletItemInfo.SuccessChance = (int)reader["SuccessChance"];
		return amuletItemInfo;
	}

	public AmuletGradeItemInfo[] GetAllAmuletGradeItem()
	{
		List<AmuletGradeItemInfo> list = new List<AmuletGradeItemInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Amulet_Grade_Item_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitAmuletGradeItem(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllAmuletGradeItem", exception);
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

	public AmuletGradeItemInfo InitAmuletGradeItem(SqlDataReader reader)
	{
		AmuletGradeItemInfo amuletGradeItemInfo = new AmuletGradeItemInfo();
		amuletGradeItemInfo.WahsLevel = (int)reader["WahsLevel"];
		amuletGradeItemInfo.WahsTimes = (int)reader["WahsTimes"];
		amuletGradeItemInfo.Minvalue = (int)reader["Minvalue"];
		amuletGradeItemInfo.Maxvalue = (int)reader["Maxvalue"];
		return amuletGradeItemInfo;
	}

	public SubWeaponEvolutionTemplateInfo[] GetAllSubWeaponEvolutionTemplate()
	{
		List<SubWeaponEvolutionTemplateInfo> list = new List<SubWeaponEvolutionTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Sub_Weapon_Evolution_Template_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitSubWeaponEvolutionTemplate(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllSubWeaponEvolutionTemplate", exception);
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

	public SubWeaponEvolutionTemplateInfo InitSubWeaponEvolutionTemplate(SqlDataReader reader)
	{
		SubWeaponEvolutionTemplateInfo subWeaponEvolutionTemplateInfo = new SubWeaponEvolutionTemplateInfo();
		subWeaponEvolutionTemplateInfo.Level = (int)reader["Level"];
		subWeaponEvolutionTemplateInfo.Exp = (int)reader["Exp"];
		subWeaponEvolutionTemplateInfo.AddBlood = (int)reader["AddBlood"];
		subWeaponEvolutionTemplateInfo.ReduceDamage = (int)reader["ReduceDamage"];
		return subWeaponEvolutionTemplateInfo;
	}

	public MaxLevelTemplateInfo[] GetAllMaxLevelTemplate()
	{
		List<MaxLevelTemplateInfo> list = new List<MaxLevelTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Max_Level_Template_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitMaxLevelTemplate(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllMaxLevelTemplate", exception);
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

	public MaxLevelTemplateInfo InitMaxLevelTemplate(SqlDataReader reader)
	{
		MaxLevelTemplateInfo maxLevelTemplateInfo = new MaxLevelTemplateInfo();
		maxLevelTemplateInfo.Level = (int)reader["Level"];
		maxLevelTemplateInfo.Attack = (int)reader["Attack"];
		maxLevelTemplateInfo.Defence = (int)reader["Defence"];
		maxLevelTemplateInfo.Agility = (int)reader["Agility"];
		maxLevelTemplateInfo.Lucky = (int)reader["Lucky"];
		maxLevelTemplateInfo.MagicAttack = (int)reader["MagicAttack"];
		maxLevelTemplateInfo.MagicDefence = (int)reader["MagicDefence"];
		maxLevelTemplateInfo.Cost = (int)reader["Cost"];
		return maxLevelTemplateInfo;
	}

	public RuneAdvanceTemplateInfo[] GetAllRuneAdvanceTemplate()
	{
		List<RuneAdvanceTemplateInfo> list = new List<RuneAdvanceTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Rune_Advance_Template_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitRuneAdvanceTemplate(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllRuneAdvanceTemplate", exception);
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

	public RuneAdvanceTemplateInfo InitRuneAdvanceTemplate(SqlDataReader reader)
	{
		RuneAdvanceTemplateInfo runeAdvanceTemplateInfo = new RuneAdvanceTemplateInfo();
		runeAdvanceTemplateInfo.AdvancedTempId = (int)reader["AdvancedTempId"];
		runeAdvanceTemplateInfo.RuneName = ((reader["RuneName"] == null) ? "" : reader["RuneName"].ToString());
		runeAdvanceTemplateInfo.MainMaterials = ((reader["MainMaterials"] == null) ? "" : reader["MainMaterials"].ToString());
		runeAdvanceTemplateInfo.Quality = (int)reader["Quality"];
		runeAdvanceTemplateInfo.MaxLevelTempRunId = (int)reader["MaxLevelTempRunId"];
		runeAdvanceTemplateInfo.AuxiliaryMaterials = (string)reader["AuxiliaryMaterials"];
		runeAdvanceTemplateInfo.AdvanceDesc = ((reader["AdvanceDesc"] == null) ? "" : reader["AdvanceDesc"].ToString());
		return runeAdvanceTemplateInfo;
	}

	public CardAchievementInfo[] GetAllCardAchievement()
	{
		List<CardAchievementInfo> list = new List<CardAchievementInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Card_Achievement_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitCardAchievement(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllCardAchievement", exception);
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

	public CardAchievementInfo InitCardAchievement(SqlDataReader reader)
	{
		CardAchievementInfo cardAchievementInfo = new CardAchievementInfo();
		cardAchievementInfo.AchievementID = (int)reader["AchievementID"];
		cardAchievementInfo.AddAttack = (int)reader["AddAttack"];
		cardAchievementInfo.AddBlood = (int)reader["AddBlood"];
		cardAchievementInfo.AddDamage = (int)reader["AddDamage"];
		cardAchievementInfo.AddDefend = (int)reader["AddDefend"];
		cardAchievementInfo.AddGuard = (int)reader["AddGuard"];
		cardAchievementInfo.AddLucky = (int)reader["AddLucky"];
		cardAchievementInfo.AddMagicAttack = (int)reader["AddMagicAttack"];
		cardAchievementInfo.AddMagicDefend = (int)reader["AddMagicDefend"];
		cardAchievementInfo.Desc = ((reader["Desc"] == null) ? "" : reader["Desc"].ToString());
		cardAchievementInfo.Honor_id = (int)reader["Honor_id"];
		cardAchievementInfo.IsPrompt = (int)reader["IsPrompt"];
		cardAchievementInfo.Name = ((reader["Name"] == null) ? "" : reader["Name"].ToString());
		cardAchievementInfo.RequireGroupid = (int)reader["RequireGroupid"];
		cardAchievementInfo.RequireGroupNum = (int)reader["RequireGroupNum"];
		cardAchievementInfo.RequireNum = (int)reader["RequireNum"];
		cardAchievementInfo.RequireType = (int)reader["RequireType"];
		cardAchievementInfo.Type = (int)reader["Type"];
		return cardAchievementInfo;
	}

	public HomeTempPracticeInfo[] GetAllHomeTempPractice()
	{
		List<HomeTempPracticeInfo> list = new List<HomeTempPracticeInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Home_Temp_Practice_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitHomeTempPractice(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllHomeTempPractice", exception);
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

	public HomeTempPracticeInfo InitHomeTempPractice(SqlDataReader reader)
	{
		HomeTempPracticeInfo homeTempPracticeInfo = new HomeTempPracticeInfo();
		homeTempPracticeInfo.Level = (int)reader["Level"];
		homeTempPracticeInfo.Exp = (int)reader["Exp"];
		homeTempPracticeInfo.Attack = (int)reader["Attack"];
		homeTempPracticeInfo.Defence = (int)reader["Defence"];
		homeTempPracticeInfo.Guard = (int)reader["Guard"];
		homeTempPracticeInfo.Luck = (int)reader["Luck"];
		homeTempPracticeInfo.Blood = (int)reader["Blood"];
		homeTempPracticeInfo.MagicDefence = (int)reader["MagicDefence"];
		return homeTempPracticeInfo;
	}

	public GuardCoreLevelTemplateInfo[] GetAllGuardCoreLevelTemplate()
	{
		List<GuardCoreLevelTemplateInfo> list = new List<GuardCoreLevelTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Guard_Core_Level_Template_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitGuardCoreLevelTemplate(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllGuardCoreLevelTemplate", exception);
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

	public GuardCoreLevelTemplateInfo InitGuardCoreLevelTemplate(SqlDataReader reader)
	{
		GuardCoreLevelTemplateInfo guardCoreLevelTemplateInfo = new GuardCoreLevelTemplateInfo();
		guardCoreLevelTemplateInfo.Grade = (int)reader["Grade"];
		guardCoreLevelTemplateInfo.Exp = (int)reader["Exp"];
		guardCoreLevelTemplateInfo.Gold = (int)reader["Gold"];
		guardCoreLevelTemplateInfo.Guard = (int)reader["Guard"];
		return guardCoreLevelTemplateInfo;
	}

	public GuardCoreTemplateInfo[] GetAllGuardCoreTemplate()
	{
		List<GuardCoreTemplateInfo> list = new List<GuardCoreTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Guard_Core_Template_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitGuardCoreTemplate(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllGuardCoreTemplate", exception);
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

	public GuardCoreTemplateInfo InitGuardCoreTemplate(SqlDataReader reader)
	{
		GuardCoreTemplateInfo guardCoreTemplateInfo = new GuardCoreTemplateInfo();
		guardCoreTemplateInfo.ID = (int)reader["ID"];
		guardCoreTemplateInfo.Name = ((reader["Name"] == null) ? "" : reader["Name"].ToString());
		guardCoreTemplateInfo.Description = ((reader["Description"] == null) ? "" : reader["Description"].ToString());
		guardCoreTemplateInfo.TipsDescription = ((reader["TipsDescription"] == null) ? "" : reader["TipsDescription"].ToString());
		guardCoreTemplateInfo.Type = (int)reader["Type"];
		guardCoreTemplateInfo.GainGrade = (int)reader["GainGrade"];
		guardCoreTemplateInfo.Parameter1 = (int)reader["Parameter1"];
		guardCoreTemplateInfo.Parameter2 = (int)reader["Parameter2"];
		guardCoreTemplateInfo.Parameter3 = (int)reader["Parameter3"];
		guardCoreTemplateInfo.Parameter4 = (int)reader["Parameter4"];
		guardCoreTemplateInfo.KeepTurn = (int)reader["KeepTurn"];
		guardCoreTemplateInfo.GroupType = (int)reader["GroupType"];
		guardCoreTemplateInfo.GuardGrade = (int)reader["GuardGrade"];
		guardCoreTemplateInfo.SkillGrade = (int)reader["SkillGrade"];
		return guardCoreTemplateInfo;
	}

	public BattleBonusInfo[] GetAllBattleBonus()
	{
		List<BattleBonusInfo> list = new List<BattleBonusInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Battle_Bonus_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitBattleBonus(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllBattleBonus", exception);
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

	public BattleBonusInfo InitBattleBonus(SqlDataReader reader)
	{
		BattleBonusInfo battleBonusInfo = new BattleBonusInfo();
		battleBonusInfo.ID = (int)reader["ID"];
		battleBonusInfo.Rate = (int)reader["Rate"];
		battleBonusInfo.MinValue = (int)reader["MinValue"];
		battleBonusInfo.MaxValue = (int)reader["MaxValue"];
		battleBonusInfo.BeginTime = (DateTime)reader["BeginTime"];
		battleBonusInfo.EndTime = (DateTime)reader["EndTime"];
		battleBonusInfo.Notice = ((reader["Notice"] == null) ? "" : reader["Notice"].ToString());
		battleBonusInfo.TemplateID = (int)reader["TemplateID"];
		battleBonusInfo.Type = (int)reader["Type"];
		battleBonusInfo.IsLog = (bool)reader["IsLog"];
		battleBonusInfo.IsBinds = (bool)reader["IsBinds"];
		battleBonusInfo.ValidDate = (int)reader["ValidDate"];
		battleBonusInfo.StrengthenLevel = (int)reader["StrengthenLevel"];
		battleBonusInfo.AttackCompose = (int)reader["AttackCompose"];
		battleBonusInfo.DefendCompose = (int)reader["DefendCompose"];
		battleBonusInfo.AgilityCompose = (int)reader["AgilityCompose"];
		battleBonusInfo.LuckCompose = (int)reader["LuckCompose"];
		battleBonusInfo.Random = (int)reader["Random"];
		battleBonusInfo.MagicAttack = (int)reader["MagicAttack"];
		battleBonusInfo.MagicDefence = (int)reader["MagicDefence"];
		battleBonusInfo.HasValidate = (bool)reader["HasValidate"];
		battleBonusInfo.goldValidate = (int)reader["goldValidate"];
		battleBonusInfo.Detail = ((reader["Detail"] == null) ? "" : reader["Detail"].ToString());
		battleBonusInfo.DisableRandom = (bool)reader["DisableRandom"];
		battleBonusInfo.IsExist = (bool)reader["IsExist"];
		return battleBonusInfo;
	}

	public EdictumInfo[] GetAllEdictum()
	{
		List<EdictumInfo> list = new List<EdictumInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Edictum_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitEdictum(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllEdictum", exception);
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

	public EdictumInfo InitEdictum(SqlDataReader reader)
	{
		EdictumInfo edictumInfo = new EdictumInfo();
		edictumInfo.ID = (int)reader["ID"];
		edictumInfo.Title = ((reader["Title"] == null) ? "" : reader["Title"].ToString());
		edictumInfo.BeginDate = (DateTime)reader["BeginDate"];
		edictumInfo.BeginTime = (DateTime)reader["BeginTime"];
		edictumInfo.EndDate = (DateTime)reader["EndDate"];
		edictumInfo.EndTime = (DateTime)reader["EndTime"];
		edictumInfo.Text = ((reader["Text"] == null) ? "" : reader["Text"].ToString());
		edictumInfo.IsExist = (bool)reader["IsExist"];
		return edictumInfo;
	}

	public LoveLevelInfo[] GetAllLoveLevel()
	{
		List<LoveLevelInfo> list = new List<LoveLevelInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Love_level_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitLoveLevel(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllLoveLevel", exception);
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

	public LoveLevelInfo InitLoveLevel(SqlDataReader reader)
	{
		LoveLevelInfo loveLevelInfo = new LoveLevelInfo();
		loveLevelInfo.Level = (int)reader["Level"];
		loveLevelInfo.Exp = (int)reader["Exp"];
		loveLevelInfo.Animation = (int)reader["Animation"];
		loveLevelInfo.SkillID = (int)reader["SkillID"];
		loveLevelInfo.Attack = (int)reader["Attack"];
		loveLevelInfo.Defence = (int)reader["Defence"];
		loveLevelInfo.Agility = (int)reader["Agility"];
		loveLevelInfo.Luck = (int)reader["Luck"];
		return loveLevelInfo;
	}

	public PetBreakInfo[] GetAllPetBreak()
	{
		List<PetBreakInfo> list = new List<PetBreakInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Pet_Break_Info_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitPetBreak(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllPetBreak", exception);
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

	public PetBreakInfo InitPetBreak(SqlDataReader reader)
	{
		PetBreakInfo petBreakInfo = new PetBreakInfo();
		petBreakInfo.BreakGrade = (int)reader["BreakGrade"];
		petBreakInfo.Star = (int)reader["Star"];
		petBreakInfo.stoneNumber = (int)reader["stoneNumber"];
		petBreakInfo.breakGrade1 = (int)reader["breakGrade1"];
		petBreakInfo.star1 = (int)reader["star1"];
		petBreakInfo.breakGrade2 = (int)reader["breakGrade2"];
		petBreakInfo.star2 = (int)reader["star2"];
		petBreakInfo.breakAttack = (int)reader["breakAttack"];
		petBreakInfo.breakDefence = (int)reader["breakDefence"];
		petBreakInfo.breakAgility = (int)reader["breakAgility"];
		petBreakInfo.breakLuck = (int)reader["breakLuck"];
		petBreakInfo.breakBlood = (int)reader["breakBlood"];
		petBreakInfo.targetGrade = (int)reader["targetGrade"];
		return petBreakInfo;
	}

	public SetsBuildTempInfo[] GetAllSetsBuildTemp()
	{
		List<SetsBuildTempInfo> list = new List<SetsBuildTempInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Sets_Build_Temp_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitSetsBuildTemp(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllSetsBuildTemp", exception);
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

	public SetsBuildTempInfo InitSetsBuildTemp(SqlDataReader reader)
	{
		SetsBuildTempInfo setsBuildTempInfo = new SetsBuildTempInfo();
		setsBuildTempInfo.Level = (int)reader["Level"];
		setsBuildTempInfo.SetsType = (int)reader["SetsType"];
		setsBuildTempInfo.UseItemTemplate = (int)reader["UseItemTemplate"];
		setsBuildTempInfo.Exp = (int)reader["Exp"];
		setsBuildTempInfo.DefenceGrow = (int)reader["DefenceGrow"];
		setsBuildTempInfo.BloodGrow = (int)reader["BloodGrow"];
		setsBuildTempInfo.LuckGrow = (int)reader["LuckGrow"];
		setsBuildTempInfo.AgilityGrow = (int)reader["AgilityGrow"];
		setsBuildTempInfo.MagicDefenceGrow = (int)reader["MagicDefenceGrow"];
		setsBuildTempInfo.GuardGrow = (int)reader["GuardGrow"];
		return setsBuildTempInfo;
	}

	public VipStoreInfo[] GetAllVipStore()
	{
		List<VipStoreInfo> list = new List<VipStoreInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Vip_Store_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitVipStore(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllVipStore", exception);
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

	public VipStoreInfo InitVipStore(SqlDataReader reader)
	{
		VipStoreInfo vipStoreInfo = new VipStoreInfo();
		vipStoreInfo.ID = (int)reader["ID"];
		vipStoreInfo.Type = (int)reader["Type"];
		vipStoreInfo.GoodsID = (int)reader["GoodsID"];
		vipStoreInfo.Discount = (int)reader["Discount"];
		vipStoreInfo.Price = (int)reader["Price"];
		vipStoreInfo.ValidDate = (int)reader["ValidDate"];
		vipStoreInfo.Vip1Quantity = (int)reader["Vip1Quantity"];
		vipStoreInfo.Vip2Quantity = (int)reader["Vip2Quantity"];
		vipStoreInfo.Vip3Quantity = (int)reader["Vip3Quantity"];
		vipStoreInfo.Vip4Quantity = (int)reader["Vip4Quantity"];
		vipStoreInfo.Vip5Quantity = (int)reader["Vip5Quantity"];
		vipStoreInfo.Vip6Quantity = (int)reader["Vip6Quantity"];
		vipStoreInfo.Vip7Quantity = (int)reader["Vip7Quantity"];
		vipStoreInfo.Vip8Quantity = (int)reader["Vip8Quantity"];
		vipStoreInfo.Vip9Quantity = (int)reader["Vip9Quantity"];
		vipStoreInfo.Vip10Quantity = (int)reader["Vip10Quantity"];
		vipStoreInfo.Vip11Quantity = (int)reader["Vip11Quantity"];
		vipStoreInfo.Vip12Quantity = (int)reader["Vip12Quantity"];
		return vipStoreInfo;
	}

	public bool UpdateCommunalActive(CommunalActiveInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[12]
			{
				new SqlParameter("@ActiveID", item.ActiveID),
				new SqlParameter("@BeginTime", item.BeginTime),
				new SqlParameter("@EndTime", item.EndTime),
				new SqlParameter("@LimitGrade", item.LimitGrade),
				new SqlParameter("@DayMaxScore", item.DayMaxScore),
				new SqlParameter("@MinScore", item.MinScore),
				new SqlParameter("@AddPropertyByMoney", item.AddPropertyByMoney),
				new SqlParameter("@AddPropertyByProp", item.AddPropertyByProp),
				new SqlParameter("@IsReset", item.CanReset),
				new SqlParameter("@IsSendAward", item.CanSendAward),
				new SqlParameter("@IsOpen", item.IsOpen),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[11].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Communal_Active_Update", array);
			result = (int)array[11].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("UpdateCommunalActive", exception);
			}
		}
		return result;
	}

	public TotemInfo[] GetAllTotem()
	{
		List<TotemInfo> list = new List<TotemInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Totem_All");
			while (ResultDataReader.Read())
			{
				TotemInfo totemInfo = new TotemInfo();
				totemInfo.ID = (int)ResultDataReader["ID"];
				totemInfo.ConsumeExp = (int)ResultDataReader["ConsumeExp"];
				totemInfo.ConsumeHonor = (int)ResultDataReader["ConsumeHonor"];
				totemInfo.AddAttack = (int)ResultDataReader["AddAttack"];
				totemInfo.AddDefence = (int)ResultDataReader["AddDefence"];
				totemInfo.AddAgility = (int)ResultDataReader["AddAgility"];
				totemInfo.AddLuck = (int)ResultDataReader["AddLuck"];
				totemInfo.AddBlood = (int)ResultDataReader["AddBlood"];
				totemInfo.AddDamage = (int)ResultDataReader["AddDamage"];
				totemInfo.AddGuard = (int)ResultDataReader["AddGuard"];
				totemInfo.Random = (int)ResultDataReader["Random"];
				totemInfo.Page = (int)ResultDataReader["Page"];
				totemInfo.Layers = (int)ResultDataReader["Layers"];
				totemInfo.Location = (int)ResultDataReader["Location"];
				totemInfo.Point = (int)ResultDataReader["Point"];
				list.Add(totemInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetTotemAll", exception);
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

	public FightSpiritTemplateInfo[] GetAllFightSpiritTemplate()
	{
		List<FightSpiritTemplateInfo> list = new List<FightSpiritTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_FightSpiritTemplate_All");
			while (ResultDataReader.Read())
			{
				FightSpiritTemplateInfo fightSpiritTemplateInfo = new FightSpiritTemplateInfo();
				fightSpiritTemplateInfo.FightSpiritID = (int)ResultDataReader["FightSpiritID"];
				fightSpiritTemplateInfo.FightSpiritIcon = (string)ResultDataReader["FightSpiritIcon"];
				fightSpiritTemplateInfo.Level = (int)ResultDataReader["Level"];
				fightSpiritTemplateInfo.Exp = (int)ResultDataReader["Exp"];
				fightSpiritTemplateInfo.Attack = (int)ResultDataReader["Attack"];
				fightSpiritTemplateInfo.Defence = (int)ResultDataReader["Defence"];
				fightSpiritTemplateInfo.Agility = (int)ResultDataReader["Agility"];
				fightSpiritTemplateInfo.Lucky = (int)ResultDataReader["Lucky"];
				fightSpiritTemplateInfo.Blood = (int)ResultDataReader["Blood"];
				list.Add(fightSpiritTemplateInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetFightSpiritTemplateAll", exception);
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

	public FairBattleRewardInfo[] GetAllFairBattleReward()
	{
		List<FairBattleRewardInfo> list = new List<FairBattleRewardInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_FairBattleReward_All");
			while (ResultDataReader.Read())
			{
				FairBattleRewardInfo fairBattleRewardInfo = new FairBattleRewardInfo();
				fairBattleRewardInfo.Prestige = (int)ResultDataReader["Prestige"];
				fairBattleRewardInfo.Level = (int)ResultDataReader["Level"];
				fairBattleRewardInfo.Name = (string)ResultDataReader["Name"];
				fairBattleRewardInfo.PrestigeForWin = (int)ResultDataReader["PrestigeForWin"];
				fairBattleRewardInfo.PrestigeForLose = (int)ResultDataReader["PrestigeForLose"];
				fairBattleRewardInfo.Title = (string)ResultDataReader["Title"];
				list.Add(fairBattleRewardInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllFairBattleReward", exception);
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

	public TreasureAwardInfo[] GetAllTreasureAward()
	{
		List<TreasureAwardInfo> list = new List<TreasureAwardInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Treasure_All");
			while (ResultDataReader.Read())
			{
				TreasureAwardInfo treasureAwardInfo = new TreasureAwardInfo();
				treasureAwardInfo.ID = (int)ResultDataReader["ID"];
				treasureAwardInfo.TemplateID = (int)ResultDataReader["TemplateID"];
				treasureAwardInfo.Name = (string)ResultDataReader["Name"];
				treasureAwardInfo.Count = (int)ResultDataReader["Count"];
				treasureAwardInfo.Validate = (int)ResultDataReader["Validate"];
				treasureAwardInfo.Random = (int)ResultDataReader["Random"];
				list.Add(treasureAwardInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetTreasureAwardAll", exception);
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

	public TotemHonorTemplateInfo[] GetAllTotemHonorTemplate()
	{
		List<TotemHonorTemplateInfo> list = new List<TotemHonorTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_TotemHonorTemplate_All");
			while (ResultDataReader.Read())
			{
				TotemHonorTemplateInfo totemHonorTemplateInfo = new TotemHonorTemplateInfo();
				totemHonorTemplateInfo.ID = (int)ResultDataReader["ID"];
				totemHonorTemplateInfo.NeedMoney = (int)ResultDataReader["NeedMoney"];
				totemHonorTemplateInfo.Type = (int)ResultDataReader["Type"];
				totemHonorTemplateInfo.AddHonor = (int)ResultDataReader["AddHonor"];
				list.Add(totemHonorTemplateInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetTotemHonorTemplateInfo", exception);
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

	public ConsortiaBossConfigInfo[] GetConsortiaBossConfigAll()
	{
		List<ConsortiaBossConfigInfo> list = new List<ConsortiaBossConfigInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Consortia_Boss_Config_All");
			while (ResultDataReader.Read())
			{
				ConsortiaBossConfigInfo consortiaBossConfigInfo = new ConsortiaBossConfigInfo();
				consortiaBossConfigInfo.Level = (int)ResultDataReader["Level"];
				consortiaBossConfigInfo.NpcID = (int)ResultDataReader["NpcID"];
				consortiaBossConfigInfo.MissionID = (int)ResultDataReader["MissionID"];
				consortiaBossConfigInfo.AwardID = (int)ResultDataReader["AwardID"];
				consortiaBossConfigInfo.CostRich = (int)ResultDataReader["CostRich"];
				consortiaBossConfigInfo.ProlongRich = (int)ResultDataReader["ProlongRich"];
				consortiaBossConfigInfo.BossLevel = (int)ResultDataReader["BossLevel"];
				list.Add(consortiaBossConfigInfo);
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

	public LevelInfo[] GetAllLevel()
	{
		List<LevelInfo> list = new List<LevelInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Level_All");
			while (ResultDataReader.Read())
			{
				LevelInfo levelInfo = new LevelInfo();
				levelInfo.Grade = (int)ResultDataReader["Grade"];
				levelInfo.GP = (int)ResultDataReader["GP"];
				levelInfo.Blood = (int)ResultDataReader["Blood"];
				list.Add(levelInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllLevel", exception);
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

	public ExerciseInfo[] GetAllExercise()
	{
		List<ExerciseInfo> list = new List<ExerciseInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Exercise_All");
			while (ResultDataReader.Read())
			{
				ExerciseInfo exerciseInfo = new ExerciseInfo();
				exerciseInfo.Grage = (int)ResultDataReader["Grage"];
				exerciseInfo.GP = (int)ResultDataReader["GP"];
				exerciseInfo.ExerciseA = (int)ResultDataReader["ExerciseA"];
				exerciseInfo.ExerciseAG = (int)ResultDataReader["ExerciseAG"];
				exerciseInfo.ExerciseD = (int)ResultDataReader["ExerciseD"];
				exerciseInfo.ExerciseH = (int)ResultDataReader["ExerciseH"];
				exerciseInfo.ExerciseL = (int)ResultDataReader["ExerciseL"];
				exerciseInfo.ExerciseMA = (int)ResultDataReader["ExerciseMA"];
				exerciseInfo.ExerciseMD = (int)ResultDataReader["ExerciseMD"];
				exerciseInfo.TrainCrit = (int)ResultDataReader["TrainCrit"];
				exerciseInfo.TrainSunder = (int)ResultDataReader["TrainSunder"];
				exerciseInfo.TrainCritDmg = (int)ResultDataReader["TrainCritDmg"];
				exerciseInfo.TrainSpeed = (int)ResultDataReader["TrainSpeed"];
				exerciseInfo.TrainTricKill = (int)ResultDataReader["TrainTricKill"];
				exerciseInfo.TrainDmg = (int)ResultDataReader["TrainDmg"];
				exerciseInfo.TrainArmor = (int)ResultDataReader["TrainArmor"];
				list.Add(exerciseInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllExercise", exception);
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

	public PetFormDataInfo[] GetAllPetFormData()
	{
		List<PetFormDataInfo> list = new List<PetFormDataInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Pet_Form_Data_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitPetFormDataInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("InitPetFormDataInfo", exception);
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

	public PetFormDataInfo InitPetFormDataInfo(SqlDataReader dr)
	{
		PetFormDataInfo petFormDataInfo = new PetFormDataInfo();
		petFormDataInfo.Appearance = (string)dr["Appearance"];
		petFormDataInfo.DamageReduce = (int)dr["DamageReduce"];
		petFormDataInfo.HeathUp = (int)dr["HeathUp"];
		petFormDataInfo.Name = (string)dr["Name"];
		petFormDataInfo.Resource = (string)dr["Resource"];
		petFormDataInfo.TemplateID = (int)dr["TemplateID"];
		return petFormDataInfo;
	}

	public PetConfig[] GetAllPetConfig()
	{
		List<PetConfig> list = new List<PetConfig>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_PetConfig_All");
			while (ResultDataReader.Read())
			{
				PetConfig petConfig = new PetConfig();
				petConfig.ID = (int)ResultDataReader["ID"];
				petConfig.Name = ResultDataReader["Name"].ToString();
				petConfig.Value = ResultDataReader["Value"].ToString();
				list.Add(petConfig);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllPetConfig", exception);
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

	public PetLevel[] GetAllPetLevel()
	{
		List<PetLevel> list = new List<PetLevel>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_PetLevel_All");
			while (ResultDataReader.Read())
			{
				PetLevel petLevel = new PetLevel();
				petLevel.Level = (int)ResultDataReader["Level"];
				petLevel.GP = (int)ResultDataReader["GP"];
				list.Add(petLevel);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllPetLevel", exception);
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

	public PetTemplateInfo[] GetAllPetTemplateInfo()
	{
		List<PetTemplateInfo> list = new List<PetTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_PetTemplateInfo_All");
			while (ResultDataReader.Read())
			{
				PetTemplateInfo petTemplateInfo = new PetTemplateInfo();
				petTemplateInfo.TemplateID = (int)ResultDataReader["TemplateID"];
				petTemplateInfo.Name = (string)ResultDataReader["Name"];
				petTemplateInfo.KindID = (int)ResultDataReader["KindID"];
				petTemplateInfo.Description = (string)ResultDataReader["Description"];
				petTemplateInfo.Pic = (string)ResultDataReader["Pic"];
				petTemplateInfo.RareLevel = (int)ResultDataReader["RareLevel"];
				petTemplateInfo.MP = (int)ResultDataReader["MP"];
				petTemplateInfo.StarLevel = (int)ResultDataReader["StarLevel"];
				petTemplateInfo.GameAssetUrl = (string)ResultDataReader["GameAssetUrl"];
				petTemplateInfo.HighAgility = (int)ResultDataReader["HighAgility"];
				petTemplateInfo.HighAgilityGrow = (int)ResultDataReader["HighAgilityGrow"];
				petTemplateInfo.HighAttack = (int)ResultDataReader["HighAttack"];
				petTemplateInfo.HighAttackGrow = (int)ResultDataReader["HighAttackGrow"];
				petTemplateInfo.HighBlood = (int)ResultDataReader["HighBlood"];
				petTemplateInfo.HighBloodGrow = (int)ResultDataReader["HighBloodGrow"];
				petTemplateInfo.HighDamage = (int)ResultDataReader["HighDamage"];
				petTemplateInfo.HighDamageGrow = (int)ResultDataReader["HighDamageGrow"];
				petTemplateInfo.HighDefence = (int)ResultDataReader["HighDefence"];
				petTemplateInfo.HighDefenceGrow = (int)ResultDataReader["HighDefenceGrow"];
				petTemplateInfo.HighGuard = (int)ResultDataReader["HighGuard"];
				petTemplateInfo.HighGuardGrow = (int)ResultDataReader["HighGuardGrow"];
				petTemplateInfo.HighLuck = (int)ResultDataReader["HighLuck"];
				petTemplateInfo.HighLuckGrow = (int)ResultDataReader["HighLuckGrow"];
				petTemplateInfo.EvolutionID = (int)ResultDataReader["EvolutionID"];
				petTemplateInfo.LowBloodGrow = (int)ResultDataReader["LowBloodGrow"];
				petTemplateInfo.LowAttackGrow = (int)ResultDataReader["LowAttackGrow"];
				petTemplateInfo.LowDefenceGrow = (int)ResultDataReader["LowDefenceGrow"];
				petTemplateInfo.LowAgilityGrow = (int)ResultDataReader["LowAgilityGrow"];
				petTemplateInfo.LowLuckGrow = (int)ResultDataReader["LowLuckGrow"];
				petTemplateInfo.WashGetCount = (int)ResultDataReader["WashGetCount"];
				list.Add(petTemplateInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllPetTemplateInfo", exception);
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

	public PetSkillTemplateInfo[] GetAllPetSkillTemplateInfo()
	{
		List<PetSkillTemplateInfo> list = new List<PetSkillTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_PetSkillTemplateInfo_All");
			while (ResultDataReader.Read())
			{
				PetSkillTemplateInfo petSkillTemplateInfo = new PetSkillTemplateInfo();
				petSkillTemplateInfo.PetTemplateID = (int)ResultDataReader["PetTemplateID"];
				petSkillTemplateInfo.KindID = (int)ResultDataReader["KindID"];
				petSkillTemplateInfo.Type = (int)ResultDataReader["GetType"];
				petSkillTemplateInfo.SkillID = (int)ResultDataReader["SkillID"];
				petSkillTemplateInfo.SkillBookID = (int)ResultDataReader["SkillBookID"];
				petSkillTemplateInfo.MinLevel = (int)ResultDataReader["MinLevel"];
				petSkillTemplateInfo.DeleteSkillIDs = ResultDataReader["DeleteSkillIDs"].ToString();
				list.Add(petSkillTemplateInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllPetSkillTemplateInfo", exception);
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

	public PetSkillInfo[] GetAllPetSkillInfo()
	{
		List<PetSkillInfo> list = new List<PetSkillInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_PetSkillInfo_All");
			while (ResultDataReader.Read())
			{
				PetSkillInfo petSkillInfo = new PetSkillInfo();
				petSkillInfo.ID = (int)ResultDataReader["ID"];
				petSkillInfo.Name = ResultDataReader["Name"].ToString();
				petSkillInfo.ElementIDs = ResultDataReader["ElementIDs"].ToString();
				petSkillInfo.Description = ResultDataReader["Description"].ToString();
				petSkillInfo.BallType = (int)ResultDataReader["BallType"];
				petSkillInfo.NewBallID = (int)ResultDataReader["NewBallID"];
				petSkillInfo.CostMP = (int)ResultDataReader["CostMP"];
				petSkillInfo.Pic = (int)ResultDataReader["Pic"];
				petSkillInfo.Action = ResultDataReader["Action"].ToString();
				petSkillInfo.EffectPic = ResultDataReader["EffectPic"].ToString();
				petSkillInfo.Delay = (int)ResultDataReader["Delay"];
				petSkillInfo.ColdDown = (int)ResultDataReader["ColdDown"];
				petSkillInfo.GameType = (int)ResultDataReader["GameType"];
				petSkillInfo.Probability = (int)ResultDataReader["Probability"];
				list.Add(petSkillInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllPetSkillInfo", exception);
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

	public PetSkillElementInfo[] GetAllPetSkillElementInfo()
	{
		List<PetSkillElementInfo> list = new List<PetSkillElementInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_PetSkillElementInfo_All");
			while (ResultDataReader.Read())
			{
				PetSkillElementInfo petSkillElementInfo = new PetSkillElementInfo();
				petSkillElementInfo.ID = (int)ResultDataReader["ID"];
				petSkillElementInfo.Name = ResultDataReader["Name"].ToString();
				petSkillElementInfo.EffectPic = ResultDataReader["EffectPic"].ToString();
				petSkillElementInfo.Description = ResultDataReader["Description"].ToString();
				petSkillElementInfo.Pic = (int)ResultDataReader["Pic"];
				list.Add(petSkillElementInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllPetSkillElementInfo", exception);
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

	public PetExpItemPriceInfo[] GetAllPetExpItemPrice()
	{
		List<PetExpItemPriceInfo> list = new List<PetExpItemPriceInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_PetExpItemPriceInfo_All");
			while (ResultDataReader.Read())
			{
				PetExpItemPriceInfo petExpItemPriceInfo = new PetExpItemPriceInfo();
				petExpItemPriceInfo.ID = (int)ResultDataReader["ID"];
				petExpItemPriceInfo.Count = (int)ResultDataReader["Count"];
				petExpItemPriceInfo.Money = (int)ResultDataReader["Money"];
				petExpItemPriceInfo.ItemCount = (int)ResultDataReader["ItemCount"];
				list.Add(petExpItemPriceInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllPetTemplateInfo", exception);
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

	public PetFightPropertyInfo[] GetAllPetFightProperty()
	{
		List<PetFightPropertyInfo> list = new List<PetFightPropertyInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_PetFightProperty_All");
			while (ResultDataReader.Read())
			{
				PetFightPropertyInfo petFightPropertyInfo = new PetFightPropertyInfo();
				petFightPropertyInfo.ID = (int)ResultDataReader["ID"];
				petFightPropertyInfo.Exp = (int)ResultDataReader["Exp"];
				petFightPropertyInfo.Attack = (int)ResultDataReader["Attack"];
				petFightPropertyInfo.Agility = (int)ResultDataReader["Agility"];
				petFightPropertyInfo.Defence = (int)ResultDataReader["Defence"];
				petFightPropertyInfo.Lucky = (int)ResultDataReader["Lucky"];
				petFightPropertyInfo.Blood = (int)ResultDataReader["Blood"];
				list.Add(petFightPropertyInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllPetFightProperty", exception);
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

	public PetStarExpInfo[] GetAllPetStarExp()
	{
		List<PetStarExpInfo> list = new List<PetStarExpInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_PetStarExp_All");
			while (ResultDataReader.Read())
			{
				PetStarExpInfo petStarExpInfo = new PetStarExpInfo();
				petStarExpInfo.Exp = (int)ResultDataReader["Exp"];
				petStarExpInfo.OldID = (int)ResultDataReader["OldID"];
				petStarExpInfo.NewID = (int)ResultDataReader["NewID"];
				list.Add(petStarExpInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllPetStarExp", exception);
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

	public ConsortiaBadgeConfigInfo[] GetAllConsortiaBadgeConfig()
	{
		List<ConsortiaBadgeConfigInfo> list = new List<ConsortiaBadgeConfigInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Consortia_Badge_Config_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitConsortiaBadgeConfig(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllConsortiaBadgeConfig", exception);
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

	public ConsortiaBadgeConfigInfo InitConsortiaBadgeConfig(SqlDataReader reader)
	{
		ConsortiaBadgeConfigInfo consortiaBadgeConfigInfo = new ConsortiaBadgeConfigInfo();
		consortiaBadgeConfigInfo.BadgeID = (int)reader["BadgeID"];
		consortiaBadgeConfigInfo.BadgeName = ((reader["BadgeName"] == null) ? "" : reader["BadgeName"].ToString());
		consortiaBadgeConfigInfo.Cost = (int)reader["Cost"];
		consortiaBadgeConfigInfo.LimitLevel = (int)reader["LimitLevel"];
		consortiaBadgeConfigInfo.ValidDate = (int)reader["ValidDate"];
		return consortiaBadgeConfigInfo;
	}

	public ConsortiaLevelInfo[] GetAllConsortiaLevel()
	{
		List<ConsortiaLevelInfo> list = new List<ConsortiaLevelInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Consortia_Level_All");
			while (ResultDataReader.Read())
			{
				ConsortiaLevelInfo consortiaLevelInfo = new ConsortiaLevelInfo();
				consortiaLevelInfo.Count = (int)ResultDataReader["Count"];
				consortiaLevelInfo.Deduct = (int)ResultDataReader["Deduct"];
				consortiaLevelInfo.Level = (int)ResultDataReader["Level"];
				consortiaLevelInfo.NeedGold = (int)ResultDataReader["NeedGold"];
				consortiaLevelInfo.NeedItem = (int)ResultDataReader["NeedItem"];
				consortiaLevelInfo.Reward = (int)ResultDataReader["Reward"];
				consortiaLevelInfo.Riches = (int)ResultDataReader["Riches"];
				consortiaLevelInfo.ShopRiches = (int)ResultDataReader["ShopRiches"];
				consortiaLevelInfo.SmithRiches = (int)ResultDataReader["SmithRiches"];
				consortiaLevelInfo.StoreRiches = (int)ResultDataReader["StoreRiches"];
				consortiaLevelInfo.BufferRiches = (int)ResultDataReader["BufferRiches"];
				list.Add(consortiaLevelInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllConsortiaLevel", exception);
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

	public ConsortiaBufferTempInfo[] GetAllConsortiaBuffTemp()
	{
		List<ConsortiaBufferTempInfo> list = new List<ConsortiaBufferTempInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Consortia_Buff_Temp_All");
			while (ResultDataReader.Read())
			{
				ConsortiaBufferTempInfo consortiaBufferTempInfo = new ConsortiaBufferTempInfo();
				consortiaBufferTempInfo.id = (int)ResultDataReader["id"];
				consortiaBufferTempInfo.name = (string)ResultDataReader["name"];
				consortiaBufferTempInfo.descript = (string)ResultDataReader["descript"];
				consortiaBufferTempInfo.type = (int)ResultDataReader["type"];
				consortiaBufferTempInfo.level = (int)ResultDataReader["level"];
				consortiaBufferTempInfo.value = (int)ResultDataReader["value"];
				consortiaBufferTempInfo.riches = (int)ResultDataReader["riches"];
				consortiaBufferTempInfo.metal = (int)ResultDataReader["metal"];
				consortiaBufferTempInfo.pic = (int)ResultDataReader["pic"];
				consortiaBufferTempInfo.group = (int)ResultDataReader["group"];
				list.Add(consortiaBufferTempInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllConsortiaBuffTemp", exception);
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

	public HomeFishInfo[] GetAllHomeFish()
	{
		List<HomeFishInfo> list = new List<HomeFishInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Home_Fish_Info_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitHomeFishInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("InitHomeFishInfo", exception);
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

	public HomeFishInfo InitHomeFishInfo(SqlDataReader dr)
	{
		HomeFishInfo homeFishInfo = new HomeFishInfo();
		homeFishInfo.ID = (int)dr["ID"];
		homeFishInfo.Name = ((dr["Name"] == null) ? "" : dr["Name"].ToString());
		homeFishInfo.Score = (int)dr["Score"];
		homeFishInfo.Pic = (int)dr["Pic"];
		return homeFishInfo;
	}

	public MagicFusionDataInfo[] GetAllMagicFusionData()
	{
		List<MagicFusionDataInfo> list = new List<MagicFusionDataInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Magic_Fusion_Data_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitMagicFusionDataInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("InitMagicFusionDataInfo", exception);
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

	public MagicFusionDataInfo InitMagicFusionDataInfo(SqlDataReader dr)
	{
		MagicFusionDataInfo magicFusionDataInfo = new MagicFusionDataInfo();
		magicFusionDataInfo.ID = (int)dr["ID"];
		magicFusionDataInfo.ItemID = (int)dr["ItemID"];
		magicFusionDataInfo.Type = (int)dr["Type"];
		magicFusionDataInfo.NeedGold = (int)dr["NeedGold"];
		magicFusionDataInfo.NeedKey = (int)dr["NeedKey"];
		magicFusionDataInfo.Item1 = (int)dr["Item1"];
		magicFusionDataInfo.Item1Count = (int)dr["Item1Count"];
		magicFusionDataInfo.Item2 = (int)dr["Item2"];
		magicFusionDataInfo.Item2Count = (int)dr["Item2Count"];
		magicFusionDataInfo.Item3 = (int)dr["Item3"];
		magicFusionDataInfo.Item3Count = (int)dr["Item3Count"];
		magicFusionDataInfo.Item4 = (int)dr["Item4"];
		magicFusionDataInfo.Item4Count = (int)dr["Item4Count"];
		magicFusionDataInfo.GetKeys = (int)dr["GetKeys"];
		return magicFusionDataInfo;
	}

	public MysteryShopInfo[] GetAllMysteryShop()
	{
		List<MysteryShopInfo> list = new List<MysteryShopInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Mystery_Shop_Items_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitMysteryShopInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("InitMysteryShopInfo", exception);
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

	public MysteryShopInfo InitMysteryShopInfo(SqlDataReader dr)
	{
		MysteryShopInfo mysteryShopInfo = new MysteryShopInfo();
		mysteryShopInfo.ID = (int)dr["ID"];
		mysteryShopInfo.LableType = (int)dr["LableType"];
		mysteryShopInfo.InfoID = (int)dr["InfoID"];
		mysteryShopInfo.Unit = (int)dr["Unit"];
		mysteryShopInfo.Num = (int)dr["Num"];
		mysteryShopInfo.Price = (int)dr["Price"];
		mysteryShopInfo.CanBuy = (int)dr["CanBuy"];
		mysteryShopInfo.Random = (int)dr["Random"];
		return mysteryShopInfo;
	}

	public BoguAdventureRewardInfo[] GetAllBoguAdventureReward()
	{
		List<BoguAdventureRewardInfo> list = new List<BoguAdventureRewardInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_BoguAdventure_Reward_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitBoguAdventureRewardInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("InitBoguAdventureRewardInfo", exception);
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

	public BoguAdventureRewardInfo InitBoguAdventureRewardInfo(SqlDataReader dr)
	{
		BoguAdventureRewardInfo boguAdventureRewardInfo = new BoguAdventureRewardInfo();
		boguAdventureRewardInfo.AwardID = (int)dr["AwardID"];
		boguAdventureRewardInfo.TemplateID = (int)dr["TemplateID"];
		boguAdventureRewardInfo.Count = (int)dr["Count"];
		boguAdventureRewardInfo.ValidDate = (int)dr["ValidDate"];
		boguAdventureRewardInfo.StrengthenLevel = (int)dr["StrengthenLevel"];
		boguAdventureRewardInfo.AttackCompose = (int)dr["AttackCompose"];
		boguAdventureRewardInfo.DefendCompose = (int)dr["DefendCompose"];
		boguAdventureRewardInfo.AgilityCompose = (int)dr["AgilityCompose"];
		boguAdventureRewardInfo.LuckCompose = (int)dr["LuckCompose"];
		boguAdventureRewardInfo.IsBinds = (bool)dr["IsBinds"];
		return boguAdventureRewardInfo;
	}

	public TreasurePuzzleRewardInfo[] GetAllTreasurePuzzleReward()
	{
		List<TreasurePuzzleRewardInfo> list = new List<TreasurePuzzleRewardInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Treasure_Puzzle_Reward_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitTreasurePuzzleRewardInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("InitTreasurePuzzleRewardInfo", exception);
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

	public TreasurePuzzleRewardInfo InitTreasurePuzzleRewardInfo(SqlDataReader dr)
	{
		TreasurePuzzleRewardInfo treasurePuzzleRewardInfo = new TreasurePuzzleRewardInfo();
		treasurePuzzleRewardInfo.PuzzleID = (int)dr["PuzzleID"];
		treasurePuzzleRewardInfo.TemplateID = (int)dr["TemplateID"];
		treasurePuzzleRewardInfo.Count = (int)dr["Count"];
		treasurePuzzleRewardInfo.ValidDate = (int)dr["ValidDate"];
		treasurePuzzleRewardInfo.StrengthenLevel = (int)dr["StrengthenLevel"];
		treasurePuzzleRewardInfo.AttackCompose = (int)dr["AttackCompose"];
		treasurePuzzleRewardInfo.DefendCompose = (int)dr["DefendCompose"];
		treasurePuzzleRewardInfo.AgilityCompose = (int)dr["AgilityCompose"];
		treasurePuzzleRewardInfo.LuckCompose = (int)dr["LuckCompose"];
		treasurePuzzleRewardInfo.IsBinds = (bool)dr["IsBinds"];
		return treasurePuzzleRewardInfo;
	}

	public MountDrawTemplateInfo[] GetAllMountDrawTemplate()
	{
		List<MountDrawTemplateInfo> list = new List<MountDrawTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Mount_Draw_Template_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitMountDrawTemplateInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("InitMountDrawTemplateInfo", exception);
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

	public MountDrawTemplateInfo InitMountDrawTemplateInfo(SqlDataReader dr)
	{
		MountDrawTemplateInfo mountDrawTemplateInfo = new MountDrawTemplateInfo();
		mountDrawTemplateInfo.ID = (int)dr["ID"];
		mountDrawTemplateInfo.TemplateId = (int)dr["TemplateId"];
		mountDrawTemplateInfo.AddHurt = (int)dr["AddHurt"];
		mountDrawTemplateInfo.AddGuard = (int)dr["AddGuard"];
		mountDrawTemplateInfo.MagicAttack = (int)dr["MagicAttack"];
		mountDrawTemplateInfo.MagicDefence = (int)dr["MagicDefence"];
		mountDrawTemplateInfo.AddBlood = (int)dr["AddBlood"];
		mountDrawTemplateInfo.Name = (string)dr["Name"];
		return mountDrawTemplateInfo;
	}

	public MountSkillElementTemplateInfo[] GetAllMountSkillElementTemplateInfo()
	{
		List<MountSkillElementTemplateInfo> list = new List<MountSkillElementTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_MountSkillElementTemplate_All");
			while (ResultDataReader.Read())
			{
				MountSkillElementTemplateInfo mountSkillElementTemplateInfo = new MountSkillElementTemplateInfo();
				mountSkillElementTemplateInfo.ID = (int)ResultDataReader["ID"];
				mountSkillElementTemplateInfo.Name = ResultDataReader["Name"].ToString();
				mountSkillElementTemplateInfo.EffectPic = ResultDataReader["EffectPic"].ToString();
				mountSkillElementTemplateInfo.Description = ResultDataReader["Description"].ToString();
				mountSkillElementTemplateInfo.Pic = (int)ResultDataReader["Pic"];
				list.Add(mountSkillElementTemplateInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetMountSkillElementTemplate", exception);
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

	public MountSkillGetTemplateInfo[] GetAllMountSkillGetTemplateInfo()
	{
		List<MountSkillGetTemplateInfo> list = new List<MountSkillGetTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_MountSkillGetTemplate_All");
			while (ResultDataReader.Read())
			{
				MountSkillGetTemplateInfo mountSkillGetTemplateInfo = new MountSkillGetTemplateInfo();
				mountSkillGetTemplateInfo.ID = (int)ResultDataReader["ID"];
				mountSkillGetTemplateInfo.Level = (int)ResultDataReader["Level"];
				mountSkillGetTemplateInfo.SkillID = (int)ResultDataReader["SkillID"];
				mountSkillGetTemplateInfo.NextID = (int)ResultDataReader["NextID"];
				mountSkillGetTemplateInfo.Type = (int)ResultDataReader["Type"];
				mountSkillGetTemplateInfo.Exp = (int)ResultDataReader["Exp"];
				list.Add(mountSkillGetTemplateInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetMountSkillGetTemplate", exception);
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

	public MountSkillTemplateInfo[] GetAllMountSkillTemplateInfo()
	{
		List<MountSkillTemplateInfo> list = new List<MountSkillTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_MountSkillTemplate_All");
			while (ResultDataReader.Read())
			{
				MountSkillTemplateInfo mountSkillTemplateInfo = new MountSkillTemplateInfo();
				mountSkillTemplateInfo.ID = (int)ResultDataReader["ID"];
				mountSkillTemplateInfo.Name = ResultDataReader["Name"].ToString();
				mountSkillTemplateInfo.ElementIDs = ResultDataReader["ElementIDs"].ToString();
				mountSkillTemplateInfo.Description = ResultDataReader["Description"].ToString();
				mountSkillTemplateInfo.BallType = (int)ResultDataReader["BallType"];
				mountSkillTemplateInfo.NewBallID = (int)ResultDataReader["NewBallID"];
				mountSkillTemplateInfo.CostMP = (int)ResultDataReader["CostMP"];
				mountSkillTemplateInfo.Pic = (int)ResultDataReader["Pic"];
				mountSkillTemplateInfo.Action = ResultDataReader["Action"].ToString();
				mountSkillTemplateInfo.EffectPic = ResultDataReader["EffectPic"].ToString();
				mountSkillTemplateInfo.Delay = (int)ResultDataReader["Delay"];
				mountSkillTemplateInfo.ColdDown = (int)ResultDataReader["ColdDown"];
				mountSkillTemplateInfo.GameType = (int)ResultDataReader["GameType"];
				mountSkillTemplateInfo.Probability = (int)ResultDataReader["Probability"];
				mountSkillTemplateInfo.CostEnergy = (int)ResultDataReader["CostEnergy"];
				mountSkillTemplateInfo.UseCount = (int)ResultDataReader["UseCount"];
				list.Add(mountSkillTemplateInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetMountSkillTemplate", exception);
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

	public MountTemplateInfo[] GetAllMountTemplateInfo()
	{
		List<MountTemplateInfo> list = new List<MountTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_MountTemplate_All");
			while (ResultDataReader.Read())
			{
				MountTemplateInfo mountTemplateInfo = new MountTemplateInfo();
				mountTemplateInfo.Grade = (int)ResultDataReader["Grade"];
				mountTemplateInfo.Experience = (int)ResultDataReader["Experience"];
				mountTemplateInfo.SkillID = (int)ResultDataReader["SkillID"];
				mountTemplateInfo.AddDamage = (int)ResultDataReader["AddDamage"];
				mountTemplateInfo.AddGuard = (int)ResultDataReader["AddGuard"];
				mountTemplateInfo.MagicAttack = (int)ResultDataReader["MagicAttack"];
				mountTemplateInfo.MagicDefence = (int)ResultDataReader["MagicDefence"];
				mountTemplateInfo.AddBlood = (int)ResultDataReader["AddBlood"];
				list.Add(mountTemplateInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetMountTemplate", exception);
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

	public ActivitySystemItemInfo[] GetAllActivitySystemItem()
	{
		List<ActivitySystemItemInfo> list = new List<ActivitySystemItemInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_ActivitySystemItem_All");
			while (ResultDataReader.Read())
			{
				ActivitySystemItemInfo activitySystemItemInfo = new ActivitySystemItemInfo();
				activitySystemItemInfo.ID = (int)ResultDataReader["ID"];
				activitySystemItemInfo.ActivityType = (int)ResultDataReader["ActivityType"];
				activitySystemItemInfo.Quality = (int)ResultDataReader["Quality"];
				activitySystemItemInfo.TemplateID = (int)ResultDataReader["TemplateID"];
				activitySystemItemInfo.Count = (int)ResultDataReader["Count"];
				activitySystemItemInfo.ValidDate = (int)ResultDataReader["ValidDate"];
				activitySystemItemInfo.IsBind = (bool)ResultDataReader["IsBind"];
				activitySystemItemInfo.StrengthLevel = (int)ResultDataReader["StrengthLevel"];
				activitySystemItemInfo.AttackCompose = (int)ResultDataReader["AttackCompose"];
				activitySystemItemInfo.DefendCompose = (int)ResultDataReader["DefendCompose"];
				activitySystemItemInfo.AgilityCompose = (int)ResultDataReader["AgilityCompose"];
				activitySystemItemInfo.LuckCompose = (int)ResultDataReader["LuckCompose"];
				activitySystemItemInfo.Probability = (int)ResultDataReader["Probability"];
				activitySystemItemInfo.canRenew = (bool)ResultDataReader["canRenew"];
				activitySystemItemInfo.canTransfer = (bool)ResultDataReader["canTransfer"];
				list.Add(activitySystemItemInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllActivitySystemItem", exception);
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

	public TreeTemplateInfo[] GetAllTreeTemplate()
	{
		List<TreeTemplateInfo> list = new List<TreeTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Tree_Template_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitTreeTemplateInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("InitTreeTemplateInfo", exception);
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

	public TreeTemplateInfo InitTreeTemplateInfo(SqlDataReader dr)
	{
		TreeTemplateInfo treeTemplateInfo = new TreeTemplateInfo();
		treeTemplateInfo.Level = (int)dr["Level"];
		treeTemplateInfo.AwardID = (int)dr["AwardID"];
		treeTemplateInfo.CostExp = (int)dr["CostExp"];
		treeTemplateInfo.MonsterExp = (int)dr["MonsterExp"];
		treeTemplateInfo.MonsterID = (int)dr["MonsterID"];
		treeTemplateInfo.MonsterName = (string)dr["MonsterName"];
		treeTemplateInfo.Exp = (int)dr["Exp"];
		return treeTemplateInfo;
	}

	public SuitTemplateInfo[] GetAllSuitTemplate()
	{
		List<SuitTemplateInfo> list = new List<SuitTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Suit_Template_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitSuitTemplateInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("InitSuitTemplateInfo", exception);
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

	public SuitTemplateInfo InitSuitTemplateInfo(SqlDataReader dr)
	{
		SuitTemplateInfo suitTemplateInfo = new SuitTemplateInfo();
		suitTemplateInfo.SuitId = (int)dr["SuitId"];
		suitTemplateInfo.SuitName = (string)dr["SuitName"];
		suitTemplateInfo.EqipCount1 = (int)dr["EqipCount1"];
		suitTemplateInfo.SkillDescribe1 = (string)dr["SkillDescribe1"];
		suitTemplateInfo.Skill1 = (string)dr["Skill1"];
		suitTemplateInfo.EqipCount2 = (int)dr["EqipCount2"];
		suitTemplateInfo.SkillDescribe2 = (string)dr["SkillDescribe2"];
		suitTemplateInfo.Skill2 = (string)dr["Skill2"];
		suitTemplateInfo.EqipCount3 = (int)dr["EqipCount3"];
		suitTemplateInfo.SkillDescribe3 = (string)dr["SkillDescribe3"];
		suitTemplateInfo.Skill3 = (string)dr["Skill3"];
		suitTemplateInfo.EqipCount4 = (int)dr["EqipCount4"];
		suitTemplateInfo.SkillDescribe4 = (string)dr["SkillDescribe4"];
		suitTemplateInfo.Skill4 = (string)dr["Skill4"];
		suitTemplateInfo.EqipCount5 = (int)dr["EqipCount5"];
		suitTemplateInfo.SkillDescribe5 = (string)dr["SkillDescribe5"];
		suitTemplateInfo.Skill5 = (string)dr["Skill5"];
		return suitTemplateInfo;
	}

	public SuitPartEquipInfo[] GetAllSuitPartEquip()
	{
		List<SuitPartEquipInfo> list = new List<SuitPartEquipInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Suit_Part_Equip_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitSuitPartEquipInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("InitSuitPartEquipInfo", exception);
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

	public SuitPartEquipInfo InitSuitPartEquipInfo(SqlDataReader dr)
	{
		SuitPartEquipInfo suitPartEquipInfo = new SuitPartEquipInfo();
		suitPartEquipInfo.ID = (int)dr["ID"];
		suitPartEquipInfo.ContainEquip = (string)dr["ContainEquip"];
		suitPartEquipInfo.PartName = (string)dr["PartName"];
		return suitPartEquipInfo;
	}

	public SearchGoodsPayMoneyInfo[] GetAllSearchGoodsPayMoney()
	{
		List<SearchGoodsPayMoneyInfo> list = new List<SearchGoodsPayMoneyInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Search_Goods_Pay_Money_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitSearchGoodsPayMoneyInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("InitSearchGoodsPayMoneyInfo", exception);
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

	public SearchGoodsPayMoneyInfo InitSearchGoodsPayMoneyInfo(SqlDataReader dr)
	{
		SearchGoodsPayMoneyInfo searchGoodsPayMoneyInfo = new SearchGoodsPayMoneyInfo();
		searchGoodsPayMoneyInfo.Number = (int)dr["Number"];
		searchGoodsPayMoneyInfo.NeedMoney = (int)dr["NeedMoney"];
		return searchGoodsPayMoneyInfo;
	}

	public RankTemplateInfo[] GetAllRankTemplate()
	{
		List<RankTemplateInfo> list = new List<RankTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Rank_Template_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitRankTemplateInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("InitRankTemplateInfo", exception);
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

	public RankTemplateInfo InitRankTemplateInfo(SqlDataReader dr)
	{
		RankTemplateInfo rankTemplateInfo = new RankTemplateInfo();
		rankTemplateInfo.Rank = (string)dr["Rank"];
		rankTemplateInfo.Attack = (int)dr["Attack"];
		rankTemplateInfo.Defend = (int)dr["Defend"];
		rankTemplateInfo.Agility = (int)dr["Agility"];
		rankTemplateInfo.Lucky = (int)dr["Lucky"];
		return rankTemplateInfo;
	}

	public PetMoePropertyInfo[] GetAllPetMoeProperty()
	{
		List<PetMoePropertyInfo> list = new List<PetMoePropertyInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Pet_Moe_Property_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitPetMoePropertyInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("InitPetMoePropertyInfo", exception);
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

	public PetMoePropertyInfo InitPetMoePropertyInfo(SqlDataReader dr)
	{
		PetMoePropertyInfo petMoePropertyInfo = new PetMoePropertyInfo();
		petMoePropertyInfo.Level = (int)dr["Level"];
		petMoePropertyInfo.Attack = (int)dr["Attack"];
		petMoePropertyInfo.Lucky = (int)dr["Lucky"];
		petMoePropertyInfo.Agility = (int)dr["Agility"];
		petMoePropertyInfo.Blood = (int)dr["Blood"];
		petMoePropertyInfo.Defence = (int)dr["Defence"];
		petMoePropertyInfo.Guard = (int)dr["Guard"];
		petMoePropertyInfo.Exp = (int)dr["Exp"];
		return petMoePropertyInfo;
	}

	public NewTitleInfo[] GetAllNewTitle()
	{
		List<NewTitleInfo> list = new List<NewTitleInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_New_Title_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitNewTitleInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("InitNewTitleInfo", exception);
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

	public NewTitleInfo InitNewTitleInfo(SqlDataReader dr)
	{
		NewTitleInfo newTitleInfo = new NewTitleInfo();
		newTitleInfo.ID = (int)dr["ID"];
		newTitleInfo.Show = (int)dr["Show"];
		newTitleInfo.Name = (string)dr["Name"];
		newTitleInfo.Pic = (int)dr["Pic"];
		newTitleInfo.Att = (int)dr["Att"];
		newTitleInfo.Def = (int)dr["Def"];
		newTitleInfo.Agi = (int)dr["Agi"];
		newTitleInfo.Luck = (int)dr["Luck"];
		return newTitleInfo;
	}

	public LotteryShowTemplateInfo[] GetAllLotteryShowTemplate()
	{
		List<LotteryShowTemplateInfo> list = new List<LotteryShowTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Lottery_Show_Template_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitLotteryShowTemplateInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("InitLotteryShowTemplateInfo", exception);
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

	public LotteryShowTemplateInfo InitLotteryShowTemplateInfo(SqlDataReader dr)
	{
		LotteryShowTemplateInfo lotteryShowTemplateInfo = new LotteryShowTemplateInfo();
		lotteryShowTemplateInfo.BoxType = (int)dr["BoxType"];
		lotteryShowTemplateInfo.SortID = (int)dr["SortID"];
		lotteryShowTemplateInfo.TemplateID = (int)dr["TemplateID"];
		lotteryShowTemplateInfo.Count = (int)dr["Count"];
		return lotteryShowTemplateInfo;
	}

	public LoginAwardItemTemplateInfo[] GetAllLoginAwardItemTemplate()
	{
		List<LoginAwardItemTemplateInfo> list = new List<LoginAwardItemTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Login_Award_Item_Template_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitLoginAwardItemTemplateInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("InitLoginAwardItemTemplateInfo", exception);
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

	public LoginAwardItemTemplateInfo InitLoginAwardItemTemplateInfo(SqlDataReader dr)
	{
		LoginAwardItemTemplateInfo loginAwardItemTemplateInfo = new LoginAwardItemTemplateInfo();
		loginAwardItemTemplateInfo.ID = (int)dr["ID"];
		loginAwardItemTemplateInfo.Count = (int)dr["Count"];
		loginAwardItemTemplateInfo.RewardItemID = (int)dr["RewardItemID"];
		loginAwardItemTemplateInfo.IsSelect = (bool)dr["IsSelect"];
		loginAwardItemTemplateInfo.IsBind = (bool)dr["IsBind"];
		loginAwardItemTemplateInfo.RewardItemValid = (int)dr["RewardItemValid"];
		loginAwardItemTemplateInfo.RewardItemCount = (int)dr["RewardItemCount"];
		loginAwardItemTemplateInfo.StrengthenLevel = (int)dr["StrengthenLevel"];
		loginAwardItemTemplateInfo.AttackCompose = (int)dr["AttackCompose"];
		loginAwardItemTemplateInfo.DefendCompose = (int)dr["DefendCompose"];
		loginAwardItemTemplateInfo.AgilityCompose = (int)dr["AgilityCompose"];
		loginAwardItemTemplateInfo.LuckCompose = (int)dr["LuckCompose"];
		return loginAwardItemTemplateInfo;
	}

	public MagicItemTemplateInfo[] GetAllMagicItemTemplate()
	{
		List<MagicItemTemplateInfo> list = new List<MagicItemTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Magic_Item_Template_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitMagicItemTemplateInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("InitMagicItemTemplateInfo", exception);
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

	public MagicItemTemplateInfo InitMagicItemTemplateInfo(SqlDataReader dr)
	{
		MagicItemTemplateInfo magicItemTemplateInfo = new MagicItemTemplateInfo();
		magicItemTemplateInfo.Lv = (int)dr["Lv"];
		magicItemTemplateInfo.Exp = (int)dr["Exp"];
		magicItemTemplateInfo.MagicAttack = (int)dr["MagicAttack"];
		magicItemTemplateInfo.MagicDefence = (int)dr["MagicDefence"];
		return magicItemTemplateInfo;
	}

	public ChargeActiveTemplateInfo[] GetAllChargeActiveTemplate()
	{
		List<ChargeActiveTemplateInfo> list = new List<ChargeActiveTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Charge_Active_Template_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitChargeActiveTemplateInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("InitChargeActiveTemplateInfo", exception);
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

	public ChargeActiveTemplateInfo InitChargeActiveTemplateInfo(SqlDataReader dr)
	{
		ChargeActiveTemplateInfo chargeActiveTemplateInfo = new ChargeActiveTemplateInfo();
		chargeActiveTemplateInfo.ID = (int)dr["ID"];
		chargeActiveTemplateInfo.Condition = (int)dr["Condition"];
		chargeActiveTemplateInfo.Description = (string)dr["Description"];
		chargeActiveTemplateInfo.RegetType = (int)dr["RegetType"];
		chargeActiveTemplateInfo.TypeID = (int)dr["TypeID"];
		return chargeActiveTemplateInfo;
	}

	public AllQuestionsInfo[] GetAllAllQuestions()
	{
		List<AllQuestionsInfo> list = new List<AllQuestionsInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_All_Questions_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitAllQuestionsInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("InitAllQuestionsInfo", exception);
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

	public AllQuestionsInfo InitAllQuestionsInfo(SqlDataReader dr)
	{
		AllQuestionsInfo allQuestionsInfo = new AllQuestionsInfo();
		allQuestionsInfo.QuestionCatalogID = (int)dr["QuestionCatalogID"];
		allQuestionsInfo.QuestionID = (int)dr["QuestionID"];
		allQuestionsInfo.QuestionContent = (string)dr["QuestionContent"];
		allQuestionsInfo.Option1 = (string)dr["Option1"];
		allQuestionsInfo.Option2 = (string)dr["Option2"];
		allQuestionsInfo.Option3 = (string)dr["Option3"];
		allQuestionsInfo.Option4 = (string)dr["Option4"];
		return allQuestionsInfo;
	}

	public HelpGameRewardInfo[] GetAllHelpGameReward()
	{
		List<HelpGameRewardInfo> list = new List<HelpGameRewardInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Help_Game_Reward_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitHelpGameRewardInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("InitHelpGameRewardInfo", exception);
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

	public HelpGameRewardInfo InitHelpGameRewardInfo(SqlDataReader dr)
	{
		HelpGameRewardInfo helpGameRewardInfo = new HelpGameRewardInfo();
		helpGameRewardInfo.MissionID = (int)dr["MissionID"];
		helpGameRewardInfo.Star = (int)dr["Star"];
		helpGameRewardInfo.TemplateID = (int)dr["TemplateID"];
		helpGameRewardInfo.StrengthenLevel = (int)dr["StrengthenLevel"];
		helpGameRewardInfo.AttackCompose = (int)dr["AttackCompose"];
		helpGameRewardInfo.DefendCompose = (int)dr["DefendCompose"];
		helpGameRewardInfo.AgilityCompose = (int)dr["AgilityCompose"];
		helpGameRewardInfo.LuckCompose = (int)dr["LuckCompose"];
		helpGameRewardInfo.Count = (int)dr["Count"];
		helpGameRewardInfo.IsBind = (bool)dr["IsBind"];
		helpGameRewardInfo.IsTime = (bool)dr["IsTime"];
		helpGameRewardInfo.ValidDate = (int)dr["ValidDate"];
		return helpGameRewardInfo;
	}

	public FightLabDropItemInfo[] GetAllFightLabDropItem()
	{
		List<FightLabDropItemInfo> list = new List<FightLabDropItemInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Fight_Lab_Drop_Item_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitFightLabDropItemInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("InitFightLabDropItemInfo", exception);
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

	public FightLabDropItemInfo InitFightLabDropItemInfo(SqlDataReader dr)
	{
		FightLabDropItemInfo fightLabDropItemInfo = new FightLabDropItemInfo();
		fightLabDropItemInfo.ID = (int)dr["ID"];
		fightLabDropItemInfo.Easy = (int)dr["Easy"];
		fightLabDropItemInfo.AwardItem = (int)dr["AwardItem"];
		fightLabDropItemInfo.Count = (int)dr["Count"];
		return fightLabDropItemInfo;
	}

	public EveryDayActiveRewardTemplateInfo[] GetAllEveryDayActiveRewardTemplate()
	{
		List<EveryDayActiveRewardTemplateInfo> list = new List<EveryDayActiveRewardTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Every_Day_Active_Reward_Template_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitEveryDayActiveRewardTemplateInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("InitEveryDayActiveRewardTemplateInfo", exception);
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

	public EveryDayActiveRewardTemplateInfo InitEveryDayActiveRewardTemplateInfo(SqlDataReader dr)
	{
		EveryDayActiveRewardTemplateInfo everyDayActiveRewardTemplateInfo = new EveryDayActiveRewardTemplateInfo();
		everyDayActiveRewardTemplateInfo.ID = (int)dr["ID"];
		everyDayActiveRewardTemplateInfo.RewardID = (int)dr["RewardID"];
		everyDayActiveRewardTemplateInfo.RewardItemID = (string)dr["RewardItemID"];
		everyDayActiveRewardTemplateInfo.IsSelect = (bool)dr["IsSelect"];
		everyDayActiveRewardTemplateInfo.RewardItemValid = (int)dr["RewardItemValid"];
		everyDayActiveRewardTemplateInfo.RewardItemCount = (int)dr["RewardItemCount"];
		everyDayActiveRewardTemplateInfo.StrengthenLevel = (int)dr["StrengthenLevel"];
		everyDayActiveRewardTemplateInfo.AttackCompose = (int)dr["AttackCompose"];
		everyDayActiveRewardTemplateInfo.DefendCompose = (int)dr["DefendCompose"];
		everyDayActiveRewardTemplateInfo.AgilityCompose = (int)dr["AgilityCompose"];
		everyDayActiveRewardTemplateInfo.LuckCompose = (int)dr["LuckCompose"];
		everyDayActiveRewardTemplateInfo.IsBind = (bool)dr["IsBind"];
		return everyDayActiveRewardTemplateInfo;
	}

	public EveryDayActiveProgressInfo[] GetAllEveryDayActiveProgress()
	{
		List<EveryDayActiveProgressInfo> list = new List<EveryDayActiveProgressInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Every_Day_Active_Progress_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitEveryDayActiveProgressInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("InitEveryDayActiveProgressInfo", exception);
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

	public EveryDayActiveProgressInfo InitEveryDayActiveProgressInfo(SqlDataReader dr)
	{
		EveryDayActiveProgressInfo everyDayActiveProgressInfo = new EveryDayActiveProgressInfo();
		everyDayActiveProgressInfo.ID = (int)dr["ID"];
		everyDayActiveProgressInfo.ActiveName = (string)dr["ActiveName"];
		everyDayActiveProgressInfo.ActiveTime = (string)dr["ActiveTime"];
		everyDayActiveProgressInfo.Count = (string)dr["Count"];
		everyDayActiveProgressInfo.Description = (string)dr["Description"];
		everyDayActiveProgressInfo.JumpType = (int)dr["JumpType"];
		everyDayActiveProgressInfo.LevelLimit = (int)dr["LevelLimit"];
		everyDayActiveProgressInfo.DayOfWeek = (string)dr["DayOfWeek"];
		return everyDayActiveProgressInfo;
	}

	public EveryDayActivePointTemplateInfo[] GetAllEveryDayActivePointTemplate()
	{
		List<EveryDayActivePointTemplateInfo> list = new List<EveryDayActivePointTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Every_Day_Active_Point_Template_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitEveryDayActivePointTemplateInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("InitEveryDayActivePointTemplateInfo", exception);
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

	public EveryDayActivePointTemplateInfo InitEveryDayActivePointTemplateInfo(SqlDataReader dr)
	{
		EveryDayActivePointTemplateInfo everyDayActivePointTemplateInfo = new EveryDayActivePointTemplateInfo();
		everyDayActivePointTemplateInfo.ID = (int)dr["ID"];
		everyDayActivePointTemplateInfo.MinLevel = (int)dr["MinLevel"];
		everyDayActivePointTemplateInfo.MaxLevel = (int)dr["MaxLevel"];
		everyDayActivePointTemplateInfo.ActivityType = (int)dr["ActivityType"];
		everyDayActivePointTemplateInfo.JumpType = (int)dr["JumpType"];
		everyDayActivePointTemplateInfo.Description = (string)dr["Description"];
		everyDayActivePointTemplateInfo.Count = (int)dr["Count"];
		everyDayActivePointTemplateInfo.ActivePoint = (int)dr["ActivePoint"];
		everyDayActivePointTemplateInfo.MoneyPoint = (int)dr["MoneyPoint"];
		return everyDayActivePointTemplateInfo;
	}

	public DiceGameAwardInfo[] GetAllDiceGameAward()
	{
		List<DiceGameAwardInfo> list = new List<DiceGameAwardInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Dice_Game_Award_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitDiceGameAwardInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("InitDiceGameAwardInfo", exception);
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

	public DiceGameAwardInfo InitDiceGameAwardInfo(SqlDataReader dr)
	{
		DiceGameAwardInfo diceGameAwardInfo = new DiceGameAwardInfo();
		diceGameAwardInfo.rank = (int)dr["rank"];
		diceGameAwardInfo.template = (int)dr["template"];
		diceGameAwardInfo.count = (int)dr["count"];
		return diceGameAwardInfo;
	}

	public DailyLeagueAwardInfo[] GetAllDailyLeagueAward()
	{
		List<DailyLeagueAwardInfo> list = new List<DailyLeagueAwardInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Daily_League_Award_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitDailyLeagueAwardInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("InitDailyLeagueAwardInfo", exception);
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

	public DailyLeagueAwardInfo InitDailyLeagueAwardInfo(SqlDataReader dr)
	{
		DailyLeagueAwardInfo dailyLeagueAwardInfo = new DailyLeagueAwardInfo();
		dailyLeagueAwardInfo.Level = (int)dr["Level"];
		dailyLeagueAwardInfo.Class = (int)dr["Class"];
		dailyLeagueAwardInfo.Count = (int)dr["Count"];
		dailyLeagueAwardInfo.TemplateID = (int)dr["TemplateID"];
		dailyLeagueAwardInfo.RewardID = (int)dr["RewardID"];
		dailyLeagueAwardInfo.StrengthenLevel = (int)dr["StrengthenLevel"];
		dailyLeagueAwardInfo.ItemValid = (int)dr["ItemValid"];
		dailyLeagueAwardInfo.IsBind = (bool)dr["IsBind"];
		dailyLeagueAwardInfo.AgilityCompose = (int)dr["AgilityCompose"];
		dailyLeagueAwardInfo.AttackCompose = (int)dr["AttackCompose"];
		dailyLeagueAwardInfo.DefendCompose = (int)dr["DefendCompose"];
		dailyLeagueAwardInfo.LuckCompose = (int)dr["LuckCompose"];
		dailyLeagueAwardInfo.Hole1 = (int)dr["Hole1"];
		dailyLeagueAwardInfo.Hole2 = (int)dr["Hole2"];
		dailyLeagueAwardInfo.Hole3 = (int)dr["Hole3"];
		dailyLeagueAwardInfo.Hole4 = (int)dr["Hole4"];
		dailyLeagueAwardInfo.Hole5 = (int)dr["Hole5"];
		dailyLeagueAwardInfo.Hole5Exp = (int)dr["Hole5Exp"];
		dailyLeagueAwardInfo.Hole5Level = (int)dr["Hole5Level"];
		dailyLeagueAwardInfo.Hole6 = (int)dr["Hole6"];
		dailyLeagueAwardInfo.Hole6Exp = (int)dr["Hole6Exp"];
		dailyLeagueAwardInfo.Hole6Level = (int)dr["Hole6Level"];
		return dailyLeagueAwardInfo;
	}

	public CommunalActiveInfo[] GetAllCommunalActive()
	{
		List<CommunalActiveInfo> list = new List<CommunalActiveInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_CommunalActive_All");
			while (ResultDataReader.Read())
			{
				CommunalActiveInfo communalActiveInfo = new CommunalActiveInfo();
				communalActiveInfo.ActiveID = (int)ResultDataReader["ActiveID"];
				communalActiveInfo.BeginTime = (DateTime)ResultDataReader["BeginTime"];
				communalActiveInfo.EndTime = (DateTime)ResultDataReader["EndTime"];
				communalActiveInfo.LimitGrade = (int)ResultDataReader["LimitGrade"];
				communalActiveInfo.DayMaxScore = (int)ResultDataReader["DayMaxScore"];
				communalActiveInfo.MinScore = (int)ResultDataReader["MinScore"];
				communalActiveInfo.AddPropertyByMoney = (string)ResultDataReader["AddPropertyByMoney"];
				communalActiveInfo.AddPropertyByProp = (string)ResultDataReader["AddPropertyByProp"];
				communalActiveInfo.CanReset = (bool)ResultDataReader["IsReset"];
				communalActiveInfo.CanSendAward = (bool)ResultDataReader["IsSendAward"];
				communalActiveInfo.IsOpen = (bool)ResultDataReader["IsOpen"];
				communalActiveInfo.CanAwardArea = (bool)ResultDataReader["CanAwardArea"];
				list.Add(communalActiveInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllCommunalActive", exception);
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

	public CommunalActiveAwardInfo[] GetAllCommunalActiveAward()
	{
		List<CommunalActiveAwardInfo> list = new List<CommunalActiveAwardInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_CommunalActiveAward_All");
			while (ResultDataReader.Read())
			{
				CommunalActiveAwardInfo communalActiveAwardInfo = new CommunalActiveAwardInfo();
				communalActiveAwardInfo.ID = (int)ResultDataReader["ID"];
				communalActiveAwardInfo.ActiveID = (int)ResultDataReader["ActiveID"];
				communalActiveAwardInfo.IsArea = (int)ResultDataReader["IsArea"];
				communalActiveAwardInfo.RandID = (int)ResultDataReader["RandID"];
				communalActiveAwardInfo.TemplateID = (int)ResultDataReader["TemplateID"];
				communalActiveAwardInfo.StrengthenLevel = (int)ResultDataReader["StrengthenLevel"];
				communalActiveAwardInfo.AttackCompose = (int)ResultDataReader["AttackCompose"];
				communalActiveAwardInfo.DefendCompose = (int)ResultDataReader["DefendCompose"];
				communalActiveAwardInfo.AgilityCompose = (int)ResultDataReader["AgilityCompose"];
				communalActiveAwardInfo.LuckCompose = (int)ResultDataReader["LuckCompose"];
				communalActiveAwardInfo.Count = (int)ResultDataReader["Count"];
				communalActiveAwardInfo.IsBind = (bool)ResultDataReader["IsBind"];
				communalActiveAwardInfo.IsTime = (bool)ResultDataReader["IsTime"];
				communalActiveAwardInfo.ValidDate = (int)ResultDataReader["ValidDate"];
				list.Add(communalActiveAwardInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllCommunalActiveAward", exception);
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

	public CommunalActiveExpInfo[] GetAllCommunalActiveExp()
	{
		List<CommunalActiveExpInfo> list = new List<CommunalActiveExpInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_CommunalActiveExp_All");
			while (ResultDataReader.Read())
			{
				CommunalActiveExpInfo communalActiveExpInfo = new CommunalActiveExpInfo();
				communalActiveExpInfo.ActiveID = (int)ResultDataReader["ActiveID"];
				communalActiveExpInfo.Grade = (int)ResultDataReader["Grade"];
				communalActiveExpInfo.Exp = (int)ResultDataReader["Exp"];
				communalActiveExpInfo.AddExpPlus = (int)ResultDataReader["AddExpPlus"];
				list.Add(communalActiveExpInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllCommunalActiveExp", exception);
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

	public ChargeSpendRewardTemplateInfo[] GetAllChargeSpendRewardTemplate()
	{
		List<ChargeSpendRewardTemplateInfo> list = new List<ChargeSpendRewardTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Charge_Spend_Reward_Template_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitChargeSpendRewardTemplateInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("InitChargeSpendRewardTemplateInfo", exception);
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

	public ChargeSpendRewardTemplateInfo InitChargeSpendRewardTemplateInfo(SqlDataReader dr)
	{
		ChargeSpendRewardTemplateInfo chargeSpendRewardTemplateInfo = new ChargeSpendRewardTemplateInfo();
		chargeSpendRewardTemplateInfo.ID = (int)dr["ID"];
		chargeSpendRewardTemplateInfo.RewardID = (int)dr["RewardID"];
		chargeSpendRewardTemplateInfo.RewardItemID = (int)dr["RewardItemID"];
		chargeSpendRewardTemplateInfo.IsSelect = (bool)dr["IsSelect"];
		chargeSpendRewardTemplateInfo.IsBind = (bool)dr["IsBind"];
		chargeSpendRewardTemplateInfo.RewardItemValid = (int)dr["RewardItemValid"];
		chargeSpendRewardTemplateInfo.RewardItemCount = (int)dr["RewardItemCount"];
		chargeSpendRewardTemplateInfo.StrengthenLevel = (int)dr["StrengthenLevel"];
		chargeSpendRewardTemplateInfo.AttackCompose = (int)dr["AttackCompose"];
		chargeSpendRewardTemplateInfo.DefendCompose = (int)dr["DefendCompose"];
		chargeSpendRewardTemplateInfo.AgilityCompose = (int)dr["AgilityCompose"];
		chargeSpendRewardTemplateInfo.LuckCompose = (int)dr["LuckCompose"];
		return chargeSpendRewardTemplateInfo;
	}

	public CardBuffInfo[] GetAllCardBuff()
	{
		List<CardBuffInfo> list = new List<CardBuffInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Card_Buff_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitCardBuffInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("InitCardBuffInfo", exception);
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

	public CardBuffInfo InitCardBuffInfo(SqlDataReader dr)
	{
		CardBuffInfo cardBuffInfo = new CardBuffInfo();
		cardBuffInfo.condition = (int)dr["condition"];
		cardBuffInfo.PropertiesDscripID = (int)dr["PropertiesDscripID"];
		cardBuffInfo.value = (string)dr["value"];
		cardBuffInfo.Description = (string)dr["Description"];
		return cardBuffInfo;
	}

	public CardInfo[] GetAllCard()
	{
		List<CardInfo> list = new List<CardInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Card_Info_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitCardInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("InitCardInfo", exception);
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

	public CardInfo InitCardInfo(SqlDataReader dr)
	{
		CardInfo cardInfo = new CardInfo();
		cardInfo.ID = (int)dr["ID"];
		cardInfo.SuitID = (int)dr["SuitID"];
		cardInfo.Name = (string)dr["Name"];
		cardInfo.Description = (string)dr["Description"];
		return cardInfo;
	}

	public CardGrooveUpdateInfo[] GetAllCardGrooveUpdate()
	{
		List<CardGrooveUpdateInfo> list = new List<CardGrooveUpdateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_CardGrooveUpdate_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitCardGrooveUpdate(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllCardGrooveUpdate", exception);
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

	public CardGrooveUpdateInfo InitCardGrooveUpdate(SqlDataReader reader)
	{
		CardGrooveUpdateInfo cardGrooveUpdateInfo = new CardGrooveUpdateInfo();
		cardGrooveUpdateInfo.ID = (int)reader["ID"];
		cardGrooveUpdateInfo.Attack = (int)reader["Attack"];
		cardGrooveUpdateInfo.Defend = (int)reader["Defend"];
		cardGrooveUpdateInfo.Agility = (int)reader["Agility"];
		cardGrooveUpdateInfo.Lucky = (int)reader["Lucky"];
		cardGrooveUpdateInfo.Damage = (int)reader["Damage"];
		cardGrooveUpdateInfo.Guard = (int)reader["Guard"];
		cardGrooveUpdateInfo.Level = (int)reader["Level"];
		cardGrooveUpdateInfo.Type = (int)reader["Type"];
		cardGrooveUpdateInfo.Exp = (int)reader["Exp"];
		return cardGrooveUpdateInfo;
	}

	public CardTemplateInfo[] GetAllCardTemplate()
	{
		List<CardTemplateInfo> list = new List<CardTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_CardTemplate_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitCardTemplate(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllCardTemplateInfo", exception);
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

	public CardTemplateInfo InitCardTemplate(SqlDataReader reader)
	{
		CardTemplateInfo cardTemplateInfo = new CardTemplateInfo();
		cardTemplateInfo.ID = (int)reader["ID"];
		cardTemplateInfo.CardID = (int)reader["CardID"];
		cardTemplateInfo.CardType = (int)reader["CardType"];
		cardTemplateInfo.probability = (int)reader["probability"];
		cardTemplateInfo.AttackRate = (int)reader["AttackRate"];
		cardTemplateInfo.AddAttack = (int)reader["AddAttack"];
		cardTemplateInfo.DefendRate = (int)reader["DefendRate"];
		cardTemplateInfo.AddDefend = (int)reader["AddDefend"];
		cardTemplateInfo.AgilityRate = (int)reader["AgilityRate"];
		cardTemplateInfo.AddAgility = (int)reader["AddAgility"];
		cardTemplateInfo.LuckyRate = (int)reader["LuckyRate"];
		cardTemplateInfo.AddLucky = (int)reader["AddLucky"];
		cardTemplateInfo.DamageRate = (int)reader["DamageRate"];
		cardTemplateInfo.AddDamage = (int)reader["AddDamage"];
		cardTemplateInfo.GuardRate = (int)reader["GuardRate"];
		cardTemplateInfo.AddGuard = (int)reader["AddGuard"];
		return cardTemplateInfo;
	}

	public ItemStrengThenDataInfo[] GetAllItemStrengThenData()
	{
		List<ItemStrengThenDataInfo> list = new List<ItemStrengThenDataInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Item_Streng_Then_Data_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitItemStrengThenDataInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("InitItemStrengThenDataInfo", exception);
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

	public ItemStrengThenDataInfo InitItemStrengThenDataInfo(SqlDataReader dr)
	{
		ItemStrengThenDataInfo itemStrengThenDataInfo = new ItemStrengThenDataInfo();
		itemStrengThenDataInfo.TemplateID = (int)dr["TemplateID"];
		itemStrengThenDataInfo.StrengthenLevel = (int)dr["StrengthenLevel"];
		itemStrengThenDataInfo.Data = (int)dr["Data"];
		return itemStrengThenDataInfo;
	}

	public DiceLevelAwardInfo[] GetDiceLevelAwardInfos()
	{
		List<DiceLevelAwardInfo> list = new List<DiceLevelAwardInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_DiceLevelAward_All");
			while (ResultDataReader.Read())
			{
				DiceLevelAwardInfo diceLevelAwardInfo = new DiceLevelAwardInfo();
				diceLevelAwardInfo.ID = (int)ResultDataReader["ID"];
				diceLevelAwardInfo.DiceLevel = (int)ResultDataReader["DiceLevel"];
				diceLevelAwardInfo.TemplateID = (int)ResultDataReader["TemplateID"];
				diceLevelAwardInfo.Count = (int)ResultDataReader["Count"];
				diceLevelAwardInfo.ValidDate = (int)ResultDataReader["ValidDate"];
				diceLevelAwardInfo.IsBinds = (bool)ResultDataReader["IsBinds"];
				diceLevelAwardInfo.StrengthenLevel = (int)ResultDataReader["StrengthenLevel"];
				diceLevelAwardInfo.AttackCompose = (int)ResultDataReader["AttackCompose"];
				diceLevelAwardInfo.DefendCompose = (int)ResultDataReader["DefendCompose"];
				diceLevelAwardInfo.AgilityCompose = (int)ResultDataReader["AgilityCompose"];
				diceLevelAwardInfo.LuckCompose = (int)ResultDataReader["LuckCompose"];
				diceLevelAwardInfo.Random = (int)ResultDataReader["Random"];
				list.Add(diceLevelAwardInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllDiceLevelAward", exception);
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

	public ClothPropertyTemplateInfo[] GetAllClothPropertyTemplateInfos()
	{
		List<ClothPropertyTemplateInfo> list = new List<ClothPropertyTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_ClothPropertyTemplate_All");
			while (ResultDataReader.Read())
			{
				ClothPropertyTemplateInfo clothPropertyTemplateInfo = new ClothPropertyTemplateInfo();
				clothPropertyTemplateInfo.ID = (int)ResultDataReader["ID"];
				clothPropertyTemplateInfo.Sex = (int)ResultDataReader["Sex"];
				clothPropertyTemplateInfo.Name = ((ResultDataReader["Name"] == null) ? "" : ResultDataReader["Name"].ToString());
				clothPropertyTemplateInfo.Attack = (int)ResultDataReader["Attack"];
				clothPropertyTemplateInfo.Defend = (int)ResultDataReader["Defend"];
				clothPropertyTemplateInfo.Agility = (int)ResultDataReader["Agility"];
				clothPropertyTemplateInfo.Luck = (int)ResultDataReader["Luck"];
				clothPropertyTemplateInfo.Blood = (int)ResultDataReader["Blood"];
				clothPropertyTemplateInfo.Damage = (int)ResultDataReader["Damage"];
				clothPropertyTemplateInfo.Guard = (int)ResultDataReader["Guard"];
				clothPropertyTemplateInfo.Cost = (int)ResultDataReader["Cost"];
				clothPropertyTemplateInfo.Type = (int)ResultDataReader["Type"];
				list.Add(clothPropertyTemplateInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllClothPropertyTemplateInfos", exception);
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

	public ClothGroupTemplateInfo[] GetAllClothGroupTemplateInfos()
	{
		List<ClothGroupTemplateInfo> list = new List<ClothGroupTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_ClothGroupTemplate_All");
			while (ResultDataReader.Read())
			{
				ClothGroupTemplateInfo clothGroupTemplateInfo = new ClothGroupTemplateInfo();
				clothGroupTemplateInfo.ID = (int)ResultDataReader["ID"];
				clothGroupTemplateInfo.TemplateID = (int)ResultDataReader["TemplateID"];
				clothGroupTemplateInfo.Sex = (int)ResultDataReader["Sex"];
				clothGroupTemplateInfo.Description = (int)ResultDataReader["Description"];
				clothGroupTemplateInfo.Cost = (int)ResultDataReader["Cost"];
				clothGroupTemplateInfo.Type = (int)ResultDataReader["Type"];
				clothGroupTemplateInfo.Type = (int)ResultDataReader["Type"];
				clothGroupTemplateInfo.OtherTemplateID = ((ResultDataReader["OtherTemplateID"] == null) ? "" : ResultDataReader["OtherTemplateID"].ToString());
				list.Add(clothGroupTemplateInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllClothGroupTemplateInfos", exception);
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

	public EventAwardInfo[] GetEventAwardInfos()
	{
		List<EventAwardInfo> list = new List<EventAwardInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_EventAwardItem_All");
			while (ResultDataReader.Read())
			{
				EventAwardInfo eventAwardInfo = new EventAwardInfo();
				eventAwardInfo.ID = (int)ResultDataReader["ID"];
				eventAwardInfo.ActivityType = (int)ResultDataReader["ActivityType"];
				eventAwardInfo.TemplateID = (int)ResultDataReader["TemplateID"];
				eventAwardInfo.Count = (int)ResultDataReader["Count"];
				eventAwardInfo.ValidDate = (int)ResultDataReader["ValidDate"];
				eventAwardInfo.IsBinds = (bool)ResultDataReader["IsBinds"];
				eventAwardInfo.StrengthenLevel = (int)ResultDataReader["StrengthenLevel"];
				eventAwardInfo.AttackCompose = (int)ResultDataReader["AttackCompose"];
				eventAwardInfo.DefendCompose = (int)ResultDataReader["DefendCompose"];
				eventAwardInfo.AgilityCompose = (int)ResultDataReader["AgilityCompose"];
				eventAwardInfo.LuckCompose = (int)ResultDataReader["LuckCompose"];
				eventAwardInfo.Random = (int)ResultDataReader["Random"];
				eventAwardInfo.MagicAttack = (int)ResultDataReader["MagicAttack"];
				eventAwardInfo.MagicDefence = (int)ResultDataReader["MagicDefence"];
				eventAwardInfo.SelectIndex = (int)ResultDataReader["SelectIndex"];
				eventAwardInfo.IsSelect = (bool)ResultDataReader["IsSelect"];
				list.Add(eventAwardInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllEventAward", exception);
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

	public LoadUserBoxInfo[] GetAllTimeBoxAward()
	{
		List<LoadUserBoxInfo> list = new List<LoadUserBoxInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_TimeBox_Award_All");
			while (ResultDataReader.Read())
			{
				LoadUserBoxInfo loadUserBoxInfo = new LoadUserBoxInfo();
				loadUserBoxInfo.ID = (int)ResultDataReader["ID"];
				loadUserBoxInfo.Type = (int)ResultDataReader["Type"];
				loadUserBoxInfo.Level = (int)ResultDataReader["Level"];
				loadUserBoxInfo.Condition = (int)ResultDataReader["Condition"];
				loadUserBoxInfo.TemplateID = (int)ResultDataReader["TemplateID"];
				list.Add(loadUserBoxInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllDaily", exception);
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

	public QQtipsMessagesInfo[] GetAllQQtipsMessagesLoad()
	{
		List<QQtipsMessagesInfo> list = new List<QQtipsMessagesInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_QQtipsMessages_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitQQtipsMessagesLoad(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllQQtipsMessagesLoad", exception);
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

	public QQtipsMessagesInfo InitQQtipsMessagesLoad(SqlDataReader reader)
	{
		QQtipsMessagesInfo qQtipsMessagesInfo = new QQtipsMessagesInfo();
		qQtipsMessagesInfo.ID = (int)reader["ID"];
		qQtipsMessagesInfo.title = ((reader["title"] == null) ? "QQTips" : reader["title"].ToString());
		qQtipsMessagesInfo.content = ((reader["content"] == null) ? "Thông báo, gợi ý hệ thống" : reader["content"].ToString());
		qQtipsMessagesInfo.maxLevel = (int)reader["maxLevel"];
		qQtipsMessagesInfo.minLevel = (int)reader["minLevel"];
		qQtipsMessagesInfo.outInType = (int)reader["outInType"];
		qQtipsMessagesInfo.moduleType = (int)reader["moduleType"];
		qQtipsMessagesInfo.inItemID = (int)reader["inItemID"];
		qQtipsMessagesInfo.url = ((reader["url"] == null) ? "http://gunny.zing.vn" : reader["url"].ToString());
		qQtipsMessagesInfo.ZoneID = (int)reader["ZoneID"];
		return qQtipsMessagesInfo;
	}

	public MagicStoneInfo[] GetAllMagicStoneTemplate()
	{
		List<MagicStoneInfo> list = new List<MagicStoneInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_MagicStone_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitMagicStoneTemplate(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("Get All MagicStone", exception);
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

	public MagicStoneInfo InitMagicStoneTemplate(SqlDataReader reader)
	{
		MagicStoneInfo magicStoneInfo = new MagicStoneInfo();
		magicStoneInfo.ID = (int)reader["ID"];
		magicStoneInfo.TemplateID = (int)reader["TemplateID"];
		magicStoneInfo.Level = (int)reader["Level"];
		magicStoneInfo.Exp = (int)reader["Exp"];
		magicStoneInfo.Attack = (int)reader["Attack"];
		magicStoneInfo.Defence = (int)reader["Defence"];
		magicStoneInfo.Agility = (int)reader["Agility"];
		magicStoneInfo.Luck = (int)reader["Luck"];
		magicStoneInfo.MagicAttack = (int)reader["MagicAttack"];
		magicStoneInfo.MagicDefence = (int)reader["MagicDefence"];
		return magicStoneInfo;
	}

	public GoldEquipTemplateInfo[] GetAllGoldEquipTemplateLoad()
	{
		List<GoldEquipTemplateInfo> list = new List<GoldEquipTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_GoldEquipTemplateLoad_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitGoldEquipTemplateLoad(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllGoldEquipTemplateLoad", exception);
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

	public GoldEquipTemplateInfo InitGoldEquipTemplateLoad(SqlDataReader reader)
	{
		GoldEquipTemplateInfo goldEquipTemplateInfo = new GoldEquipTemplateInfo();
		goldEquipTemplateInfo.ID = (int)reader["ID"];
		goldEquipTemplateInfo.OldTemplateId = (int)reader["OldTemplateId"];
		goldEquipTemplateInfo.NewTemplateId = (int)reader["NewTemplateId"];
		goldEquipTemplateInfo.CategoryID = (int)reader["CategoryID"];
		goldEquipTemplateInfo.Strengthen = (int)reader["Strengthen"];
		goldEquipTemplateInfo.Attack = (int)reader["Attack"];
		goldEquipTemplateInfo.Defence = (int)reader["Defence"];
		goldEquipTemplateInfo.Agility = (int)reader["Agility"];
		goldEquipTemplateInfo.Luck = (int)reader["Luck"];
		goldEquipTemplateInfo.Damage = (int)reader["Damage"];
		goldEquipTemplateInfo.Guard = (int)reader["Guard"];
		goldEquipTemplateInfo.Boold = (int)reader["Boold"];
		goldEquipTemplateInfo.BlessID = (int)reader["BlessID"];
		goldEquipTemplateInfo.Pic = ((reader["pic"] == null) ? "" : reader["pic"].ToString());
		return goldEquipTemplateInfo;
	}

	public AchievementInfo[] GetALlAchievement()
	{
		List<AchievementInfo> list = new List<AchievementInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Achievement_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitAchievement(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetALlAchievement:", exception);
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

	public AchievementConditionInfo[] GetALlAchievementCondition()
	{
		List<AchievementConditionInfo> list = new List<AchievementConditionInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Achievement_Condition_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitAchievementCondition(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetALlAchievementCondition:", exception);
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

	public AchievementRewardInfo[] GetALlAchievementReward()
	{
		List<AchievementRewardInfo> list = new List<AchievementRewardInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Achievement_Reward_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitAchievementReward(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetALlAchievementReward", exception);
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

	public List<BigBugleInfo> GetAllAreaBigBugleRecord()
	{
		SqlDataReader ResultDataReader = null;
		List<BigBugleInfo> list = new List<BigBugleInfo>();
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Get_AreaBigBugle_Record");
			while (ResultDataReader.Read())
			{
				BigBugleInfo item = new BigBugleInfo
				{
					ID = (int)ResultDataReader["ID"],
					UserID = (int)ResultDataReader["UserID"],
					AreaID = (int)ResultDataReader["AreaID"],
					NickName = ((ResultDataReader["NickName"] == null) ? "" : ResultDataReader["NickName"].ToString()),
					Message = ((ResultDataReader["Message"] == null) ? "" : ResultDataReader["Message"].ToString()),
					State = (bool)ResultDataReader["State"],
					IP = ((ResultDataReader["IP"] == null) ? "" : ResultDataReader["IP"].ToString())
				};
				list.Add(item);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllAreaBigBugleRecord", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list;
	}

	public AchievementInfo InitAchievement(SqlDataReader reader)
	{
		return new AchievementInfo
		{
			ID = (int)reader["ID"],
			PlaceID = (int)reader["PlaceID"],
			Title = ((reader["Title"] == null) ? "" : reader["Title"].ToString()),
			Detail = ((reader["Detail"] == null) ? "" : reader["Detail"].ToString()),
			NeedMinLevel = (int)reader["NeedMinLevel"],
			NeedMaxLevel = (int)reader["NeedMaxLevel"],
			PreAchievementID = ((reader["PreAchievementID"] == null) ? "" : reader["PreAchievementID"].ToString()),
			IsOther = (int)reader["IsOther"],
			AchievementType = (int)reader["AchievementType"],
			CanHide = (bool)reader["CanHide"],
			StartDate = (DateTime)reader["StartDate"],
			EndDate = (DateTime)reader["EndDate"],
			AchievementPoint = (int)reader["AchievementPoint"],
			IsActive = (int)reader["IsActive"],
			PicID = (int)reader["PicID"],
			IsShare = (bool)reader["IsShare"]
		};
	}

	public AchievementConditionInfo InitAchievementCondition(SqlDataReader reader)
	{
		return new AchievementConditionInfo
		{
			AchievementID = (int)reader["AchievementID"],
			CondictionID = (int)reader["CondictionID"],
			CondictionType = (int)reader["CondictionType"],
			Condiction_Para1 = ((reader["Condiction_Para1"] == null) ? "" : reader["Condiction_Para1"].ToString()),
			Condiction_Para2 = (int)reader["Condiction_Para2"]
		};
	}

	public AchievementRewardInfo InitAchievementReward(SqlDataReader reader)
	{
		return new AchievementRewardInfo
		{
			AchievementID = (int)reader["AchievementID"],
			RewardType = (int)reader["RewardType"],
			RewardPara = ((reader["RewardPara"] == null) ? "" : reader["RewardPara"].ToString()),
			RewardValueId = (int)reader["RewardValueId"],
			RewardCount = (int)reader["RewardCount"]
		};
	}

	public ItemRecordTypeInfo[] GetAllItemRecordType()
	{
		List<ItemRecordTypeInfo> list = new List<ItemRecordTypeInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Item_Record_Type_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitItemRecordType(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllItemRecordType:", exception);
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

	public ItemRecordTypeInfo InitItemRecordType(SqlDataReader reader)
	{
		return new ItemRecordTypeInfo
		{
			RecordID = (int)reader["RecordID"],
			Name = ((reader["Name"] == null) ? "" : reader["Name"].ToString()),
			Description = ((reader["Description"] == null) ? "" : reader["Description"].ToString())
		};
	}

	public ItemTemplateInfo[] GetAllGoodsASC()
	{
		List<ItemTemplateInfo> list = new List<ItemTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Items_All_ASC");
			while (ResultDataReader.Read())
			{
				list.Add(InitItemTemplateInfo(ResultDataReader));
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

	public ItemTemplateInfo[] GetAllGoods()
	{
		List<ItemTemplateInfo> list = new List<ItemTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Items_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitItemTemplateInfo(ResultDataReader));
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

	public bool AddItemTemplate(ItemTemplateInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[44]
			{
				new SqlParameter("@TemplateID", info.TemplateID),
				new SqlParameter("@AddTime", info.AddTime),
				new SqlParameter("@Agility", info.Agility),
				new SqlParameter("@Attack", info.Attack),
				new SqlParameter("@CanCompose", info.CanCompose),
				new SqlParameter("@CanDelete", info.CanDelete),
				new SqlParameter("@CanDrop", info.CanDrop),
				new SqlParameter("@CanEquip", info.CanEquip),
				new SqlParameter("@CanStrengthen", info.CanStrengthen),
				new SqlParameter("@CanUse", info.CanUse),
				new SqlParameter("@CategoryID", info.CategoryID),
				new SqlParameter("@Colors", info.Colors),
				new SqlParameter("@Defence", info.Defence),
				new SqlParameter("@Description", info.Description),
				new SqlParameter("@Level", info.Level),
				new SqlParameter("@Luck", info.Luck),
				new SqlParameter("@MaxCount", info.MaxCount),
				new SqlParameter("@Name", info.Name),
				new SqlParameter("@NeedLevel", info.NeedLevel),
				new SqlParameter("@NeedSex", info.NeedSex),
				new SqlParameter("@Pic", info.Pic),
				new SqlParameter("@Data", info.Data),
				new SqlParameter("@Property1", info.Property1),
				new SqlParameter("@Property2", info.Property2),
				new SqlParameter("@Property3", info.Property3),
				new SqlParameter("@Property4", info.Property4),
				new SqlParameter("@Property5", info.Property5),
				new SqlParameter("@Property6", info.Property6),
				new SqlParameter("@Property7", info.Property7),
				new SqlParameter("@Property8", info.Property8),
				new SqlParameter("@Quality", info.Quality),
				new SqlParameter("@Script", info.Script),
				new SqlParameter("@BindType", info.BindType),
				new SqlParameter("@FusionType", info.FusionType),
				new SqlParameter("@FusionRate", info.FusionRate),
				new SqlParameter("@FusionNeedRate", info.FusionNeedRate),
				new SqlParameter("@RefineryLevel", info.RefineryLevel),
				new SqlParameter("@Hole", info.Hole),
				new SqlParameter("@ReclaimValue", info.ReclaimValue),
				new SqlParameter("@ReclaimType", info.ReclaimType),
				new SqlParameter("@CanRecycle", info.CanRecycle),
				new SqlParameter("@FloorPrice", info.FloorPrice),
				new SqlParameter("@SuitId", info.SuitId),
				new SqlParameter("@CanTransfer", info.CanTransfer)
			};
			result = db.RunProcedure("SP_Shop_Goods_Add", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("SP_Shop_Goods_Add", exception);
			}
		}
		return result;
	}

	public ShopGoodsShowListInfo InitShopGoodsShowListInfo(SqlDataReader reader)
	{
		ShopGoodsShowListInfo shopGoodsShowListInfo = new ShopGoodsShowListInfo();
		shopGoodsShowListInfo.Type = (int)reader["Type"];
		shopGoodsShowListInfo.ShopId = (int)reader["ShopId"];
		return shopGoodsShowListInfo;
	}

	public ShopGoodsShowListInfo[] GetAllShopGoodsShowList()
	{
		List<ShopGoodsShowListInfo> list = new List<ShopGoodsShowListInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_ShopGoodsShowList_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitShopGoodsShowListInfo(ResultDataReader));
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

	public ItemBoxInfo[] GetSingleItemsBox(int DataID)
	{
		List<ItemBoxInfo> list = new List<ItemBoxInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@ID", SqlDbType.Int, 4)
			};
			array[0].Value = DataID;
			db.GetReader(ref ResultDataReader, "SP_ItemsBox_Single", array);
			while (ResultDataReader.Read())
			{
				list.Add(InitItemBoxInfo(ResultDataReader));
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

	public ItemTemplateInfo GetSingleGoods(int goodsID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@ID", SqlDbType.Int, 4)
			};
			array[0].Value = goodsID;
			db.GetReader(ref ResultDataReader, "SP_Items_Single", array);
			if (ResultDataReader.Read())
			{
				return InitItemTemplateInfo(ResultDataReader);
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

	public ItemTemplateInfo[] GetSingleCategory(int CategoryID)
	{
		List<ItemTemplateInfo> list = new List<ItemTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@CategoryID", SqlDbType.Int, 4)
			};
			array[0].Value = CategoryID;
			db.GetReader(ref ResultDataReader, "SP_Items_Category_Single", array);
			while (ResultDataReader.Read())
			{
				list.Add(InitItemTemplateInfo(ResultDataReader));
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

	public ItemTemplateInfo[] GetFusionType()
	{
		List<ItemTemplateInfo> list = new List<ItemTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Items_FusionType");
			while (ResultDataReader.Read())
			{
				list.Add(InitItemTemplateInfo(ResultDataReader));
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

	public ItemTemplateInfo InitItemTemplateInfo(SqlDataReader reader)
	{
		ItemTemplateInfo itemTemplateInfo = new ItemTemplateInfo();
		itemTemplateInfo.AddTime = reader["AddTime"].ToString();
		itemTemplateInfo.Agility = (int)reader["Agility"];
		itemTemplateInfo.Attack = (int)reader["Attack"];
		itemTemplateInfo.CanDelete = (bool)reader["CanDelete"];
		itemTemplateInfo.CanDrop = (bool)reader["CanDrop"];
		itemTemplateInfo.CanEquip = (bool)reader["CanEquip"];
		itemTemplateInfo.CanUse = (bool)reader["CanUse"];
		itemTemplateInfo.CategoryID = (int)reader["CategoryID"];
		itemTemplateInfo.Colors = reader["Colors"].ToString();
		itemTemplateInfo.Defence = (int)reader["Defence"];
		itemTemplateInfo.Description = reader["Description"].ToString();
		itemTemplateInfo.Level = (int)reader["Level"];
		itemTemplateInfo.Luck = (int)reader["Luck"];
		itemTemplateInfo.MaxCount = (int)reader["MaxCount"];
		itemTemplateInfo.Name = reader["Name"].ToString();
		itemTemplateInfo.NeedSex = (int)reader["NeedSex"];
		itemTemplateInfo.Pic = reader["Pic"].ToString();
		itemTemplateInfo.Data = ((reader["Data"] == null) ? "" : reader["Data"].ToString());
		itemTemplateInfo.Property1 = (int)reader["Property1"];
		itemTemplateInfo.Property2 = (int)reader["Property2"];
		itemTemplateInfo.Property3 = (int)reader["Property3"];
		itemTemplateInfo.Property4 = (int)reader["Property4"];
		itemTemplateInfo.Property5 = (int)reader["Property5"];
		itemTemplateInfo.Property6 = (int)reader["Property6"];
		itemTemplateInfo.Property7 = (int)reader["Property7"];
		itemTemplateInfo.Property8 = (int)reader["Property8"];
		itemTemplateInfo.Quality = (int)reader["Quality"];
		itemTemplateInfo.Script = reader["Script"].ToString();
		itemTemplateInfo.TemplateID = (int)reader["TemplateID"];
		itemTemplateInfo.CanCompose = (bool)reader["CanCompose"];
		itemTemplateInfo.CanStrengthen = (bool)reader["CanStrengthen"];
		itemTemplateInfo.NeedLevel = (int)reader["NeedLevel"];
		itemTemplateInfo.BindType = (int)reader["BindType"];
		itemTemplateInfo.FusionType = (int)reader["FusionType"];
		itemTemplateInfo.FusionRate = (int)reader["FusionRate"];
		itemTemplateInfo.FusionNeedRate = (int)reader["FusionNeedRate"];
		itemTemplateInfo.Hole = ((reader["Hole"] == null) ? "" : reader["Hole"].ToString());
		itemTemplateInfo.RefineryLevel = (int)reader["RefineryLevel"];
		itemTemplateInfo.ReclaimValue = (int)reader["ReclaimValue"];
		itemTemplateInfo.ReclaimType = (int)reader["ReclaimType"];
		itemTemplateInfo.CanRecycle = (int)reader["CanRecycle"];
		itemTemplateInfo.SuitId = (int)reader["SuitId"];
		itemTemplateInfo.FloorPrice = (int)reader["FloorPrice"];
		itemTemplateInfo.CanTransfer = (int)reader["CanTransfer"];
		itemTemplateInfo.IsDirty = false;
		return itemTemplateInfo;
	}

	public AccumulAtiveLoginAwardInfo[] GetAccumulAtiveLoginAwardInfos()
	{
		List<AccumulAtiveLoginAwardInfo> list = new List<AccumulAtiveLoginAwardInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_AccumulAtiveLoginAward_All");
			while (ResultDataReader.Read())
			{
				AccumulAtiveLoginAwardInfo accumulAtiveLoginAwardInfo = new AccumulAtiveLoginAwardInfo();
				accumulAtiveLoginAwardInfo.ID = (int)ResultDataReader["ID"];
				accumulAtiveLoginAwardInfo.RewardItemID = (int)ResultDataReader["RewardItemID"];
				accumulAtiveLoginAwardInfo.Count = (int)ResultDataReader["Count"];
				accumulAtiveLoginAwardInfo.IsSelect = (bool)ResultDataReader["IsSelect"];
				accumulAtiveLoginAwardInfo.IsBind = (bool)ResultDataReader["IsBind"];
				accumulAtiveLoginAwardInfo.RewardItemValid = (int)ResultDataReader["RewardItemValid"];
				accumulAtiveLoginAwardInfo.RewardItemCount = (int)ResultDataReader["RewardItemCount"];
				accumulAtiveLoginAwardInfo.StrengthenLevel = (int)ResultDataReader["StrengthenLevel"];
				accumulAtiveLoginAwardInfo.AttackCompose = (int)ResultDataReader["AttackCompose"];
				accumulAtiveLoginAwardInfo.DefendCompose = (int)ResultDataReader["DefendCompose"];
				accumulAtiveLoginAwardInfo.AgilityCompose = (int)ResultDataReader["AgilityCompose"];
				accumulAtiveLoginAwardInfo.LuckCompose = (int)ResultDataReader["LuckCompose"];
				list.Add(accumulAtiveLoginAwardInfo);
			}
		}
		catch (Exception ex)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("Accumul_Ative_Login_Award：" + ex);
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

	public ItemBoxInfo[] GetItemBoxInfos()
	{
		List<ItemBoxInfo> list = new List<ItemBoxInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_ItemsBox_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitItemBoxInfo(ResultDataReader));
			}
		}
		catch (Exception ex)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("Init@Shop_Goods_Box：" + ex);
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

	public ItemBoxInfo InitItemBoxInfo(SqlDataReader reader)
	{
		ItemBoxInfo itemBoxInfo = new ItemBoxInfo();
		itemBoxInfo.ID = (int)reader["ID"];
		itemBoxInfo.TemplateId = (int)reader["TemplateId"];
		itemBoxInfo.IsSelect = (bool)reader["IsSelect"];
		itemBoxInfo.IsBind = (bool)reader["IsBind"];
		itemBoxInfo.ItemValid = (int)reader["ItemValid"];
		itemBoxInfo.ItemCount = (int)reader["ItemCount"];
		itemBoxInfo.StrengthenLevel = (int)reader["StrengthenLevel"];
		itemBoxInfo.AttackCompose = (int)reader["AttackCompose"];
		itemBoxInfo.DefendCompose = (int)reader["DefendCompose"];
		itemBoxInfo.AgilityCompose = (int)reader["AgilityCompose"];
		itemBoxInfo.LuckCompose = (int)reader["LuckCompose"];
		itemBoxInfo.Random = (int)reader["Random"];
		itemBoxInfo.IsTips = (int)reader["IsTips"];
		itemBoxInfo.IsLogs = (bool)reader["IsLogs"];
		return itemBoxInfo;
	}

	public bool UpdatePlayerInfoHistory(PlayerInfoHistory info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[4]
			{
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@LastQuestsTime", info.LastQuestsTime),
				new SqlParameter("@LastTreasureTime", info.LastTreasureTime),
				new SqlParameter("@OutPut", SqlDbType.Int)
			};
			array[3].Direction = ParameterDirection.Output;
			db.RunProcedure("SP_User_Update_History", array);
			result = (int)array[6].Value == 1;
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("User_Update_BoxProgression", exception);
			}
		}
		return result;
	}

	public CategoryInfo[] GetAllCategory()
	{
		List<CategoryInfo> list = new List<CategoryInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Items_Category_All");
			while (ResultDataReader.Read())
			{
				CategoryInfo categoryInfo = new CategoryInfo();
				categoryInfo.ID = (int)ResultDataReader["ID"];
				categoryInfo.Name = ((ResultDataReader["Name"] == null) ? "" : ResultDataReader["Name"].ToString());
				categoryInfo.Place = (int)ResultDataReader["Place"];
				categoryInfo.Remark = ((ResultDataReader["Remark"] == null) ? "" : ResultDataReader["Remark"].ToString());
				list.Add(categoryInfo);
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

	public PropInfo[] GetAllProp()
	{
		List<PropInfo> list = new List<PropInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Prop_All");
			while (ResultDataReader.Read())
			{
				PropInfo propInfo = new PropInfo();
				propInfo.AffectArea = (int)ResultDataReader["AffectArea"];
				propInfo.AffectTimes = (int)ResultDataReader["AffectTimes"];
				propInfo.AttackTimes = (int)ResultDataReader["AttackTimes"];
				propInfo.BoutTimes = (int)ResultDataReader["BoutTimes"];
				propInfo.BuyGold = (int)ResultDataReader["BuyGold"];
				propInfo.BuyMoney = (int)ResultDataReader["BuyMoney"];
				propInfo.Category = (int)ResultDataReader["Category"];
				propInfo.Delay = (int)ResultDataReader["Delay"];
				propInfo.Description = ResultDataReader["Description"].ToString();
				propInfo.Icon = ResultDataReader["Icon"].ToString();
				propInfo.ID = (int)ResultDataReader["ID"];
				propInfo.Name = ResultDataReader["Name"].ToString();
				propInfo.Parameter = (int)ResultDataReader["Parameter"];
				propInfo.Pic = ResultDataReader["Pic"].ToString();
				propInfo.Property1 = (int)ResultDataReader["Property1"];
				propInfo.Property2 = (int)ResultDataReader["Property2"];
				propInfo.Property3 = (int)ResultDataReader["Property3"];
				propInfo.Random = (int)ResultDataReader["Random"];
				propInfo.Script = ResultDataReader["Script"].ToString();
				list.Add(propInfo);
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

	public BallInfo[] GetAllBall()
	{
		List<BallInfo> list = new List<BallInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Ball_All");
			while (ResultDataReader.Read())
			{
				BallInfo ballInfo = new BallInfo();
				ballInfo.Amount = (int)ResultDataReader["Amount"];
				ballInfo.ID = (int)ResultDataReader["ID"];
				ballInfo.Name = ResultDataReader["Name"].ToString();
				ballInfo.Crater = ((ResultDataReader["Crater"] == null) ? "" : ResultDataReader["Crater"].ToString());
				ballInfo.Power = (double)ResultDataReader["Power"];
				ballInfo.Radii = (int)ResultDataReader["Radii"];
				ballInfo.AttackResponse = (int)ResultDataReader["AttackResponse"];
				ballInfo.BombPartical = ResultDataReader["BombPartical"].ToString();
				ballInfo.FlyingPartical = ResultDataReader["FlyingPartical"].ToString();
				ballInfo.IsSpin = (bool)ResultDataReader["IsSpin"];
				ballInfo.Mass = (int)ResultDataReader["Mass"];
				ballInfo.SpinV = (int)ResultDataReader["SpinV"];
				ballInfo.SpinVA = (double)ResultDataReader["SpinVA"];
				ballInfo.Wind = (int)ResultDataReader["Wind"];
				ballInfo.DragIndex = (int)ResultDataReader["DragIndex"];
				ballInfo.Weight = (int)ResultDataReader["Weight"];
				ballInfo.Shake = (bool)ResultDataReader["Shake"];
				ballInfo.Delay = (int)ResultDataReader["Delay"];
				ballInfo.ShootSound = ((ResultDataReader["ShootSound"] == null) ? "" : ResultDataReader["ShootSound"].ToString());
				ballInfo.BombSound = ((ResultDataReader["BombSound"] == null) ? "" : ResultDataReader["BombSound"].ToString());
				ballInfo.ActionType = (int)ResultDataReader["ActionType"];
				ballInfo.HasTunnel = (bool)ResultDataReader["HasTunnel"];
				ballInfo.BombType = (int)ResultDataReader["BombType"];
				list.Add(ballInfo);
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

	public BallConfigInfo[] GetAllBallConfig()
	{
		List<BallConfigInfo> list = new List<BallConfigInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Ball_Config_All");
			while (ResultDataReader.Read())
			{
				BallConfigInfo ballConfigInfo = new BallConfigInfo();
				ballConfigInfo.Common = (int)ResultDataReader["Common"];
				ballConfigInfo.TemplateID = (int)ResultDataReader["TemplateID"];
				ballConfigInfo.CommonAddWound = (int)ResultDataReader["CommonAddWound"];
				ballConfigInfo.CommonMultiBall = (int)ResultDataReader["CommonMultiBall"];
				ballConfigInfo.Special = (int)ResultDataReader["Special"];
				list.Add(ballConfigInfo);
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

	public MinigameShopTemplateInfo[] GetAllMinigameShop()
	{
		List<MinigameShopTemplateInfo> list = new List<MinigameShopTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_MinigameShop_All");
			while (ResultDataReader.Read())
			{
				MinigameShopTemplateInfo minigameShopTemplateInfo = new MinigameShopTemplateInfo();
				minigameShopTemplateInfo.ID = (int)ResultDataReader["ID"];
				minigameShopTemplateInfo.ItemID = (int)ResultDataReader["ItemID"];
				minigameShopTemplateInfo.Count = (int)ResultDataReader["Count"];
				minigameShopTemplateInfo.IsBind = (bool)ResultDataReader["IsBind"];
				minigameShopTemplateInfo.Price = (int)ResultDataReader["Price"];
				minigameShopTemplateInfo.LimitCount = (int)ResultDataReader["LimitCount"];
				minigameShopTemplateInfo.Type = (int)ResultDataReader["Type"];
				minigameShopTemplateInfo.Valid = (int)ResultDataReader["Valid"];
				list.Add(minigameShopTemplateInfo);
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

	public ShopItemInfo[] GetALllShop()
	{
		List<ShopItemInfo> list = new List<ShopItemInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Shop_All");
			while (ResultDataReader.Read())
			{
				ShopItemInfo shopItemInfo = new ShopItemInfo();
				shopItemInfo.ID = int.Parse(ResultDataReader["ID"].ToString());
				shopItemInfo.ShopID = int.Parse(ResultDataReader["ShopID"].ToString());
				shopItemInfo.GroupID = int.Parse(ResultDataReader["GroupID"].ToString());
				shopItemInfo.TemplateID = int.Parse(ResultDataReader["TemplateID"].ToString());
				shopItemInfo.BuyType = int.Parse(ResultDataReader["BuyType"].ToString());
				shopItemInfo.IsContinue = bool.Parse(ResultDataReader["IsContinue"].ToString());
				shopItemInfo.IsBind = int.Parse(ResultDataReader["IsBind"].ToString());
				shopItemInfo.Label = int.Parse(ResultDataReader["Label"].ToString());
				shopItemInfo.Beat = decimal.Parse(ResultDataReader["Beat"].ToString());
				shopItemInfo.AUnit = int.Parse(ResultDataReader["AUnit"].ToString());
				shopItemInfo.APrice1 = int.Parse(ResultDataReader["APrice1"].ToString());
				shopItemInfo.AValue1 = int.Parse(ResultDataReader["AValue1"].ToString());
				shopItemInfo.APrice2 = int.Parse(ResultDataReader["APrice2"].ToString());
				shopItemInfo.AValue2 = int.Parse(ResultDataReader["AValue2"].ToString());
				shopItemInfo.APrice3 = int.Parse(ResultDataReader["APrice3"].ToString());
				shopItemInfo.AValue3 = int.Parse(ResultDataReader["AValue3"].ToString());
				shopItemInfo.BUnit = int.Parse(ResultDataReader["BUnit"].ToString());
				shopItemInfo.BPrice1 = int.Parse(ResultDataReader["BPrice1"].ToString());
				shopItemInfo.BValue1 = int.Parse(ResultDataReader["BValue1"].ToString());
				shopItemInfo.BPrice2 = int.Parse(ResultDataReader["BPrice2"].ToString());
				shopItemInfo.BValue2 = int.Parse(ResultDataReader["BValue2"].ToString());
				shopItemInfo.BPrice3 = int.Parse(ResultDataReader["BPrice3"].ToString());
				shopItemInfo.BValue3 = int.Parse(ResultDataReader["BValue3"].ToString());
				shopItemInfo.CUnit = int.Parse(ResultDataReader["CUnit"].ToString());
				shopItemInfo.CPrice1 = int.Parse(ResultDataReader["CPrice1"].ToString());
				shopItemInfo.CValue1 = int.Parse(ResultDataReader["CValue1"].ToString());
				shopItemInfo.CPrice2 = int.Parse(ResultDataReader["CPrice2"].ToString());
				shopItemInfo.CValue2 = int.Parse(ResultDataReader["CValue2"].ToString());
				shopItemInfo.CPrice3 = int.Parse(ResultDataReader["CPrice3"].ToString());
				shopItemInfo.CValue3 = int.Parse(ResultDataReader["CValue3"].ToString());
				shopItemInfo.IsCheap = bool.Parse(ResultDataReader["IsCheap"].ToString());
				shopItemInfo.LimitCount = int.Parse(ResultDataReader["LimitCount"].ToString());
				shopItemInfo.StartDate = DateTime.Parse(ResultDataReader["StartDate"].ToString());
				shopItemInfo.EndDate = DateTime.Parse(ResultDataReader["EndDate"].ToString());
				shopItemInfo.CanBuy = bool.Parse(ResultDataReader["CanBuy"].ToString());
				shopItemInfo.LimitPersonalCount = int.Parse(ResultDataReader["LimitPersonalCount"].ToString());
				shopItemInfo.LimitGrade = int.Parse(ResultDataReader["LimitGrade"].ToString());
				list.Add(shopItemInfo);
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

	public FusionInfo[] GetAllFusion()
	{
		List<FusionInfo> list = new List<FusionInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Fusion_All");
			while (ResultDataReader.Read())
			{
				FusionInfo fusionInfo = new FusionInfo();
				fusionInfo.FusionID = (int)ResultDataReader["FusionID"];
				fusionInfo.Item1 = (int)ResultDataReader["Item1"];
				fusionInfo.Item2 = (int)ResultDataReader["Item2"];
				fusionInfo.Item3 = (int)ResultDataReader["Item3"];
				fusionInfo.Item4 = (int)ResultDataReader["Item4"];
				fusionInfo.Count1 = (int)ResultDataReader["Count1"];
				fusionInfo.Count2 = (int)ResultDataReader["Count2"];
				fusionInfo.Count3 = (int)ResultDataReader["Count3"];
				fusionInfo.Count4 = (int)ResultDataReader["Count4"];
				fusionInfo.Formula = (int)ResultDataReader["Formula"];
				fusionInfo.FusionRate = (int)ResultDataReader["FusionRate"];
				fusionInfo.FusionType = (int)ResultDataReader["FusionType"];
				fusionInfo.Reward = (int)ResultDataReader["Reward"];
				fusionInfo.NeedPower = (int)ResultDataReader["NeedPower"];
				list.Add(fusionInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllFusion", exception);
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

	public StrengthenInfo[] GetAllStrengthen()
	{
		List<StrengthenInfo> list = new List<StrengthenInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Item_Strengthen_All");
			while (ResultDataReader.Read())
			{
				StrengthenInfo strengthenInfo = new StrengthenInfo();
				strengthenInfo.StrengthenLevel = (int)ResultDataReader["StrengthenLevel"];
				strengthenInfo.Random = (int)ResultDataReader["Random"];
				strengthenInfo.Rock = (int)ResultDataReader["Rock"];
				strengthenInfo.Rock1 = (int)ResultDataReader["Rock1"];
				strengthenInfo.Rock2 = (int)ResultDataReader["Rock2"];
				strengthenInfo.Rock3 = (int)ResultDataReader["Rock3"];
				strengthenInfo.StoneLevelMin = (int)ResultDataReader["StoneLevelMin"];
				list.Add(strengthenInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllStrengthen", exception);
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

	public RuneTemplateInfo[] GetAllRuneTemplate()
	{
		List<RuneTemplateInfo> list = new List<RuneTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_RuneTemplate_All");
			while (ResultDataReader.Read())
			{
				RuneTemplateInfo runeTemplateInfo = new RuneTemplateInfo();
				runeTemplateInfo.TemplateID = (int)ResultDataReader["TemplateID"];
				runeTemplateInfo.NextTemplateID = (int)ResultDataReader["NextTemplateID"];
				runeTemplateInfo.Name = (string)ResultDataReader["Name"];
				runeTemplateInfo.BaseLevel = (int)ResultDataReader["BaseLevel"];
				runeTemplateInfo.MaxLevel = (int)ResultDataReader["MaxLevel"];
				runeTemplateInfo.Type1 = (int)ResultDataReader["Type1"];
				runeTemplateInfo.Attribute1 = (string)ResultDataReader["Attribute1"];
				runeTemplateInfo.Turn1 = (int)ResultDataReader["Turn1"];
				runeTemplateInfo.Rate1 = (int)ResultDataReader["Rate1"];
				runeTemplateInfo.Type2 = (int)ResultDataReader["Type2"];
				runeTemplateInfo.Attribute2 = (string)ResultDataReader["Attribute2"];
				runeTemplateInfo.Turn2 = (int)ResultDataReader["Turn2"];
				runeTemplateInfo.Rate2 = (int)ResultDataReader["Rate2"];
				runeTemplateInfo.Type3 = (int)ResultDataReader["Type3"];
				runeTemplateInfo.Attribute3 = (string)ResultDataReader["Attribute3"];
				runeTemplateInfo.Turn3 = (int)ResultDataReader["Turn3"];
				runeTemplateInfo.Rate3 = (int)ResultDataReader["Rate3"];
				list.Add(runeTemplateInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetRuneTemplateInfo", exception);
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

	public StrengThenExpInfo[] GetAllStrengThenExp()
	{
		List<StrengThenExpInfo> list = new List<StrengThenExpInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_StrengThenExp_All");
			while (ResultDataReader.Read())
			{
				StrengThenExpInfo strengThenExpInfo = new StrengThenExpInfo();
				strengThenExpInfo.ID = (int)ResultDataReader["ID"];
				strengThenExpInfo.Level = (int)ResultDataReader["Level"];
				strengThenExpInfo.Exp = (int)ResultDataReader["Exp"];
				strengThenExpInfo.NecklaceStrengthExp = (int)ResultDataReader["NecklaceStrengthExp"];
				strengThenExpInfo.NecklaceStrengthPlus = (int)ResultDataReader["NecklaceStrengthPlus"];
				list.Add(strengThenExpInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetStrengThenExpInfo", exception);
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

	public StrengthenGoodsInfo[] GetAllStrengthenGoodsInfo()
	{
		List<StrengthenGoodsInfo> list = new List<StrengthenGoodsInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Item_StrengthenGoodsInfo_All");
			while (ResultDataReader.Read())
			{
				StrengthenGoodsInfo strengthenGoodsInfo = new StrengthenGoodsInfo();
				strengthenGoodsInfo.ID = (int)ResultDataReader["ID"];
				strengthenGoodsInfo.Level = (int)ResultDataReader["Level"];
				strengthenGoodsInfo.CurrentEquip = (int)ResultDataReader["CurrentEquip"];
				strengthenGoodsInfo.GainEquip = (int)ResultDataReader["GainEquip"];
				strengthenGoodsInfo.OriginalEquip = (int)ResultDataReader["OriginalEquip"];
				list.Add(strengthenGoodsInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllStrengthenGoodsInfo", exception);
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

	public StrengthenInfo[] GetAllRefineryStrengthen()
	{
		List<StrengthenInfo> list = new List<StrengthenInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Item_Refinery_Strengthen_All");
			while (ResultDataReader.Read())
			{
				StrengthenInfo strengthenInfo = new StrengthenInfo();
				strengthenInfo.StrengthenLevel = (int)ResultDataReader["StrengthenLevel"];
				strengthenInfo.Rock = (int)ResultDataReader["Rock"];
				list.Add(strengthenInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllRefineryStrengthen", exception);
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

	public List<RefineryInfo> GetAllRefineryInfo()
	{
		List<RefineryInfo> list = new List<RefineryInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Item_Refinery_All");
			while (ResultDataReader.Read())
			{
				RefineryInfo refineryInfo = new RefineryInfo();
				refineryInfo.RefineryID = (int)ResultDataReader["RefineryID"];
				refineryInfo.m_Equip.Add((int)ResultDataReader["Equip1"]);
				refineryInfo.m_Equip.Add((int)ResultDataReader["Equip2"]);
				refineryInfo.m_Equip.Add((int)ResultDataReader["Equip3"]);
				refineryInfo.m_Equip.Add((int)ResultDataReader["Equip4"]);
				refineryInfo.Item1 = (int)ResultDataReader["Item1"];
				refineryInfo.Item2 = (int)ResultDataReader["Item2"];
				refineryInfo.Item3 = (int)ResultDataReader["Item3"];
				refineryInfo.Item1Count = (int)ResultDataReader["Item1Count"];
				refineryInfo.Item2Count = (int)ResultDataReader["Item2Count"];
				refineryInfo.Item3Count = (int)ResultDataReader["Item3Count"];
				refineryInfo.m_Reward.Add((int)ResultDataReader["Material1"]);
				refineryInfo.m_Reward.Add((int)ResultDataReader["Operate1"]);
				refineryInfo.m_Reward.Add((int)ResultDataReader["Reward1"]);
				refineryInfo.m_Reward.Add((int)ResultDataReader["Material2"]);
				refineryInfo.m_Reward.Add((int)ResultDataReader["Operate2"]);
				refineryInfo.m_Reward.Add((int)ResultDataReader["Reward2"]);
				refineryInfo.m_Reward.Add((int)ResultDataReader["Material3"]);
				refineryInfo.m_Reward.Add((int)ResultDataReader["Operate3"]);
				refineryInfo.m_Reward.Add((int)ResultDataReader["Reward3"]);
				refineryInfo.m_Reward.Add((int)ResultDataReader["Material4"]);
				refineryInfo.m_Reward.Add((int)ResultDataReader["Operate4"]);
				refineryInfo.m_Reward.Add((int)ResultDataReader["Reward4"]);
				list.Add(refineryInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllRefineryInfo", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list;
	}

	public QuestInfo[] GetALlQuest()
	{
		List<QuestInfo> list = new List<QuestInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Quest_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitQuest(ResultDataReader));
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

	public QuestAwardInfo[] GetAllQuestGoods()
	{
		List<QuestAwardInfo> list = new List<QuestAwardInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Quest_Goods_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitQuestGoods(ResultDataReader));
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

	public QuestConditionInfo[] GetAllQuestCondiction()
	{
		List<QuestConditionInfo> list = new List<QuestConditionInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Quest_Condiction_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitQuestCondiction(ResultDataReader));
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

	public QuestRateInfo[] GetAllQuestRate()
	{
		List<QuestRateInfo> list = new List<QuestRateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Quest_Rate_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitQuestRate(ResultDataReader));
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

	public QuestInfo GetSingleQuest(int questID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@QuestID", SqlDbType.Int, 4)
			};
			array[0].Value = questID;
			db.GetReader(ref ResultDataReader, "SP_Quest_Single", array);
			if (ResultDataReader.Read())
			{
				return InitQuest(ResultDataReader);
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

	public QuestInfo InitQuest(SqlDataReader reader)
	{
		QuestInfo questInfo = new QuestInfo();
		questInfo.ID = (int)reader["ID"];
		questInfo.QuestID = (int)reader["QuestID"];
		questInfo.Title = ((reader["Title"] == null) ? "" : reader["Title"].ToString());
		questInfo.Detail = ((reader["Detail"] == null) ? "" : reader["Detail"].ToString());
		questInfo.Objective = ((reader["Objective"] == null) ? "" : reader["Objective"].ToString());
		questInfo.NeedMinLevel = (int)reader["NeedMinLevel"];
		questInfo.NeedMaxLevel = (int)reader["NeedMaxLevel"];
		questInfo.PreQuestID = ((reader["PreQuestID"] == null) ? "" : reader["PreQuestID"].ToString());
		questInfo.NextQuestID = ((reader["NextQuestID"] == null) ? "" : reader["NextQuestID"].ToString());
		questInfo.IsOther = (int)reader["IsOther"];
		questInfo.CanRepeat = (bool)reader["CanRepeat"];
		questInfo.RepeatInterval = (int)reader["RepeatInterval"];
		questInfo.RepeatMax = (int)reader["RepeatMax"];
		questInfo.RewardGP = (int)reader["RewardGP"];
		questInfo.RewardGold = (int)reader["RewardGold"];
		questInfo.RewardBindMoney = (int)reader["RewardBindMoney"];
		questInfo.RewardOffer = (int)reader["RewardOffer"];
		questInfo.RewardRiches = (int)reader["RewardRiches"];
		questInfo.RewardBuffID = (int)reader["RewardBuffID"];
		questInfo.RewardBuffDate = (int)reader["RewardBuffDate"];
		questInfo.RewardMoney = (int)reader["RewardMoney"];
		questInfo.Rands = (decimal)reader["Rands"];
		questInfo.RandDouble = (int)reader["RandDouble"];
		questInfo.TimeMode = (bool)reader["TimeMode"];
		questInfo.StartDate = (DateTime)reader["StartDate"];
		questInfo.EndDate = (DateTime)reader["EndDate"];
		questInfo.MapID = (int)reader["MapID"];
		questInfo.AutoEquip = (bool)reader["AutoEquip"];
		questInfo.OneKeyFinishNeedMoney = (int)reader["OneKeyFinishNeedMoney"];
		questInfo.Rank = ((reader["Rank"] == null) ? "" : reader["Rank"].ToString());
		questInfo.StarLev = (int)reader["StarLev"];
		questInfo.NotMustCount = (int)reader["NotMustCount"];
		questInfo.Level2NeedMoney = (int)reader["Level2NeedMoney"];
		questInfo.Level3NeedMoney = (int)reader["Level3NeedMoney"];
		questInfo.Level4NeedMoney = (int)reader["Level4NeedMoney"];
		questInfo.Level5NeedMoney = (int)reader["Level5NeedMoney"];
		questInfo.CollocationCost = (int)reader["CollocationCost"];
		questInfo.CollocationColdTime = (int)reader["CollocationColdTime"];
		return questInfo;
	}

	public QuestAwardInfo InitQuestGoods(SqlDataReader reader)
	{
		QuestAwardInfo questAwardInfo = new QuestAwardInfo();
		questAwardInfo.QuestID = (int)reader["QuestID"];
		questAwardInfo.RewardItemID = (int)reader["RewardItemID"];
		questAwardInfo.IsSelect = (bool)reader["IsSelect"];
		questAwardInfo.RewardItemValid = (int)reader["RewardItemValid"];
		questAwardInfo.RewardItemCount1 = (int)reader["RewardItemCount1"];
		questAwardInfo.RewardItemCount2 = (int)reader["RewardItemCount2"];
		questAwardInfo.RewardItemCount3 = (int)reader["RewardItemCount3"];
		questAwardInfo.RewardItemCount4 = (int)reader["RewardItemCount4"];
		questAwardInfo.RewardItemCount5 = (int)reader["RewardItemCount5"];
		questAwardInfo.StrengthenLevel = (int)reader["StrengthenLevel"];
		questAwardInfo.AttackCompose = (int)reader["AttackCompose"];
		questAwardInfo.DefendCompose = (int)reader["DefendCompose"];
		questAwardInfo.AgilityCompose = (int)reader["AgilityCompose"];
		questAwardInfo.LuckCompose = (int)reader["LuckCompose"];
		questAwardInfo.IsCount = (bool)reader["IsCount"];
		questAwardInfo.IsBind = (bool)reader["IsBind"];
		return questAwardInfo;
	}

	public QuestConditionInfo InitQuestCondiction(SqlDataReader reader)
	{
		QuestConditionInfo questConditionInfo = new QuestConditionInfo();
		questConditionInfo.QuestID = (int)reader["QuestID"];
		questConditionInfo.CondictionID = (int)reader["CondictionID"];
		questConditionInfo.CondictionTitle = ((reader["CondictionTitle"] == null) ? "" : reader["CondictionTitle"].ToString());
		questConditionInfo.CondictionType = (int)reader["CondictionType"];
		questConditionInfo.Para1 = (int)reader["Para1"];
		questConditionInfo.Para2 = (int)reader["Para2"];
		questConditionInfo.isOpitional = (bool)reader["isOpitional"];
		return questConditionInfo;
	}

	public QuestRateInfo InitQuestRate(SqlDataReader reader)
	{
		QuestRateInfo questRateInfo = new QuestRateInfo();
		questRateInfo.BindMoneyRate = ((reader["BindMoneyRate"] == null) ? "" : reader["BindMoneyRate"].ToString());
		questRateInfo.ExpRate = ((reader["ExpRate"] == null) ? "" : reader["ExpRate"].ToString());
		questRateInfo.GoldRate = ((reader["GoldRate"] == null) ? "" : reader["GoldRate"].ToString());
		questRateInfo.ExploitRate = ((reader["ExploitRate"] == null) ? "" : reader["ExploitRate"].ToString());
		questRateInfo.CanOneKeyFinishTime = (int)reader["CanOneKeyFinishTime"];
		return questRateInfo;
	}

	public DropCondiction[] GetAllDropCondictions()
	{
		List<DropCondiction> list = new List<DropCondiction>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Drop_Condiction_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitDropCondiction(ResultDataReader));
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

	public DropItem[] GetAllDropItems()
	{
		List<DropItem> list = new List<DropItem>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Drop_Item_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitDropItem(ResultDataReader));
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

	public DropCondiction InitDropCondiction(SqlDataReader reader)
	{
		DropCondiction dropCondiction = new DropCondiction();
		dropCondiction.DropId = (int)reader["DropID"];
		dropCondiction.CondictionType = (int)reader["CondictionType"];
		dropCondiction.Para1 = (string)reader["Para1"];
		dropCondiction.Para2 = (string)reader["Para2"];
		return dropCondiction;
	}

	public DropItem InitDropItem(SqlDataReader reader)
	{
		DropItem dropItem = new DropItem();
		dropItem.Id = (int)reader["Id"];
		dropItem.DropId = (int)reader["DropId"];
		dropItem.ItemId = (int)reader["ItemId"];
		dropItem.ValueDate = (int)reader["ValueDate"];
		dropItem.IsBind = (bool)reader["IsBind"];
		dropItem.Random = (int)reader["Random"];
		dropItem.BeginData = (int)reader["BeginData"];
		dropItem.EndData = (int)reader["EndData"];
		dropItem.IsLogs = (bool)reader["IsLogs"];
		dropItem.IsTips = (bool)reader["IsTips"];
		return dropItem;
	}

	public AASInfo[] GetAllAASInfo()
	{
		List<AASInfo> list = new List<AASInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_AASInfo_All");
			while (ResultDataReader.Read())
			{
				AASInfo aASInfo = new AASInfo();
				aASInfo.UserID = (int)ResultDataReader["ID"];
				aASInfo.Name = ResultDataReader["Name"].ToString();
				aASInfo.IDNumber = ResultDataReader["IDNumber"].ToString();
				aASInfo.State = (int)ResultDataReader["State"];
				list.Add(aASInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllAASInfo", exception);
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

	public bool AddAASInfo(AASInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[5]
			{
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@Name", info.Name),
				new SqlParameter("@IDNumber", info.IDNumber),
				new SqlParameter("@State", info.State),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[4].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_ASSInfo_Add", array);
			result = (int)array[4].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("UpdateAASInfo", exception);
			}
		}
		return result;
	}

	public string GetASSInfoSingle(int UserID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@UserID", UserID)
			};
			db.GetReader(ref ResultDataReader, "SP_ASSInfo_Single", sqlParameters);
			if (ResultDataReader.Read())
			{
				return ResultDataReader["IDNumber"].ToString();
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetASSInfoSingle", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return "";
	}

	public bool AddDailyLogList(DailyLogListInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[5]
			{
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@UserAwardLog", info.UserAwardLog),
				new SqlParameter("@DayLog", info.DayLog),
				new SqlParameter("@Result", SqlDbType.Int),
				null
			};
			array[3].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_DailyLogList_Add", array);
			result = (int)array[3].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("UpdateAASInfo", exception);
			}
		}
		return result;
	}

	public SearchGoodsTempInfo[] GetAllSearchGoodsTemp()
	{
		List<SearchGoodsTempInfo> list = new List<SearchGoodsTempInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_SearchGoodsTemp_All");
			while (ResultDataReader.Read())
			{
				SearchGoodsTempInfo searchGoodsTempInfo = new SearchGoodsTempInfo();
				searchGoodsTempInfo.StarID = (int)ResultDataReader["StarID"];
				searchGoodsTempInfo.NeedMoney = (int)ResultDataReader["NeedMoney"];
				searchGoodsTempInfo.DestinationReward = (int)ResultDataReader["DestinationReward"];
				searchGoodsTempInfo.VIPLevel = (int)ResultDataReader["VIPLevel"];
				searchGoodsTempInfo.ExtractNumber = ((ResultDataReader["ExtractNumber"] == null) ? "" : ResultDataReader["ExtractNumber"].ToString());
				list.Add(searchGoodsTempInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllDaily", exception);
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

	public DailyAwardInfo[] GetAllDailyAward()
	{
		List<DailyAwardInfo> list = new List<DailyAwardInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Daily_Award_All");
			while (ResultDataReader.Read())
			{
				DailyAwardInfo dailyAwardInfo = new DailyAwardInfo();
				dailyAwardInfo.Count = (int)ResultDataReader["Count"];
				dailyAwardInfo.ID = (int)ResultDataReader["ID"];
				dailyAwardInfo.IsBinds = (bool)ResultDataReader["IsBinds"];
				dailyAwardInfo.TemplateID = (int)ResultDataReader["TemplateID"];
				dailyAwardInfo.Type = (int)ResultDataReader["Type"];
				dailyAwardInfo.ValidDate = (int)ResultDataReader["ValidDate"];
				dailyAwardInfo.Sex = (int)ResultDataReader["Sex"];
				dailyAwardInfo.Remark = ((ResultDataReader["Remark"] == null) ? "" : ResultDataReader["Remark"].ToString());
				dailyAwardInfo.CountRemark = ((ResultDataReader["CountRemark"] == null) ? "" : ResultDataReader["CountRemark"].ToString());
				dailyAwardInfo.GetWay = (int)ResultDataReader["GetWay"];
				dailyAwardInfo.AwardDays = (int)ResultDataReader["AwardDays"];
				list.Add(dailyAwardInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllDaily", exception);
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

	public NpcInfo[] GetAllNPCInfo()
	{
		List<NpcInfo> list = new List<NpcInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_NPC_Info_All");
			while (ResultDataReader.Read())
			{
				NpcInfo npcInfo = new NpcInfo();
				npcInfo.ID = (int)ResultDataReader["ID"];
				npcInfo.Name = ((ResultDataReader["Name"] == null) ? "" : ResultDataReader["Name"].ToString());
				npcInfo.Level = (int)ResultDataReader["Level"];
				npcInfo.Camp = (int)ResultDataReader["Camp"];
				npcInfo.Type = (int)ResultDataReader["Type"];
				npcInfo.Blood = (int)ResultDataReader["Blood"];
				npcInfo.X = (int)ResultDataReader["X"];
				npcInfo.Y = (int)ResultDataReader["Y"];
				npcInfo.Width = (int)ResultDataReader["Width"];
				npcInfo.Height = (int)ResultDataReader["Height"];
				npcInfo.MoveMin = (int)ResultDataReader["MoveMin"];
				npcInfo.MoveMax = (int)ResultDataReader["MoveMax"];
				npcInfo.BaseDamage = (int)ResultDataReader["BaseDamage"];
				npcInfo.BaseGuard = (int)ResultDataReader["BaseGuard"];
				npcInfo.Attack = (int)ResultDataReader["Attack"];
				npcInfo.Defence = (int)ResultDataReader["Defence"];
				npcInfo.Agility = (int)ResultDataReader["Agility"];
				npcInfo.Lucky = (int)ResultDataReader["Lucky"];
				npcInfo.ModelID = ((ResultDataReader["ModelID"] == null) ? "" : ResultDataReader["ModelID"].ToString());
				npcInfo.ResourcesPath = ((ResultDataReader["ResourcesPath"] == null) ? "" : ResultDataReader["ResourcesPath"].ToString());
				npcInfo.DropRate = ((ResultDataReader["DropRate"] == null) ? "" : ResultDataReader["DropRate"].ToString());
				npcInfo.Experience = (int)ResultDataReader["Experience"];
				npcInfo.Delay = (int)ResultDataReader["Delay"];
				npcInfo.Immunity = (int)ResultDataReader["Immunity"];
				npcInfo.Alert = (int)ResultDataReader["Alert"];
				npcInfo.Range = (int)ResultDataReader["Range"];
				npcInfo.Preserve = (int)ResultDataReader["Preserve"];
				npcInfo.Script = ((ResultDataReader["Script"] == null) ? "" : ResultDataReader["Script"].ToString());
				npcInfo.FireX = (int)ResultDataReader["FireX"];
				npcInfo.FireY = (int)ResultDataReader["FireY"];
				npcInfo.DropId = (int)ResultDataReader["DropId"];
				npcInfo.MagicAttack = (int)ResultDataReader["MagicAttack"];
				npcInfo.MagicDefence = (int)ResultDataReader["MagicDefence"];
				npcInfo.CurrentBallId = (int)ResultDataReader["CurrentBallId"];
				npcInfo.speed = (int)ResultDataReader["speed"];
				npcInfo.FireAttack = (int)ResultDataReader["FireAttack"];
				npcInfo.FireDefence = (int)ResultDataReader["FireDefence"];
				npcInfo.WaterAttack = (int)ResultDataReader["WaterAttack"];
				npcInfo.WaterDefence = (int)ResultDataReader["WaterDefence"];
				npcInfo.WindAttack = (int)ResultDataReader["WindAttack"];
				npcInfo.WindDefence = (int)ResultDataReader["WindDefence"];
				npcInfo.LandAttack = (int)ResultDataReader["LandAttack"];
				npcInfo.LandDefence = (int)ResultDataReader["LandDefence"];
				list.Add(npcInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllNPCInfo", exception);
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

	public MissionEnergyInfo[] GetAllMissionEnergyInfo()
	{
		List<MissionEnergyInfo> list = new List<MissionEnergyInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_MissionEnergyInfo_All");
			while (ResultDataReader.Read())
			{
				MissionEnergyInfo missionEnergyInfo = new MissionEnergyInfo();
				missionEnergyInfo.Count = (int)ResultDataReader["Count"];
				missionEnergyInfo.Money = (int)ResultDataReader["Money"];
				missionEnergyInfo.Energy = (int)ResultDataReader["Energy"];
				list.Add(missionEnergyInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllMissionEnergyInfo", exception);
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

	public MissionInfo[] GetAllMissionInfo()
	{
		List<MissionInfo> list = new List<MissionInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Mission_Info_All");
			while (ResultDataReader.Read())
			{
				MissionInfo missionInfo = new MissionInfo();
				missionInfo.Id = (int)ResultDataReader["ID"];
				missionInfo.Name = ((ResultDataReader["Name"] == null) ? "" : ResultDataReader["Name"].ToString());
				missionInfo.TotalCount = (int)ResultDataReader["TotalCount"];
				missionInfo.TotalTurn = (int)ResultDataReader["TotalTurn"];
				missionInfo.Script = ((ResultDataReader["Script"] == null) ? "" : ResultDataReader["Script"].ToString());
				missionInfo.Success = ((ResultDataReader["Success"] == null) ? "" : ResultDataReader["Success"].ToString());
				missionInfo.Failure = ((ResultDataReader["Failure"] == null) ? "" : ResultDataReader["Failure"].ToString());
				missionInfo.Description = ((ResultDataReader["Description"] == null) ? "" : ResultDataReader["Description"].ToString());
				missionInfo.IncrementDelay = (int)ResultDataReader["IncrementDelay"];
				missionInfo.Delay = (int)ResultDataReader["Delay"];
				missionInfo.Title = ((ResultDataReader["Title"] == null) ? "" : ResultDataReader["Title"].ToString());
				missionInfo.Param1 = (int)ResultDataReader["Param1"];
				missionInfo.Param2 = (int)ResultDataReader["Param2"];
				list.Add(missionInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllMissionInfo", exception);
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

	public BombKingPlayerInfo[] GetBombKingPlayer()
	{
		List<BombKingPlayerInfo> list = new List<BombKingPlayerInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_BombKing_All");
			while (ResultDataReader.Read())
			{
				BombKingPlayerInfo item = new BombKingPlayerInfo
				{
					UserID = (int)ResultDataReader["userId"],
					AreaID = (int)ResultDataReader["areaId"],
					Place = (int)ResultDataReader["place"],
					NickName = (string)ResultDataReader["NickName"],
					AreaName = (string)ResultDataReader["areaName"],
					vipType = (int)ResultDataReader["vipType"],
					vipLevel = (int)ResultDataReader["vipLvl"],
					myRank = (int)ResultDataReader["myRank"],
					myScore = (int)ResultDataReader["myScore"],
					Score = (int)ResultDataReader["Score"],
					RankType = (int)ResultDataReader["RankType"],
					status = (int)ResultDataReader["status"],
					style = (string)ResultDataReader["style"],
					color = (string)ResultDataReader["color"],
					sex = (bool)ResultDataReader["sex"],
					smg = (string)ResultDataReader["smg"]
				};
				list.Add(item);
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

	public BombKingRewardInfo[] GetBombKingReward()
	{
		List<BombKingRewardInfo> list = new List<BombKingRewardInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_GetBombKingReward_All");
			while (ResultDataReader.Read())
			{
				BombKingRewardInfo item = new BombKingRewardInfo
				{
					ID = (int)ResultDataReader["ID"],
					ItemId = (int)ResultDataReader["ItemId"],
					Count = (int)ResultDataReader["Count"],
					Valid = (int)ResultDataReader["Valid"],
					Strengthen = (int)ResultDataReader["Strengthen"],
					Attack = (int)ResultDataReader["At"],
					Defend = (int)ResultDataReader["Def"],
					Agility = (int)ResultDataReader["Agi"],
					Luck = (int)ResultDataReader["Lu"],
					Week = (int)ResultDataReader["Week"],
					Type = (int)ResultDataReader["Type"],
					Note = (string)ResultDataReader["Note"]
				};
				list.Add(item);
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

	public bool AddBombKingPlayer(BombKingPlayerInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[17]
			{
				new SqlParameter("@userId", info.UserID),
				new SqlParameter("@areaId", info.AreaID),
				new SqlParameter("@place", info.Place),
				new SqlParameter("@NickName", (info.NickName == null) ? "" : info.NickName),
				new SqlParameter("@areaName", (info.AreaName == null) ? "" : info.AreaName),
				new SqlParameter("@vipType", info.vipType),
				new SqlParameter("@vipLvl", info.vipLevel),
				new SqlParameter("@myRank", info.myRank),
				new SqlParameter("@myScore", info.myScore),
				new SqlParameter("@Score", info.Score),
				new SqlParameter("@RankType", info.RankType),
				new SqlParameter("@status", info.status),
				new SqlParameter("@style", (info.style == null) ? "" : info.style),
				new SqlParameter("@color", (info.color == null) ? "" : info.color),
				new SqlParameter("@sex", info.sex),
				new SqlParameter("@smg", (info.smg == null) ? "" : info.smg),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[16].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_BombKing_Add", array);
			result = (int)array[16].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("SP_BombKing_Add", exception);
			}
		}
		return result;
	}

	public bool UpdateBombKingPlayer(BombKingPlayerInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[17]
			{
				new SqlParameter("@userId", info.UserID),
				new SqlParameter("@areaId", info.AreaID),
				new SqlParameter("@place", info.Place),
				new SqlParameter("@NickName", (info.NickName == null) ? "" : info.NickName),
				new SqlParameter("@areaName", (info.AreaName == null) ? "" : info.AreaName),
				new SqlParameter("@vipType", info.vipType),
				new SqlParameter("@vipLvl", info.vipLevel),
				new SqlParameter("@myRank", info.myRank),
				new SqlParameter("@myScore", info.myScore),
				new SqlParameter("@Score", info.Score),
				new SqlParameter("@RankType", info.RankType),
				new SqlParameter("@status", info.status),
				new SqlParameter("@style", (info.style == null) ? "" : info.style),
				new SqlParameter("@color", (info.color == null) ? "" : info.color),
				new SqlParameter("@sex", info.sex),
				new SqlParameter("@smg", (info.smg == null) ? "" : info.smg),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[16].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_BombKing_Update", array);
			result = (int)array[16].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("SP_BombKing_Update", exception);
			}
		}
		return result;
	}

	public bool DeleteBombKingPlayer()
	{
		try
		{
			db.RunProcedure("SP_BombKing_Delete");
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("SP_BombKing_Delete", exception);
			}
		}
		return true;
	}

	public CampWarItemsInfo[] GetCampWarItems()
	{
		List<CampWarItemsInfo> list = new List<CampWarItemsInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_CampWarItems");
			while (ResultDataReader.Read())
			{
				CampWarItemsInfo item = new CampWarItemsInfo
				{
					ID = (int)ResultDataReader["ID"],
					MinRank = (int)ResultDataReader["MinRank"],
					MaxRank = (int)ResultDataReader["MaxRank"],
					IsCamp = (bool)ResultDataReader["IsCamp"],
					ItemID = (int)ResultDataReader["ItemID"],
					Valid = (int)ResultDataReader["Valid"],
					Count = (int)ResultDataReader["Count"],
					StrengthenLevel = (int)ResultDataReader["StrengthenLevel"],
					AttackCompose = (int)ResultDataReader["AttackCompose"],
					DefendCompose = (int)ResultDataReader["DefendCompose"],
					AgilityCompose = (int)ResultDataReader["AgilityCompose"],
					LuckCompose = (int)ResultDataReader["LuckCompose"],
					IsBind = (bool)ResultDataReader["IsBind"]
				};
				list.Add(item);
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

	public ThreeClearPointAwardInfo[] GetThreeCleanPointAwards()
	{
		List<ThreeClearPointAwardInfo> list = new List<ThreeClearPointAwardInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_ThreeClearPointAward_All");
			while (ResultDataReader.Read())
			{
				ThreeClearPointAwardInfo item = new ThreeClearPointAwardInfo
				{
					ID = (int)ResultDataReader["ID"],
					ItemID = (int)ResultDataReader["ItemID"],
					Count = (int)ResultDataReader["Count"],
					IsBind = (bool)ResultDataReader["IsBind"],
					Point = (int)ResultDataReader["Point"],
					Type = (int)ResultDataReader["Type"],
					Valid = (int)ResultDataReader["Valid"]
				};
				list.Add(item);
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

	public CityOccupationSystemsInfo[] GetAllCityOccupationSystems()
	{
		List<CityOccupationSystemsInfo> list = new List<CityOccupationSystemsInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_CityOccupationSystems_All");
			while (ResultDataReader.Read())
			{
				CityOccupationSystemsInfo item = new CityOccupationSystemsInfo
				{
					ID = (int)ResultDataReader["ID"],
					Quality = (int)ResultDataReader["Quality"],
					Probability = (int)ResultDataReader["Probability"],
					TemplateID = (int)ResultDataReader["TemplateID"],
					ValidDate = (int)ResultDataReader["ValidDate"],
					Count = (int)ResultDataReader["Count"],
					IsBind = (bool)ResultDataReader["IsBind"],
					StrengthLevel = (int)ResultDataReader["StrengthLevel"],
					AttackCompose = (int)ResultDataReader["AttackCompose"],
					DefendCompose = (int)ResultDataReader["DefendCompose"],
					AgilityCompose = (int)ResultDataReader["AgilityCompose"],
					LuckCompose = (int)ResultDataReader["LuckCompose"],
					NeedScore = (int)ResultDataReader["NeedScore"],
					ExchangeCount = (int)ResultDataReader["ExchangeCount"]
				};
				list.Add(item);
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

	public Dictionary<string, ServerDataInfo> GetAllServerData()
	{
		Dictionary<string, ServerDataInfo> dictionary = new Dictionary<string, ServerDataInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Server_Data_All");
			while (ResultDataReader.Read())
			{
				ServerDataInfo serverDataInfo = new ServerDataInfo
				{
					Name = (string)ResultDataReader["Name"],
					ValString = ((ResultDataReader["ValString"] == DBNull.Value) ? null : ((string)ResultDataReader["ValString"])),
					ValInt = (int)ResultDataReader["ValInt"],
					ValDateTime = ((ResultDataReader["ValDateTime"] == DBNull.Value) ? DateTime.Now : ((DateTime)ResultDataReader["ValDateTime"]))
				};
				serverDataInfo.IsDirty = false;
				if (!dictionary.ContainsKey(serverDataInfo.Name))
				{
					dictionary.Add(serverDataInfo.Name, serverDataInfo);
				}
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
		return dictionary;
	}

	public bool AddServerData(ServerDataInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[4]
			{
				new SqlParameter("@Name", info.Name),
				new SqlParameter("@ValString", info.ValString),
				new SqlParameter("@ValInt", info.ValInt),
				null
			};
			_ = info.ValDateTime;
			array[3] = new SqlParameter("@ValDateTime", (info.ValDateTime == DateTime.MinValue) ? DateTime.Now : info.ValDateTime);
			db.RunProcedure("SP_Server_Data_Add", array);
			info.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("SP_Server_Data_Add", exception);
			}
		}
		return result;
	}

	public QiYuanAwardInfo[] GetAllQiYuanAward()
	{
		List<QiYuanAwardInfo> list = new List<QiYuanAwardInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_QiYuanAward");
			while (ResultDataReader.Read())
			{
				QiYuanAwardInfo item = new QiYuanAwardInfo
				{
					ID = (int)ResultDataReader["ID"],
					Type = (int)ResultDataReader["Type"],
					Rank = (int)ResultDataReader["Rank"],
					TemplateID = (int)ResultDataReader["TemplateID"],
					Count = (int)ResultDataReader["Count"],
					IsBind = (bool)ResultDataReader["IsBind"],
					VaildDate = (int)ResultDataReader["VaildDate"],
					Probability = (int)ResultDataReader["Probability"]
				};
				list.Add(item);
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

	public MarkOpenBoxInfo[] GetAllMarkOpenBox()
	{
		List<MarkOpenBoxInfo> list = new List<MarkOpenBoxInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Mark_Open_Data_All");
			while (ResultDataReader.Read())
			{
				MarkOpenBoxInfo item = new MarkOpenBoxInfo
				{
					ID = (int)ResultDataReader["ID"],
					Type = (int)ResultDataReader["Type"],
					TemplateID = (int)ResultDataReader["TemplateID"],
					BornLevel = (int)ResultDataReader["BornLevel"],
					IsBind = (bool)ResultDataReader["IsBind"],
					Random = (int)ResultDataReader["Random"]
				};
				list.Add(item);
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

	public bool AddMarkBox(ItemTemplateInfo item, int type, int random)
	{
		bool result = false;
		try
		{
			result = db.Exesqlcomm(string.Format("INSERT INTO Mark_Open_Data (Type, TemplateID, BornLevel, IsBind, Random) VALUES ({0},{1},{2},'{3}',{4})", type, item.TemplateID, 1, "True", random));
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

	public EventoPntsCategory[] Get_EventoPntsCategorys()
	{
		List<EventoPntsCategory> list = new List<EventoPntsCategory>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_EVENT_PNTS_CATEGORY");
			while (ResultDataReader.Read())
			{
				EventoPntsCategory item = new EventoPntsCategory
				{
					id = (int)ResultDataReader["id"],
					name = (string)ResultDataReader["name"],
					price = (int)ResultDataReader["price"]
				};
				list.Add(item);
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

	public EventoPntsRewards[] Get_EventoPntsRewards()
	{
		List<EventoPntsRewards> list = new List<EventoPntsRewards>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_EVENT_PNTS_REWARDS");
			while (ResultDataReader.Read())
			{
				EventoPntsRewards item = new EventoPntsRewards
				{
					id = (int)ResultDataReader["id"],
					categoryid = (int)ResultDataReader["categoryid"],
					templateid = (int)ResultDataReader["templateid"],
					maxcomp = (int)ResultDataReader["maxcomp"],
					mincomp = (int)ResultDataReader["mincomp"],
					validate = (int)ResultDataReader["validate"],
					maxLevel = (int)ResultDataReader["maxLevel"],
					minLevel = (int)ResultDataReader["minLevel"],
					canRenew = (bool)ResultDataReader["canRenew"],
					canTransfer = (bool)ResultDataReader["canTransfer"]
				};
				list.Add(item);
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

	public Dictionary<int, List<PetSkillLacre>> GetAllPetSkillsLacre()
	{
		Dictionary<int, List<PetSkillLacre>> dictionary = new Dictionary<int, List<PetSkillLacre>>();
		List<PetSkillInfo> list = new List<PetSkillInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_PetSkils_Lacre_All");
			while (ResultDataReader.Read())
			{
				PetSkillLacre petSkillLacre = new PetSkillLacre();
				petSkillLacre.KindID = (int)ResultDataReader["KindID"];
				petSkillLacre.SkillID = (int)ResultDataReader["SkillID"];
				petSkillLacre.Slot = (int)ResultDataReader["Slot"];
				petSkillLacre.IsAdvance = (bool)ResultDataReader["IsAdvance"];
				if (!dictionary.ContainsKey(petSkillLacre.KindID))
				{
					dictionary.Add(petSkillLacre.KindID, new List<PetSkillLacre>());
				}
				dictionary[petSkillLacre.KindID].Add(petSkillLacre);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllPetSkillsLacre", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return dictionary;
	}

	public HappyRechargeData HappyRechargePrize()
	{
		HappyRechargeData result = null;
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Get_HappyRecharge_Prize");
			while (ResultDataReader.Read())
			{
				result = new HappyRechargeData
				{
					TemplateID = (int)ResultDataReader["TemplateID"],
					Count = (int)ResultDataReader["Count"],
					ValiDate = (int)ResultDataReader["ValiDate"]
				};
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("HappyRechargePrize", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && ((!ResultDataReader.IsClosed) ? true : false))
			{
				ResultDataReader.Close();
			}
		}
		return result;
	}

	public List<HappyRechargeTicketReward> GetRewardHappyTicket()
	{
		List<HappyRechargeTicketReward> list = new List<HappyRechargeTicketReward>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Get_HappyRecharge_TicketReward");
			while (ResultDataReader.Read())
			{
				HappyRechargeTicketReward item = new HappyRechargeTicketReward
				{
					TemplateID = (int)ResultDataReader["TemplateID"],
					Count = (int)ResultDataReader["Count"],
					ValiDate = (int)ResultDataReader["ValiDate"],
					TicketPrice = (string)ResultDataReader["ticketPrice"]
				};
				list.Add(item);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetRewardHappyTicket", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && ((!ResultDataReader.IsClosed) ? true : false))
			{
				ResultDataReader.Close();
			}
		}
		return list;
	}

	public HappyRechargeDataPrize[] GetHappyRechargeData()
	{
		List<HappyRechargeDataPrize> list = new List<HappyRechargeDataPrize>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Get_HappyRecharge_Data");
			while (ResultDataReader.Read())
			{
				HappyRechargeDataPrize item = new HappyRechargeDataPrize
				{
					PrizeType = (int)ResultDataReader["PrizeType"],
					Count = (int)ResultDataReader["Count"],
					NickName = (string)ResultDataReader["NickName"]
				};
				list.Add(item);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetHappyRechargeData", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && ((!ResultDataReader.IsClosed) ? true : false))
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public List<HappyRechargeReward> HappyRechargeReward()
	{
		List<HappyRechargeReward> list = new List<HappyRechargeReward>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Get_HappyRecharge_Rewards");
			while (ResultDataReader.Read())
			{
				HappyRechargeReward item = new HappyRechargeReward
				{
					ID = (int)ResultDataReader["ID"],
					TemplateID = (int)ResultDataReader["TemplateID"],
					Count = (int)ResultDataReader["Count"],
					ValiDate = (int)ResultDataReader["ValiDate"],
					isBind = (bool)ResultDataReader["isBind"]
				};
				list.Add(item);
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("HappyRechargeReward", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && ((!ResultDataReader.IsClosed) ? true : false))
			{
				ResultDataReader.Close();
			}
		}
		return list;
	}

	public bool UpdateHappyRechargeDataPrize(int prizeType, int count, string NickName)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[3]
			{
				new SqlParameter("@PrizeType", prizeType),
				new SqlParameter("@Count", count),
				new SqlParameter("@NickName", NickName)
			};
			result = db.RunProcedure("SP_Update_HappyRecharge_Data", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("HappyRechargeDataUpdate", exception);
			}
		}
		return result;
	}

	public bool UpdateHappyRechargePrize(int count)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@Count", count)
			};
			result = db.RunProcedure("SP_Update_HappyRecharge_Prize", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("HappyRechargePfizeUpdate", exception);
			}
		}
		return result;
	}

	public GM_Active_Reward[] GetAllGmActiveReward()
	{
		List<GM_Active_Reward> list = new List<GM_Active_Reward>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_GM_Active_Reward_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitGmActiveRewardInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("InitGmActiveRewardInfo", exception);
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

	public GM_Active_Reward InitGmActiveRewardInfo(SqlDataReader dr)
	{
		return new GM_Active_Reward
		{
			giftId = (string)dr["giftId"],
			templateId = (int)dr["templateId"],
			count = (int)dr["count"],
			isBind = (int)dr["isBind"],
			occupationOrSex = (int)dr["occupationOrSex"],
			rewardType = (int)dr["rewardType"],
			validDate = (int)dr["validDate"],
			property = (string)dr["property"],
			remain1 = (string)dr["remain1"]
		};
	}

	public GM_Active_Condition[] GetAllGmActiveCondition()
	{
		List<GM_Active_Condition> list = new List<GM_Active_Condition>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_GM_Active_Condition_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitGmActiveConditionInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("InitGmActiveConditionInfo", exception);
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

	public GM_Active_Condition InitGmActiveConditionInfo(SqlDataReader dr)
	{
		return new GM_Active_Condition
		{
			giftbagId = (string)dr["giftbagId"],
			conditionIndex = (int)dr["conditionIndex"],
			conditionValue = (int)dr["conditionValue"],
			remain1 = (int)dr["remain1"],
			remain2 = (string)dr["remain2"]
		};
	}

	public GM_Gift[] GetAllGmGift()
	{
		List<GM_Gift> list = new List<GM_Gift>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_GM_Gift_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitGmGiftInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("InitGmGiftInfo", exception);
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

	public GM_Gift InitGmGiftInfo(SqlDataReader dr)
	{
		return new GM_Gift
		{
			giftbagId = (string)dr["giftbagId"],
			activityId = (string)dr["activityId"],
			rewardMark = (int)dr["rewardMark"],
			giftbagOrder = (int)dr["giftbagOrder"]
		};
	}

	public GM_Activity[] GetAllGmActivity()
	{
		List<GM_Activity> list = new List<GM_Activity>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_GM_Activity_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitGmActivityInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("InitGmActivityInfo", exception);
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

	public GM_Activity InitGmActivityInfo(SqlDataReader dr)
	{
		return new GM_Activity
		{
			activityId = (string)dr["activityId"],
			activityName = (string)dr["activityName"],
			activityType = (int)dr["activityType"],
			activityChildType = (int)dr["activityChildType"],
			getWay = (int)dr["getWay"],
			desc = (string)dr["desc"],
			rewardDesc = (string)dr["rewardDesc"],
			beginTime = (DateTime)dr["beginTime"],
			beginShowTime = (DateTime)dr["beginShowTime"],
			endTime = (DateTime)dr["endTime"],
			endShowTime = (DateTime)dr["endShowTime"],
			icon = (int)dr["icon"],
			isContinue = (int)dr["isContinue"],
			status = (int)dr["status"],
			remain1 = (int)dr["remain1"],
			remain2 = (string)dr["remain2"]
		};
	}
}
