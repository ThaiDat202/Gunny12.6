using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using Bussiness.CenterService;
using Bussiness.Managers;
using Newtonsoft.Json;
using SqlDataProvider.BaseClass;
using SqlDataProvider.Data;

namespace Bussiness;

public class PlayerBussiness : BaseBussiness
{
	private string AreaName = "Area 1";

	private int ZoneID = 1;

	private string VersionFlash = "7.0";

	public PlayerBussiness()
	{
	}

	public bool SendMail(MailInfo mail)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[29];
			array[0] = new SqlParameter("@ID", mail.ID);
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@Annex1", (mail.Annex1 == null) ? "" : mail.Annex1);
			array[2] = new SqlParameter("@Annex2", (mail.Annex2 == null) ? "" : mail.Annex2);
			array[3] = new SqlParameter("@Content", (mail.Content == null) ? "" : mail.Content);
			array[4] = new SqlParameter("@Gold", mail.Gold);
			array[5] = new SqlParameter("@IsExist", true);
			array[6] = new SqlParameter("@Money", mail.Money);
			array[7] = new SqlParameter("@Receiver", (mail.Receiver == null) ? "" : mail.Receiver);
			array[8] = new SqlParameter("@ReceiverID", mail.ReceiverID);
			array[9] = new SqlParameter("@Sender", (mail.Sender == null) ? "" : mail.Sender);
			array[10] = new SqlParameter("@SenderID", mail.SenderID);
			array[11] = new SqlParameter("@Title", (mail.Title == null) ? "" : mail.Title);
			array[12] = new SqlParameter("@IfDelS", false);
			array[13] = new SqlParameter("@IsDelete", false);
			array[14] = new SqlParameter("@IsDelR", false);
			array[15] = new SqlParameter("@IsRead", false);
			array[16] = new SqlParameter("@SendTime", DateTime.Now);
			array[17] = new SqlParameter("@Type", mail.Type);
			array[18] = new SqlParameter("@Annex1Name", (mail.Annex1Name == null) ? "" : mail.Annex1Name);
			array[19] = new SqlParameter("@Annex2Name", (mail.Annex2Name == null) ? "" : mail.Annex2Name);
			array[20] = new SqlParameter("@Annex3", (mail.Annex3 == null) ? "" : mail.Annex3);
			array[21] = new SqlParameter("@Annex4", (mail.Annex4 == null) ? "" : mail.Annex4);
			array[22] = new SqlParameter("@Annex5", (mail.Annex5 == null) ? "" : mail.Annex5);
			array[23] = new SqlParameter("@Annex3Name", (mail.Annex3Name == null) ? "" : mail.Annex3Name);
			array[24] = new SqlParameter("@Annex4Name", (mail.Annex4Name == null) ? "" : mail.Annex4Name);
			array[25] = new SqlParameter("@Annex5Name", (mail.Annex5Name == null) ? "" : mail.Annex5Name);
			array[26] = new SqlParameter("@ValidDate", mail.ValidDate);
			array[27] = new SqlParameter("@AnnexRemark", (mail.AnnexRemark == null) ? "" : mail.AnnexRemark);
			array[28] = new SqlParameter("@GiftToken", mail.GiftToken);
			result = db.RunProcedure("SP_Mail_Send", array);
			mail.ID = (int)array[0].Value;
			using CenterServiceClient centerServiceClient = new CenterServiceClient();
			centerServiceClient.MailNotice(mail.ReceiverID, ZoneID);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public DailyRecordInfo[] GetDailyRecord(int UserID)
	{
		List<DailyRecordInfo> list = new List<DailyRecordInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@UserID", UserID)
			};
			db.GetReader(ref ResultDataReader, "SP_DailyRecordInfo_Single", sqlParameters);
			while (ResultDataReader.Read())
			{
				DailyRecordInfo item = new DailyRecordInfo
				{
					UserID = (int)ResultDataReader["UserID"],
					Type = (int)ResultDataReader["Type"],
					Value = (string)ResultDataReader["Value"]
				};
				list.Add(item);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetDailyRecord", exception);
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

	public bool DeleteDailyRecord(int UserID, int Type)
	{
		bool result = false;
		try
		{
			result = db.RunProcedure("SP_DailyRecordInfo_Delete", new SqlParameter[2]
			{
				new SqlParameter("@UserID", UserID),
				new SqlParameter("@Type", Type)
			});
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_DailyRecordInfo_Delete", exception);
			}
		}
		return result;
	}

	public int SendMailAndItem(string title, string content, int UserID, int templateID, int count, int validDate, int gold, int money, int StrengthenLevel, int AttackCompose, int DefendCompose, int AgilityCompose, int LuckCompose, bool isBinds)
	{
		MailInfo mailInfo = new MailInfo();
		mailInfo.Annex1 = "";
		mailInfo.Content = title;
		mailInfo.Gold = gold;
		mailInfo.Money = money;
		mailInfo.Receiver = "";
		mailInfo.ReceiverID = UserID;
		mailInfo.Sender = "Administrators";
		mailInfo.SenderID = 0;
		mailInfo.Title = content;
		ItemInfo itemInfo = new ItemInfo(null);
		itemInfo.AgilityCompose = AgilityCompose;
		itemInfo.AttackCompose = AttackCompose;
		itemInfo.BeginDate = DateTime.Now;
		itemInfo.Color = "";
		itemInfo.DefendCompose = DefendCompose;
		itemInfo.IsDirty = false;
		itemInfo.IsExist = true;
		itemInfo.IsJudge = true;
		itemInfo.LuckCompose = LuckCompose;
		itemInfo.StrengthenLevel = StrengthenLevel;
		itemInfo.TemplateID = templateID;
		itemInfo.ValidDate = validDate;
		itemInfo.Count = count;
		itemInfo.IsBinds = isBinds;
		int returnValue = 1;
		SendMailAndItem(mailInfo, itemInfo, ref returnValue);
		return returnValue;
	}

	public bool SendMailAndItem(MailInfo mail, ItemInfo item, ref int returnValue)
	{
		bool flag = false;
		try
		{
			SqlParameter[] array = new SqlParameter[34]
			{
				new SqlParameter("@ItemID", item.ItemID),
				new SqlParameter("@UserID", item.UserID),
				new SqlParameter("@TemplateID", item.TemplateID),
				new SqlParameter("@Place", item.Place),
				new SqlParameter("@AgilityCompose", item.AgilityCompose),
				new SqlParameter("@AttackCompose", item.AttackCompose),
				new SqlParameter("@BeginDate", item.BeginDate),
				new SqlParameter("@Color", (item.Color == null) ? "" : item.Color),
				new SqlParameter("@Count", item.Count),
				new SqlParameter("@DefendCompose", item.DefendCompose),
				new SqlParameter("@IsBinds", item.IsBinds),
				new SqlParameter("@IsExist", item.IsExist),
				new SqlParameter("@IsJudge", item.IsJudge),
				new SqlParameter("@LuckCompose", item.LuckCompose),
				new SqlParameter("@StrengthenLevel", item.StrengthenLevel),
				new SqlParameter("@ValidDate", item.ValidDate),
				new SqlParameter("@BagType", item.BagType),
				new SqlParameter("@ID", mail.ID),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[17].Direction = ParameterDirection.Output;
			array[18] = new SqlParameter("@Annex1", (mail.Annex1 == null) ? "" : mail.Annex1);
			array[19] = new SqlParameter("@Annex2", (mail.Annex2 == null) ? "" : mail.Annex2);
			array[20] = new SqlParameter("@Content", (mail.Content == null) ? "" : mail.Content);
			array[21] = new SqlParameter("@Gold", mail.Gold);
			array[22] = new SqlParameter("@Money", mail.Money);
			array[23] = new SqlParameter("@Receiver", (mail.Receiver == null) ? "" : mail.Receiver);
			array[24] = new SqlParameter("@ReceiverID", mail.ReceiverID);
			array[25] = new SqlParameter("@Sender", (mail.Sender == null) ? "" : mail.Sender);
			array[26] = new SqlParameter("@SenderID", mail.SenderID);
			array[27] = new SqlParameter("@Title", (mail.Title == null) ? "" : mail.Title);
			array[28] = new SqlParameter("@IfDelS", false);
			array[29] = new SqlParameter("@IsDelete", false);
			array[30] = new SqlParameter("@IsDelR", false);
			array[31] = new SqlParameter("@IsRead", false);
			array[32] = new SqlParameter("@SendTime", DateTime.Now);
			array[33] = new SqlParameter("@Result", SqlDbType.Int);
			array[33].Direction = ParameterDirection.ReturnValue;
			flag = db.RunProcedure("SP_Admin_SendUserItem", array);
			returnValue = (int)array[33].Value;
			flag = returnValue == 0;
			if (flag)
			{
				using CenterServiceClient centerServiceClient = new CenterServiceClient();
				centerServiceClient.MailNotice(mail.ReceiverID, ZoneID);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return flag;
	}

	public PlayerBussiness(AreaConfigInfo config)
	{
		string text = "Data Source=DESKTOP-SKTCRVL;Initial Catalog=Db_Tank_All;Persist Security Info=True;User ID=sa;Password=123456;";
		AreaName = config.AreaName;
		ZoneID = config.AreaID;
		VersionFlash = config.Version;
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

	public bool GetInformationAchema(string table)
	{
		SqlDataReader Sdr = null;
		try
		{
			db.FillSqlDataReader(ref Sdr, "select count (*) as Active  from information_schema.tables where table_name = '" + table + "'");
			if (Sdr.Read())
			{
				return (int)Sdr["Active"] == 1;
			}
		}
		catch (Exception exception)
		{
			BaseBussiness.log.Error("GetSigleKingGradeData", exception);
		}
		finally
		{
			if (Sdr != null && !Sdr.IsClosed)
			{
				Sdr.Close();
			}
		}
		return false;
	}

	public UserTrailEliteInfo GetSigleUserTrailElite(int UserID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@UserID", UserID)
			};
			db.GetReader(ref ResultDataReader, "SP_Sys_Users_Trail_Elite_Single", sqlParameters);
			if (ResultDataReader.Read())
			{
				return InitUserTrailElite(ResultDataReader);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetSigleUserTrailElite", exception);
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

	public UserTrailEliteInfo InitUserTrailElite(SqlDataReader reader)
	{
		UserTrailEliteInfo userTrailEliteInfo = new UserTrailEliteInfo();
		userTrailEliteInfo.ID = (int)reader["ID"];
		userTrailEliteInfo.UserID = (int)reader["UserID"];
		userTrailEliteInfo.trailEliteScore = (int)reader["trailEliteScore"];
		userTrailEliteInfo.trailEliteTotal = (int)reader["trailEliteTotal"];
		userTrailEliteInfo.trailEliteWin = (int)reader["trailEliteWin"];
		userTrailEliteInfo.trailEliteRankTotal = (int)reader["trailEliteRankTotal"];
		userTrailEliteInfo.trailEliteRankWin = (int)reader["trailEliteRankWin"];
		userTrailEliteInfo.trailEliteIsUpRank = (bool)reader["trailEliteIsUpRank"];
		userTrailEliteInfo.trailEliteLevel = (int)reader["trailEliteLevel"];
		return userTrailEliteInfo;
	}

	public bool AddUserTrailElite(UserTrailEliteInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[10]
			{
				new SqlParameter("@ID", item.ID),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@UserID", item.UserID);
			array[2] = new SqlParameter("@trailEliteScore", item.trailEliteScore);
			array[3] = new SqlParameter("@trailEliteTotal", item.trailEliteTotal);
			array[4] = new SqlParameter("@trailEliteWin", item.trailEliteWin);
			array[5] = new SqlParameter("@trailEliteRankTotal", item.trailEliteRankTotal);
			array[6] = new SqlParameter("@trailEliteRankWin", item.trailEliteRankWin);
			array[7] = new SqlParameter("@trailEliteIsUpRank", item.trailEliteIsUpRank);
			array[8] = new SqlParameter("@trailEliteLevel", item.trailEliteLevel);
			array[9] = new SqlParameter("@Result", SqlDbType.Int);
			array[9].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Sys_Users_Trail_Elite_Add", array);
			result = (int)array[9].Value == 0;
			item.ID = (int)array[0].Value;
			item.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("AddUserTrailElite", exception);
			}
		}
		return result;
	}

	public bool UpdateUserTrailElite(UserTrailEliteInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[10]
			{
				new SqlParameter("@ID", item.ID),
				new SqlParameter("@UserID", item.UserID),
				new SqlParameter("@trailEliteScore", item.trailEliteScore),
				new SqlParameter("@trailEliteTotal", item.trailEliteTotal),
				new SqlParameter("@trailEliteWin", item.trailEliteWin),
				new SqlParameter("@trailEliteRankTotal", item.trailEliteRankTotal),
				new SqlParameter("@trailEliteRankWin", item.trailEliteRankWin),
				new SqlParameter("@trailEliteIsUpRank", item.trailEliteIsUpRank),
				new SqlParameter("@trailEliteLevel", item.trailEliteLevel),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[9].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Sys_Users_Trail_Elite_Update", array);
			result = (int)array[9].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("UpdateUserTrailElite", exception);
			}
		}
		return result;
	}

	public KingGradeDataInfo GetSigleKingGradeData(int UserID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@UserID", UserID)
			};
			db.GetReader(ref ResultDataReader, "SP_King_Grade_Data_Single", sqlParameters);
			if (ResultDataReader.Read())
			{
				return InitKingGradeData(ResultDataReader);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetSigleKingGradeData", exception);
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

	public KingGradeDataInfo InitKingGradeData(SqlDataReader reader)
	{
		KingGradeDataInfo kingGradeDataInfo = new KingGradeDataInfo();
		kingGradeDataInfo.UserID = (int)reader["UserID"];
		kingGradeDataInfo.ddtKingGrade = (int)reader["ddtKingGrade"];
		kingGradeDataInfo.currentExp = (int)reader["currentExp"];
		kingGradeDataInfo.usableNum = (int)reader["usableNum"];
		kingGradeDataInfo.AttackLevel = (int)reader["AttackLevel"];
		kingGradeDataInfo.DefenceLevel = (int)reader["DefenceLevel"];
		kingGradeDataInfo.AgilityLevel = (int)reader["AgilityLevel"];
		kingGradeDataInfo.LuckyLevel = (int)reader["LuckyLevel"];
		kingGradeDataInfo.MagicAttackLevel = (int)reader["MagicAttackLevel"];
		kingGradeDataInfo.MagicDefenceLevel = (int)reader["MagicDefenceLevel"];
		return kingGradeDataInfo;
	}

	public bool AddKingGradeData(KingGradeDataInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[11]
			{
				new SqlParameter("@UserID", item.UserID),
				new SqlParameter("@ddtKingGrade", item.ddtKingGrade),
				new SqlParameter("@currentExp", item.currentExp),
				new SqlParameter("@usableNum", item.usableNum),
				new SqlParameter("@AttackLevel", item.AttackLevel),
				new SqlParameter("@DefenceLevel", item.DefenceLevel),
				new SqlParameter("@AgilityLevel", item.AgilityLevel),
				new SqlParameter("@LuckyLevel", item.LuckyLevel),
				new SqlParameter("@MagicAttackLevel", item.MagicAttackLevel),
				new SqlParameter("@MagicDefenceLevel", item.MagicDefenceLevel),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[10].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_King_Grade_Data_Add", array);
			result = (int)array[10].Value == 0;
			item.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("AddKingGradeData", exception);
			}
		}
		return result;
	}

	public bool UpdateKingGradeData(KingGradeDataInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[11]
			{
				new SqlParameter("@UserID", item.UserID),
				new SqlParameter("@ddtKingGrade", item.ddtKingGrade),
				new SqlParameter("@currentExp", item.currentExp),
				new SqlParameter("@usableNum", item.usableNum),
				new SqlParameter("@AttackLevel", item.AttackLevel),
				new SqlParameter("@DefenceLevel", item.DefenceLevel),
				new SqlParameter("@AgilityLevel", item.AgilityLevel),
				new SqlParameter("@LuckyLevel", item.LuckyLevel),
				new SqlParameter("@MagicAttackLevel", item.MagicAttackLevel),
				new SqlParameter("@MagicDefenceLevel", item.MagicDefenceLevel),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[10].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_King_Grade_Data_Update", array);
			result = (int)array[10].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("UpdateKingGradeData", exception);
			}
		}
		return result;
	}

	public int PullDown(int activeID, string awardID, int userID, ref string msg)
	{
		int num = 1;
		try
		{
			SqlParameter[] array = new SqlParameter[4]
			{
				new SqlParameter("@ActiveID", activeID),
				new SqlParameter("@AwardID", awardID),
				new SqlParameter("@UserID", userID),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[3].Direction = ParameterDirection.ReturnValue;
			if (db.RunProcedure("SP_Active_PullDown", array))
			{
				num = (int)array[3].Value;
				switch (num)
				{
				case 0:
					msg = "ActiveBussiness.Msg0";
					break;
				case 1:
					msg = "ActiveBussiness.Msg1";
					break;
				case 2:
					msg = "ActiveBussiness.Msg2";
					break;
				case 3:
					msg = "ActiveBussiness.Msg3";
					break;
				case 4:
					msg = "ActiveBussiness.Msg4";
					break;
				case 5:
					msg = "ActiveBussiness.Msg5";
					break;
				case 6:
					msg = "ActiveBussiness.Msg6";
					break;
				case 7:
					msg = "ActiveBussiness.Msg7";
					break;
				case 8:
					msg = "ActiveBussiness.Msg8";
					break;
				default:
					msg = "ActiveBussiness.Msg9";
					break;
				}
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return num;
	}

	public bool DeleteAllActive()
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[0];
			result = db.RunProcedure("SP_Active_Delete", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_Active_Delete", exception);
			}
		}
		return result;
	}

	public bool AddActive(ActiveInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[19]
			{
				new SqlParameter("@ActiveID", info.ActiveID),
				new SqlParameter("@Description", info.Description),
				new SqlParameter("@Content", info.Content),
				new SqlParameter("@AwardContent", info.AwardContent),
				new SqlParameter("@HasKey", info.HasKey),
				new SqlParameter("@EndDate", info.EndDate),
				new SqlParameter("@IsOnly", info.IsOnly),
				new SqlParameter("@StartDate", info.StartDate),
				new SqlParameter("@Title", info.Title),
				new SqlParameter("@Type", info.Type),
				new SqlParameter("@ActiveType", info.ActiveType),
				new SqlParameter("@ActionTimeContent", info.ActionTimeContent),
				new SqlParameter("@IsAdvance", info.IsAdvance),
				new SqlParameter("@GoodsExchangeTypes", info.GoodsExchangeTypes),
				new SqlParameter("@GoodsExchangeNum", info.GoodsExchangeNum),
				new SqlParameter("@limitType", info.limitType),
				new SqlParameter("@limitValue", info.limitValue),
				new SqlParameter("@IsShow", info.IsShow),
				new SqlParameter("@IconID", info.IconID)
			};
			result = db.RunProcedure("SP_Active_Add", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_Active_Add", exception);
			}
		}
		return result;
	}

	public bool AddActiveAward(ActiveAwardInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[13]
			{
				new SqlParameter("@ActiveID", info.ActiveID),
				new SqlParameter("@ItemID", info.ItemID),
				new SqlParameter("@Count", info.Count),
				new SqlParameter("@ValidDate", info.ValidDate),
				new SqlParameter("@StrengthenLevel", info.StrengthenLevel),
				new SqlParameter("@AttackCompose", info.AttackCompose),
				new SqlParameter("@DefendCompose", info.DefendCompose),
				new SqlParameter("@LuckCompose", info.LuckCompose),
				new SqlParameter("@AgilityCompose", info.AgilityCompose),
				new SqlParameter("@Gold", info.Gold),
				new SqlParameter("@Money", info.Money),
				new SqlParameter("@Sex", info.Sex),
				new SqlParameter("@Mark", info.Mark)
			};
			result = db.RunProcedure("SP_Active_Award", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_Active_Award_Add", exception);
			}
		}
		return result;
	}

	public UserHomeInfo GetAllUserHomeByID(int userID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = userID;
			db.GetReader(ref ResultDataReader, "SP_Sys_User_Home_All", array);
			if (ResultDataReader.Read())
			{
				return InitUserHomeInfo(ResultDataReader);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("InitUserHomeInfo", exception);
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

	public UserHomeInfo InitUserHomeInfo(SqlDataReader dr)
	{
		UserHomeInfo userHomeInfo = new UserHomeInfo();
		userHomeInfo.ID = (int)dr["ID"];
		userHomeInfo.UserID = (int)dr["UserID"];
		userHomeInfo.houseStage = (int)dr["houseStage"];
		userHomeInfo.energyCount = (int)dr["energyCount"];
		userHomeInfo.drinkCount = (int)dr["drinkCount"];
		userHomeInfo.cdEndTime = (DateTime)dr["cdEndTime"];
		userHomeInfo.pourCount = (int)dr["pourCount"];
		userHomeInfo.tasteCount = (int)dr["tasteCount"];
		userHomeInfo.enableFight = (int)dr["enableFight"];
		userHomeInfo.Gold = (int)dr["Gold"];
		userHomeInfo.Blood = (int)dr["Blood"];
		userHomeInfo.yardAdornInfo = ((dr["yardAdornInfo"] == null) ? "" : dr["yardAdornInfo"].ToString());
		userHomeInfo.houseAdornInfo = ((dr["houseAdornInfo"] == null) ? "" : dr["houseAdornInfo"].ToString());
		userHomeInfo.LogTaste = ((dr["LogTaste"] == null) ? "" : dr["LogTaste"].ToString());
		userHomeInfo.LastLogin = (DateTime)dr["LastLogin"];
		userHomeInfo.fishGameTime = (int)dr["fishGameTime"];
		userHomeInfo.fishScore = (int)dr["fishScore"];
		userHomeInfo.dayScore = (int)dr["dayScore"];
		userHomeInfo.maxScore = (int)dr["maxScore"];
		userHomeInfo.remainCount = (int)dr["remainCount"];
		userHomeInfo.NumRedPkgRemain = (int)dr["NumRedPkgRemain"];
		userHomeInfo.MoneyTrees = ((dr["MoneyTrees"] == null) ? "" : dr["MoneyTrees"].ToString());
		return userHomeInfo;
	}

	public bool AddUserHome(UserHomeInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[22]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@houseStage", info.houseStage),
				new SqlParameter("@energyCount", info.energyCount),
				new SqlParameter("@drinkCount", info.drinkCount),
				new SqlParameter("@cdEndTime", info.cdEndTime),
				new SqlParameter("@pourCount", info.pourCount),
				new SqlParameter("@tasteCount", info.tasteCount),
				new SqlParameter("@enableFight", info.enableFight),
				new SqlParameter("@Gold", info.Gold),
				new SqlParameter("@Blood", info.Blood),
				new SqlParameter("@yardAdornInfo", info.yardAdornInfo),
				new SqlParameter("@houseAdornInfo", info.houseAdornInfo),
				new SqlParameter("@LogTaste", info.LogTaste),
				new SqlParameter("@LastLogin", info.LastLogin),
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[0].Direction = ParameterDirection.Output;
			array[15] = new SqlParameter("@fishGameTime", info.fishGameTime);
			array[16] = new SqlParameter("@fishScore", info.fishScore);
			array[17] = new SqlParameter("@dayScore", info.dayScore);
			array[18] = new SqlParameter("@maxScore", info.maxScore);
			array[19] = new SqlParameter("@remainCount", info.remainCount);
			array[20] = new SqlParameter("@NumRedPkgRemain", info.NumRedPkgRemain);
			array[21] = new SqlParameter("@MoneyTrees", info.MoneyTrees);
			result = db.RunProcedure("SP_Sys_User_Home_Add", array);
			info.ID = (int)array[0].Value;
			info.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_Sys_User_Home_Add", exception);
			}
		}
		return result;
	}

	public bool UpdateUserHome(UserHomeInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[22]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@houseStage", info.houseStage),
				new SqlParameter("@energyCount", info.energyCount),
				new SqlParameter("@drinkCount", info.drinkCount),
				new SqlParameter("@cdEndTime", info.cdEndTime),
				new SqlParameter("@pourCount", info.pourCount),
				new SqlParameter("@tasteCount", info.tasteCount),
				new SqlParameter("@enableFight", info.enableFight),
				new SqlParameter("@Gold", info.Gold),
				new SqlParameter("@Blood", info.Blood),
				new SqlParameter("@yardAdornInfo", info.yardAdornInfo),
				new SqlParameter("@houseAdornInfo", info.houseAdornInfo),
				new SqlParameter("@LogTaste", info.LogTaste),
				new SqlParameter("@LastLogin", info.LastLogin),
				new SqlParameter("@fishGameTime", info.fishGameTime),
				new SqlParameter("@fishScore", info.fishScore),
				new SqlParameter("@dayScore", info.dayScore),
				new SqlParameter("@maxScore", info.maxScore),
				new SqlParameter("@remainCount", info.remainCount),
				new SqlParameter("@NumRedPkgRemain", info.NumRedPkgRemain),
				new SqlParameter("@MoneyTrees", info.MoneyTrees)
			};
			result = db.RunProcedure("SP_Sys_User_Home_Update", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_Sys_User_Home_Update", exception);
			}
		}
		return result;
	}

	public bool UpdateAdorn(AdornInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[4]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@TemplateId", info.TemplateId),
				new SqlParameter("@Category", info.Category)
			};
			result = db.RunProcedure("SP_Sys_User_Adorn_Update", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_Sys_User_Adorn_Update", exception);
			}
		}
		return result;
	}

	public LuckstarActivityRankInfo[] GetAllLuckstarActivityRank()
	{
		List<LuckstarActivityRankInfo> list = new List<LuckstarActivityRankInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Luckstar_Activity_Rank_All");
			int num = 1;
			while (ResultDataReader.Read())
			{
				LuckstarActivityRankInfo luckstarActivityRankInfo = new LuckstarActivityRankInfo();
				luckstarActivityRankInfo.rank = num;
				luckstarActivityRankInfo.UserID = (int)ResultDataReader["UserID"];
				luckstarActivityRankInfo.useStarNum = (int)ResultDataReader["useStarNum"];
				luckstarActivityRankInfo.isVip = (int)ResultDataReader["isVip"];
				luckstarActivityRankInfo.nickName = (string)ResultDataReader["nickName"];
				list.Add(luckstarActivityRankInfo);
				num++;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
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

	public bool AddAdorn(AdornInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[4]
			{
				new SqlParameter("@ID", info.ID),
				null,
				null,
				null
			};
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@UserID", info.UserID);
			array[2] = new SqlParameter("@TemplateId", info.TemplateId);
			array[3] = new SqlParameter("@Category", info.Category);
			result = db.RunProcedure("SP_Sys_User_Adorn_Add", array);
			info.ID = (int)array[0].Value;
			info.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_Sys_User_Adorn_Add", exception);
			}
		}
		return result;
	}

	public AdornInfo[] GetAllAdornByUserID(int UserID)
	{
		List<AdornInfo> list = new List<AdornInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_Sys_User_Adorn_All", array);
			while (ResultDataReader.Read())
			{
				list.Add(InitAdornInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("InitAdornInfo", exception);
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

	public AdornInfo InitAdornInfo(SqlDataReader dr)
	{
		AdornInfo adornInfo = new AdornInfo();
		adornInfo.ID = (int)dr["ID"];
		adornInfo.UserID = (int)dr["UserID"];
		adornInfo.TemplateId = (int)dr["TemplateId"];
		adornInfo.Category = (int)dr["Category"];
		return adornInfo;
	}

	public int ActiveChickCode(int UserID, string Code)
	{
		int result = 3;
		try
		{
			SqlParameter[] array = new SqlParameter[3]
			{
				new SqlParameter("@UserID", UserID),
				new SqlParameter("@ActiveCode", Code),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[2].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Active_ChickCode", array);
			result = (int)array[2].Value;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public CardAchievementDataInfo[] GetAllCardAchievementData(int UserID)
	{
		List<CardAchievementDataInfo> list = new List<CardAchievementDataInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@UserID", UserID)
			};
			db.GetReader(ref ResultDataReader, "SP_Card_Achievement_Data_Single", sqlParameters);
			while (ResultDataReader.Read())
			{
				list.Add(InitCardAchievementData(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetSigleCardAchievementData", exception);
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

	public CardAchievementDataInfo InitCardAchievementData(SqlDataReader reader)
	{
		CardAchievementDataInfo cardAchievementDataInfo = new CardAchievementDataInfo();
		cardAchievementDataInfo.UserID = (int)reader["UserID"];
		cardAchievementDataInfo.AchievementID = (int)reader["AchievementID"];
		cardAchievementDataInfo.CompletedDate = (DateTime)reader["CompletedDate"];
		cardAchievementDataInfo.IsComplete = (int)reader["IsComplete"];
		cardAchievementDataInfo.Condition = (int)reader["Condition"];
		return cardAchievementDataInfo;
	}

	public bool UpdateCardAchievementData(CardAchievementDataInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[6]
			{
				new SqlParameter("@UserID", item.UserID),
				new SqlParameter("@AchievementID", item.AchievementID),
				new SqlParameter("@CompletedDate", item.CompletedDate),
				new SqlParameter("@IsComplete", item.IsComplete),
				new SqlParameter("@Condition", item.Condition),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[5].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Card_Achievement_Data_Update", array);
			result = (int)array[5].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("UpdateCardAchievementData", exception);
			}
		}
		return result;
	}

	public AchievementDataInfo[] GetAllAchievementData(int userID)
	{
		List<AchievementDataInfo> list = new List<AchievementDataInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@UserID", userID)
			};
			db.GetReader(ref ResultDataReader, "SP_Achievement_Data_All", sqlParameters);
			while (ResultDataReader.Read())
			{
				list.Add(InitAchievementData(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllAchievementData", exception);
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

	public bool UpdateAchievementData(AchievementDataInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[4]
			{
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@AchievementID", info.AchievementID),
				new SqlParameter("@CompletedDate", info.CompletedDate),
				new SqlParameter("@IsComplete", info.IsComplete)
			};
			result = db.RunProcedure("SP_AchievementData_Add", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_Gypsy_Item_Data_Update", exception);
			}
		}
		return result;
	}

	public AchievementDataInfo InitAchievementData(SqlDataReader reader)
	{
		return new AchievementDataInfo
		{
			UserID = (int)reader["UserID"],
			AchievementID = (int)reader["AchievementID"],
			IsComplete = (bool)reader["IsComplete"],
			CompletedDate = (DateTime)reader["CompletedDate"]
		};
	}

	public bool UpdateGypsyItemData(GypsyItemDataInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[9]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@GypsyID", info.GypsyID),
				new SqlParameter("@InfoID", info.InfoID),
				new SqlParameter("@Unit", info.Unit),
				new SqlParameter("@Num", info.Num),
				new SqlParameter("@Price", info.Price),
				new SqlParameter("@CanBuy", info.CanBuy),
				new SqlParameter("@Quality", info.Quality)
			};
			result = db.RunProcedure("SP_Gypsy_Item_Data_Update", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_Gypsy_Item_Data_Update", exception);
			}
		}
		return result;
	}

	public bool AddGypsyItemData(GypsyItemDataInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[9]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@GypsyID", info.GypsyID),
				new SqlParameter("@InfoID", info.InfoID),
				new SqlParameter("@Unit", info.Unit),
				new SqlParameter("@Num", info.Num),
				new SqlParameter("@Price", info.Price),
				new SqlParameter("@CanBuy", info.CanBuy),
				new SqlParameter("@Quality", info.Quality)
			};
			array[0].Direction = ParameterDirection.Output;
			result = db.RunProcedure("SP_Gypsy_Item_Data_Add", array);
			info.ID = (int)array[0].Value;
			info.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_Gypsy_Item_Data_Add", exception);
			}
		}
		return result;
	}

	public GypsyItemDataInfo[] GetAllGypsyItemDataByID(int ID)
	{
		List<GypsyItemDataInfo> list = new List<GypsyItemDataInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@ID", SqlDbType.Int, 4)
			};
			array[0].Value = ID;
			db.GetReader(ref ResultDataReader, "SP_Gypsy_Item_Data_All", array);
			while (ResultDataReader.Read())
			{
				list.Add(InitGypsyItemDataInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("InitGypsyItemDataInfo", exception);
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

	public GypsyItemDataInfo InitGypsyItemDataInfo(SqlDataReader dr)
	{
		GypsyItemDataInfo gypsyItemDataInfo = new GypsyItemDataInfo();
		gypsyItemDataInfo.ID = (int)dr["ID"];
		gypsyItemDataInfo.UserID = (int)dr["UserID"];
		gypsyItemDataInfo.GypsyID = (int)dr["GypsyID"];
		gypsyItemDataInfo.InfoID = (int)dr["InfoID"];
		gypsyItemDataInfo.Unit = (int)dr["Unit"];
		gypsyItemDataInfo.Num = (int)dr["Num"];
		gypsyItemDataInfo.Price = (int)dr["Price"];
		gypsyItemDataInfo.CanBuy = (int)dr["CanBuy"];
		gypsyItemDataInfo.Quality = (int)dr["Quality"];
		return gypsyItemDataInfo;
	}

	public CloudBuyLogInfo[] GetAllCloudBuyLog()
	{
		List<CloudBuyLogInfo> list = new List<CloudBuyLogInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Cloud_Buy_Log_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitCloudBuyLogInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("InitCloudBuyLogInfo", exception);
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

	public CloudBuyLogInfo InitCloudBuyLogInfo(SqlDataReader dr)
	{
		CloudBuyLogInfo cloudBuyLogInfo = new CloudBuyLogInfo();
		cloudBuyLogInfo.ID = (int)dr["ID"];
		cloudBuyLogInfo.UserID = (int)dr["UserID"];
		cloudBuyLogInfo.nickName = ((dr["nickName"] == null) ? "" : dr["nickName"].ToString());
		cloudBuyLogInfo.templateId = (int)dr["templateId"];
		cloudBuyLogInfo.validate = (int)dr["validate"];
		cloudBuyLogInfo.count = (int)dr["count"];
		cloudBuyLogInfo.property = ((dr["property"] == null) ? "" : dr["property"].ToString());
		return cloudBuyLogInfo;
	}

	public bool AddCloudBuyLog(CloudBuyLogInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[7]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@nickName", info.nickName),
				new SqlParameter("@templateId", info.templateId),
				new SqlParameter("@validate", info.validate),
				new SqlParameter("@count", info.count),
				new SqlParameter("@property", info.property)
			};
			result = db.RunProcedure("SP_Cloud_Buy_Log_Add", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_Cloud_Buy_Log_Add", exception);
			}
		}
		return result;
	}

	public bool ResetLuckCount()
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[0];
			result = db.RunProcedure("SP_Reset_Luck_Count", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_Reset_Luck_Count", exception);
			}
		}
		return result;
	}

	public bool UpdateCloudBuyLottery(CloudBuyLotteryInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[9]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@templateId", info.templateId),
				new SqlParameter("@templatedIdCount", info.templatedIdCount),
				new SqlParameter("@validDate", info.validDate),
				new SqlParameter("@property", info.property),
				new SqlParameter("@buyItemsArr", info.buyItemsArr),
				new SqlParameter("@buyMoney", info.buyMoney),
				new SqlParameter("@maxNum", info.maxNum),
				new SqlParameter("@currentNum", info.currentNum)
			};
			result = db.RunProcedure("SP_Cloud_Buy_Lottery_Update", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_Cloud_Buy_Lottery_Update", exception);
			}
		}
		return result;
	}

	public bool UpdateHalloweenRank(HalloweenRankInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[5]
			{
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@useNum", info.useNum),
				new SqlParameter("@nickName", info.nickName),
				new SqlParameter("@isVip", info.isVip),
				new SqlParameter("@rank", info.rank)
			};
			result = db.RunProcedure("SP_Halloween_Rank_Update", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_Halloween_Rank_Update", exception);
			}
		}
		return result;
	}

	public WarriorFamRankInfo[] GetAllWarriorFamRank()
	{
		List<WarriorFamRankInfo> list = new List<WarriorFamRankInfo>();
		SqlDataReader ResultDataReader = null;
		int num = 1;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[0];
			db.GetReader(ref ResultDataReader, "SP_Get_All_Warrior_Fam_Rank_All", sqlParameters);
			while (ResultDataReader.Read())
			{
				WarriorFamRankInfo warriorFamRankInfo = new WarriorFamRankInfo();
				warriorFamRankInfo.PlayerRank = num;
				warriorFamRankInfo.PlayerName = ((ResultDataReader["NickName"] == null) ? "" : ResultDataReader["NickName"].ToString());
				warriorFamRankInfo.FamLevel = (int)ResultDataReader["myProgress"];
				warriorFamRankInfo.Turn = (int)ResultDataReader["maxFinishLevelTurn"];
				list.Add(warriorFamRankInfo);
				num++;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_Get_All_Warrior_Fam_Rank_All", exception);
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

	public HalloweenRankInfo[] GetAllHalloweenRank()
	{
		List<HalloweenRankInfo> list = new List<HalloweenRankInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Halloween_Rank_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitHalloweenRankInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("InitHalloweenRankInfo", exception);
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

	public HalloweenRankInfo InitHalloweenRankInfo(SqlDataReader dr)
	{
		HalloweenRankInfo halloweenRankInfo = new HalloweenRankInfo();
		halloweenRankInfo.ID = (int)dr["ID"];
		halloweenRankInfo.UserID = (int)dr["UserID"];
		halloweenRankInfo.useNum = (int)dr["useNum"];
		halloweenRankInfo.nickName = ((dr["nickName"] == null) ? "" : dr["nickName"].ToString());
		halloweenRankInfo.isVip = (int)dr["isVip"];
		halloweenRankInfo.rank = (int)dr["rank"];
		return halloweenRankInfo;
	}

	public bool AddBoguAdventureData(BoguAdventureDataInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[13]
			{
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@currentIndex", info.currentIndex),
				new SqlParameter("@hp", info.hp),
				new SqlParameter("@isAcquireAward1", info.isAcquireAward1),
				new SqlParameter("@isAcquireAward2", info.isAcquireAward2),
				new SqlParameter("@isAcquireAward3", info.isAcquireAward3),
				new SqlParameter("@openCount", info.openCount),
				new SqlParameter("@isFreeReset", info.isFreeReset),
				new SqlParameter("@resetCount", info.resetCount),
				new SqlParameter("@cellInfo", info.cellInfo),
				new SqlParameter("@awardCount", info.awardCount),
				new SqlParameter("@ID", info.ID),
				null
			};
			array[11].Direction = ParameterDirection.Output;
			array[12] = new SqlParameter("@lastEnterGame", info.lastEnterGame);
			result = db.RunProcedure("SP_BoguAdventure_Data_Add", array);
			info.ID = (int)array[11].Value;
			info.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_BoguAdventure_Data_Add", exception);
			}
		}
		return result;
	}

	public bool UpdateBoguAdventureData(BoguAdventureDataInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[13]
			{
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@currentIndex", info.currentIndex),
				new SqlParameter("@hp", info.hp),
				new SqlParameter("@isAcquireAward1", info.isAcquireAward1),
				new SqlParameter("@isAcquireAward2", info.isAcquireAward2),
				new SqlParameter("@isAcquireAward3", info.isAcquireAward3),
				new SqlParameter("@openCount", info.openCount),
				new SqlParameter("@isFreeReset", info.isFreeReset),
				new SqlParameter("@resetCount", info.resetCount),
				new SqlParameter("@cellInfo", info.cellInfo),
				new SqlParameter("@awardCount", info.awardCount),
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@lastEnterGame", info.lastEnterGame)
			};
			result = db.RunProcedure("SP_BoguAdventure_Data_Update", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_BoguAdventure_Data_Update", exception);
			}
		}
		return result;
	}

	public BoguAdventureDataInfo GetAllBoguAdventureDataByID(int UserID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_BoguAdventure_Data_All", array);
			if (ResultDataReader.Read())
			{
				return InitBoguAdventureDataInfo(ResultDataReader);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("InitBoguAdventureDataInfo", exception);
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

	public BoguAdventureDataInfo InitBoguAdventureDataInfo(SqlDataReader dr)
	{
		BoguAdventureDataInfo boguAdventureDataInfo = new BoguAdventureDataInfo();
		boguAdventureDataInfo.ID = (int)dr["ID"];
		boguAdventureDataInfo.UserID = (int)dr["UserID"];
		boguAdventureDataInfo.currentIndex = (int)dr["currentIndex"];
		boguAdventureDataInfo.hp = (int)dr["hp"];
		boguAdventureDataInfo.isAcquireAward1 = (int)dr["isAcquireAward1"];
		boguAdventureDataInfo.isAcquireAward2 = (int)dr["isAcquireAward2"];
		boguAdventureDataInfo.isAcquireAward3 = (int)dr["isAcquireAward3"];
		boguAdventureDataInfo.openCount = (int)dr["openCount"];
		boguAdventureDataInfo.isFreeReset = (bool)dr["isFreeReset"];
		boguAdventureDataInfo.resetCount = (int)dr["resetCount"];
		boguAdventureDataInfo.cellInfo = ((dr["cellInfo"] == null) ? "" : dr["cellInfo"].ToString());
		boguAdventureDataInfo.awardCount = ((dr["awardCount"] == null) ? "" : dr["awardCount"].ToString());
		boguAdventureDataInfo.lastEnterGame = (DateTime)dr["lastEnterGame"];
		return boguAdventureDataInfo;
	}

	public bool AddRingstationConfig(RingstationConfigInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[11]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@ChallengeNum", info.ChallengeNum),
				new SqlParameter("@buyCount", info.buyCount),
				new SqlParameter("@buyPrice", info.buyPrice),
				new SqlParameter("@cdPrice", info.cdPrice),
				new SqlParameter("@AwardTime", info.AwardTime),
				new SqlParameter("@AwardNum", info.AwardNum),
				new SqlParameter("@AwardFightWin", info.AwardFightWin),
				new SqlParameter("@AwardFightLost", info.AwardFightLost),
				new SqlParameter("@ChampionText", info.ChampionText),
				new SqlParameter("@IsFirstUpdateRank", info.IsFirstUpdateRank)
			};
			array[0].Direction = ParameterDirection.Output;
			result = db.RunProcedure("SP_Ringstation_Config_Add", array);
			info.ID = (int)array[0].Value;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_Ringstation_Config_Add", exception);
			}
		}
		return result;
	}

	public bool UpdateRingstationConfig(RingstationConfigInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[11]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@ChallengeNum", info.ChallengeNum),
				new SqlParameter("@buyCount", info.buyCount),
				new SqlParameter("@buyPrice", info.buyPrice),
				new SqlParameter("@cdPrice", info.cdPrice),
				new SqlParameter("@AwardTime", info.AwardTime),
				new SqlParameter("@AwardNum", info.AwardNum),
				new SqlParameter("@AwardFightWin", info.AwardFightWin),
				new SqlParameter("@AwardFightLost", info.AwardFightLost),
				new SqlParameter("@ChampionText", info.ChampionText),
				new SqlParameter("@IsFirstUpdateRank", info.IsFirstUpdateRank)
			};
			result = db.RunProcedure("SP_Ringstation_Config_Update", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_Ringstation_Config_Update", exception);
			}
		}
		return result;
	}

	public RingstationConfigInfo GetAllRingstationConfig()
	{
		List<RingstationConfigInfo> list = new List<RingstationConfigInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Ringstation_Config_All");
			if (ResultDataReader.Read())
			{
				return InitRingstationConfigInfo(ResultDataReader);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("InitRingstationConfigInfo", exception);
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

	public RingstationConfigInfo InitRingstationConfigInfo(SqlDataReader dr)
	{
		RingstationConfigInfo ringstationConfigInfo = new RingstationConfigInfo();
		ringstationConfigInfo.ID = (int)dr["ID"];
		ringstationConfigInfo.ChallengeNum = (int)dr["ChallengeNum"];
		ringstationConfigInfo.buyCount = (int)dr["buyCount"];
		ringstationConfigInfo.buyPrice = (int)dr["buyPrice"];
		ringstationConfigInfo.cdPrice = (int)dr["cdPrice"];
		ringstationConfigInfo.AwardTime = (DateTime)dr["AwardTime"];
		ringstationConfigInfo.AwardNum = (int)dr["AwardNum"];
		ringstationConfigInfo.AwardFightWin = ((dr["AwardFightWin"] == null) ? "" : dr["AwardFightWin"].ToString());
		ringstationConfigInfo.AwardFightLost = ((dr["AwardFightLost"] == null) ? "" : dr["AwardFightLost"].ToString());
		ringstationConfigInfo.ChampionText = ((dr["ChampionText"] == null) ? "" : dr["ChampionText"].ToString());
		ringstationConfigInfo.IsFirstUpdateRank = (bool)dr["IsFirstUpdateRank"];
		return ringstationConfigInfo;
	}

	public bool AddRingstationBattleField(RingstationBattleFieldInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[7]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@DareFlag", info.DareFlag),
				new SqlParameter("@UserName", info.UserName),
				new SqlParameter("@SuccessFlag", info.SuccessFlag),
				new SqlParameter("@Level", info.Level),
				new SqlParameter("@BattleTime", info.BattleTime)
			};
			array[0].Direction = ParameterDirection.Output;
			result = db.RunProcedure("SP_Ringstation_Battle_Field_Add", array);
			info.ID = (int)array[0].Value;
			info.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_Ringstation_Battle_Field_Add", exception);
			}
		}
		return result;
	}

	public RingstationBattleFieldInfo[] GetAllRingstationBattleFieldByID(int ID)
	{
		List<RingstationBattleFieldInfo> list = new List<RingstationBattleFieldInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@ID", SqlDbType.Int, 4)
			};
			array[0].Value = ID;
			db.GetReader(ref ResultDataReader, "SP_Ringstation_Battle_Field_Single", array);
			while (ResultDataReader.Read())
			{
				list.Add(InitRingstationBattleFieldInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("InitRingstationBattleFieldInfo Single", exception);
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

	public RingstationBattleFieldInfo[] GetAllRingstationBattleField()
	{
		List<RingstationBattleFieldInfo> list = new List<RingstationBattleFieldInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Ringstation_Battle_Field_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitRingstationBattleFieldInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("InitRingstationBattleFieldInfo All", exception);
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

	public RingstationBattleFieldInfo InitRingstationBattleFieldInfo(SqlDataReader dr)
	{
		RingstationBattleFieldInfo ringstationBattleFieldInfo = new RingstationBattleFieldInfo();
		ringstationBattleFieldInfo.ID = (int)dr["ID"];
		ringstationBattleFieldInfo.UserID = (int)dr["UserID"];
		ringstationBattleFieldInfo.DareFlag = (bool)dr["DareFlag"];
		ringstationBattleFieldInfo.UserName = ((dr["UserName"] == null) ? "" : dr["UserName"].ToString());
		ringstationBattleFieldInfo.SuccessFlag = (bool)dr["SuccessFlag"];
		ringstationBattleFieldInfo.Level = (int)dr["Level"];
		ringstationBattleFieldInfo.BattleTime = (DateTime)dr["BattleTime"];
		return ringstationBattleFieldInfo;
	}

	public bool AddUserRingStation(UserRingStationInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[14]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@Rank", info.Rank),
				new SqlParameter("@WeaponID", info.WeaponID),
				new SqlParameter("@signMsg", info.signMsg),
				new SqlParameter("@ChallengeNum", info.ChallengeNum),
				new SqlParameter("@buyCount", info.buyCount),
				new SqlParameter("@Total", info.Total),
				new SqlParameter("@ChallengeTime", info.ChallengeTime),
				new SqlParameter("@LastDate", info.LastDate),
				new SqlParameter("@BaseDamage", info.BaseDamage),
				new SqlParameter("@BaseGuard", info.BaseGuard),
				new SqlParameter("@BaseEnergy", info.BaseEnergy),
				new SqlParameter("@ReardEnable", info.ReardEnable)
			};
			array[0].Direction = ParameterDirection.Output;
			result = db.RunProcedure("SP_Sys_User_RingStation_Add", array);
			info.ID = (int)array[0].Value;
			info.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_Sys_User_RingStation_Add", exception);
			}
		}
		return result;
	}

	public bool UpdateUserRingStation(UserRingStationInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[14]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@Rank", info.Rank),
				new SqlParameter("@WeaponID", info.WeaponID),
				new SqlParameter("@signMsg", info.signMsg),
				new SqlParameter("@ChallengeNum", info.ChallengeNum),
				new SqlParameter("@buyCount", info.buyCount),
				new SqlParameter("@Total", info.Total),
				new SqlParameter("@ChallengeTime", info.ChallengeTime),
				new SqlParameter("@LastDate", info.LastDate),
				new SqlParameter("@BaseDamage", info.BaseDamage),
				new SqlParameter("@BaseGuard", info.BaseGuard),
				new SqlParameter("@BaseEnergy", info.BaseEnergy),
				new SqlParameter("@ReardEnable", info.ReardEnable)
			};
			result = db.RunProcedure("SP_Sys_User_RingStation_Update", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_Sys_User_RingStation_Update", exception);
			}
		}
		return result;
	}

	public UserRingStationInfo GetAllUserRingStationByID(int ID)
	{
		List<UserRingStationInfo> list = new List<UserRingStationInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = ID;
			db.GetReader(ref ResultDataReader, "SP_Sys_User_RingStation_Single", array);
			while (ResultDataReader.Read())
			{
				list.Add(InitUserRingStationInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_Sys_User_RingStation_Single", exception);
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

	public UserRingStationInfo[] GetAllUserRingStation()
	{
		List<UserRingStationInfo> list = new List<UserRingStationInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Sys_User_RingStation_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitUserRingStationInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_Sys_User_RingStation_All", exception);
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

	public UserRingStationInfo InitUserRingStationInfo(SqlDataReader dr)
	{
		UserRingStationInfo userRingStationInfo = new UserRingStationInfo();
		userRingStationInfo.ID = (int)dr["ID"];
		userRingStationInfo.UserID = (int)dr["UserID"];
		userRingStationInfo.Rank = (int)dr["Rank"];
		userRingStationInfo.WeaponID = (int)dr["WeaponID"];
		userRingStationInfo.signMsg = ((dr["signMsg"] == null) ? "" : dr["signMsg"].ToString());
		userRingStationInfo.ChallengeNum = (int)dr["ChallengeNum"];
		userRingStationInfo.buyCount = (int)dr["buyCount"];
		userRingStationInfo.Total = (int)dr["Total"];
		userRingStationInfo.ChallengeTime = (DateTime)dr["ChallengeTime"];
		userRingStationInfo.LastDate = (DateTime)dr["LastDate"];
		userRingStationInfo.BaseDamage = (int)dr["BaseDamage"];
		userRingStationInfo.BaseGuard = (int)dr["BaseGuard"];
		userRingStationInfo.BaseEnergy = (int)dr["BaseEnergy"];
		return userRingStationInfo;
	}

	public bool AddUsersPetForm(UsersPetFormInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[4]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@PetsID", info.PetsID),
				new SqlParameter("@ActivePets", info.ActivePets)
			};
			array[0].Direction = ParameterDirection.Output;
			result = db.RunProcedure("SP_Sys_User_Pet_Form_Add", array);
			info.ID = (int)array[0].Value;
			info.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_Sys_User_Pet_Form_Add", exception);
			}
		}
		return result;
	}

	public bool UpdateUsersPetForm(UsersPetFormInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[4]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@PetsID", info.PetsID),
				new SqlParameter("@ActivePets", info.ActivePets)
			};
			result = db.RunProcedure("SP_Sys_User_Pet_Form_Update", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_Sys_User_Pet_Form_Update", exception);
			}
		}
		return result;
	}

	public UsersPetFormInfo GetAllUsersPetFormByID(int ID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = ID;
			db.GetReader(ref ResultDataReader, "SP_Sys_User_Pet_Form_All", array);
			if (ResultDataReader.Read())
			{
				return InitUsersPetFormInfo(ResultDataReader);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("InitUsersPetFormInfo", exception);
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

	public UsersPetFormInfo InitUsersPetFormInfo(SqlDataReader dr)
	{
		UsersPetFormInfo usersPetFormInfo = new UsersPetFormInfo();
		usersPetFormInfo.ID = (int)dr["ID"];
		usersPetFormInfo.UserID = (int)dr["UserID"];
		usersPetFormInfo.PetsID = (int)dr["PetsID"];
		usersPetFormInfo.ActivePets = ((dr["ActivePets"] == null) ? "" : dr["ActivePets"].ToString());
		return usersPetFormInfo;
	}

	public EatPetsInfo GetAllEatPetsByID(int ID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@ID", SqlDbType.Int, 4)
			};
			array[0].Value = ID;
			db.GetReader(ref ResultDataReader, "SP_Sys_Eat_Pets_All", array);
			if (ResultDataReader.Read())
			{
				return InitEatPetsInfo(ResultDataReader);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("InitEatPetsInfo", exception);
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

	public EatPetsInfo InitEatPetsInfo(SqlDataReader dr)
	{
		EatPetsInfo eatPetsInfo = new EatPetsInfo();
		eatPetsInfo.ID = (int)dr["ID"];
		eatPetsInfo.UserID = (int)dr["UserID"];
		eatPetsInfo.weaponExp = (int)dr["weaponExp"];
		eatPetsInfo.weaponLevel = (int)dr["weaponLevel"];
		eatPetsInfo.clothesExp = (int)dr["clothesExp"];
		eatPetsInfo.clothesLevel = (int)dr["clothesLevel"];
		eatPetsInfo.hatExp = (int)dr["hatExp"];
		eatPetsInfo.hatLevel = (int)dr["hatLevel"];
		return eatPetsInfo;
	}

	public bool UpdateEatPets(EatPetsInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[8]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@weaponExp", info.weaponExp),
				new SqlParameter("@weaponLevel", info.weaponLevel),
				new SqlParameter("@clothesExp", info.clothesExp),
				new SqlParameter("@clothesLevel", info.clothesLevel),
				new SqlParameter("@hatExp", info.hatExp),
				new SqlParameter("@hatLevel", info.hatLevel)
			};
			result = db.RunProcedure("SP_Sys_Eat_Pets_Update", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_Sys_Eat_Pets_Update", exception);
			}
		}
		return result;
	}

	public bool AddEatPets(EatPetsInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[8]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@weaponExp", info.weaponExp),
				new SqlParameter("@weaponLevel", info.weaponLevel),
				new SqlParameter("@clothesExp", info.clothesExp),
				new SqlParameter("@clothesLevel", info.clothesLevel),
				new SqlParameter("@hatExp", info.hatExp),
				new SqlParameter("@hatLevel", info.hatLevel)
			};
			array[0].Direction = ParameterDirection.Output;
			result = db.RunProcedure("SP_Sys_Eat_Pets_Add", array);
			info.ID = (int)array[0].Value;
			info.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_Sys_Eat_Pets_Add", exception);
			}
		}
		return result;
	}

	public bool AddUsersMagicHouse(UsersMagicHouseInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[17]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@isOpen", info.isOpen),
				new SqlParameter("@isMagicRoomShow", info.isMagicRoomShow),
				new SqlParameter("@magicJuniorLv", info.magicJuniorLv),
				new SqlParameter("@magicJuniorExp", info.magicJuniorExp),
				new SqlParameter("@magicMidLv", info.magicMidLv),
				new SqlParameter("@magicMidExp", info.magicMidExp),
				new SqlParameter("@magicSeniorLv", info.magicSeniorLv),
				new SqlParameter("@magicSeniorExp", info.magicSeniorExp),
				new SqlParameter("@freeGetCount", info.freeGetCount),
				new SqlParameter("@freeGetTime", info.freeGetTime),
				new SqlParameter("@chargeGetCount", info.chargeGetCount),
				new SqlParameter("@chargeGetTime", info.chargeGetTime),
				new SqlParameter("@depotCount", info.depotCount),
				new SqlParameter("@activityWeapons", info.activityWeapons),
				new SqlParameter("@essence", info.essence)
			};
			array[0].Direction = ParameterDirection.Output;
			result = db.RunProcedure("SP_Sys_User_MagicHouse_Add", array);
			info.ID = (int)array[0].Value;
			info.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_Sys_User_MagicHouse_Add", exception);
			}
		}
		return result;
	}

	public bool UpdateUsersMagicHouse(UsersMagicHouseInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[16]
			{
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@isOpen", info.isOpen),
				new SqlParameter("@isMagicRoomShow", info.isMagicRoomShow),
				new SqlParameter("@magicJuniorLv", info.magicJuniorLv),
				new SqlParameter("@magicJuniorExp", info.magicJuniorExp),
				new SqlParameter("@magicMidLv", info.magicMidLv),
				new SqlParameter("@magicMidExp", info.magicMidExp),
				new SqlParameter("@magicSeniorLv", info.magicSeniorLv),
				new SqlParameter("@magicSeniorExp", info.magicSeniorExp),
				new SqlParameter("@freeGetCount", info.freeGetCount),
				new SqlParameter("@freeGetTime", info.freeGetTime),
				new SqlParameter("@chargeGetCount", info.chargeGetCount),
				new SqlParameter("@chargeGetTime", info.chargeGetTime),
				new SqlParameter("@depotCount", info.depotCount),
				new SqlParameter("@activityWeapons", info.activityWeapons),
				new SqlParameter("@essence", info.essence)
			};
			result = db.RunProcedure("SP_Sys_User_MagicHouse_Update", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_Sys_User_MagicHouse_Update", exception);
			}
		}
		return result;
	}

	public UsersMagicHouseInfo GetAllUsersMagicHouseByID(int UserID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_Sys_User_MagicHouse_Single", array);
			if (ResultDataReader.Read())
			{
				return InitUsersMagicHouseInfo(ResultDataReader);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("InitUsersMagicHouseInfo", exception);
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

	public UsersMagicHouseInfo InitUsersMagicHouseInfo(SqlDataReader dr)
	{
		UsersMagicHouseInfo usersMagicHouseInfo = new UsersMagicHouseInfo();
		usersMagicHouseInfo.ID = (int)dr["ID"];
		usersMagicHouseInfo.UserID = (int)dr["UserID"];
		usersMagicHouseInfo.isOpen = (bool)dr["isOpen"];
		usersMagicHouseInfo.isMagicRoomShow = (bool)dr["isMagicRoomShow"];
		usersMagicHouseInfo.magicJuniorLv = (int)dr["magicJuniorLv"];
		usersMagicHouseInfo.magicJuniorExp = (int)dr["magicJuniorExp"];
		usersMagicHouseInfo.magicMidLv = (int)dr["magicMidLv"];
		usersMagicHouseInfo.magicMidExp = (int)dr["magicMidExp"];
		usersMagicHouseInfo.magicSeniorLv = (int)dr["magicSeniorLv"];
		usersMagicHouseInfo.magicSeniorExp = (int)dr["magicSeniorExp"];
		usersMagicHouseInfo.freeGetCount = (int)dr["freeGetCount"];
		usersMagicHouseInfo.freeGetTime = (DateTime)dr["freeGetTime"];
		usersMagicHouseInfo.chargeGetCount = (int)dr["chargeGetCount"];
		usersMagicHouseInfo.chargeGetTime = (DateTime)dr["chargeGetTime"];
		usersMagicHouseInfo.depotCount = (int)dr["depotCount"];
		usersMagicHouseInfo.activityWeapons = ((dr["activityWeapons"] == null) ? "" : dr["activityWeapons"].ToString());
		usersMagicHouseInfo.essence = (int)dr["essence"];
		return usersMagicHouseInfo;
	}

	public DiceDataInfo GetSingleDiceData(int UserID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_GetSingle_DiceData", array);
			if (ResultDataReader.Read())
			{
				DiceDataInfo diceDataInfo = new DiceDataInfo();
				diceDataInfo.ID = (int)ResultDataReader["ID"];
				diceDataInfo.UserID = (int)ResultDataReader["UserID"];
				diceDataInfo.LuckIntegral = (int)ResultDataReader["LuckIntegral"];
				diceDataInfo.LuckIntegralLevel = (int)ResultDataReader["LuckIntegralLevel"];
				diceDataInfo.Level = (int)ResultDataReader["Level"];
				diceDataInfo.FreeCount = (int)ResultDataReader["FreeCount"];
				diceDataInfo.CurrentPosition = (int)ResultDataReader["CurrentPosition"];
				diceDataInfo.UserFirstCell = (bool)ResultDataReader["UserFirstCell"];
				diceDataInfo.AwardArray = ((ResultDataReader["AwardArray"] == null) ? "" : ResultDataReader["AwardArray"].ToString());
				return diceDataInfo;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_GetSingleDiceData", exception);
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

	public bool AddDiceData(DiceDataInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[10]
			{
				new SqlParameter("@ID", info.ID),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@UserID", info.UserID);
			array[2] = new SqlParameter("@LuckIntegral", info.LuckIntegral);
			array[3] = new SqlParameter("@LuckIntegralLevel", info.LuckIntegralLevel);
			array[4] = new SqlParameter("@Level", info.Level);
			array[5] = new SqlParameter("@FreeCount", info.FreeCount);
			array[6] = new SqlParameter("@CurrentPosition", info.CurrentPosition);
			array[7] = new SqlParameter("@UserFirstCell", info.UserFirstCell);
			array[8] = new SqlParameter("@AwardArray", info.AwardArray);
			array[9] = new SqlParameter("@Result", SqlDbType.Int);
			array[9].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_DiceData_Add", array);
			result = (int)array[9].Value == 0;
			info.ID = (int)array[0].Value;
			info.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_DiceData_Add", exception);
			}
		}
		finally
		{
		}
		return result;
	}

	public bool UpdateDiceData(DiceDataInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[10]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@LuckIntegral", info.LuckIntegral),
				new SqlParameter("@LuckIntegralLevel", info.LuckIntegralLevel),
				new SqlParameter("@Level", info.Level),
				new SqlParameter("@FreeCount", info.FreeCount),
				new SqlParameter("@CurrentPosition", info.CurrentPosition),
				new SqlParameter("@UserFirstCell", info.UserFirstCell),
				new SqlParameter("@AwardArray", info.AwardArray),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[9].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Update_DiceData", array);
			result = (int)array[9].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_Update_DiceData", exception);
			}
		}
		return result;
	}

	public bool AddStore(ItemInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[14]
			{
				new SqlParameter("@ItemID", item.ItemID),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@UserID", item.UserID);
			array[2] = new SqlParameter("@TemplateID", item.Template.TemplateID);
			array[3] = new SqlParameter("@Place", item.Place);
			array[4] = new SqlParameter("@AgilityCompose", item.AgilityCompose);
			array[5] = new SqlParameter("@AttackCompose", item.AttackCompose);
			array[6] = new SqlParameter("@BeginDate", item.BeginDate);
			array[7] = new SqlParameter("@Color", (item.Color == null) ? "" : item.Color);
			array[8] = new SqlParameter("@Count", item.Count);
			array[9] = new SqlParameter("@DefendCompose", item.DefendCompose);
			array[10] = new SqlParameter("@IsBinds", item.IsBinds);
			array[11] = new SqlParameter("@IsExist", item.IsExist);
			array[12] = new SqlParameter("@IsJudge", item.IsJudge);
			array[13] = new SqlParameter("@LuckCompose", item.LuckCompose);
			result = db.RunProcedure("SP_Users_Items_Add", array);
			item.ItemID = (int)array[0].Value;
			item.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool UpateStore(ItemInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[14]
			{
				new SqlParameter("@ItemID", item.ItemID),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@UserID", item.UserID);
			array[2] = new SqlParameter("@TemplateID", item.Template.TemplateID);
			array[3] = new SqlParameter("@Place", item.Place);
			array[4] = new SqlParameter("@AgilityCompose", item.AgilityCompose);
			array[5] = new SqlParameter("@AttackCompose", item.AttackCompose);
			array[6] = new SqlParameter("@BeginDate", item.BeginDate);
			array[7] = new SqlParameter("@Color", (item.Color == null) ? "" : item.Color);
			array[8] = new SqlParameter("@Count", item.Count);
			array[9] = new SqlParameter("@DefendCompose", item.DefendCompose);
			array[10] = new SqlParameter("@IsBinds", item.IsBinds);
			array[11] = new SqlParameter("@IsExist", item.IsExist);
			array[12] = new SqlParameter("@IsJudge", item.IsJudge);
			array[13] = new SqlParameter("@LuckCompose", item.LuckCompose);
			result = db.RunProcedure("SP_Users_Items_Add", array);
			item.ItemID = (int)array[0].Value;
			item.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool UpdateBuyStore(int storeId)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@StoreID", storeId)
			};
			result = db.RunProcedure("SP_Update_Buy_Store", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_Update_Buy_Store", exception);
			}
		}
		finally
		{
		}
		return result;
	}

	public ConsortiaUserInfo[] GetAllMemberByConsortia(int ConsortiaID)
	{
		List<ConsortiaUserInfo> list = new List<ConsortiaUserInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@ConsortiaID", SqlDbType.Int, 4)
			};
			array[0].Value = ConsortiaID;
			db.GetReader(ref ResultDataReader, "SP_Consortia_Users_All", array);
			while (ResultDataReader.Read())
			{
				list.Add(InitConsortiaUserInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
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

	public ConsortiaUserInfo InitConsortiaUserInfo(SqlDataReader dr)
	{
		ConsortiaUserInfo consortiaUserInfo = new ConsortiaUserInfo();
		consortiaUserInfo.ID = (int)dr["ID"];
		consortiaUserInfo.ConsortiaID = (int)dr["ConsortiaID"];
		consortiaUserInfo.DutyID = (int)dr["DutyID"];
		consortiaUserInfo.DutyName = dr["DutyName"].ToString();
		consortiaUserInfo.IsExist = (bool)dr["IsExist"];
		consortiaUserInfo.RatifierID = (int)dr["RatifierID"];
		consortiaUserInfo.RatifierName = dr["RatifierName"].ToString();
		consortiaUserInfo.Remark = dr["Remark"].ToString();
		consortiaUserInfo.UserID = (int)dr["UserID"];
		consortiaUserInfo.UserName = dr["UserName"].ToString();
		consortiaUserInfo.Grade = (int)dr["Grade"];
		consortiaUserInfo.GP = Convert.ToInt64(dr["GP"]);
		consortiaUserInfo.Repute = (int)dr["Repute"];
		consortiaUserInfo.State = (int)dr["State"];
		consortiaUserInfo.Right = (int)dr["Right"];
		consortiaUserInfo.Offer = (int)dr["Offer"];
		consortiaUserInfo.Colors = dr["Colors"].ToString();
		consortiaUserInfo.Style = dr["Style"].ToString();
		consortiaUserInfo.Hide = (int)dr["Hide"];
		consortiaUserInfo.Skin = ((dr["Skin"] == null) ? "" : consortiaUserInfo.Skin);
		consortiaUserInfo.Level = (int)dr["Level"];
		consortiaUserInfo.LastDate = (DateTime)dr["LastDate"];
		consortiaUserInfo.Sex = (bool)dr["Sex"];
		consortiaUserInfo.IsBanChat = (bool)dr["IsBanChat"];
		consortiaUserInfo.Win = (int)dr["Win"];
		consortiaUserInfo.Total = (int)dr["Total"];
		consortiaUserInfo.Escape = (int)dr["Escape"];
		consortiaUserInfo.RichesOffer = (int)dr["RichesOffer"];
		consortiaUserInfo.RichesRob = (int)dr["RichesRob"];
		consortiaUserInfo.LoginName = ((dr["LoginName"] == null) ? "" : dr["LoginName"].ToString());
		consortiaUserInfo.Nimbus = (int)dr["Nimbus"];
		consortiaUserInfo.FightPower = (int)dr["FightPower"];
		consortiaUserInfo.typeVIP = Convert.ToByte(dr["typeVIP"]);
		consortiaUserInfo.VIPLevel = (int)dr["VIPLevel"];
		return consortiaUserInfo;
	}

	public bool ActivePlayer(ref PlayerInfo player, string userName, string passWord, bool sex, int gold, int money, string IP, string site)
	{
		bool result = false;
		try
		{
			player = new PlayerInfo();
			player.Agility = 0;
			player.Attack = 0;
			player.Colors = ",,,,,,";
			player.Skin = "";
			player.ConsortiaID = 0;
			player.Defence = 0;
			player.Gold = 0;
			player.GP = 1L;
			player.Grade = 1;
			player.ID = 0;
			player.Luck = 0;
			player.Money = 0;
			player.NickName = "";
			player.Sex = sex;
			player.State = 0;
			player.Style = ",,,,,,";
			player.Hide = 1111111111;
			SqlParameter[] array = new SqlParameter[21];
			array[0] = new SqlParameter("@UserID", SqlDbType.Int);
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@Attack", player.Attack);
			array[2] = new SqlParameter("@Colors", (player.Colors == null) ? "" : player.Colors);
			array[3] = new SqlParameter("@ConsortiaID", player.ConsortiaID);
			array[4] = new SqlParameter("@Defence", player.Defence);
			array[5] = new SqlParameter("@Gold", player.Gold);
			array[6] = new SqlParameter("@GP", player.GP);
			array[7] = new SqlParameter("@Grade", player.Grade);
			array[8] = new SqlParameter("@Luck", player.Luck);
			array[9] = new SqlParameter("@Money", player.Money);
			array[10] = new SqlParameter("@Style", (player.Style == null) ? "" : player.Style);
			array[11] = new SqlParameter("@Agility", player.Agility);
			array[12] = new SqlParameter("@State", player.State);
			array[13] = new SqlParameter("@UserName", userName);
			array[14] = new SqlParameter("@PassWord", passWord);
			array[15] = new SqlParameter("@Sex", sex);
			array[16] = new SqlParameter("@Hide", player.Hide);
			array[17] = new SqlParameter("@ActiveIP", IP);
			array[18] = new SqlParameter("@Skin", (player.Skin == null) ? "" : player.Skin);
			array[19] = new SqlParameter("@Result", SqlDbType.Int);
			array[19].Direction = ParameterDirection.ReturnValue;
			array[20] = new SqlParameter("@Site", site);
			result = db.RunProcedure("SP_Users_Active", array);
			player.ID = (int)array[0].Value;
			result = (int)array[19].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool RegisterPlayer(string userName, string passWord, string nickName, string bStyle, string gStyle, string armColor, string hairColor, string faceColor, string clothColor, string hatColor, int sex, ref string msg, int validDate)
	{
		bool result = false;
		try
		{
			string[] array = bStyle.Split(',');
			string[] array2 = gStyle.Split(',');
			SqlParameter[] array3 = new SqlParameter[21]
			{
				new SqlParameter("@UserName", userName),
				new SqlParameter("@PassWord", passWord),
				new SqlParameter("@NickName", nickName),
				new SqlParameter("@BArmID", int.Parse(array[0])),
				new SqlParameter("@BHairID", int.Parse(array[1])),
				new SqlParameter("@BFaceID", int.Parse(array[2])),
				new SqlParameter("@BClothID", int.Parse(array[3])),
				new SqlParameter("@BHatID", int.Parse(array[4])),
				new SqlParameter("@GArmID", int.Parse(array2[0])),
				new SqlParameter("@GHairID", int.Parse(array2[1])),
				new SqlParameter("@GFaceID", int.Parse(array2[2])),
				new SqlParameter("@GClothID", int.Parse(array2[3])),
				new SqlParameter("@GHatID", int.Parse(array2[4])),
				new SqlParameter("@ArmColor", armColor),
				new SqlParameter("@HairColor", hairColor),
				new SqlParameter("@FaceColor", faceColor),
				new SqlParameter("@ClothColor", clothColor),
				new SqlParameter("@HatColor", clothColor),
				new SqlParameter("@Sex", sex),
				new SqlParameter("@StyleDate", validDate),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array3[20].Direction = ParameterDirection.ReturnValue;
			result = db.RunProcedure("SP_Users_RegisterNotValidate", array3);
			int num = (int)array3[20].Value;
			result = num == 0;
			switch (num)
			{
			case 2:
				msg = LanguageMgr.GetTranslation("PlayerBussiness.RegisterPlayer.Msg2");
				break;
			case 3:
				msg = LanguageMgr.GetTranslation("PlayerBussiness.RegisterPlayer.Msg3");
				break;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool RenameNick(string userName, string nickName, string newNickName, ref string msg)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[4]
			{
				new SqlParameter("@UserName", userName),
				new SqlParameter("@NickName", nickName),
				new SqlParameter("@NewNickName", newNickName),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[3].Direction = ParameterDirection.ReturnValue;
			result = db.RunProcedure("SP_Users_RenameNick", array);
			int num = (int)array[3].Value;
			result = num == 0;
			int num2 = num;
			int num3 = num2;
			if ((uint)(num3 - 4) <= 1u)
			{
				msg = LanguageMgr.GetTranslation("PlayerBussiness.RenameNick.Msg4");
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("RenameNick", exception);
			}
		}
		return result;
	}

	public bool ChangeSex(int UserId, bool newSex)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[3]
			{
				new SqlParameter("@UserId", UserId),
				new SqlParameter("@Sex", newSex),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[2].Direction = ParameterDirection.ReturnValue;
			result = db.RunProcedure("SP_Users_ChangSexByCard", array);
			int num = (int)array[2].Value;
			result = num == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_Users_ChangSexByCard ", exception);
			}
		}
		return result;
	}

	public bool RenameNick(string userName, string nickName, string newNickName)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[4]
			{
				new SqlParameter("@UserName", userName),
				new SqlParameter("@NickName", nickName),
				new SqlParameter("@NewNickName", newNickName),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[3].Direction = ParameterDirection.ReturnValue;
			result = db.RunProcedure("SP_Users_RenameByCard", array);
			result = (int)array[3].Value == 0;
		}
		catch (Exception ex)
		{
			Exception exception = ex;
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("RenameNick", exception);
			}
		}
		return result;
	}

	public bool DisableUser(string userName, bool isExit)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[3]
			{
				new SqlParameter("@UserName", userName),
				new SqlParameter("@IsExist", isExit),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[2].Direction = ParameterDirection.ReturnValue;
			result = db.RunProcedure("SP_Disable_User", array);
			if ((int)array[2].Value == 0)
			{
				result = true;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("DisableUser", exception);
			}
		}
		return result;
	}

	public bool UpdatePassWord(int userID, string password)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[2]
			{
				new SqlParameter("@UserID", userID),
				new SqlParameter("@Password", password)
			};
			result = db.RunProcedure("SP_Users_UpdatePassword", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool UpdatePasswordInfo(int userID, string PasswordQuestion1, string PasswordAnswer1, string PasswordQuestion2, string PasswordAnswer2, int Count)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[6]
			{
				new SqlParameter("@UserID", userID),
				new SqlParameter("@PasswordQuestion1", PasswordQuestion1),
				new SqlParameter("@PasswordAnswer1", PasswordAnswer1),
				new SqlParameter("@PasswordQuestion2", PasswordQuestion2),
				new SqlParameter("@PasswordAnswer2", PasswordAnswer2),
				new SqlParameter("@FailedPasswordAttemptCount", Count)
			};
			result = db.RunProcedure("SP_Users_Password_Add", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public void GetPasswordInfo(int userID, ref string PasswordQuestion1, ref string PasswordAnswer1, ref string PasswordQuestion2, ref string PasswordAnswer2, ref int Count)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@UserID", userID)
			};
			db.GetReader(ref ResultDataReader, "SP_Users_PasswordInfo", sqlParameters);
			while (ResultDataReader.Read())
			{
				PasswordQuestion1 = ((ResultDataReader["PasswordQuestion1"] == null) ? "" : ResultDataReader["PasswordQuestion1"].ToString());
				PasswordAnswer1 = ((ResultDataReader["PasswordAnswer1"] == null) ? "" : ResultDataReader["PasswordAnswer1"].ToString());
				PasswordQuestion2 = ((ResultDataReader["PasswordQuestion2"] == null) ? "" : ResultDataReader["PasswordQuestion2"].ToString());
				PasswordAnswer2 = ((ResultDataReader["PasswordAnswer2"] == null) ? "" : ResultDataReader["PasswordAnswer2"].ToString());
				DateTime dateTime = (DateTime)ResultDataReader["LastFindDate"];
				if (dateTime == DateTime.Today)
				{
					Count = (int)ResultDataReader["FailedPasswordAttemptCount"];
				}
				else
				{
					Count = 5;
				}
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
	}

	public bool UpdatePasswordTwo(int userID, string passwordTwo)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[2]
			{
				new SqlParameter("@UserID", userID),
				new SqlParameter("@PasswordTwo", passwordTwo)
			};
			result = db.RunProcedure("SP_Users_UpdatePasswordTwo", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public PlayerInfo[] GetUserLoginList(string userName)
	{
		List<PlayerInfo> list = new List<PlayerInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserName", SqlDbType.NVarChar, 200)
			};
			array[0].Value = userName;
			db.GetReader(ref ResultDataReader, "SP_Users_LoginList", array);
			while (ResultDataReader.Read())
			{
				list.Add(InitPlayerInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
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

	public PlayerInfo[] GetRandomPlayers()
	{
		List<PlayerInfo> list = new List<PlayerInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Users_GetRandom");
			while (ResultDataReader.Read())
			{
				list.Add(InitPlayerInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
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

	public PlayerInfo LoginGame(string username, ref int isFirst, ref bool isExist, ref bool isError, bool firstValidate, ref DateTime forbidDate, string nickname)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[4]
			{
				new SqlParameter("@UserName", username),
				new SqlParameter("@Password", ""),
				new SqlParameter("@FirstValidate", firstValidate),
				new SqlParameter("@Nickname", nickname)
			};
			db.GetReader(ref ResultDataReader, "SP_Users_LoginWeb", sqlParameters);
			if (ResultDataReader.Read())
			{
				isFirst = (int)ResultDataReader["IsFirst"];
				isExist = (bool)ResultDataReader["IsExist"];
				forbidDate = (DateTime)ResultDataReader["ForbidDate"];
				if (isFirst > 1)
				{
					isFirst--;
				}
				return InitPlayerInfo(ResultDataReader);
			}
		}
		catch (Exception exception)
		{
			isError = true;
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
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

	public PlayerInfo LoginGame(string username, string password)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[2]
			{
				new SqlParameter("@UserName", username),
				new SqlParameter("@Password", password)
			};
			db.GetReader(ref ResultDataReader, "SP_Users_Login", sqlParameters);
			if (ResultDataReader.Read())
			{
				return InitPlayerInfo(ResultDataReader);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
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

	public PlayerInfo ReLoadPlayer(int ID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@ID", ID)
			};
			db.GetReader(ref ResultDataReader, "SP_Users_Reload", sqlParameters);
			if (ResultDataReader.Read())
			{
				return InitPlayerInfo(ResultDataReader);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
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

	public bool UpdatePlayer(PlayerInfo player)
	{
		bool flag = false;
		try
		{
			if (player.Grade < 1)
			{
				return false;
			}
			SqlParameter[] array = new SqlParameter[127];
			array[0] = new SqlParameter("@UserID", player.ID);
			array[1] = new SqlParameter("@Attack", player.Attack);
			array[2] = new SqlParameter("@Colors", (player.Colors == null) ? "" : player.Colors);
			array[3] = new SqlParameter("@ConsortiaID", player.ConsortiaID);
			array[4] = new SqlParameter("@Defence", player.Defence);
			array[5] = new SqlParameter("@Gold", player.Gold);
			array[6] = new SqlParameter("@GP", player.GP);
			array[7] = new SqlParameter("@Grade", player.Grade);
			array[8] = new SqlParameter("@Luck", player.Luck);
			array[9] = new SqlParameter("@Money", player.Money);
			array[10] = new SqlParameter("@Style", (player.Style == null) ? "" : player.Style);
			array[11] = new SqlParameter("@Agility", player.Agility);
			array[12] = new SqlParameter("@State", player.State);
			array[13] = new SqlParameter("@Hide", player.Hide);
			array[14] = new SqlParameter("@ExpendDate", (!player.ExpendDate.HasValue) ? "" : player.ExpendDate.ToString());
			array[15] = new SqlParameter("@Win", player.Win);
			array[16] = new SqlParameter("@Total", player.Total);
			array[17] = new SqlParameter("@Escape", player.Escape);
			array[18] = new SqlParameter("@Skin", (player.Skin == null) ? "" : player.Skin);
			array[19] = new SqlParameter("@Offer", player.Offer);
			array[20] = new SqlParameter("@AntiAddiction", player.AntiAddiction);
			array[20].Direction = ParameterDirection.InputOutput;
			array[21] = new SqlParameter("@Result", SqlDbType.Int);
			array[21].Direction = ParameterDirection.ReturnValue;
			array[22] = new SqlParameter("@RichesOffer", player.RichesOffer);
			array[23] = new SqlParameter("@RichesRob", player.RichesRob);
			array[24] = new SqlParameter("@CheckCount", player.CheckCount);
			array[24].Direction = ParameterDirection.InputOutput;
			array[25] = new SqlParameter("@MarryInfoID", player.MarryInfoID);
			array[26] = new SqlParameter("@DayLoginCount", player.DayLoginCount);
			array[27] = new SqlParameter("@Nimbus", player.Nimbus);
			array[28] = new SqlParameter("@LastAward", player.LastAward);
			array[29] = new SqlParameter("@GiftToken", player.DDTMoney);
			array[30] = new SqlParameter("@QuestSite", player.QuestSite);
			array[31] = new SqlParameter("@PvePermission", player.PvePermission);
			array[32] = new SqlParameter("@FightPower", player.FightPower);
			array[33] = new SqlParameter("@AnswerSite", player.AnswerSite);
			array[34] = new SqlParameter("@LastAuncherAward", player.LastAward);
			array[35] = new SqlParameter("@hp", player.hp);
			array[36] = new SqlParameter("@ChatCount", player.ChatCount);
			array[37] = new SqlParameter("@SpaPubGoldRoomLimit", player.SpaPubGoldRoomLimit);
			array[38] = new SqlParameter("@LastSpaDate", player.LastSpaDate);
			array[39] = new SqlParameter("@FightLabPermission", player.FightLabPermission);
			array[40] = new SqlParameter("@SpaPubMoneyRoomLimit", player.SpaPubMoneyRoomLimit);
			array[41] = new SqlParameter("@IsInSpaPubGoldToday", player.IsInSpaPubGoldToday);
			array[42] = new SqlParameter("@IsInSpaPubMoneyToday", player.IsInSpaPubMoneyToday);
			array[43] = new SqlParameter("@AchievementPoint", player.AchievementPoint);
			array[44] = new SqlParameter("@LastWeekly", player.LastWeekly);
			array[45] = new SqlParameter("@LastWeeklyVersion", player.LastWeeklyVersion);
			array[46] = new SqlParameter("@GiftGp", player.GiftGp);
			array[47] = new SqlParameter("@GiftLevel", player.GiftLevel);
			array[48] = new SqlParameter("@IsOpenGift", player.IsOpenGift);
			array[49] = new SqlParameter("@WeaklessGuildProgressStr", player.WeaklessGuildProgressStr);
			array[50] = new SqlParameter("@IsOldPlayer", player.IsOldPlayer);
			array[51] = new SqlParameter("@VIPLevel", player.VIPLevel);
			array[52] = new SqlParameter("@VIPExp", player.VIPExp);
			array[53] = new SqlParameter("@Score", player.Score);
			array[54] = new SqlParameter("@OptionOnOff", player.OptionOnOff);
			array[55] = new SqlParameter("@isOldPlayerHasValidEquitAtLogin", player.isOldPlayerHasValidEquitAtLogin);
			array[56] = new SqlParameter("@badLuckNumber", player.badLuckNumber);
			array[57] = new SqlParameter("@luckyNum", player.luckyNum);
			array[58] = new SqlParameter("@lastLuckyNumDate", player.lastLuckyNumDate.ToString());
			array[59] = new SqlParameter("@lastLuckNum", player.lastLuckNum);
			array[60] = new SqlParameter("@CardSoul", player.CardSoul);
			array[61] = new SqlParameter("@uesedFinishTime", player.uesedFinishTime);
			array[62] = new SqlParameter("@totemId", player.totemId);
			array[63] = new SqlParameter("@damageScores", player.damageScores);
			array[64] = new SqlParameter("@petScore", player.petScore);
			array[65] = new SqlParameter("@IsShowConsortia", player.IsShowConsortia);
			array[66] = new SqlParameter("@LastRefreshPet", player.LastRefreshPet.ToString());
			array[67] = new SqlParameter("@GetSoulCount", player.GetSoulCount);
			array[68] = new SqlParameter("@isFirstDivorce", player.isFirstDivorce);
			array[69] = new SqlParameter("@needGetBoxTime", player.needGetBoxTime);
			array[70] = new SqlParameter("@myScore", player.myScore);
			array[71] = new SqlParameter("@TimeBox", player.TimeBox.ToString());
			array[72] = new SqlParameter("@IsFistGetPet", player.IsFistGetPet);
			array[73] = new SqlParameter("@MaxBuyHonor", player.MaxBuyHonor);
			array[74] = new SqlParameter("@Medal", player.medal);
			array[75] = new SqlParameter("@myHonor", player.myHonor);
			array[76] = new SqlParameter("@LeagueMoney", player.LeagueMoney);
			array[77] = new SqlParameter("@Honor", player.Honor);
			array[78] = new SqlParameter("@necklaceExp", player.necklaceExp);
			array[79] = new SqlParameter("@necklaceExpAdd", player.necklaceExpAdd);
			array[80] = new SqlParameter("@hardCurrency", player.hardCurrency);
			array[81] = new SqlParameter("@accumulativeLoginDays", player.accumulativeLoginDays);
			array[82] = new SqlParameter("@accumulativeAwardDays", player.accumulativeAwardDays);
			array[83] = new SqlParameter("@PveEpicPermission", player.PveEpicPermission);
			array[84] = new SqlParameter("@evolutionGrade", player.evolutionGrade);
			array[85] = new SqlParameter("@evolutionExp", player.evolutionExp);
			array[86] = new SqlParameter("@MagicAttack", player.MagicAttack);
			array[87] = new SqlParameter("@MagicDefence", player.MagicDefence);
			array[88] = new SqlParameter("@honorId", player.honorId);
			array[89] = new SqlParameter("@eggNum", player.eggNum);
			array[90] = new SqlParameter("@flowerNum", player.flowerNum);
			array[91] = new SqlParameter("@isClicked", player.isClicked);
			array[92] = new SqlParameter("@horseRaceCanRaceTime", player.horseRaceCanRaceTime);
			array[93] = new SqlParameter("@isAttest", player.isAttest);
			array[94] = new SqlParameter("@ImagePath", player.ImagePath);
			array[95] = new SqlParameter("@SubName", player.SubName);
			array[96] = new SqlParameter("@IsShow", player.IsShow);
			array[97] = new SqlParameter("@SubID", player.SubID);
			array[98] = new SqlParameter("@fineSuitExp", player.fineSuitExp);
			array[99] = new SqlParameter("@BattleCount", player.BattleCount);
			array[100] = new SqlParameter("@RingExp", player.RingExp);
			array[101] = new SqlParameter("@desertEnterCount", player.desertEnterCount);
			array[102] = new SqlParameter("@guardCoreID", player.guardCoreID);
			array[103] = new SqlParameter("@guardCoreGrade", player.guardCoreGrade);
			array[104] = new SqlParameter("@ticketNum", player.ticketNum);
			array[105] = new SqlParameter("@createPlayerDate", player.createPlayerDate);
			array[106] = new SqlParameter("@vipDiscount", player.vipDiscount);
			array[107] = new SqlParameter("@vipDiscountValidity", player.vipDiscountValidity);
			array[108] = new SqlParameter("@stive", player.stive);
			array[109] = new SqlParameter("@jampsCurrency", player.jampsCurrency);
			array[110] = new SqlParameter("@buyStiveNum", player.buyStiveNum);
			array[111] = new SqlParameter("@GhostEquipList", player.GhostEquipList);
			array[112] = new SqlParameter("@BaseAttack", player.BaseAttack);
			array[113] = new SqlParameter("@BaseDefence", player.BaseDefence);
			array[114] = new SqlParameter("@VIPGiftWeekDate", player.VIPGiftWeekDate);
			array[115] = new SqlParameter("@IsVIPOldPlayerScan", player.IsVIPOldPlayerScan);
			array[116] = new SqlParameter("@TotemUpgrade", player.TotemUpgrade);
			array[117] = new SqlParameter("@IsBanChat", player.IsBanChat);
			_ = player.BanChatEndDate;
			array[118] = new SqlParameter("@BanChatEndDate", player.BanChatEndDate);
			array[119] = new SqlParameter("@pntsBattle", player.pntsBattle);
			array[120] = new SqlParameter("@VipIntegral", player.VipIntegral);
			array[121] = new SqlParameter("@VipShop", (player.VipShop == null) ? "" : player.VipShop);
			array[122] = new SqlParameter("@necklaceCastLevel", player.necklaceCastLevel);
			array[123] = new SqlParameter("@necklaceArtificeNeedCount", player.necklaceArtificeNeedCount);
			array[124] = new SqlParameter("@IsHideElf", player.IsHideElf);
			array[125] = new SqlParameter("@LoadingScore", player.LoadingScore);
            array[126] = new SqlParameter("@SlotPet", player.SlotPet);
            db.RunProcedure("SP_Users_Update", array);
			flag = (int)array[21].Value == 0;
			if (flag)
			{
				player.AntiAddiction = (int)array[20].Value;
				player.CheckCount = (int)array[24].Value;
			}
			player.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return flag;
	}

	public bool UpdatePlayerMarry(PlayerInfo player)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[7]
			{
				new SqlParameter("@UserID", player.ID),
				new SqlParameter("@IsMarried", player.IsMarried),
				new SqlParameter("@SpouseID", player.SpouseID),
				new SqlParameter("@SpouseName", player.SpouseName),
				new SqlParameter("@IsCreatedMarryRoom", player.IsCreatedMarryRoom),
				new SqlParameter("@SelfMarryRoomID", player.SelfMarryRoomID),
				new SqlParameter("@IsGotRing", player.IsGotRing)
			};
			result = db.RunProcedure("SP_Users_Marry", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("UpdatePlayerMarry", exception);
			}
		}
		return result;
	}

	public bool UpdatePlayerLastAward(int id, int type)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[2]
			{
				new SqlParameter("@UserID", id),
				new SqlParameter("@Type", type)
			};
			result = db.RunProcedure("SP_Users_LastAward", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("UpdatePlayerAward", exception);
			}
		}
		return result;
	}

	public PlayerInfo InitPlayerInfo(SqlDataReader reader)
	{
		PlayerInfo playerInfo = new PlayerInfo();
		playerInfo.Password = (string)reader["Password"];
		playerInfo.IsConsortia = (bool)reader["IsConsortia"];
		playerInfo.Agility = (int)reader["Agility"];
		playerInfo.Attack = (int)reader["Attack"];
		playerInfo.hp = (int)reader["hp"];
		playerInfo.Colors = ((reader["Colors"] == null) ? "" : reader["Colors"].ToString());
		playerInfo.ConsortiaID = (int)reader["ConsortiaID"];
		playerInfo.Defence = (int)reader["Defence"];
		playerInfo.Gold = (int)reader["Gold"];
		playerInfo.GP = Convert.ToInt64(reader["GP"]);
		playerInfo.Grade = (int)reader["Grade"];
		playerInfo.ID = (int)reader["UserID"];
		playerInfo.Luck = (int)reader["Luck"];
		playerInfo.Money = (int)reader["Money"];
		playerInfo.NickName = ((reader["NickName"] == null) ? "" : ((string)reader["NickName"]));
		playerInfo.Sex = (bool)reader["Sex"];
		playerInfo.State = (int)reader["State"];
		playerInfo.Style = ((reader["Style"] == null) ? "" : reader["Style"].ToString());
		playerInfo.Hide = (int)reader["Hide"];
		playerInfo.Repute = (int)reader["Repute"];
		playerInfo.UserName = ((reader["UserName"] == null) ? "" : reader["UserName"].ToString());
		playerInfo.ConsortiaName = ((reader["ConsortiaName"] == null) ? "" : reader["ConsortiaName"].ToString());
		playerInfo.Offer = (int)reader["Offer"];
		playerInfo.Win = (int)reader["Win"];
		playerInfo.Total = (int)reader["Total"];
		playerInfo.Escape = (int)reader["Escape"];
		playerInfo.Skin = ((reader["Skin"] == null) ? "" : reader["Skin"].ToString());
		playerInfo.IsBanChat = (bool)reader["IsBanChatNew"];
		playerInfo.BanChatEndDate = ((reader["BanChatEndDate"] == DBNull.Value) ? DateTime.Now : ((DateTime)reader["BanChatEndDate"]));
		playerInfo.ReputeOffer = (int)reader["ReputeOffer"];
		playerInfo.ConsortiaRepute = (int)reader["ConsortiaRepute"];
		playerInfo.ConsortiaLevel = (int)reader["ConsortiaLevel"];
		playerInfo.StoreLevel = (int)reader["StoreLevel"];
		playerInfo.ShopLevel = (int)reader["ShopLevel"];
		playerInfo.SmithLevel = (int)reader["SmithLevel"];
		playerInfo.ConsortiaHonor = (int)reader["ConsortiaHonor"];
		playerInfo.RichesOffer = (int)reader["RichesOffer"];
		playerInfo.RichesRob = (int)reader["RichesRob"];
		playerInfo.AntiAddiction = (int)reader["AntiAddiction"];
		playerInfo.DutyLevel = (int)reader["DutyLevel"];
		playerInfo.DutyName = ((reader["DutyName"] == null) ? "" : reader["DutyName"].ToString());
		playerInfo.Right = (int)reader["Right"];
		playerInfo.ChairmanName = ((reader["ChairmanName"] == null) ? "" : reader["ChairmanName"].ToString());
		playerInfo.AddDayGP = (int)reader["AddDayGP"];
		playerInfo.AddDayOffer = (int)reader["AddDayOffer"];
		playerInfo.AddWeekGP = (int)reader["AddWeekGP"];
		playerInfo.AddWeekOffer = (int)reader["AddWeekOffer"];
		playerInfo.ConsortiaRiches = (int)reader["ConsortiaRiches"];
		playerInfo.CheckCount = (int)reader["CheckCount"];
		playerInfo.IsMarried = (bool)reader["IsMarried"];
		playerInfo.MarryDate = (DateTime)reader["MarryDate"];
		playerInfo.SpouseID = (int)reader["SpouseID"];
		playerInfo.SpouseName = ((reader["SpouseName"] == null) ? "" : reader["SpouseName"].ToString());
		playerInfo.MarryInfoID = (int)reader["MarryInfoID"];
		playerInfo.IsCreatedMarryRoom = (bool)reader["IsCreatedMarryRoom"];
		playerInfo.DayLoginCount = (int)reader["DayLoginCount"];
		playerInfo.PasswordTwo = ((reader["PasswordTwo"] == null) ? "" : reader["PasswordTwo"].ToString());
		playerInfo.SelfMarryRoomID = (int)reader["SelfMarryRoomID"];
		playerInfo.IsGotRing = (bool)reader["IsGotRing"];
		playerInfo.Rename = (bool)reader["Rename"];
		playerInfo.ConsortiaRename = (bool)reader["ConsortiaRename"];
		playerInfo.IsDirty = false;
		playerInfo.IsFirst = (int)reader["IsFirst"];
		playerInfo.Nimbus = (int)reader["Nimbus"];
		playerInfo.LastAward = (DateTime)reader["LastAward"];
		playerInfo.DDTMoney = (int)reader["GiftToken"];
		playerInfo.QuestSite = ((reader["QuestSite"] == null) ? new byte[200] : ((byte[])reader["QuestSite"]));
		playerInfo.PvePermission = ((reader["PvePermission"] == null) ? "" : reader["PvePermission"].ToString());
		playerInfo.FightPower = (int)reader["FightPower"];
		playerInfo.PasswordQuest1 = ((reader["PasswordQuestion1"] == null) ? "" : reader["PasswordQuestion1"].ToString());
		playerInfo.PasswordQuest2 = ((reader["PasswordQuestion2"] == null) ? "" : reader["PasswordQuestion2"].ToString());
		_ = (DateTime)reader["LastFindDate"];
		playerInfo.FailedPasswordAttemptCount = (int)reader["FailedPasswordAttemptCount"];
		playerInfo.AnswerSite = (int)reader["AnswerSite"];
		playerInfo.medal = (int)reader["Medal"];
		playerInfo.ChatCount = (int)reader["ChatCount"];
		playerInfo.SpaPubGoldRoomLimit = (int)reader["SpaPubGoldRoomLimit"];
		playerInfo.LastSpaDate = (DateTime)reader["LastSpaDate"];
		playerInfo.FightLabPermission = (string)reader["FightLabPermission"];
		playerInfo.SpaPubMoneyRoomLimit = (int)reader["SpaPubMoneyRoomLimit"];
		playerInfo.IsInSpaPubGoldToday = (bool)reader["IsInSpaPubGoldToday"];
		playerInfo.IsInSpaPubMoneyToday = (bool)reader["IsInSpaPubMoneyToday"];
		playerInfo.AchievementPoint = (int)reader["AchievementPoint"];
		playerInfo.LastWeekly = (DateTime)reader["LastWeekly"];
		playerInfo.LastWeeklyVersion = (int)reader["LastWeeklyVersion"];
		playerInfo.GiftGp = (int)reader["GiftGp"];
		playerInfo.GiftLevel = (int)reader["GiftLevel"];
		playerInfo.IsOpenGift = (bool)reader["IsOpenGift"];
		playerInfo.badgeID = (int)reader["badgeID"];
		playerInfo.typeVIP = Convert.ToByte(reader["typeVIP"]);
		playerInfo.VIPLevel = (int)reader["VIPLevel"];
		playerInfo.VIPExp = (int)reader["VIPExp"];
		playerInfo.VIPExpireDay = (DateTime)reader["VIPExpireDay"];
		playerInfo.LastVIPPackTime = (DateTime)reader["LastVIPPackTime"];
		playerInfo.CanTakeVipReward = (bool)reader["CanTakeVipReward"];
		playerInfo.WeaklessGuildProgressStr = (string)reader["WeaklessGuildProgressStr"];
		playerInfo.IsOldPlayer = (bool)reader["IsOldPlayer"];
		playerInfo.LastDate = (DateTime)reader["LastDate"];
		playerInfo.VIPLastDate = (DateTime)reader["VIPLastDate"];
		playerInfo.Score = (int)reader["Score"];
		playerInfo.OptionOnOff = (int)reader["OptionOnOff"];
		playerInfo.isOldPlayerHasValidEquitAtLogin = (bool)reader["isOldPlayerHasValidEquitAtLogin"];
		playerInfo.badLuckNumber = (int)reader["badLuckNumber"];
		playerInfo.luckyNum = (int)reader["luckyNum"];
		playerInfo.lastLuckyNumDate = (DateTime)reader["lastLuckyNumDate"];
		playerInfo.lastLuckNum = (int)reader["lastLuckNum"];
		playerInfo.CardSoul = (int)reader["CardSoul"];
		playerInfo.uesedFinishTime = (int)reader["uesedFinishTime"];
		playerInfo.totemId = (int)reader["totemId"];
		playerInfo.damageScores = (int)reader["damageScores"];
		playerInfo.petScore = (int)reader["petScore"];
		playerInfo.IsShowConsortia = (bool)reader["IsShowConsortia"];
		playerInfo.LastRefreshPet = (DateTime)reader["LastRefreshPet"];
		playerInfo.GetSoulCount = (int)reader["GetSoulCount"];
		playerInfo.isFirstDivorce = (int)reader["isFirstDivorce"];
		playerInfo.myScore = (int)reader["myScore"];
		playerInfo.LastGetEgg = (DateTime)reader["LastGetEgg"];
		playerInfo.TimeBox = (DateTime)reader["TimeBox"];
		playerInfo.IsFistGetPet = (bool)reader["IsFistGetPet"];
		playerInfo.myHonor = (int)reader["myHonor"];
		playerInfo.hardCurrency = (int)reader["hardCurrency"];
		playerInfo.MaxBuyHonor = (int)reader["MaxBuyHonor"];
		playerInfo.LeagueMoney = (int)reader["LeagueMoney"];
		playerInfo.Honor = (string)reader["Honor"];
		playerInfo.necklaceExp = (int)reader["necklaceExp"];
		playerInfo.necklaceExpAdd = (int)reader["necklaceExpAdd"];
		playerInfo.accumulativeLoginDays = (int)reader["accumulativeLoginDays"];
		playerInfo.accumulativeAwardDays = (int)reader["accumulativeAwardDays"];
		playerInfo.MountsType = (int)reader["MountsType"];
		playerInfo.PveEpicPermission = (string)reader["PveEpicPermission"];
		playerInfo.evolutionGrade = (int)reader["evolutionGrade"];
		playerInfo.evolutionExp = (int)reader["evolutionExp"];
		playerInfo.MagicAttack = (int)reader["MagicAttack"];
		playerInfo.MagicDefence = (int)reader["MagicDefence"];
		playerInfo.honorId = (int)reader["honorId"];
		playerInfo.MountExp = (int)reader["curExp"];
		playerInfo.MountLv = (int)reader["curLevel"];
		playerInfo.PetsID = (int)reader["PetsID"];
		playerInfo.horseRaceCanRaceTime = (int)reader["horseRaceCanRaceTime"];
		playerInfo.eggNum = (int)reader["eggNum"];
		playerInfo.flowerNum = (int)reader["flowerNum"];
		playerInfo.isClicked = (bool)reader["isClicked"];
		playerInfo.isAttest = (bool)reader["isAttest"];
		playerInfo.ImagePath = ((reader["ImagePath"] == null) ? "" : reader["ImagePath"].ToString());
		playerInfo.SubName = ((reader["SubName"] == null) ? "" : reader["SubName"].ToString());
		playerInfo.IsShow = (bool)reader["IsShow"];
		playerInfo.SubID = (int)reader["SubID"];
		playerInfo.fineSuitExp = (int)reader["fineSuitExp"];
		playerInfo.BattleCount = (int)reader["BattleCount"];
		playerInfo.VipIntegral = (int)reader["VipIntegral"];
		playerInfo.VipShop = ((reader["VipShop"] == DBNull.Value) ? "" : ((string)reader["VipShop"]));
		playerInfo.RingExp = (int)reader["RingExp"];
		playerInfo.desertEnterCount = (int)reader["desertEnterCount"];
		playerInfo.guardCoreID = (int)reader["guardCoreID"];
		playerInfo.guardCoreGrade = (int)reader["guardCoreGrade"];
		playerInfo.ticketNum = (int)reader["ticketNum"];
		playerInfo.createPlayerDate = (DateTime)reader["createPlayerDate"];
		playerInfo.vipDiscount = (int)reader["vipDiscount"];
		playerInfo.vipDiscountValidity = (DateTime)reader["vipDiscountValidity"];
		playerInfo.stive = (int)reader["stive"];
		playerInfo.jampsCurrency = (int)reader["jampsCurrency"];
		playerInfo.buyStiveNum = (int)reader["buyStiveNum"];
		playerInfo.GhostEquipList = ((reader["GhostEquipList"] == DBNull.Value) ? "" : ((string)reader["GhostEquipList"]));
		playerInfo.BaseAttack = (int)reader["BaseAttack"];
		playerInfo.BaseDefence = (int)reader["BaseDefence"];
		playerInfo.VIPGiftWeekDate = ((reader["VIPGiftWeekDate"] == DBNull.Value) ? DateTime.Now.AddYears(-1) : ((DateTime)reader["VIPGiftWeekDate"]));
		playerInfo.IsVIPOldPlayerScan = reader["IsVIPOldPlayerScan"] != DBNull.Value && (bool)reader["IsVIPOldPlayerScan"];
		playerInfo.TotemUpgrade = ((reader["TotemUpgrade"] == DBNull.Value) ? null : ((string)reader["TotemUpgrade"]));
		playerInfo.pntsBattle = ((reader["pntsBattle"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader["pntsBattle"]));
		playerInfo.necklaceCastLevel = (int)reader["necklaceCastLevel"];
		playerInfo.necklaceArtificeNeedCount = (int)reader["necklaceArtificeNeedCount"];
		playerInfo.IsHideElf = (bool)reader["IsHideElf"];
		playerInfo.LoadingScore = (int)reader["LoadingScore"];
		playerInfo.SlotPet = (int)reader["SlotPet"];
        return playerInfo;
	}

	public ItemInfo InitItem(SqlDataReader reader)
	{
		ItemInfo itemInfo = new ItemInfo(ItemMgr.FindItemTemplate((int)reader["TemplateID"]));
		if (itemInfo.Template == null)
		{
			return null;
		}
		itemInfo.AgilityCompose = (int)reader["AgilityCompose"];
		itemInfo.AttackCompose = (int)reader["AttackCompose"];
		itemInfo.Color = reader["Color"].ToString();
		itemInfo.Count = (int)reader["Count"];
		itemInfo.DefendCompose = (int)reader["DefendCompose"];
		itemInfo.ItemID = (int)reader["ItemID"];
		itemInfo.LuckCompose = (int)reader["LuckCompose"];
		itemInfo.Place = (int)reader["Place"];
		itemInfo.StrengthenLevel = (int)reader["StrengthenLevel"];
		itemInfo.TemplateID = (int)reader["TemplateID"];
		itemInfo.UserID = (int)reader["UserID"];
		itemInfo.ValidDate = (int)reader["ValidDate"];
		itemInfo.IsDirty = false;
		itemInfo.IsExist = (bool)reader["IsExist"];
		itemInfo.IsBinds = (bool)reader["IsBinds"];
		itemInfo.IsUsed = (bool)reader["IsUsed"];
		itemInfo.BeginDate = (DateTime)reader["BeginDate"];
		itemInfo.IsJudge = (bool)reader["IsJudge"];
		itemInfo.BagType = (int)reader["BagType"];
		itemInfo.Skin = reader["Skin"].ToString();
		itemInfo.RemoveDate = (DateTime)reader["RemoveDate"];
		itemInfo.RemoveType = (int)reader["RemoveType"];
		itemInfo.Hole1 = (int)reader["Hole1"];
		itemInfo.Hole2 = (int)reader["Hole2"];
		itemInfo.Hole3 = (int)reader["Hole3"];
		itemInfo.Hole4 = (int)reader["Hole4"];
		itemInfo.Hole5 = (int)reader["Hole5"];
		itemInfo.Hole6 = (int)reader["Hole6"];
		itemInfo.StrengthenTimes = (int)reader["StrengthenTimes"];
		itemInfo.StrengthenExp = (int)reader["StrengthenExp"];
		itemInfo.Hole5Level = (int)reader["Hole5Level"];
		itemInfo.Hole5Exp = (int)reader["Hole5Exp"];
		itemInfo.Hole6Level = (int)reader["Hole6Level"];
		itemInfo.Hole6Exp = (int)reader["Hole6Exp"];
		itemInfo.goldBeginTime = (DateTime)reader["goldBeginTime"];
		itemInfo.goldValidDate = (int)reader["goldValidDate"];
		itemInfo.beadExp = (int)reader["beadExp"];
		itemInfo.beadLevel = (int)reader["beadLevel"];
		itemInfo.beadIsLock = (bool)reader["beadIsLock"];
		itemInfo.isShowBind = (bool)reader["isShowBind"];
		itemInfo.latentEnergyCurStr = (string)reader["latentEnergyCurStr"];
		itemInfo.latentEnergyNewStr = (string)reader["latentEnergyNewStr"];
		itemInfo.latentEnergyEndTime = (DateTime)reader["latentEnergyEndTime"];
		itemInfo.Damage = (int)reader["Damage"];
		itemInfo.Guard = (int)reader["Guard"];
		itemInfo.Blood = (int)reader["Blood"];
		itemInfo.Bless = (int)reader["Bless"];
		itemInfo.AdvanceDate = (DateTime)reader["AdvanceDate"];
		itemInfo.AvatarActivity = (bool)reader["AvatarActivity"];
		itemInfo.goodsLock = (bool)reader["goodsLock"];
		itemInfo.MagicAttack = (int)reader["MagicAttack"];
		itemInfo.MagicDefence = (int)reader["MagicDefence"];
		itemInfo.IsGold = (bool)reader["IsGold"];
		itemInfo.GoldEquip = ItemMgr.FindGoldItemTemplate(itemInfo.TemplateID, itemInfo.IsGold);
		itemInfo.MagicExp = (int)reader["MagicExp"];
		itemInfo.MagicLevel = (int)reader["MagicLevel"];
		itemInfo.curExp = (int)reader["curExp"];
		itemInfo.cellLocked = (bool)reader["cellLocked"];
		itemInfo.canRenew = (bool)reader["canRenew"];
		itemInfo.canTransfer = (bool)reader["canTransfer"];
		itemInfo.enchantSpiritLevel = (int)reader["enchantSpiritLevel"];
		return itemInfo;
	}

	public UsersPetInfo InitPet(SqlDataReader reader)
	{
		UsersPetInfo usersPetInfo = new UsersPetInfo();
		usersPetInfo.ID = (int)reader["ID"];
		usersPetInfo.TemplateID = (int)reader["TemplateID"];
		usersPetInfo.Name = reader["Name"].ToString();
		usersPetInfo.UserID = (int)reader["UserID"];
		usersPetInfo.Attack = (int)reader["Attack"];
		usersPetInfo.AttackGrow = (int)reader["AttackGrow"];
		usersPetInfo.Agility = (int)reader["Agility"];
		usersPetInfo.AgilityGrow = (int)reader["AgilityGrow"];
		usersPetInfo.Defence = (int)reader["Defence"];
		usersPetInfo.DefenceGrow = (int)reader["DefenceGrow"];
		usersPetInfo.Luck = (int)reader["Luck"];
		usersPetInfo.LuckGrow = (int)reader["LuckGrow"];
		usersPetInfo.Blood = (int)reader["Blood"];
		usersPetInfo.BloodGrow = (int)reader["BloodGrow"];
		usersPetInfo.Damage = (int)reader["Damage"];
		usersPetInfo.DamageGrow = (int)reader["DamageGrow"];
		usersPetInfo.Guard = (int)reader["Guard"];
		usersPetInfo.GuardGrow = (int)reader["GuardGrow"];
		usersPetInfo.Level = (int)reader["Level"];
		usersPetInfo.GP = (int)reader["GP"];
		usersPetInfo.MaxGP = (int)reader["MaxGP"];
		usersPetInfo.Hunger = (int)reader["Hunger"];
		usersPetInfo.MP = (int)reader["MP"];
		usersPetInfo.Place = (int)reader["Place"];
		usersPetInfo.IsEquip = (bool)reader["IsEquip"];
		usersPetInfo.IsExit = (bool)reader["IsExit"];
		usersPetInfo.Skill = reader["Skill"].ToString();
		usersPetInfo.SkillEquip = reader["SkillEquip"].ToString();
		usersPetInfo.currentStarExp = (int)reader["currentStarExp"];
		usersPetInfo.breakGrade = (int)reader["breakGrade"];
		usersPetInfo.breakAttack = (int)reader["breakAttack"];
		usersPetInfo.breakDefence = (int)reader["breakDefence"];
		usersPetInfo.breakAgility = (int)reader["breakAgility"];
		usersPetInfo.breakLuck = (int)reader["breakLuck"];
		usersPetInfo.breakBlood = (int)reader["breakBlood"];
		usersPetInfo.eQPets = ((reader["eQPets"] == null) ? "" : reader["eQPets"].ToString());
		usersPetInfo.BaseProp = ((reader["BaseProp"] == null) ? "" : reader["BaseProp"].ToString());
		usersPetInfo.IsSeal = (bool)reader["IsSeal"];
		return usersPetInfo;
	}

	public UsersCardInfo InitCard(SqlDataReader reader)
	{
		UsersCardInfo usersCardInfo = new UsersCardInfo();
		usersCardInfo.UserID = (int)reader["UserID"];
		usersCardInfo.TemplateID = (int)reader["TemplateID"];
		usersCardInfo.CardID = (int)reader["CardID"];
		usersCardInfo.CardType = (int)reader["CardType"];
		usersCardInfo.Attack = (int)reader["Attack"];
		usersCardInfo.Agility = (int)reader["Agility"];
		usersCardInfo.Defence = (int)reader["Defence"];
		usersCardInfo.Luck = (int)reader["Luck"];
		usersCardInfo.Damage = (int)reader["Damage"];
		usersCardInfo.Guard = (int)reader["Guard"];
		usersCardInfo.Level = (int)reader["Level"];
		usersCardInfo.Place = (int)reader["Place"];
		usersCardInfo.isFirstGet = (bool)reader["isFirstGet"];
		usersCardInfo.Type = (int)reader["Type"];
		usersCardInfo.CardGP = (int)reader["CardGP"];
		return usersCardInfo;
	}

	public TexpInfo InitTexpInfo(SqlDataReader reader)
	{
		TexpInfo texpInfo = new TexpInfo();
		texpInfo.UserID = (int)reader["UserID"];
		texpInfo.attTexpExp = (int)reader["attTexpExp"];
		texpInfo.defTexpExp = (int)reader["defTexpExp"];
		texpInfo.hpTexpExp = (int)reader["hpTexpExp"];
		texpInfo.lukTexpExp = (int)reader["lukTexpExp"];
		texpInfo.spdTexpExp = (int)reader["spdTexpExp"];
		texpInfo.texpCount = (int)reader["texpCount"];
		texpInfo.texpTaskCount = (int)reader["texpTaskCount"];
		texpInfo.texpTaskDate = (DateTime)reader["texpTaskDate"];
		texpInfo.magicAtkTexpExp = (int)reader["magicAtkTexpExp"];
		texpInfo.magicDefTexpExp = (int)reader["magicDefTexpExp"];
		texpInfo.magicTexpCount = (int)reader["magicTexpCount"];
		texpInfo.TrainCritTexpExp = (int)reader["TrainCritTexpExp"];
		texpInfo.TrainSunderTexpExp = (int)reader["TrainSunderTexpExp"];
		texpInfo.TrainCritDmgTexpExp = (int)reader["TrainCritDmgTexpExp"];
		texpInfo.TrainSpeedTexpExp = (int)reader["TrainSpeedTexpExp"];
		texpInfo.TrainTricKillTexpExp = (int)reader["TrainTricKillTexpExp"];
		texpInfo.TrainDmgTexpExp = (int)reader["TrainDmgTexpExp"];
		texpInfo.TrainArmorTexpExp = (int)reader["TrainArmorTexpExp"];
		texpInfo.SecondTexpCount = (int)reader["SecondTexpCount"];
        texpInfo.TrainAvoidDmgTextExp = (int)reader["TrainAvoidDmgTextExp"];
        texpInfo.TrainTricDefTexpExp = (int)reader["TrainTricDefTexpExp"];
        texpInfo.TrainTenacTexpExp = (int)reader["TrainTenacTexpExp"];
        texpInfo.TrainGuardTexpExp = (int)reader["TrainGuardTexpExp"];
        texpInfo.TrainFireDefTexpExp = (int)reader["TrainFireDefTexpExp"];
        texpInfo.TrainWaterDefTexpExp = (int)reader["TrainWaterDefTexpExp"];
        texpInfo.TrainWindDefTexpExp = (int)reader["TrainWindDefTexpExp"];
        texpInfo.TrainLandDefTexpExp = (int)reader["TrainLandDefTexpExp"];
        texpInfo.ThirdTexpCount = (int)reader["ThirdTexpCount"];
        texpInfo.lastReset = (DateTime)reader["LastReset"];
		texpInfo.resetCount = (int)reader["ResetCount"];
		return texpInfo;
	}

	public PlayerInfo GetUserSingleByUserID(int UserID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_Users_SingleByUserID", array);
			if (ResultDataReader.Read())
			{
				return InitPlayerInfo(ResultDataReader);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
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

	public PlayerLimitInfo GetUserLimitByUserName(string userName)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserName", SqlDbType.NVarChar, 200)
			};
			array[0].Value = userName;
			db.GetReader(ref ResultDataReader, "SP_Users_LimitByUserName", array);
			if (ResultDataReader.Read())
			{
				PlayerLimitInfo playerLimitInfo = new PlayerLimitInfo();
				playerLimitInfo.ID = (int)ResultDataReader["UserID"];
				playerLimitInfo.NickName = (string)ResultDataReader["NickName"];
				return playerLimitInfo;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
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

	public PlayerInfo GetUserSingleByUserName(string userName)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserName", SqlDbType.NVarChar, 200)
			};
			array[0].Value = userName;
			db.GetReader(ref ResultDataReader, "SP_Users_SingleByUserName", array);
			if (ResultDataReader.Read())
			{
				return InitPlayerInfo(ResultDataReader);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
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

	public PlayerInfo GetUserSingleByNickName(string nickName)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@NickName", SqlDbType.NVarChar, 200)
			};
			array[0].Value = nickName;
			db.GetReader(ref ResultDataReader, "SP_Users_SingleByNickName", array);
			if (ResultDataReader.Read())
			{
				return InitPlayerInfo(ResultDataReader);
			}
		}
		catch
		{
			throw new Exception();
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

	public UserRankDateInfo GetUserRankDateByID(int userID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = userID;
			db.GetReader(ref ResultDataReader, "SP_Sys_Users_Rank_Date", array);
			if (ResultDataReader.Read())
			{
				return InitUserRankDateInfo(ResultDataReader);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("InitUserRankDateInfo", exception);
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

	public UserRankDateInfo[] GetAllUserRankDate()
	{
		List<UserRankDateInfo> list = new List<UserRankDateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[0];
			db.GetReader(ref ResultDataReader, "SP_Sys_Users_Rank_Date_All", sqlParameters);
			while (ResultDataReader.Read())
			{
				list.Add(InitUserRankDateInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("InitUserRankDateInfo", exception);
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

	public UserRankDateInfo InitUserRankDateInfo(SqlDataReader dr)
	{
		UserRankDateInfo userRankDateInfo = new UserRankDateInfo();
		userRankDateInfo.UserID = (int)dr["UserID"];
		userRankDateInfo.ConsortiaID = (int)dr["ConsortiaID"];
		userRankDateInfo.FightPower = (int)dr["FightPower"];
		userRankDateInfo.PrevFightPower = (int)dr["PrevFightPower"];
		userRankDateInfo.GP = (int)dr["GP"];
		userRankDateInfo.PrevGP = (int)dr["PrevGP"];
		userRankDateInfo.AchievementPoint = (int)dr["AchievementPoint"];
		userRankDateInfo.PrevAchievementPoint = (int)dr["PrevAchievementPoint"];
		userRankDateInfo.GiftGp = (int)dr["GiftGp"];
		userRankDateInfo.PrevGiftGp = (int)dr["PrevGiftGp"];
		userRankDateInfo.LeagueAddWeek = (int)dr["LeagueAddWeek"];
		userRankDateInfo.PrevLeagueAddWeek = (int)dr["PrevLeagueAddWeek"];
		userRankDateInfo.MountExp = (int)dr["MountExp"];
		userRankDateInfo.PrevMountExp = (int)dr["PrevMountExp"];
		userRankDateInfo.ConsortiaFightPower = (int)dr["ConsortiaFightPower"];
		userRankDateInfo.ConsortiaPrevFightPower = (int)dr["ConsortiaPrevFightPower"];
		userRankDateInfo.ConsortiaLevel = (int)dr["ConsortiaLevel"];
		userRankDateInfo.ConsortiaPrevLevel = (int)dr["ConsortiaPrevLevel"];
		userRankDateInfo.ConsortiaRiches = (int)dr["ConsortiaRiches"];
		userRankDateInfo.ConsortiaPrevRiches = (int)dr["ConsortiaPrevRiches"];
		userRankDateInfo.ConsortiaGiftGp = (int)dr["ConsortiaGiftGp"];
		userRankDateInfo.ConsortiaPrevGiftGp = (int)dr["ConsortiaPrevGiftGp"];
		return userRankDateInfo;
	}

	public DailyLogListInfo GetDailyLogListSingle(int UserID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@UserID", UserID)
			};
			db.GetReader(ref ResultDataReader, "SP_DailyLogList_Single", sqlParameters);
			if (ResultDataReader.Read())
			{
				DailyLogListInfo dailyLogListInfo = new DailyLogListInfo();
				dailyLogListInfo.ID = (int)ResultDataReader["ID"];
				dailyLogListInfo.UserID = (int)ResultDataReader["UserID"];
				dailyLogListInfo.UserAwardLog = (int)ResultDataReader["UserAwardLog"];
				dailyLogListInfo.DayLog = (string)ResultDataReader["DayLog"];
				dailyLogListInfo.LastDate = (DateTime)ResultDataReader["LastDate"];
				return dailyLogListInfo;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("DailyLogList", exception);
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

	public bool UpdateDailyLogList(DailyLogListInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[5]
			{
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@UserAwardLog", info.UserAwardLog),
				new SqlParameter("@DayLog", info.DayLog),
				new SqlParameter("@LastDate", info.LastDate.ToString()),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[4].Direction = ParameterDirection.ReturnValue;
			result = db.RunProcedure("SP_DailyLogList_Update", array);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("User_Update_BoxProgression", exception);
			}
		}
		return result;
	}

	public bool UpdateRank()
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[0];
			result = db.RunProcedure("SP_Sys_Update_Consortia_DayList", sqlParameters);
			result = db.RunProcedure("SP_Sys_Update_Consortia_FightPower", sqlParameters);
			result = db.RunProcedure("SP_Sys_Update_Consortia_Honor", sqlParameters);
			result = db.RunProcedure("SP_Sys_Update_Consortia_List", sqlParameters);
			result = db.RunProcedure("SP_Sys_Update_Consortia_WeekList", sqlParameters);
			result = db.RunProcedure("SP_Sys_Update_OfferList", sqlParameters);
			result = db.RunProcedure("SP_Sys_Update_Users_DayList", sqlParameters);
			result = db.RunProcedure("SP_Sys_Update_Users_List", sqlParameters);
			result = db.RunProcedure("SP_Sys_Update_Users_WeekList", sqlParameters);
			result = db.RunProcedure("SP_Sys_Update_Users_Rank_Date", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init UpdatePersonalRank", exception);
			}
		}
		return result;
	}

	public PlayerInfo[] GetPlayerPage(int page, int size, ref int total, int order, int userID, ref bool resultValue)
	{
		List<PlayerInfo> list = new List<PlayerInfo>();
		try
		{
			string text = " IsExist=1 and IsFirst<> 0 ";
			if (userID != -1)
			{
				text = text + " and UserID =" + userID + " ";
			}
			string text2 = "GP desc";
			switch (order)
			{
			case 1:
				text2 = "Offer desc";
				break;
			case 2:
				text2 = "AddDayGP desc";
				break;
			case 3:
				text2 = "AddWeekGP desc";
				break;
			case 4:
				text2 = "AddDayOffer desc";
				break;
			case 5:
				text2 = "AddWeekOffer desc";
				break;
			case 6:
				text2 = "FightPower desc";
				break;
			case 7:
				text2 = "AchievementPoint desc";
				break;
			case 8:
				text2 = "AddDayAchievementPoint desc";
				break;
			case 9:
				text2 = "AddWeekAchievementPoint desc";
				break;
			case 10:
				text2 = "GiftGp desc";
				break;
			case 11:
				text2 = "AddDayGiftGp desc";
				break;
			case 12:
				text2 = "AddWeekGiftGp desc";
				break;
			case 13:
				text2 = "totalPrestige desc";
				break;
			case 14:
				text2 = "curExp desc";
				break;
			}
			text2 += ",UserID";
			DataTable page2 = GetPage("V_Sys_Users_Detail", text, page, size, "*", text2, "UserID", ref total);
			foreach (DataRow row in page2.Rows)
			{
				PlayerInfo playerInfo = new PlayerInfo();
				playerInfo.ZoneID = ZoneID;
				playerInfo.ZoneName = AreaName;
				playerInfo.Agility = (int)row["Agility"];
				playerInfo.Attack = (int)row["Attack"];
				playerInfo.Colors = ((row["Colors"] == null) ? "" : row["Colors"].ToString());
				playerInfo.ConsortiaID = (int)row["ConsortiaID"];
				playerInfo.Defence = (int)row["Defence"];
				playerInfo.Gold = (int)row["Gold"];
				playerInfo.GP = (int)row["GP"];
				playerInfo.Grade = (int)row["Grade"];
				playerInfo.ID = (int)row["UserID"];
				playerInfo.Luck = (int)row["Luck"];
				playerInfo.Money = (int)row["Money"];
				playerInfo.NickName = ((row["NickName"] == null) ? "" : row["NickName"].ToString());
				playerInfo.Sex = (bool)row["Sex"];
				playerInfo.State = (int)row["State"];
				playerInfo.Style = ((row["Style"] == null) ? "" : row["Style"].ToString());
				playerInfo.Hide = (int)row["Hide"];
				playerInfo.Repute = (int)row["Repute"];
				playerInfo.UserName = ((row["UserName"] == null) ? "" : row["UserName"].ToString());
				playerInfo.ConsortiaName = ((row["ConsortiaName"] == null) ? "" : row["ConsortiaName"].ToString());
				playerInfo.Offer = (int)row["Offer"];
				playerInfo.Skin = ((row["Skin"] == null) ? "" : row["Skin"].ToString());
				playerInfo.IsBanChat = (bool)row["IsBanChat"];
				playerInfo.ReputeOffer = (int)row["ReputeOffer"];
				playerInfo.ConsortiaRepute = (int)row["ConsortiaRepute"];
				playerInfo.ConsortiaLevel = (int)row["ConsortiaLevel"];
				playerInfo.StoreLevel = (int)row["StoreLevel"];
				playerInfo.ShopLevel = (int)row["ShopLevel"];
				playerInfo.SmithLevel = (int)row["SmithLevel"];
				playerInfo.ConsortiaHonor = (int)row["ConsortiaHonor"];
				playerInfo.RichesOffer = (int)row["RichesOffer"];
				playerInfo.RichesRob = (int)row["RichesRob"];
				playerInfo.DutyLevel = (int)row["DutyLevel"];
				playerInfo.DutyName = ((row["DutyName"] == null) ? "" : row["DutyName"].ToString());
				playerInfo.Right = (int)row["Right"];
				playerInfo.ChairmanName = ((row["ChairmanName"] == null) ? "" : row["ChairmanName"].ToString());
				playerInfo.Win = (int)row["Win"];
				playerInfo.Total = (int)row["Total"];
				playerInfo.Escape = (int)row["Escape"];
				playerInfo.AddDayGP = (((int)row["AddDayGP"] == 0) ? ((playerInfo.GP > int.MaxValue) ? int.MaxValue : Convert.ToInt32(playerInfo.GP)) : ((int)row["AddDayGP"]));
				playerInfo.AddDayOffer = (((int)row["AddDayOffer"] == 0) ? playerInfo.Offer : ((int)row["AddDayOffer"]));
				playerInfo.AddWeekGP = (((int)row["AddWeekGP"] == 0) ? ((playerInfo.GP > int.MaxValue) ? int.MaxValue : Convert.ToInt32(playerInfo.GP)) : ((int)row["AddWeekGP"]));
				playerInfo.AddWeekOffer = (((int)row["AddWeekOffer"] == 0) ? playerInfo.Offer : ((int)row["AddWeekOffer"]));
				playerInfo.ConsortiaRiches = (int)row["ConsortiaRiches"];
				playerInfo.CheckCount = (int)row["CheckCount"];
				playerInfo.Nimbus = (int)row["Nimbus"];
				playerInfo.DDTMoney = (int)row["GiftToken"];
				playerInfo.QuestSite = ((row["QuestSite"] == null) ? new byte[200] : ((byte[])row["QuestSite"]));
				playerInfo.PvePermission = ((row["PvePermission"] == null) ? "" : row["PvePermission"].ToString());
				playerInfo.FightPower = (int)row["FightPower"];
				playerInfo.Honor = ((row["Honor"] == null) ? "" : row["Honor"].ToString());
				playerInfo.GiftGp = (int)row["GiftGp"];
				playerInfo.GiftLevel = (int)row["GiftLevel"];
				playerInfo.AddWeekLeagueScore = (int)row["weeklyScore"];
				playerInfo.TotalPrestige = (int)row["totalPrestige"];
				playerInfo.MountExp = (int)row["curExp"];
				playerInfo.MountLv = (int)row["curLevel"];
				playerInfo.IsOldPlayer = (bool)row["IsOldPlayer"];
				list.Add(playerInfo);
			}
			resultValue = true;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
		}
		return list.ToArray();
	}

	public ItemInfo[] GetUserItem(int UserID)
	{
		List<ItemInfo> list = new List<ItemInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_Users_Items_All", array);
			while (ResultDataReader.Read())
			{
				ItemInfo itemInfo = InitItem(ResultDataReader);
				if (itemInfo != null)
				{
					list.Add(itemInfo);
				}
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
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

	public ItemInfo[] GetUserBagByType(int UserID, int bagType)
	{
		List<ItemInfo> list = new List<ItemInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[2]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4),
				null
			};
			array[0].Value = UserID;
			array[1] = new SqlParameter("@BagType", bagType);
			db.GetReader(ref ResultDataReader, "SP_Users_BagByType", array);
			while (ResultDataReader.Read())
			{
				ItemInfo itemInfo = InitItem(ResultDataReader);
				if (itemInfo != null)
				{
					list.Add(itemInfo);
				}
			}
		}
		catch (Exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error($"GetUserBagByType Error UserID:{UserID}, bagType:{bagType}");
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

	public List<ItemInfo> GetUserEuqip(int UserID)
	{
		List<ItemInfo> list = new List<ItemInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_Users_Items_Equip", array);
			while (ResultDataReader.Read())
			{
				ItemInfo itemInfo = InitItem(ResultDataReader);
				if (itemInfo != null)
				{
					list.Add(itemInfo);
				}
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
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

	public List<ItemInfo> GetUserBeadEuqip(int UserID)
	{
		List<ItemInfo> list = new List<ItemInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_Users_Bead_Equip", array);
			while (ResultDataReader.Read())
			{
				ItemInfo itemInfo = InitItem(ResultDataReader);
				if (itemInfo != null)
				{
					list.Add(itemInfo);
				}
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
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

	public List<ItemInfo> GetUserMagicstoneEuqip(int UserID)
	{
		List<ItemInfo> list = new List<ItemInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_Magicstone_Equip", array);
			while (ResultDataReader.Read())
			{
				ItemInfo itemInfo = InitItem(ResultDataReader);
				if (itemInfo != null)
				{
					list.Add(itemInfo);
				}
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
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

	public List<ItemInfo> GetUserEuqipByNick(string Nick)
	{
		List<ItemInfo> list = new List<ItemInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@NickName", SqlDbType.NVarChar, 200)
			};
			array[0].Value = Nick;
			db.GetReader(ref ResultDataReader, "SP_Users_Items_Equip_By_Nick", array);
			while (ResultDataReader.Read())
			{
				ItemInfo itemInfo = InitItem(ResultDataReader);
				if (itemInfo != null)
				{
					list.Add(itemInfo);
				}
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
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

	public ItemInfo GetUserItemSingle(int itemID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@ID", SqlDbType.Int, 4)
			};
			array[0].Value = itemID;
			db.GetReader(ref ResultDataReader, "SP_Users_Items_Single", array);
			if (ResultDataReader.Read())
			{
				return InitItem(ResultDataReader);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
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

	public bool AddGoods(ItemInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[58];
			array[0] = new SqlParameter("@ItemID", item.ItemID);
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@UserID", item.UserID);
			array[2] = new SqlParameter("@TemplateID", item.Template.TemplateID);
			array[3] = new SqlParameter("@Place", item.Place);
			array[4] = new SqlParameter("@AgilityCompose", item.AgilityCompose);
			array[5] = new SqlParameter("@AttackCompose", item.AttackCompose);
			array[6] = new SqlParameter("@BeginDate", item.BeginDate);
			array[7] = new SqlParameter("@Color", (item.Color == null) ? "" : item.Color);
			array[8] = new SqlParameter("@Count", item.Count);
			array[9] = new SqlParameter("@DefendCompose", item.DefendCompose);
			array[10] = new SqlParameter("@IsBinds", item.IsBinds);
			array[11] = new SqlParameter("@IsExist", item.IsExist);
			array[12] = new SqlParameter("@IsJudge", item.IsJudge);
			array[13] = new SqlParameter("@LuckCompose", item.LuckCompose);
			array[14] = new SqlParameter("@StrengthenLevel", item.StrengthenLevel);
			array[15] = new SqlParameter("@ValidDate", item.ValidDate);
			array[16] = new SqlParameter("@BagType", item.BagType);
			array[17] = new SqlParameter("@Skin", (item.Skin == null) ? "" : item.Skin);
			array[18] = new SqlParameter("@IsUsed", item.IsUsed);
			array[19] = new SqlParameter("@RemoveType", item.RemoveType);
			array[20] = new SqlParameter("@Hole1", item.Hole1);
			array[21] = new SqlParameter("@Hole2", item.Hole2);
			array[22] = new SqlParameter("@Hole3", item.Hole3);
			array[23] = new SqlParameter("@Hole4", item.Hole4);
			array[24] = new SqlParameter("@Hole5", item.Hole5);
			array[25] = new SqlParameter("@Hole6", item.Hole6);
			array[26] = new SqlParameter("@StrengthenTimes", item.StrengthenTimes);
			array[27] = new SqlParameter("@Hole5Level", item.Hole5Level);
			array[28] = new SqlParameter("@Hole5Exp", item.Hole5Exp);
			array[29] = new SqlParameter("@Hole6Level", item.Hole6Level);
			array[30] = new SqlParameter("@Hole6Exp", item.Hole6Exp);
			array[31] = new SqlParameter("@StrengthenExp", item.StrengthenExp);
			array[32] = new SqlParameter("@beadExp", item.beadExp);
			array[33] = new SqlParameter("@beadLevel", item.beadLevel);
			array[34] = new SqlParameter("@beadIsLock", item.beadIsLock);
			array[35] = new SqlParameter("@isShowBind", item.isShowBind);
			array[36] = new SqlParameter("@Damage", item.Damage);
			array[37] = new SqlParameter("@Guard", item.Guard);
			array[38] = new SqlParameter("@Blood", item.Blood);
			array[39] = new SqlParameter("@Bless", item.Bless);
			array[40] = new SqlParameter("@goldBeginTime", item.goldBeginTime);
			array[41] = new SqlParameter("@goldValidDate", item.goldValidDate);
			array[42] = new SqlParameter("@IsGold", item.IsGold);
			array[43] = new SqlParameter("@latentEnergyEndTime", item.latentEnergyEndTime);
			array[44] = new SqlParameter("@latentEnergyCurStr", item.latentEnergyCurStr);
			array[45] = new SqlParameter("@latentEnergyNewStr", item.latentEnergyNewStr);
			array[46] = new SqlParameter("@AdvanceDate", item.AdvanceDate);
			array[47] = new SqlParameter("@AvatarActivity", item.AvatarActivity);
			array[48] = new SqlParameter("@goodsLock", item.goodsLock);
			array[49] = new SqlParameter("@MagicAttack", item.MagicAttack);
			array[50] = new SqlParameter("@MagicDefence", item.MagicDefence);
			array[51] = new SqlParameter("@MagicExp", item.MagicExp);
			array[52] = new SqlParameter("@MagicLevel", item.MagicLevel);
			array[53] = new SqlParameter("@curExp", item.curExp);
			array[54] = new SqlParameter("@cellLocked", item.cellLocked);
			array[55] = new SqlParameter("@canRenew", item.canRenew);
			array[56] = new SqlParameter("@canTransfer", item.canTransfer);
			array[57] = new SqlParameter("@enchantSpiritLevel", item.enchantSpiritLevel);
			result = db.RunProcedure("SP_Users_Items_Add", array);
			item.ItemID = (int)array[0].Value;
			item.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init item.goldBeginTime: " + item.goldBeginTime.ToString() + " item.goldValidDate: " + item.goldValidDate, exception);
			}
		}
		return result;
	}

	public bool UpdateGoods(ItemInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[59]
			{
				new SqlParameter("@ItemID", item.ItemID),
				new SqlParameter("@UserID", item.UserID),
				new SqlParameter("@TemplateID", item.Template.TemplateID),
				new SqlParameter("@Place", item.Place),
				new SqlParameter("@AgilityCompose", item.AgilityCompose),
				new SqlParameter("@AttackCompose", item.AttackCompose),
				new SqlParameter("@BeginDate", item.BeginDate),
				new SqlParameter("@Color", (item.Color == null) ? "" : item.Color),
				new SqlParameter("@Count", item.Count),
				new SqlParameter("@DefendCompose", item.DefendCompose),
				new SqlParameter("@IsBinds", item.IsBinds),
				new SqlParameter("@IsExist", item.IsExist),
				new SqlParameter("@IsJudge", item.IsJudge),
				new SqlParameter("@LuckCompose", item.LuckCompose),
				new SqlParameter("@StrengthenLevel", item.StrengthenLevel),
				new SqlParameter("@ValidDate", item.ValidDate),
				new SqlParameter("@BagType", item.BagType),
				new SqlParameter("@Skin", item.Skin),
				new SqlParameter("@IsUsed", item.IsUsed),
				new SqlParameter("@RemoveDate", item.RemoveDate),
				new SqlParameter("@RemoveType", item.RemoveType),
				new SqlParameter("@Hole1", item.Hole1),
				new SqlParameter("@Hole2", item.Hole2),
				new SqlParameter("@Hole3", item.Hole3),
				new SqlParameter("@Hole4", item.Hole4),
				new SqlParameter("@Hole5", item.Hole5),
				new SqlParameter("@Hole6", item.Hole6),
				new SqlParameter("@StrengthenTimes", item.StrengthenTimes),
				new SqlParameter("@Hole5Level", item.Hole5Level),
				new SqlParameter("@Hole5Exp", item.Hole5Exp),
				new SqlParameter("@Hole6Level", item.Hole6Level),
				new SqlParameter("@Hole6Exp", item.Hole6Exp),
				new SqlParameter("@goldBeginTime", item.goldBeginTime),
				new SqlParameter("@goldValidDate", item.goldValidDate),
				new SqlParameter("@IsGold", item.IsGold),
				new SqlParameter("@StrengthenExp", item.StrengthenExp),
				new SqlParameter("@beadExp", item.beadExp),
				new SqlParameter("@beadLevel", item.beadLevel),
				new SqlParameter("@beadIsLock", item.beadIsLock),
				new SqlParameter("@isShowBind", item.isShowBind),
				new SqlParameter("@latentEnergyCurStr", item.latentEnergyCurStr),
				new SqlParameter("@latentEnergyNewStr", item.latentEnergyNewStr),
				new SqlParameter("@latentEnergyEndTime", item.latentEnergyEndTime),
				new SqlParameter("@Damage", item.Damage),
				new SqlParameter("@Guard", item.Guard),
				new SqlParameter("@Blood", item.Blood),
				new SqlParameter("@Bless", item.Bless),
				new SqlParameter("@AdvanceDate", item.AdvanceDate),
				new SqlParameter("@AvatarActivity", item.AvatarActivity),
				new SqlParameter("@goodsLock", item.goodsLock),
				new SqlParameter("@MagicAttack", item.MagicAttack),
				new SqlParameter("@MagicDefence", item.MagicDefence),
				new SqlParameter("@MagicExp", item.MagicExp),
				new SqlParameter("@MagicLevel", item.MagicLevel),
				new SqlParameter("@curExp", item.curExp),
				new SqlParameter("@cellLocked", item.cellLocked),
				new SqlParameter("@canTransfer", item.canTransfer),
				new SqlParameter("@canRenew", item.canRenew),
				new SqlParameter("@enchantSpiritLevel", item.enchantSpiritLevel)
			};
			result = db.RunProcedure("SP_Users_Items_Update", sqlParameters);
			item.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool DeleteGoods(int itemID)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@ID", itemID)
			};
			result = db.RunProcedure("SP_Users_Items_Delete", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public BestEquipInfo[] GetCelebByDayBestEquip()
	{
		List<BestEquipInfo> list = new List<BestEquipInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Users_BestEquip");
			while (ResultDataReader.Read())
			{
				BestEquipInfo bestEquipInfo = new BestEquipInfo();
				bestEquipInfo.Date = (DateTime)ResultDataReader["RemoveDate"];
				bestEquipInfo.GP = (int)ResultDataReader["GP"];
				bestEquipInfo.Grade = (int)ResultDataReader["Grade"];
				bestEquipInfo.ItemName = ((ResultDataReader["Name"] == null) ? "" : ResultDataReader["Name"].ToString());
				bestEquipInfo.NickName = ((ResultDataReader["NickName"] == null) ? "" : ResultDataReader["NickName"].ToString());
				bestEquipInfo.Sex = (bool)ResultDataReader["Sex"];
				bestEquipInfo.Strengthenlevel = (int)ResultDataReader["Strengthenlevel"];
				bestEquipInfo.UserName = ((ResultDataReader["UserName"] == null) ? "" : ResultDataReader["UserName"].ToString());
				list.Add(bestEquipInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
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

	public MailInfo InitMail(SqlDataReader reader)
	{
		MailInfo mailInfo = new MailInfo();
		mailInfo.Annex1 = reader["Annex1"].ToString();
		mailInfo.Annex2 = reader["Annex2"].ToString();
		mailInfo.Content = reader["Content"].ToString();
		mailInfo.Gold = (int)reader["Gold"];
		mailInfo.ID = (int)reader["ID"];
		mailInfo.IsExist = (bool)reader["IsExist"];
		mailInfo.Money = (int)reader["Money"];
		mailInfo.GiftToken = (int)reader["GiftToken"];
		mailInfo.Receiver = reader["Receiver"].ToString();
		mailInfo.ReceiverID = (int)reader["ReceiverID"];
		mailInfo.Sender = reader["Sender"].ToString();
		mailInfo.SenderID = (int)reader["SenderID"];
		mailInfo.Title = reader["Title"].ToString();
		mailInfo.Type = (int)reader["Type"];
		mailInfo.Remark = (string)reader["Remark"];
		mailInfo.ValidDate = (int)reader["ValidDate"];
		mailInfo.IsRead = (bool)reader["IsRead"];
		mailInfo.SendTime = (DateTime)reader["SendTime"];
		mailInfo.Annex1Name = ((reader["Annex1Name"] == null) ? "" : reader["Annex1Name"].ToString());
		mailInfo.Annex2Name = ((reader["Annex2Name"] == null) ? "" : reader["Annex2Name"].ToString());
		mailInfo.Annex3 = reader["Annex3"].ToString();
		mailInfo.Annex4 = reader["Annex4"].ToString();
		mailInfo.Annex5 = reader["Annex5"].ToString();
		mailInfo.Annex3Name = ((reader["Annex3Name"] == null) ? "" : reader["Annex3Name"].ToString());
		mailInfo.Annex4Name = ((reader["Annex4Name"] == null) ? "" : reader["Annex4Name"].ToString());
		mailInfo.Annex5Name = ((reader["Annex5Name"] == null) ? "" : reader["Annex5Name"].ToString());
		mailInfo.AnnexRemark = ((reader["AnnexRemark"] == null) ? "" : reader["AnnexRemark"].ToString());
		return mailInfo;
	}

	public MailInfo[] GetMailByUserID(int userID)
	{
		List<MailInfo> list = new List<MailInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = userID;
			db.GetReader(ref ResultDataReader, "SP_Mail_ByUserID", array);
			while (ResultDataReader.Read())
			{
				list.Add(InitMail(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
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

	public MailInfo[] GetMailBySenderID(int userID)
	{
		List<MailInfo> list = new List<MailInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = userID;
			db.GetReader(ref ResultDataReader, "SP_Mail_BySenderID", array);
			while (ResultDataReader.Read())
			{
				list.Add(InitMail(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
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

	public MailInfo GetMailSingle(int UserID, int mailID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[2]
			{
				new SqlParameter("@ID", mailID),
				new SqlParameter("@UserID", UserID)
			};
			db.GetReader(ref ResultDataReader, "SP_Mail_Single", sqlParameters);
			if (ResultDataReader.Read())
			{
				return InitMail(ResultDataReader);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
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

	public bool SendMail(MailInfo mail, int zoneId)
	{
		return SendMail(mail, zoneId, useServiceClient: true);
	}

	public bool SendMail(MailInfo mail, int zoneId, bool useServiceClient)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[29];
			array[0] = new SqlParameter("@ID", mail.ID);
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@Annex1", (mail.Annex1 == null) ? "" : mail.Annex1);
			array[2] = new SqlParameter("@Annex2", (mail.Annex2 == null) ? "" : mail.Annex2);
			array[3] = new SqlParameter("@Content", (mail.Content == null) ? "" : mail.Content);
			array[4] = new SqlParameter("@Gold", mail.Gold);
			array[5] = new SqlParameter("@IsExist", true);
			array[6] = new SqlParameter("@Money", mail.Money);
			array[7] = new SqlParameter("@Receiver", (mail.Receiver == null) ? "" : mail.Receiver);
			array[8] = new SqlParameter("@ReceiverID", mail.ReceiverID);
			array[9] = new SqlParameter("@Sender", (mail.Sender == null) ? "" : mail.Sender);
			array[10] = new SqlParameter("@SenderID", mail.SenderID);
			array[11] = new SqlParameter("@Title", (mail.Title == null) ? "" : mail.Title);
			array[12] = new SqlParameter("@IfDelS", false);
			array[13] = new SqlParameter("@IsDelete", false);
			array[14] = new SqlParameter("@IsDelR", false);
			array[15] = new SqlParameter("@IsRead", false);
			array[16] = new SqlParameter("@SendTime", DateTime.Now);
			array[17] = new SqlParameter("@Type", mail.Type);
			array[18] = new SqlParameter("@Annex1Name", (mail.Annex1Name == null) ? "" : mail.Annex1Name);
			array[19] = new SqlParameter("@Annex2Name", (mail.Annex2Name == null) ? "" : mail.Annex2Name);
			array[20] = new SqlParameter("@Annex3", (mail.Annex3 == null) ? "" : mail.Annex3);
			array[21] = new SqlParameter("@Annex4", (mail.Annex4 == null) ? "" : mail.Annex4);
			array[22] = new SqlParameter("@Annex5", (mail.Annex5 == null) ? "" : mail.Annex5);
			array[23] = new SqlParameter("@Annex3Name", (mail.Annex3Name == null) ? "" : mail.Annex3Name);
			array[24] = new SqlParameter("@Annex4Name", (mail.Annex4Name == null) ? "" : mail.Annex4Name);
			array[25] = new SqlParameter("@Annex5Name", (mail.Annex5Name == null) ? "" : mail.Annex5Name);
			array[26] = new SqlParameter("@ValidDate", mail.ValidDate);
			array[27] = new SqlParameter("@AnnexRemark", (mail.AnnexRemark == null) ? "" : mail.AnnexRemark);
			array[28] = new SqlParameter("GiftToken", mail.GiftToken);
			result = db.RunProcedure("SP_Mail_Send", array);
			mail.ID = (int)array[0].Value;
			if (useServiceClient)
			{
				using CenterServiceClient centerServiceClient = new CenterServiceClient();
				centerServiceClient.MailNotice(mail.ReceiverID, zoneId);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
		}
		return result;
	}

	public bool DeleteMail(int UserID, int mailID, out int senderID)
	{
		bool result = false;
		senderID = 0;
		try
		{
			SqlParameter[] array = new SqlParameter[4]
			{
				new SqlParameter("@ID", mailID),
				new SqlParameter("@UserID", UserID),
				new SqlParameter("@SenderID", SqlDbType.Int),
				null
			};
			array[2].Value = senderID;
			array[2].Direction = ParameterDirection.InputOutput;
			array[3] = new SqlParameter("@Result", SqlDbType.Int);
			array[3].Direction = ParameterDirection.ReturnValue;
			result = db.RunProcedure("SP_Mail_Delete", array);
			if ((int)array[3].Value == 0)
			{
				result = true;
				senderID = (int)array[2].Value;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool UpdateMail(MailInfo mail)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[30]
			{
				new SqlParameter("@ID", mail.ID),
				new SqlParameter("@Annex1", (mail.Annex1 == null) ? "" : mail.Annex1),
				new SqlParameter("@Annex2", (mail.Annex2 == null) ? "" : mail.Annex2),
				new SqlParameter("@Content", (mail.Content == null) ? "" : mail.Content),
				new SqlParameter("@Gold", mail.Gold),
				new SqlParameter("@IsExist", mail.IsExist),
				new SqlParameter("@Money", mail.Money),
				new SqlParameter("@Receiver", (mail.Receiver == null) ? "" : mail.Receiver),
				new SqlParameter("@ReceiverID", mail.ReceiverID),
				new SqlParameter("@Sender", (mail.Sender == null) ? "" : mail.Sender),
				new SqlParameter("@SenderID", mail.SenderID),
				new SqlParameter("@Title", (mail.Title == null) ? "" : mail.Title),
				new SqlParameter("@IfDelS", false),
				new SqlParameter("@IsDelete", false),
				new SqlParameter("@IsDelR", false),
				new SqlParameter("@IsRead", mail.IsRead),
				new SqlParameter("@SendTime", mail.SendTime),
				new SqlParameter("@Type", mail.Type),
				new SqlParameter("@OldMoney", mail.Money),
				new SqlParameter("@ValidDate", mail.ValidDate),
				new SqlParameter("@Annex1Name", mail.Annex1Name),
				new SqlParameter("@Annex2Name", mail.Annex2Name),
				new SqlParameter("@Result", SqlDbType.Int),
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[22].Direction = ParameterDirection.ReturnValue;
			array[23] = new SqlParameter("@Annex3", (mail.Annex3 == null) ? "" : mail.Annex3);
			array[24] = new SqlParameter("@Annex4", (mail.Annex4 == null) ? "" : mail.Annex4);
			array[25] = new SqlParameter("@Annex5", (mail.Annex5 == null) ? "" : mail.Annex5);
			array[26] = new SqlParameter("@Annex3Name", (mail.Annex3Name == null) ? "" : mail.Annex3Name);
			array[27] = new SqlParameter("@Annex4Name", (mail.Annex4Name == null) ? "" : mail.Annex4Name);
			array[28] = new SqlParameter("@Annex5Name", (mail.Annex5Name == null) ? "" : mail.Annex5Name);
			array[29] = new SqlParameter("GiftToken", mail.GiftToken);
			db.RunProcedure("SP_Mail_Update", array);
			int num = (int)array[22].Value;
			result = num == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
		}
		return result;
	}

	public bool UpdatePayMail(MailInfo mail, int oldMoney)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[29]
			{
				new SqlParameter("@ID", mail.ID),
				new SqlParameter("@Annex1", (mail.Annex1 == null) ? "" : mail.Annex1),
				new SqlParameter("@Annex2", (mail.Annex2 == null) ? "" : mail.Annex2),
				new SqlParameter("@Content", (mail.Content == null) ? "" : mail.Content),
				new SqlParameter("@Gold", mail.Gold),
				new SqlParameter("@IsExist", mail.IsExist),
				new SqlParameter("@Money", oldMoney),
				new SqlParameter("@Receiver", (mail.Receiver == null) ? "" : mail.Receiver),
				new SqlParameter("@ReceiverID", mail.ReceiverID),
				new SqlParameter("@Sender", (mail.Sender == null) ? "" : mail.Sender),
				new SqlParameter("@SenderID", mail.SenderID),
				new SqlParameter("@Title", (mail.Title == null) ? "" : mail.Title),
				new SqlParameter("@IfDelS", false),
				new SqlParameter("@IsDelete", false),
				new SqlParameter("@IsDelR", false),
				new SqlParameter("@IsRead", mail.IsRead),
				new SqlParameter("@SendTime", mail.SendTime),
				new SqlParameter("@Type", mail.Type),
				new SqlParameter("@ValidDate", mail.ValidDate),
				new SqlParameter("@Annex1Name", mail.Annex1Name),
				new SqlParameter("@Annex2Name", mail.Annex2Name),
				new SqlParameter("@Result", SqlDbType.Int),
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[21].Direction = ParameterDirection.ReturnValue;
			array[22] = new SqlParameter("@Annex3", (mail.Annex3 == null) ? "" : mail.Annex3);
			array[23] = new SqlParameter("@Annex4", (mail.Annex4 == null) ? "" : mail.Annex4);
			array[24] = new SqlParameter("@Annex5", (mail.Annex5 == null) ? "" : mail.Annex5);
			array[25] = new SqlParameter("@Annex3Name", (mail.Annex3Name == null) ? "" : mail.Annex3Name);
			array[26] = new SqlParameter("@Annex4Name", (mail.Annex4Name == null) ? "" : mail.Annex4Name);
			array[27] = new SqlParameter("@Annex5Name", (mail.Annex5Name == null) ? "" : mail.Annex5Name);
			array[28] = new SqlParameter("GiftToken", mail.GiftToken);
			db.RunProcedure("SP_Mail_Update_Pay", array);
			int num = (int)array[21].Value;
			result = num == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
		}
		return result;
	}

	public bool CancelPaymentMail(int userid, int mailID, ref int senderID)
	{
		bool flag = false;
		try
		{
			SqlParameter[] array = new SqlParameter[4]
			{
				new SqlParameter("@userid", userid),
				new SqlParameter("@mailID", mailID),
				new SqlParameter("@senderID", SqlDbType.Int),
				null
			};
			array[2].Value = senderID;
			array[2].Direction = ParameterDirection.InputOutput;
			array[3] = new SqlParameter("@Result", SqlDbType.Int);
			array[3].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Mail_PaymentCancel", array);
			int num = (int)array[3].Value;
			flag = num == 0;
			if (flag)
			{
				senderID = (int)array[2].Value;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return flag;
	}

	public bool ScanMail(ref string noticeUserID)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@NoticeUserID", SqlDbType.NVarChar, 4000)
			};
			array[0].Direction = ParameterDirection.Output;
			db.RunProcedure("SP_Mail_Scan", array);
			noticeUserID = array[0].Value.ToString();
			result = true;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
		}
		return result;
	}

	public bool SendMailAndItem(MailInfo mail, ItemInfo item, int zoneId, ref int returnValue)
	{
		bool flag = false;
		try
		{
			SqlParameter[] array = new SqlParameter[34]
			{
				new SqlParameter("@ItemID", item.ItemID),
				new SqlParameter("@UserID", item.UserID),
				new SqlParameter("@TemplateID", item.TemplateID),
				new SqlParameter("@Place", item.Place),
				new SqlParameter("@AgilityCompose", item.AgilityCompose),
				new SqlParameter("@AttackCompose", item.AttackCompose),
				new SqlParameter("@BeginDate", item.BeginDate),
				new SqlParameter("@Color", (item.Color == null) ? "" : item.Color),
				new SqlParameter("@Count", item.Count),
				new SqlParameter("@DefendCompose", item.DefendCompose),
				new SqlParameter("@IsBinds", item.IsBinds),
				new SqlParameter("@IsExist", item.IsExist),
				new SqlParameter("@IsJudge", item.IsJudge),
				new SqlParameter("@LuckCompose", item.LuckCompose),
				new SqlParameter("@StrengthenLevel", item.StrengthenLevel),
				new SqlParameter("@ValidDate", item.ValidDate),
				new SqlParameter("@BagType", item.BagType),
				new SqlParameter("@ID", mail.ID),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[17].Direction = ParameterDirection.Output;
			array[18] = new SqlParameter("@Annex1", (mail.Annex1 == null) ? "" : mail.Annex1);
			array[19] = new SqlParameter("@Annex2", (mail.Annex2 == null) ? "" : mail.Annex2);
			array[20] = new SqlParameter("@Content", (mail.Content == null) ? "" : mail.Content);
			array[21] = new SqlParameter("@Gold", mail.Gold);
			array[22] = new SqlParameter("@Money", mail.Money);
			array[23] = new SqlParameter("@Receiver", (mail.Receiver == null) ? "" : mail.Receiver);
			array[24] = new SqlParameter("@ReceiverID", mail.ReceiverID);
			array[25] = new SqlParameter("@Sender", (mail.Sender == null) ? "" : mail.Sender);
			array[26] = new SqlParameter("@SenderID", mail.SenderID);
			array[27] = new SqlParameter("@Title", (mail.Title == null) ? "" : mail.Title);
			array[28] = new SqlParameter("@IfDelS", false);
			array[29] = new SqlParameter("@IsDelete", false);
			array[30] = new SqlParameter("@IsDelR", false);
			array[31] = new SqlParameter("@IsRead", false);
			array[32] = new SqlParameter("@SendTime", DateTime.Now);
			array[33] = new SqlParameter("@Result", SqlDbType.Int);
			array[33].Direction = ParameterDirection.ReturnValue;
			flag = db.RunProcedure("SP_Admin_SendUserItem", array);
			returnValue = (int)array[33].Value;
			flag = returnValue == 0;
			if (flag)
			{
				using CenterServiceClient centerServiceClient = new CenterServiceClient();
				centerServiceClient.MailNotice(mail.ReceiverID, zoneId);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
		}
		return flag;
	}

	public bool SendMailAndMoney(MailInfo mail, ref int returnValue)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[18]
			{
				new SqlParameter("@ID", mail.ID),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@Annex1", (mail.Annex1 == null) ? "" : mail.Annex1);
			array[2] = new SqlParameter("@Annex2", (mail.Annex2 == null) ? "" : mail.Annex2);
			array[3] = new SqlParameter("@Content", (mail.Content == null) ? "" : mail.Content);
			array[4] = new SqlParameter("@Gold", mail.Gold);
			array[5] = new SqlParameter("@IsExist", true);
			array[6] = new SqlParameter("@Money", mail.Money);
			array[7] = new SqlParameter("@Receiver", (mail.Receiver == null) ? "" : mail.Receiver);
			array[8] = new SqlParameter("@ReceiverID", mail.ReceiverID);
			array[9] = new SqlParameter("@Sender", (mail.Sender == null) ? "" : mail.Sender);
			array[10] = new SqlParameter("@SenderID", mail.SenderID);
			array[11] = new SqlParameter("@Title", (mail.Title == null) ? "" : mail.Title);
			array[12] = new SqlParameter("@IfDelS", false);
			array[13] = new SqlParameter("@IsDelete", false);
			array[14] = new SqlParameter("@IsDelR", false);
			array[15] = new SqlParameter("@IsRead", false);
			array[16] = new SqlParameter("@SendTime", DateTime.Now);
			array[17] = new SqlParameter("@Result", SqlDbType.Int);
			array[17].Direction = ParameterDirection.ReturnValue;
			result = db.RunProcedure("SP_Admin_SendUserMoney", array);
			returnValue = (int)array[17].Value;
			result = returnValue == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
		}
		return result;
	}

	public int SendMailAndItem(string title, string content, int UserID, int zoneId, int templateID, int count, int validDate, int gold, int money, int StrengthenLevel, int AttackCompose, int DefendCompose, int AgilityCompose, int LuckCompose, bool isBinds)
	{
		MailInfo mailInfo = new MailInfo();
		mailInfo.Annex1 = "";
		mailInfo.Content = title;
		mailInfo.Gold = gold;
		mailInfo.Money = money;
		mailInfo.Receiver = "";
		mailInfo.ReceiverID = UserID;
		mailInfo.Sender = LanguageMgr.GetTranslation("PlayerBussiness.SendMailAndItem.Sender");
		mailInfo.SenderID = 0;
		mailInfo.Title = content;
		ItemInfo itemInfo = new ItemInfo(null);
		itemInfo.AgilityCompose = AgilityCompose;
		itemInfo.AttackCompose = AttackCompose;
		itemInfo.BeginDate = DateTime.Now;
		itemInfo.Color = "";
		itemInfo.DefendCompose = DefendCompose;
		itemInfo.IsDirty = false;
		itemInfo.IsExist = true;
		itemInfo.IsJudge = true;
		itemInfo.LuckCompose = LuckCompose;
		itemInfo.StrengthenLevel = StrengthenLevel;
		itemInfo.TemplateID = templateID;
		itemInfo.ValidDate = validDate;
		itemInfo.Count = count;
		itemInfo.IsBinds = isBinds;
		int returnValue = 1;
		SendMailAndItem(mailInfo, itemInfo, zoneId, ref returnValue);
		return returnValue;
	}

	public int SendMailAndItemByUserName(string title, string content, string userName, int zoneId, int templateID, int count, int validDate, int gold, int money, int StrengthenLevel, int AttackCompose, int DefendCompose, int AgilityCompose, int LuckCompose, bool isBinds)
	{
		PlayerInfo userSingleByUserName = GetUserSingleByUserName(userName);
		if (userSingleByUserName != null)
		{
			return SendMailAndItem(title, content, userSingleByUserName.ID, zoneId, templateID, count, validDate, gold, money, StrengthenLevel, AttackCompose, DefendCompose, AgilityCompose, LuckCompose, isBinds);
		}
		return 2;
	}

	public int SendMailAndItemByNickName(string title, string content, string NickName, int zoneId, int templateID, int count, int validDate, int gold, int money, int StrengthenLevel, int AttackCompose, int DefendCompose, int AgilityCompose, int LuckCompose, bool isBinds)
	{
		PlayerInfo userSingleByNickName = GetUserSingleByNickName(NickName);
		if (userSingleByNickName != null)
		{
			return SendMailAndItem(title, content, userSingleByNickName.ID, zoneId, templateID, count, validDate, gold, money, StrengthenLevel, AttackCompose, DefendCompose, AgilityCompose, LuckCompose, isBinds);
		}
		return 2;
	}

	public int SendMailAndItem(string title, string content, int userID, int zoneId, int gold, int money, string param)
	{
		bool flag = false;
		int num = 1;
		try
		{
			SqlParameter[] array = new SqlParameter[8]
			{
				new SqlParameter("@Title", title),
				new SqlParameter("@Content", content),
				new SqlParameter("@UserID", userID),
				new SqlParameter("@Gold", gold),
				new SqlParameter("@Money", money),
				new SqlParameter("@GiftToken", SqlDbType.BigInt),
				new SqlParameter("@Param", param),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[7].Direction = ParameterDirection.ReturnValue;
			flag = db.RunProcedure("SP_Admin_SendAllItem", array);
			num = (int)array[7].Value;
			if (num == 0)
			{
				using CenterServiceClient centerServiceClient = new CenterServiceClient();
				centerServiceClient.MailNotice(userID, zoneId);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
		}
		return num;
	}

	public int SendMailAndItemByUserName(string title, string content, string userName, int zoneId, int gold, int money, string param)
	{
		PlayerInfo userSingleByUserName = GetUserSingleByUserName(userName);
		if (userSingleByUserName != null)
		{
			return SendMailAndItem(title, content, userSingleByUserName.ID, zoneId, gold, money, param);
		}
		return 2;
	}

	public int SendMailAndItemByNickName(string title, string content, string nickName, int zoneId, int gold, int money, string param)
	{
		PlayerInfo userSingleByNickName = GetUserSingleByNickName(nickName);
		if (userSingleByNickName != null)
		{
			return SendMailAndItem(title, content, userSingleByNickName.ID, zoneId, gold, money, param);
		}
		return 2;
	}

	public Dictionary<int, int> GetFriendsIDAll(int UserID)
	{
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_Users_Friends_All", array);
			while (ResultDataReader.Read())
			{
				if (!dictionary.ContainsKey((int)ResultDataReader["FriendID"]))
				{
					dictionary.Add((int)ResultDataReader["FriendID"], (int)ResultDataReader["Relation"]);
				}
				else
				{
					dictionary[(int)ResultDataReader["FriendID"]] = (int)ResultDataReader["Relation"];
				}
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
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

	public bool AddFriends(FriendInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[7]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@AddDate", DateTime.Now),
				new SqlParameter("@FriendID", info.FriendID),
				new SqlParameter("@IsExist", true),
				new SqlParameter("@Remark", (info.Remark == null) ? "" : info.Remark),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@Relation", info.Relation)
			};
			result = db.RunProcedure("SP_Users_Friends_Add", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
		}
		return result;
	}

	public bool DeleteFriends(int UserID, int FriendID)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[2]
			{
				new SqlParameter("@ID", FriendID),
				new SqlParameter("@UserID", UserID)
			};
			result = db.RunProcedure("SP_Users_Friends_Delete", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool UpdateFriends(int UserID, int FriendID, string Remark)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[3]
			{
				new SqlParameter("@ID", FriendID),
				new SqlParameter("@UserID", UserID),
				new SqlParameter("@Remark", Remark)
			};
			result = db.RunProcedure("SP_Users_Friends_Remark", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public FriendInfo[] GetFriendsAll(int UserID)
	{
		List<FriendInfo> list = new List<FriendInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_Users_Friends", array);
			while (ResultDataReader.Read())
			{
				FriendInfo friendInfo = new FriendInfo();
				friendInfo.AddDate = (DateTime)ResultDataReader["AddDate"];
				friendInfo.FriendID = (int)ResultDataReader["FriendID"];
				friendInfo.ID = (int)ResultDataReader["ID"];
				friendInfo.IsExist = (bool)ResultDataReader["IsExist"];
				friendInfo.NickName = ((ResultDataReader["NickName"] == null) ? "" : ResultDataReader["NickName"].ToString());
				if (!string.IsNullOrEmpty(friendInfo.NickName))
				{
					friendInfo.Grade = (int)ResultDataReader["Grade"];
					friendInfo.Hide = (int)ResultDataReader["Hide"];
					friendInfo.Remark = ((ResultDataReader["Remark"] == null) ? "" : ResultDataReader["Remark"].ToString());
					friendInfo.Sex = (((bool)ResultDataReader["Sex"]) ? 1 : 0);
					friendInfo.State = (int)ResultDataReader["State"];
					friendInfo.Colors = ((ResultDataReader["Colors"] == null) ? "" : ResultDataReader["Colors"].ToString());
					friendInfo.Style = ((ResultDataReader["Style"] == null) ? "" : ResultDataReader["Style"].ToString());
					friendInfo.UserID = (int)ResultDataReader["UserID"];
					friendInfo.ConsortiaName = ((ResultDataReader["ConsortiaName"] == null) ? "" : ResultDataReader["ConsortiaName"].ToString());
					friendInfo.Offer = (int)ResultDataReader["Offer"];
					friendInfo.Win = (int)ResultDataReader["Win"];
					friendInfo.Total = (int)ResultDataReader["Total"];
					friendInfo.Escape = (int)ResultDataReader["Escape"];
					friendInfo.Relation = (int)ResultDataReader["Relation"];
					friendInfo.Repute = (int)ResultDataReader["Repute"];
					friendInfo.UserName = ((ResultDataReader["UserName"] == null) ? "" : ResultDataReader["UserName"].ToString());
					friendInfo.DutyName = ((ResultDataReader["DutyName"] == null) ? "" : ResultDataReader["DutyName"].ToString());
					friendInfo.Nimbus = (int)ResultDataReader["Nimbus"];
					friendInfo.typeVIP = Convert.ToByte(ResultDataReader["typeVIP"]);
					friendInfo.VIPLevel = (int)ResultDataReader["VIPLevel"];
					friendInfo.IsMarried = (bool)ResultDataReader["IsMarried"];
					friendInfo.LastDate = (DateTime)ResultDataReader["LastDate"];
					friendInfo.FightPower = (int)ResultDataReader["FightPower"];
					list.Add(friendInfo);
				}
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init " + UserID, exception);
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

	public ArrayList GetFriendsGood(string UserName)
	{
		ArrayList arrayList = new ArrayList();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserName", SqlDbType.NVarChar)
			};
			array[0].Value = UserName;
			db.GetReader(ref ResultDataReader, "SP_Users_Friends_Good", array);
			while (ResultDataReader.Read())
			{
				arrayList.Add((ResultDataReader["UserName"] == null) ? "" : ResultDataReader["UserName"].ToString());
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return arrayList;
	}

	public FriendInfo[] GetFriendsBbs(string condictArray)
	{
		List<FriendInfo> list = new List<FriendInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@SearchUserName", SqlDbType.NVarChar, 4000)
			};
			array[0].Value = condictArray;
			db.GetReader(ref ResultDataReader, "SP_Users_FriendsBbs", array);
			while (ResultDataReader.Read())
			{
				FriendInfo friendInfo = new FriendInfo();
				friendInfo.NickName = ((ResultDataReader["NickName"] == null) ? "" : ResultDataReader["NickName"].ToString());
				friendInfo.UserID = (int)ResultDataReader["UserID"];
				friendInfo.UserName = ((ResultDataReader["UserName"] == null) ? "" : ResultDataReader["UserName"].ToString());
				friendInfo.IsExist = (int)ResultDataReader["UserID"] > 0;
				list.Add(friendInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
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

	public QuestDataInfo[] GetUserQuest(int userID)
	{
		List<QuestDataInfo> list = new List<QuestDataInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = userID;
			db.GetReader(ref ResultDataReader, "SP_QuestData_All", array);
			while (ResultDataReader.Read())
			{
				QuestDataInfo questDataInfo = new QuestDataInfo();
				questDataInfo.CompletedDate = (DateTime)ResultDataReader["CompletedDate"];
				questDataInfo.IsComplete = (bool)ResultDataReader["IsComplete"];
				questDataInfo.Condition1 = (int)ResultDataReader["Condition1"];
				questDataInfo.Condition2 = (int)ResultDataReader["Condition2"];
				questDataInfo.Condition3 = (int)ResultDataReader["Condition3"];
				questDataInfo.Condition4 = (int)ResultDataReader["Condition4"];
				questDataInfo.Condition5 = (int)ResultDataReader["Condition5"];
				questDataInfo.Condition6 = (int)ResultDataReader["Condition6"];
				questDataInfo.Condition7 = (int)ResultDataReader["Condition7"];
				questDataInfo.Condition8 = (int)ResultDataReader["Condition8"];
				questDataInfo.QuestID = (int)ResultDataReader["QuestID"];
				questDataInfo.UserID = (int)ResultDataReader["UserId"];
				questDataInfo.IsExist = (bool)ResultDataReader["IsExist"];
				questDataInfo.RandDobule = (int)ResultDataReader["RandDobule"];
				questDataInfo.RepeatFinish = (int)ResultDataReader["RepeatFinish"];
				questDataInfo.QuestLevel = (int)ResultDataReader["QuestLevel"];
				list.Add(questDataInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
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

	public bool UpdateDbQuestDataInfo(QuestDataInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[16]
			{
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@QuestID", info.QuestID),
				new SqlParameter("@CompletedDate", info.CompletedDate),
				new SqlParameter("@IsComplete", info.IsComplete),
				new SqlParameter("@Condition1", info.Condition1),
				new SqlParameter("@Condition2", info.Condition2),
				new SqlParameter("@Condition3", info.Condition3),
				new SqlParameter("@Condition4", info.Condition4),
				new SqlParameter("@Condition5", info.Condition5),
				new SqlParameter("@Condition6", info.Condition6),
				new SqlParameter("@Condition7", info.Condition7),
				new SqlParameter("@Condition8", info.Condition8),
				new SqlParameter("@IsExist", info.IsExist),
				new SqlParameter("@RepeatFinish", info.RepeatFinish),
				new SqlParameter("@RandDobule", info.RandDobule),
				new SqlParameter("@QuestLevel", info.QuestLevel)
			};
			result = db.RunProcedure("SP_QuestData_Add", sqlParameters);
			info.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
		}
		return result;
	}

	public BufferInfo[] GetUserBuffer(int userID)
	{
		List<BufferInfo> list = new List<BufferInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = userID;
			db.GetReader(ref ResultDataReader, "SP_User_Buff_All", array);
			while (ResultDataReader.Read())
			{
				BufferInfo bufferInfo = new BufferInfo();
				bufferInfo.BeginDate = (DateTime)ResultDataReader["BeginDate"];
				bufferInfo.Data = ((ResultDataReader["Data"] == null) ? "" : ResultDataReader["Data"].ToString());
				bufferInfo.Type = (int)ResultDataReader["Type"];
				bufferInfo.UserID = (int)ResultDataReader["UserID"];
				bufferInfo.ValidDate = (int)ResultDataReader["ValidDate"];
				bufferInfo.Value = (int)ResultDataReader["Value"];
				bufferInfo.IsExist = (bool)ResultDataReader["IsExist"];
				bufferInfo.ValidCount = (int)ResultDataReader["ValidCount"];
				bufferInfo.TemplateID = (int)ResultDataReader["TemplateID"];
				bufferInfo.IsDirty = false;
				list.Add(bufferInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
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

	public ConsortiaBufferInfo[] GetUserConsortiaBuffer(int ConsortiaID)
	{
		List<ConsortiaBufferInfo> list = new List<ConsortiaBufferInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@ConsortiaID", SqlDbType.Int, 4)
			};
			array[0].Value = ConsortiaID;
			db.GetReader(ref ResultDataReader, "SP_User_Consortia_Buff_All", array);
			while (ResultDataReader.Read())
			{
				ConsortiaBufferInfo consortiaBufferInfo = new ConsortiaBufferInfo();
				consortiaBufferInfo.ConsortiaID = (int)ResultDataReader["ConsortiaID"];
				consortiaBufferInfo.BufferID = (int)ResultDataReader["BufferID"];
				consortiaBufferInfo.IsOpen = (bool)ResultDataReader["IsOpen"];
				consortiaBufferInfo.BeginDate = (DateTime)ResultDataReader["BeginDate"];
				consortiaBufferInfo.ValidDate = (int)ResultDataReader["ValidDate"];
				consortiaBufferInfo.Type = (int)ResultDataReader["Type"];
				consortiaBufferInfo.Value = (int)ResultDataReader["Value"];
				list.Add(consortiaBufferInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init SP_User_Consortia_Buff_All", exception);
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

	public ConsortiaBufferInfo GetUserConsortiaBufferSingle(int ID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@ID", SqlDbType.Int, 4)
			};
			array[0].Value = ID;
			db.GetReader(ref ResultDataReader, "SP_User_Consortia_Buff_Single", array);
			if (ResultDataReader.Read())
			{
				ConsortiaBufferInfo consortiaBufferInfo = new ConsortiaBufferInfo();
				consortiaBufferInfo.ConsortiaID = (int)ResultDataReader["ConsortiaID"];
				consortiaBufferInfo.BufferID = (int)ResultDataReader["BufferID"];
				consortiaBufferInfo.IsOpen = (bool)ResultDataReader["IsOpen"];
				consortiaBufferInfo.BeginDate = (DateTime)ResultDataReader["BeginDate"];
				consortiaBufferInfo.ValidDate = (int)ResultDataReader["ValidDate"];
				consortiaBufferInfo.Type = (int)ResultDataReader["Type"];
				consortiaBufferInfo.Value = (int)ResultDataReader["Value"];
				return consortiaBufferInfo;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init SP_User_Consortia_Buff_Single", exception);
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

	public bool SaveBuffer(BufferInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[9]
			{
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@Type", info.Type),
				new SqlParameter("@BeginDate", info.BeginDate),
				new SqlParameter("@Data", (info.Data == null) ? "" : info.Data),
				new SqlParameter("@IsExist", info.IsExist),
				new SqlParameter("@ValidDate", info.ValidDate),
				new SqlParameter("@ValidCount", info.ValidCount),
				new SqlParameter("@Value", info.Value),
				new SqlParameter("@TemplateID", info.TemplateID)
			};
			result = db.RunProcedure("SP_User_Buff_Add", sqlParameters);
			info.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
		}
		return result;
	}

	public bool SaveConsortiaBuffer(ConsortiaBufferInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[7]
			{
				new SqlParameter("@ConsortiaID", info.ConsortiaID),
				new SqlParameter("@BufferID", info.BufferID),
				new SqlParameter("@IsOpen", info.IsOpen),
				new SqlParameter("@BeginDate", info.BeginDate),
				new SqlParameter("@ValidDate", info.ValidDate),
				new SqlParameter("@Type ", info.Type),
				new SqlParameter("@Value", info.Value)
			};
			result = db.RunProcedure("SP_User_Consortia_Buff_Add", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
		}
		return result;
	}

	public LuckStarRewardRecordInfo[] GetLuckStarTopTenRank(int MinUseNum)
	{
		List<LuckStarRewardRecordInfo> list = new List<LuckStarRewardRecordInfo>();
		SqlDataReader ResultDataReader = null;
		int num = 1;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@MinUseNum", MinUseNum)
			};
			db.GetReader(ref ResultDataReader, "SP_LuckStar_Reward_Record_All", sqlParameters);
			while (ResultDataReader.Read())
			{
				LuckStarRewardRecordInfo luckStarRewardRecordInfo = new LuckStarRewardRecordInfo();
				luckStarRewardRecordInfo.PlayerID = (int)ResultDataReader["UserID"];
				luckStarRewardRecordInfo.useStarNum = (int)ResultDataReader["useStarNum"];
				luckStarRewardRecordInfo.isVip = (int)ResultDataReader["isVip"];
				luckStarRewardRecordInfo.nickName = (string)ResultDataReader["nickName"];
				luckStarRewardRecordInfo.rank = num;
				list.Add(luckStarRewardRecordInfo);
				num++;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init SP_LuckStar_Reward_Record_All", exception);
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

	public bool SaveLuckStarRankInfo(LuckStarRewardRecordInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[4]
			{
				new SqlParameter("@UserID", info.PlayerID),
				new SqlParameter("@useStarNum", info.useStarNum),
				new SqlParameter("@nickName", info.nickName),
				new SqlParameter("@isVip", info.isVip)
			};
			result = db.RunProcedure("SP_LuckStar_Rank_Info_Add", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
		}
		return result;
	}

	public bool ResetLuckStarRank()
	{
		bool result = false;
		try
		{
			result = db.RunProcedure("SP_Reset_LuckStar_Rank_Info");
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init ResetLuckStar", exception);
			}
		}
		return result;
	}

	public bool AddChargeMoney(string chargeID, string userName, int money, string payWay, decimal needMoney, out int userID, ref int isResult, DateTime date, string IP, string nickName)
	{
		bool result = false;
		userID = 0;
		try
		{
			SqlParameter[] array = new SqlParameter[10]
			{
				new SqlParameter("@ChargeID", chargeID),
				new SqlParameter("@UserName", userName),
				new SqlParameter("@Money", money),
				new SqlParameter("@Date", date.ToString("yyyy-MM-dd HH:mm:ss")),
				new SqlParameter("@PayWay", payWay),
				new SqlParameter("@NeedMoney", needMoney),
				new SqlParameter("@UserID", userID),
				null,
				null,
				null
			};
			array[6].Direction = ParameterDirection.InputOutput;
			array[7] = new SqlParameter("@Result", SqlDbType.Int);
			array[7].Direction = ParameterDirection.ReturnValue;
			array[8] = new SqlParameter("@IP", IP);
			array[9] = new SqlParameter("@NickName", nickName);
			result = db.RunProcedure("SP_Charge_Money_Add", array);
			userID = (int)array[6].Value;
			isResult = (int)array[7].Value;
			result = isResult == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool ChargeToUser(string userName, ref int money, string nickName)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[3]
			{
				new SqlParameter("@UserName", userName),
				new SqlParameter("@money", SqlDbType.Int),
				null
			};
			array[1].Direction = ParameterDirection.Output;
			array[2] = new SqlParameter("@NickName", nickName);
			result = db.RunProcedure("SP_Charge_To_User", array);
			money = (int)array[1].Value;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public ChargeRecordInfo[] GetChargeRecordInfo(DateTime date, int SaveRecordSecond)
	{
		List<ChargeRecordInfo> list = new List<ChargeRecordInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[2]
			{
				new SqlParameter("@Date", date.ToString("yyyy-MM-dd HH:mm:ss")),
				new SqlParameter("@Second", SaveRecordSecond)
			};
			db.GetReader(ref ResultDataReader, "SP_Charge_Record", sqlParameters);
			while (ResultDataReader.Read())
			{
				ChargeRecordInfo chargeRecordInfo = new ChargeRecordInfo();
				chargeRecordInfo.BoyTotalPay = (int)ResultDataReader["BoyTotalPay"];
				chargeRecordInfo.GirlTotalPay = (int)ResultDataReader["GirlTotalPay"];
				chargeRecordInfo.PayWay = ((ResultDataReader["PayWay"] == null) ? "" : ResultDataReader["PayWay"].ToString());
				chargeRecordInfo.TotalBoy = (int)ResultDataReader["TotalBoy"];
				chargeRecordInfo.TotalGirl = (int)ResultDataReader["TotalGirl"];
				list.Add(chargeRecordInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
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

	public AuctionInfo GetAuctionSingle(int auctionID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@AuctionID", auctionID)
			};
			db.GetReader(ref ResultDataReader, "SP_Auction_Single", sqlParameters);
			if (ResultDataReader.Read())
			{
				return InitAuctionInfo(ResultDataReader);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
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

	public bool AddAuction(AuctionInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[18]
			{
				new SqlParameter("@AuctionID", info.AuctionID),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@AuctioneerID", info.AuctioneerID);
			array[2] = new SqlParameter("@AuctioneerName", (info.AuctioneerName == null) ? "" : info.AuctioneerName);
			array[3] = new SqlParameter("@BeginDate", info.BeginDate);
			array[4] = new SqlParameter("@BuyerID", info.BuyerID);
			array[5] = new SqlParameter("@BuyerName", (info.BuyerName == null) ? "" : info.BuyerName);
			array[6] = new SqlParameter("@IsExist", info.IsExist);
			array[7] = new SqlParameter("@ItemID", info.ItemID);
			array[8] = new SqlParameter("@Mouthful", info.Mouthful);
			array[9] = new SqlParameter("@PayType", info.PayType);
			array[10] = new SqlParameter("@Price", info.Price);
			array[11] = new SqlParameter("@Rise", info.Rise);
			array[12] = new SqlParameter("@ValidDate", info.ValidDate);
			array[13] = new SqlParameter("@TemplateID", info.TemplateID);
			array[14] = new SqlParameter("Name", info.Name);
			array[15] = new SqlParameter("Category", info.Category);
			array[16] = new SqlParameter("Random", info.Random);
			array[17] = new SqlParameter("goodsCount", info.goodsCount);
			result = db.RunProcedure("SP_Auction_Add", array);
			info.AuctionID = (int)array[0].Value;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
		}
		return result;
	}

	public bool UpdateAuction(AuctionInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[16]
			{
				new SqlParameter("@AuctionID", info.AuctionID),
				new SqlParameter("@AuctioneerID", info.AuctioneerID),
				new SqlParameter("@AuctioneerName", (info.AuctioneerName == null) ? "" : info.AuctioneerName),
				new SqlParameter("@BeginDate", info.BeginDate),
				new SqlParameter("@BuyerID", info.BuyerID),
				new SqlParameter("@BuyerName", (info.BuyerName == null) ? "" : info.BuyerName),
				new SqlParameter("@IsExist", info.IsExist),
				new SqlParameter("@ItemID", info.ItemID),
				new SqlParameter("@Mouthful", info.Mouthful),
				new SqlParameter("@PayType", info.PayType),
				new SqlParameter("@Price", info.Price),
				new SqlParameter("@Rise", info.Rise),
				new SqlParameter("@ValidDate", info.ValidDate),
				new SqlParameter("Name", info.Name),
				new SqlParameter("Category", info.Category),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[15].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Auction_Update", array);
			int num = (int)array[15].Value;
			result = num == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
		}
		return result;
	}

	public bool DeleteAuction(int auctionID, int userID, ref string msg)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[3]
			{
				new SqlParameter("@AuctionID", auctionID),
				new SqlParameter("@UserID", userID),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[2].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Auction_Delete", array);
			int num = (int)array[2].Value;
			result = num == 0;
			switch (num)
			{
			case 0:
				msg = LanguageMgr.GetTranslation("PlayerBussiness.DeleteAuction.Msg1");
				break;
			case 1:
				msg = LanguageMgr.GetTranslation("PlayerBussiness.DeleteAuction.Msg2");
				break;
			case 2:
				msg = LanguageMgr.GetTranslation("PlayerBussiness.DeleteAuction.Msg3");
				break;
			default:
				msg = LanguageMgr.GetTranslation("PlayerBussiness.DeleteAuction.Msg4");
				break;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public AuctionInfo[] GetAuctionPage(int page, string name, int type, int pay, ref int total, int userID, int buyID, int order, bool sort, int size, string AuctionIDs)
	{
		List<AuctionInfo> list = new List<AuctionInfo>();
		try
		{
			string text = " IsExist=1 ";
			if (!string.IsNullOrEmpty(name))
			{
				text = text + " and Name like '%" + name + "%' ";
			}
			if (type != -1)
			{
				switch (type)
				{
				case 0:
				case 1:
				case 2:
				case 3:
				case 4:
				case 5:
				case 6:
				case 7:
				case 8:
				case 9:
				case 10:
				case 11:
				case 12:
				case 13:
				case 14:
				case 15:
				case 16:
				case 17:
					text = text + " and Category =" + type + " ";
					break;
				case 21:
					text += " and Category in(1,2,5,8,9) ";
					break;
				case 22:
					text += " and Category in(13,15,6,4,3) ";
					break;
				case 23:
					text += " and Category in(16,11,10) ";
					break;
				case 24:
					text += " and Category in(8,9) ";
					break;
				case 25:
					text += " and Category in (7,17) ";
					break;
				case 26:
					text += " and TemplateId>=311000 and TemplateId<=313999";
					break;
				case 27:
					text += " and TemplateId>=311000 and TemplateId<=311999 ";
					break;
				case 28:
					text += " and TemplateId>=312000 and TemplateId<=312999 ";
					break;
				case 29:
					text += " and TemplateId>=313000 and TempLateId<=313999";
					break;
				case 1100:
					text += " and TemplateID in (11019,11021,11022,11023) ";
					break;
				case 1101:
					text += " and TemplateID='11019' ";
					break;
				case 1102:
					text += " and TemplateID='11021' ";
					break;
				case 1103:
					text += " and TemplateID='11022' ";
					break;
				case 1104:
					text += " and TemplateID='11023' ";
					break;
				case 1105:
					text += " and TemplateID in (11001,11002,11003,11004,11005,11006,11007,11008,11009,11010,11011,11012,11013,11014,11015,11016) ";
					break;
				case 1106:
					text += " and TemplateID in (11001,11002,11003,11004) ";
					break;
				case 1107:
					text += " and TemplateID in (11005,11006,11007,11008) ";
					break;
				case 1108:
					text += " and TemplateID in (11009,11010,11011,11012) ";
					break;
				case 1109:
					text += " and TemplateID in (11013,11014,11015,11016) ";
					break;
				}
			}
			if (pay != -1)
			{
				text = text + " and PayType =" + pay + " ";
			}
			if (userID != -1)
			{
				text = text + " and AuctioneerID =" + userID + " ";
			}
			if (buyID != -1)
			{
				text = text + " and (BuyerID =" + buyID + " or AuctionID in (" + AuctionIDs + ")) ";
			}
			string text2 = "Category,Name,Price,dd,AuctioneerID";
			switch (order)
			{
			case 0:
				text2 = "Name";
				break;
			case 2:
				text2 = "dd";
				break;
			case 3:
				text2 = "AuctioneerName";
				break;
			case 4:
				text2 = "Price";
				break;
			case 5:
				text2 = "BuyerName";
				break;
			}
			text2 += (sort ? " desc" : "");
			text2 += ",AuctionID ";
			SqlParameter[] array = new SqlParameter[8]
			{
				new SqlParameter("@QueryStr", "V_Auction_Scan"),
				new SqlParameter("@QueryWhere", text),
				new SqlParameter("@PageSize", size),
				new SqlParameter("@PageCurrent", page),
				new SqlParameter("@FdShow", "*"),
				new SqlParameter("@FdOrder", text2),
				new SqlParameter("@FdKey", "AuctionID"),
				new SqlParameter("@TotalRow", total)
			};
			array[7].Direction = ParameterDirection.Output;
			DataTable dataTable = db.GetDataTable("Auction", "SP_CustomPage", array);
			total = (int)array[7].Value;
			foreach (DataRow row in dataTable.Rows)
			{
				AuctionInfo auctionInfo = new AuctionInfo();
				auctionInfo.AuctioneerID = (int)row["AuctioneerID"];
				auctionInfo.AuctioneerName = row["AuctioneerName"].ToString();
				auctionInfo.AuctionID = (int)row["AuctionID"];
				auctionInfo.BeginDate = (DateTime)row["BeginDate"];
				auctionInfo.BuyerID = (int)row["BuyerID"];
				auctionInfo.BuyerName = row["BuyerName"].ToString();
				auctionInfo.Category = (int)row["Category"];
				auctionInfo.IsExist = (bool)row["IsExist"];
				auctionInfo.ItemID = (int)row["ItemID"];
				auctionInfo.Name = row["Name"].ToString();
				auctionInfo.Mouthful = (int)row["Mouthful"];
				auctionInfo.PayType = (int)row["PayType"];
				auctionInfo.Price = (int)row["Price"];
				auctionInfo.Rise = (int)row["Rise"];
				auctionInfo.ValidDate = (int)row["ValidDate"];
				auctionInfo.goodsCount = (int)row["goodsCount"];
				list.Add(auctionInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
		}
		return list.ToArray();
	}

	public AuctionInfo InitAuctionInfo(SqlDataReader reader)
	{
		AuctionInfo auctionInfo = new AuctionInfo();
		auctionInfo.AuctioneerID = (int)reader["AuctioneerID"];
		auctionInfo.AuctioneerName = ((reader["AuctioneerName"] == null) ? "" : reader["AuctioneerName"].ToString());
		auctionInfo.AuctionID = (int)reader["AuctionID"];
		auctionInfo.BeginDate = (DateTime)reader["BeginDate"];
		auctionInfo.BuyerID = (int)reader["BuyerID"];
		auctionInfo.BuyerName = ((reader["BuyerName"] == null) ? "" : reader["BuyerName"].ToString());
		auctionInfo.IsExist = (bool)reader["IsExist"];
		auctionInfo.ItemID = (int)reader["ItemID"];
		auctionInfo.Mouthful = (int)reader["Mouthful"];
		auctionInfo.PayType = (int)reader["PayType"];
		auctionInfo.Price = (int)reader["Price"];
		auctionInfo.Rise = (int)reader["Rise"];
		auctionInfo.ValidDate = (int)reader["ValidDate"];
		auctionInfo.Name = reader["Name"].ToString();
		auctionInfo.Category = (int)reader["Category"];
		auctionInfo.goodsCount = (int)reader["goodsCount"];
		auctionInfo.TemplateID = (int)reader["TemplateID"];
		return auctionInfo;
	}

	public bool ScanAuction(ref string noticeUserID)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@NoticeUserID", SqlDbType.NVarChar, 4000)
			};
			array[0].Direction = ParameterDirection.Output;
			db.RunProcedure("SP_Auction_Scan", array);
			noticeUserID = array[0].Value.ToString();
			result = true;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
		}
		return result;
	}

	public bool AddMarryInfo(MarryInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[5]
			{
				new SqlParameter("@ID", info.ID),
				null,
				null,
				null,
				null
			};
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@UserID", info.UserID);
			array[2] = new SqlParameter("@IsPublishEquip", info.IsPublishEquip);
			array[3] = new SqlParameter("@Introduction", info.Introduction);
			array[4] = new SqlParameter("@RegistTime", info.RegistTime);
			result = db.RunProcedure("SP_MarryInfo_Add", array);
			info.ID = (int)array[0].Value;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("AddMarryInfo", exception);
			}
		}
		return result;
	}

	public bool DeleteMarryInfo(int ID, int userID, ref string msg)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[3]
			{
				new SqlParameter("@ID", ID),
				new SqlParameter("@UserID", userID),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[2].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_MarryInfo_Delete", array);
			int num = (int)array[2].Value;
			result = num == 0;
			if (num == 0)
			{
				msg = LanguageMgr.GetTranslation("PlayerBussiness.DeleteAuction.Succeed");
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("DeleteAuction", exception);
			}
		}
		return result;
	}

	public MarryInfo GetMarryInfoSingle(int ID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@ID", ID)
			};
			db.GetReader(ref ResultDataReader, "SP_MarryInfo_Single", sqlParameters);
			if (ResultDataReader.Read())
			{
				MarryInfo marryInfo = new MarryInfo();
				marryInfo.ID = (int)ResultDataReader["ID"];
				marryInfo.UserID = (int)ResultDataReader["UserID"];
				marryInfo.IsPublishEquip = (bool)ResultDataReader["IsPublishEquip"];
				marryInfo.Introduction = ResultDataReader["Introduction"].ToString();
				marryInfo.RegistTime = (DateTime)ResultDataReader["RegistTime"];
				return marryInfo;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetMarryInfoSingle", exception);
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

	public bool UpdateMarryInfo(MarryInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[6]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@IsPublishEquip", info.IsPublishEquip),
				new SqlParameter("@Introduction", info.Introduction),
				new SqlParameter("@RegistTime", info.RegistTime),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[5].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_MarryInfo_Update", array);
			int num = (int)array[5].Value;
			result = num == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public MarryInfo[] GetMarryInfoPage(int page, string name, bool sex, int size, ref int total)
	{
		List<MarryInfo> list = new List<MarryInfo>();
		try
		{
			string text = "";
			text = ((!sex) ? " IsExist=1 and Sex=0 and UserExist=1" : " IsExist=1 and Sex=1 and UserExist=1");
			if (!string.IsNullOrEmpty(name))
			{
				text = text + " and NickName like '%" + name + "%' ";
			}
			string value = "State desc,IsMarried";
			SqlParameter[] array = new SqlParameter[8]
			{
				new SqlParameter("@QueryStr", "V_Sys_Marry_Info"),
				new SqlParameter("@QueryWhere", text),
				new SqlParameter("@PageSize", size),
				new SqlParameter("@PageCurrent", page),
				new SqlParameter("@FdShow", "*"),
				new SqlParameter("@FdOrder", value),
				new SqlParameter("@FdKey", "ID"),
				new SqlParameter("@TotalRow", total)
			};
			array[7].Direction = ParameterDirection.Output;
			DataTable dataTable = db.GetDataTable("V_Sys_Marry_Info", "SP_CustomPage", array);
			total = (int)array[7].Value;
			foreach (DataRow row in dataTable.Rows)
			{
				MarryInfo marryInfo = new MarryInfo();
				marryInfo.ID = (int)row["ID"];
				marryInfo.UserID = (int)row["UserID"];
				marryInfo.IsPublishEquip = (bool)row["IsPublishEquip"];
				marryInfo.Introduction = row["Introduction"].ToString();
				marryInfo.NickName = row["NickName"].ToString();
				marryInfo.IsConsortia = (bool)row["IsConsortia"];
				marryInfo.ConsortiaID = (int)row["ConsortiaID"];
				marryInfo.Sex = (bool)row["Sex"];
				marryInfo.Win = (int)row["Win"];
				marryInfo.Total = (int)row["Total"];
				marryInfo.Escape = (int)row["Escape"];
				marryInfo.GP = (int)row["GP"];
				marryInfo.Honor = row["Honor"].ToString();
				marryInfo.Style = row["Style"].ToString();
				marryInfo.Colors = row["Colors"].ToString();
				marryInfo.Hide = (int)row["Hide"];
				marryInfo.Grade = (int)row["Grade"];
				marryInfo.State = (int)row["State"];
				marryInfo.Repute = (int)row["Repute"];
				marryInfo.Skin = row["Skin"].ToString();
				marryInfo.Offer = (int)row["Offer"];
				marryInfo.IsMarried = (bool)row["IsMarried"];
				marryInfo.ConsortiaName = row["ConsortiaName"].ToString();
				marryInfo.DutyName = row["DutyName"].ToString();
				marryInfo.Nimbus = (int)row["Nimbus"];
				marryInfo.FightPower = (int)row["FightPower"];
				marryInfo.typeVIP = Convert.ToByte(row["typeVIP"]);
				marryInfo.VIPLevel = (int)row["VIPLevel"];
				list.Add(marryInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
		}
		return list.ToArray();
	}

	public bool InsertPlayerMarryApply(MarryApplyInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[7]
			{
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@ApplyUserID", info.ApplyUserID),
				new SqlParameter("@ApplyUserName", info.ApplyUserName),
				new SqlParameter("@ApplyType", info.ApplyType),
				new SqlParameter("@ApplyResult", info.ApplyResult),
				new SqlParameter("@LoveProclamation", info.LoveProclamation),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[6].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Insert_Marry_Apply", array);
			result = (int)array[6].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("InsertPlayerMarryApply", exception);
			}
		}
		return result;
	}

	public bool UpdatePlayerMarryApply(int UserID, string loveProclamation, bool isExist)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[4]
			{
				new SqlParameter("@UserID", UserID),
				new SqlParameter("@LoveProclamation", loveProclamation),
				new SqlParameter("@isExist", isExist),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[3].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Update_Marry_Apply", array);
			result = (int)array[3].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("UpdatePlayerMarryApply", exception);
			}
		}
		return result;
	}

	public MarryApplyInfo[] GetPlayerMarryApply(int UserID)
	{
		SqlDataReader ResultDataReader = null;
		List<MarryApplyInfo> list = new List<MarryApplyInfo>();
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@UserID", UserID)
			};
			db.GetReader(ref ResultDataReader, "SP_Get_Marry_Apply", sqlParameters);
			while (ResultDataReader.Read())
			{
				MarryApplyInfo marryApplyInfo = new MarryApplyInfo();
				marryApplyInfo.UserID = (int)ResultDataReader["UserID"];
				marryApplyInfo.ApplyUserID = (int)ResultDataReader["ApplyUserID"];
				marryApplyInfo.ApplyUserName = ResultDataReader["ApplyUserName"].ToString();
				marryApplyInfo.ApplyType = (int)ResultDataReader["ApplyType"];
				marryApplyInfo.ApplyResult = (bool)ResultDataReader["ApplyResult"];
				marryApplyInfo.LoveProclamation = ResultDataReader["LoveProclamation"].ToString();
				marryApplyInfo.ID = (int)ResultDataReader["Id"];
				list.Add(marryApplyInfo);
			}
			return list.ToArray();
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetPlayerMarryApply", exception);
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

	public bool InsertMarryRoomInfo(MarryRoomInfo info)
	{
		bool flag = false;
		try
		{
			SqlParameter[] array = new SqlParameter[22];
			array[0] = new SqlParameter("@ID", info.ID);
			array[0].Direction = ParameterDirection.InputOutput;
			array[1] = new SqlParameter("@Name", info.Name);
			array[2] = new SqlParameter("@PlayerID", info.PlayerID);
			array[3] = new SqlParameter("@PlayerName", info.PlayerName);
			array[4] = new SqlParameter("@GroomID", info.GroomID);
			array[5] = new SqlParameter("@GroomName", info.GroomName);
			array[6] = new SqlParameter("@BrideID", info.BrideID);
			array[7] = new SqlParameter("@BrideName", info.BrideName);
			array[8] = new SqlParameter("@Pwd", info.Pwd);
			array[9] = new SqlParameter("@AvailTime", info.AvailTime);
			array[10] = new SqlParameter("@MaxCount", info.MaxCount);
			array[11] = new SqlParameter("@GuestInvite", info.GuestInvite);
			array[12] = new SqlParameter("@MapIndex", info.MapIndex);
			array[13] = new SqlParameter("@BeginTime", info.BeginTime);
			array[14] = new SqlParameter("@BreakTime", info.BreakTime);
			array[15] = new SqlParameter("@RoomIntroduction", info.RoomIntroduction);
			array[16] = new SqlParameter("@ServerID", info.ServerID);
			array[17] = new SqlParameter("@IsHymeneal", info.IsHymeneal);
			array[18] = new SqlParameter("@IsGunsaluteUsed", info.IsGunsaluteUsed);
			array[19] = new SqlParameter("@Result", SqlDbType.Int);
			array[20] = new SqlParameter("@MerryActivity", info.MerryActivity);
			array[21] = new SqlParameter("@SeniorType", info.SeniorType);
			array[19].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Insert_Marry_Room_Info", array);
			flag = (int)array[19].Value == 0;
			if (flag)
			{
				info.ID = (int)array[0].Value;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("InsertMarryRoomInfo", exception);
			}
		}
		return flag;
	}

	public bool UpdateMarryRoomInfo(MarryRoomInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[9]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@AvailTime", info.AvailTime),
				new SqlParameter("@BreakTime", info.BreakTime),
				new SqlParameter("@roomIntroduction", info.RoomIntroduction),
				new SqlParameter("@isHymeneal", info.IsHymeneal),
				new SqlParameter("@Name", info.Name),
				new SqlParameter("@Pwd", info.Pwd),
				new SqlParameter("@IsGunsaluteUsed", info.IsGunsaluteUsed),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[8].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Update_Marry_Room_Info", array);
			result = (int)array[8].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("UpdateMarryRoomInfo", exception);
			}
		}
		return result;
	}

	public bool DisposeMarryRoomInfo(int ID)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[2]
			{
				new SqlParameter("@ID", ID),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[1].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Dispose_Marry_Room_Info", array);
			result = (int)array[1].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("DisposeMarryRoomInfo", exception);
			}
		}
		return result;
	}

	public MarryRoomInfo[] GetMarryRoomInfo()
	{
		SqlDataReader ResultDataReader = null;
		List<MarryRoomInfo> list = new List<MarryRoomInfo>();
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Get_Marry_Room_Info");
			while (ResultDataReader.Read())
			{
				MarryRoomInfo item = InitMarryRoom(ResultDataReader);
				list.Add(item);
			}
			return list.ToArray();
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetMarryRoomInfo", exception);
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

	public MarryRoomInfo GetMarryRoomInfoSingle(int id)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@ID", id)
			};
			db.GetReader(ref ResultDataReader, "SP_Get_Marry_Room_Info_Single", sqlParameters);
			if (ResultDataReader.Read())
			{
				return InitMarryRoom(ResultDataReader);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetMarryRoomInfo", exception);
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

	public MarryRoomInfo InitMarryRoom(SqlDataReader reader)
	{
		MarryRoomInfo marryRoomInfo = new MarryRoomInfo();
		marryRoomInfo.ID = (int)reader["ID"];
		marryRoomInfo.Name = ((reader["Name"] == null) ? "" : reader["Name"].ToString());
		marryRoomInfo.PlayerID = (int)reader["PlayerID"];
		marryRoomInfo.PlayerName = ((reader["PlayerName"] == null) ? "" : reader["PlayerName"].ToString());
		marryRoomInfo.GroomID = (int)reader["GroomID"];
		marryRoomInfo.GroomName = ((reader["GroomName"] == null) ? "" : reader["GroomName"].ToString());
		marryRoomInfo.BrideID = (int)reader["BrideID"];
		marryRoomInfo.BrideName = ((reader["BrideName"] == null) ? "" : reader["BrideName"].ToString());
		marryRoomInfo.Pwd = ((reader["Pwd"] == null) ? "" : reader["Pwd"].ToString());
		marryRoomInfo.AvailTime = (int)reader["AvailTime"];
		marryRoomInfo.MaxCount = (int)reader["MaxCount"];
		marryRoomInfo.GuestInvite = (bool)reader["GuestInvite"];
		marryRoomInfo.MapIndex = (int)reader["MapIndex"];
		marryRoomInfo.BeginTime = (DateTime)reader["BeginTime"];
		marryRoomInfo.BreakTime = (DateTime)reader["BreakTime"];
		marryRoomInfo.RoomIntroduction = ((reader["RoomIntroduction"] == null) ? "" : reader["RoomIntroduction"].ToString());
		marryRoomInfo.ServerID = (int)reader["ServerID"];
		marryRoomInfo.IsHymeneal = (bool)reader["IsHymeneal"];
		marryRoomInfo.IsGunsaluteUsed = (bool)reader["IsGunsaluteUsed"];
		marryRoomInfo.MerryActivity = (bool)reader["MerryActivity"];
		marryRoomInfo.SeniorType = (int)reader["SeniorType"];
		return marryRoomInfo;
	}

	public bool UpdateBreakTimeWhereServerStop()
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[0].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Update_Marry_Room_Info_Sever_Stop", array);
			result = (int)array[0].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("UpdateBreakTimeWhereServerStop", exception);
			}
		}
		return result;
	}

	public MarryProp GetMarryProp(int id)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@UserID", id)
			};
			db.GetReader(ref ResultDataReader, "SP_Select_Marry_Prop", sqlParameters);
			if (ResultDataReader.Read())
			{
				MarryProp marryProp = new MarryProp();
				marryProp.IsMarried = (bool)ResultDataReader["IsMarried"];
				marryProp.SpouseID = (int)ResultDataReader["SpouseID"];
				marryProp.SpouseName = ResultDataReader["SpouseName"].ToString();
				marryProp.IsCreatedMarryRoom = (bool)ResultDataReader["IsCreatedMarryRoom"];
				marryProp.SelfMarryRoomID = (int)ResultDataReader["SelfMarryRoomID"];
				marryProp.IsGotRing = (bool)ResultDataReader["IsGotRing"];
				return marryProp;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetMarryProp", exception);
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

	public bool SavePlayerMarryNotice(MarryApplyInfo info, int answerId, ref int id)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[9]
			{
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@ApplyUserID", info.ApplyUserID),
				new SqlParameter("@ApplyUserName", info.ApplyUserName),
				new SqlParameter("@ApplyType", info.ApplyType),
				new SqlParameter("@ApplyResult", info.ApplyResult),
				new SqlParameter("@LoveProclamation", info.LoveProclamation),
				new SqlParameter("@AnswerId", answerId),
				new SqlParameter("@ouototal", SqlDbType.Int),
				null
			};
			array[7].Direction = ParameterDirection.Output;
			array[8] = new SqlParameter("@Result", SqlDbType.Int);
			array[8].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Insert_Marry_Notice", array);
			id = (int)array[7].Value;
			result = (int)array[8].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SavePlayerMarryNotice", exception);
			}
		}
		return result;
	}

	public bool UpdatePlayerGotRingProp(int groomID, int brideID)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[3]
			{
				new SqlParameter("@GroomID", groomID),
				new SqlParameter("@BrideID", brideID),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[2].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Update_GotRing_Prop", array);
			result = (int)array[2].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("UpdatePlayerGotRingProp", exception);
			}
		}
		return result;
	}

	public bool UpdateLastVIPPackTime(int ID)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[3]
			{
				new SqlParameter("@UserID", ID),
				new SqlParameter("@LastVIPPackTime", DateTime.Now.Date),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[2].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_UpdateUserLastVIPPackTime", array);
			result = true;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_UpdateUserLastVIPPackTime", exception);
			}
		}
		return result;
	}

	public bool UpdateVIPInfo(PlayerInfo p)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[11]
			{
				new SqlParameter("@Result", SqlDbType.Int),
				new SqlParameter("@ID", p.ID),
				new SqlParameter("@VIPLevel", p.VIPLevel),
				new SqlParameter("@VIPExp", p.VIPExp),
				new SqlParameter("@VIPOnlineDays", SqlDbType.BigInt),
				new SqlParameter("@VIPOfflineDays", SqlDbType.BigInt),
				new SqlParameter("@VIPExpireDay", p.VIPExpireDay.ToString()),
				new SqlParameter("@VIPLastDate", DateTime.Now),
				new SqlParameter("@VIPNextLevelDaysNeeded", SqlDbType.BigInt),
				new SqlParameter("@CanTakeVipReward", p.CanTakeVipReward),
				new SqlParameter("@VipIntegral", p.VipIntegral)
			};
			array[0].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_UpdateVIPInfo", array);
			result = true;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_UpdateVIPInfo", exception);
			}
		}
		return result;
	}

	public int VIPRenewal(string nickName, int renewalDays, ref DateTime ExpireDayOut)
	{
		int result = 0;
		try
		{
			SqlParameter[] array = new SqlParameter[4]
			{
				new SqlParameter("@NickName", nickName),
				new SqlParameter("@RenewalDays", renewalDays),
				new SqlParameter("@ExpireDayOut", DateTime.Now),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[2].Direction = ParameterDirection.Output;
			array[3].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_VIPRenewal_Single", array);
			ExpireDayOut = (DateTime)array[2].Value;
			result = (int)array[3].Value;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_VIPRenewal_Single", exception);
			}
		}
		return result;
	}

	public bool Test(string DutyName)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@DutyName", DutyName)
			};
			result = db.RunProcedure("SP_Test1", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool TankAll()
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[0];
			result = db.RunProcedure("SP_Tank_All", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool RegisterUser(string UserName, string NickName, string Password, bool Sex, int Money, int GiftToken, int Gold)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[8]
			{
				new SqlParameter("@UserName", UserName),
				new SqlParameter("@Password", Password),
				new SqlParameter("@NickName", NickName),
				new SqlParameter("@Sex", Sex),
				new SqlParameter("@Money", Money),
				new SqlParameter("@GiftToken", GiftToken),
				new SqlParameter("@Gold", Gold),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[7].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Account_Register", array);
			if ((int)array[7].Value == 0)
			{
				result = true;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init Register", exception);
			}
		}
		return result;
	}

	public bool CheckEmailIsValid(string Email)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[2]
			{
				new SqlParameter("@Email", Email),
				new SqlParameter("@count", SqlDbType.BigInt)
			};
			array[1].Direction = ParameterDirection.Output;
			db.RunProcedure("CheckEmailIsValid", array);
			if (int.Parse(array[1].Value.ToString()) == 0)
			{
				result = true;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init CheckEmailIsValid", exception);
			}
		}
		return result;
	}

	public bool RegisterUserInfo(UserInfo userinfo)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[6]
			{
				new SqlParameter("@UserID", userinfo.UserID),
				new SqlParameter("@UserEmail", userinfo.UserEmail),
				new SqlParameter("@UserPhone", (userinfo.UserPhone == null) ? string.Empty : userinfo.UserPhone),
				new SqlParameter("@UserOther1", (userinfo.UserOther1 == null) ? string.Empty : userinfo.UserOther1),
				new SqlParameter("@UserOther2", (userinfo.UserOther2 == null) ? string.Empty : userinfo.UserOther2),
				new SqlParameter("@UserOther3", (userinfo.UserOther3 == null) ? string.Empty : userinfo.UserOther3)
			};
			result = db.RunProcedure("SP_User_Info_Add", sqlParameters);
			return result;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public UserInfo GetUserInfo(int UserId)
	{
		SqlDataReader ResultDataReader = null;
		UserInfo userInfo = new UserInfo
		{
			UserID = UserId
		};
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@UserID", UserId)
			};
			db.GetReader(ref ResultDataReader, "SP_Get_User_Info", sqlParameters);
			while (ResultDataReader.Read())
			{
				userInfo.UserID = int.Parse(ResultDataReader["UserID"].ToString());
				userInfo.UserEmail = ((ResultDataReader["UserEmail"] == null) ? "" : ResultDataReader["UserEmail"].ToString());
				userInfo.UserPhone = ((ResultDataReader["UserPhone"] == null) ? "" : ResultDataReader["UserPhone"].ToString());
				userInfo.UserOther1 = ((ResultDataReader["UserOther1"] == null) ? "" : ResultDataReader["UserOther1"].ToString());
				userInfo.UserOther2 = ((ResultDataReader["UserOther2"] == null) ? "" : ResultDataReader["UserOther2"].ToString());
				userInfo.UserOther3 = ((ResultDataReader["UserOther3"] == null) ? "" : ResultDataReader["UserOther3"].ToString());
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return userInfo;
	}

	public LevelInfo GetUserLevelSingle(int Grade)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@Grade", Grade)
			};
			db.GetReader(ref ResultDataReader, "SP_Get_Level_By_Grade", sqlParameters);
			if (ResultDataReader.Read())
			{
				LevelInfo levelInfo = new LevelInfo();
				levelInfo.Grade = (int)ResultDataReader["Grade"];
				levelInfo.GP = (int)ResultDataReader["GP"];
				levelInfo.Blood = (int)ResultDataReader["Blood"];
				return levelInfo;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetLevelInfoSingle", exception);
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

	public ExerciseInfo GetExerciseSingle(int Grade)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@Grage", Grade)
			};
			db.GetReader(ref ResultDataReader, "SP_Get_Exercise_By_Grade", sqlParameters);
			if (ResultDataReader.Read())
			{
				ExerciseInfo exerciseInfo = new ExerciseInfo();
				exerciseInfo.Grage = (int)ResultDataReader["Grage"];
				exerciseInfo.GP = (int)ResultDataReader["GP"];
				exerciseInfo.ExerciseA = (int)ResultDataReader["ExerciseA"];
				exerciseInfo.ExerciseAG = (int)ResultDataReader["ExerciseAG"];
				exerciseInfo.ExerciseD = (int)ResultDataReader["ExerciseD"];
				exerciseInfo.ExerciseH = (int)ResultDataReader["ExerciseH"];
				exerciseInfo.ExerciseL = (int)ResultDataReader["ExerciseL"];
				return exerciseInfo;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetExerciseInfoSingle", exception);
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

	public List<UserGemStone> GetSingleGemstones(int ID)
	{
		List<UserGemStone> list = new List<UserGemStone>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@ID", SqlDbType.Int, 4)
			};
			array[0].Value = ID;
			db.GetReader(ref ResultDataReader, "SP_GetSingleGemStone", array);
			while (ResultDataReader.Read())
			{
				list.Add(InitGemStones(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_GetSingleUserGemStones", exception);
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

	public UserGemStone InitGemStones(SqlDataReader reader)
	{
		UserGemStone userGemStone = new UserGemStone();
		userGemStone.ID = (int)reader["ID"];
		userGemStone.UserID = (int)reader["UserID"];
		userGemStone.FigSpiritId = (int)reader["FigSpiritId"];
		userGemStone.FigSpiritIdValue = (string)reader["FigSpiritIdValue"];
		userGemStone.EquipPlace = (int)reader["EquipPlace"];
		return userGemStone;
	}

	public TexpInfo GetUserTexpInfoSingle(int ID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@UserID", ID)
			};
			db.GetReader(ref ResultDataReader, "SP_Get_UserTexp_By_ID", sqlParameters);
			if (ResultDataReader.Read())
			{
				return InitTexpInfo(ResultDataReader);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetTexpInfoSingle", exception);
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

	public bool UpdateUserTexpInfo(TexpInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[32]
			{
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@attTexpExp", info.attTexpExp),
				new SqlParameter("@defTexpExp", info.defTexpExp),
				new SqlParameter("@hpTexpExp", info.hpTexpExp),
				new SqlParameter("@lukTexpExp", info.lukTexpExp),
				new SqlParameter("@spdTexpExp", info.spdTexpExp),
				new SqlParameter("@texpCount", info.texpCount),
				new SqlParameter("@texpTaskCount", info.texpTaskCount),
				new SqlParameter("@texpTaskDate", info.texpTaskDate.ToString()),
				new SqlParameter("@Result", SqlDbType.Int),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[9].Direction = ParameterDirection.ReturnValue;
			array[10] = new SqlParameter("@magicAtkTexpExp", info.magicAtkTexpExp);
			array[11] = new SqlParameter("@magicDefTexpExp", info.magicDefTexpExp);
			array[12] = new SqlParameter("@magicTexpCount", info.magicTexpCount);
			array[13] = new SqlParameter("@TrainCritTexpExp", info.TrainCritTexpExp);
			array[14] = new SqlParameter("@TrainSunderTexpExp", info.TrainSunderTexpExp);
			array[15] = new SqlParameter("@TrainCritDmgTexpExp", info.TrainCritDmgTexpExp);
			array[16] = new SqlParameter("@TrainSpeedTexpExp", info.TrainSpeedTexpExp);
			array[17] = new SqlParameter("@TrainTricKillTexpExp", info.TrainTricKillTexpExp);
			array[18] = new SqlParameter("@TrainDmgTexpExp", info.TrainDmgTexpExp);
			array[19] = new SqlParameter("@TrainArmorTexpExp", info.TrainArmorTexpExp);
			array[20] = new SqlParameter("@SecondTexpCount", info.SecondTexpCount);
            array[21] = new SqlParameter("@TrainAvoidDmgTextExp", info.TrainAvoidDmgTextExp);
            array[22] = new SqlParameter("@TrainTricDefTexpExp", info.TrainTricDefTexpExp);
            array[23] = new SqlParameter("@TrainTenacTexpExp", info.TrainTenacTexpExp);
            array[24] = new SqlParameter("@TrainGuardTexpExp", info.TrainGuardTexpExp);
            array[25] = new SqlParameter("@TrainFireDefTexpExp", info.TrainFireDefTexpExp);
            array[26] = new SqlParameter("@TrainWaterDefTexpExp", info.TrainWaterDefTexpExp);
            array[27] = new SqlParameter("@TrainWindDefTexpExp", info.TrainWindDefTexpExp);
            array[28] = new SqlParameter("@TrainLandDefTexpExp", info.TrainLandDefTexpExp);
            array[29] = new SqlParameter("@ThirdTexpCount", info.ThirdTexpCount);
            array[30] = new SqlParameter("@LastReset", info.lastReset);
			array[31] = new SqlParameter("@ResetCount", info.resetCount);
			db.RunProcedure("SP_UserTexp_Update", array);
			int num = (int)array[9].Value;
			result = num == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool InsertUserTexpInfo(TexpInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[32]
			{
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@attTexpExp", info.attTexpExp),
				new SqlParameter("@defTexpExp", info.defTexpExp),
				new SqlParameter("@hpTexpExp", info.hpTexpExp),
				new SqlParameter("@lukTexpExp", info.lukTexpExp),
				new SqlParameter("@spdTexpExp", info.spdTexpExp),
				new SqlParameter("@texpCount", info.texpCount),
				new SqlParameter("@texpTaskCount", info.texpTaskCount),
				new SqlParameter("@texpTaskDate", info.texpTaskDate.ToString()),
				new SqlParameter("@Result", SqlDbType.Int),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
				null
            };
			array[9].Direction = ParameterDirection.ReturnValue;
			array[10] = new SqlParameter("@magicAtkTexpExp", info.magicAtkTexpExp);
			array[11] = new SqlParameter("@magicDefTexpExp", info.magicDefTexpExp);
			array[12] = new SqlParameter("@magicTexpCount", info.magicTexpCount);
			array[13] = new SqlParameter("@TrainCritTexpExp", info.TrainCritTexpExp);
			array[14] = new SqlParameter("@TrainSunderTexpExp", info.TrainSunderTexpExp);
			array[15] = new SqlParameter("@TrainCritDmgTexpExp", info.TrainCritDmgTexpExp);
			array[16] = new SqlParameter("@TrainSpeedTexpExp", info.TrainSpeedTexpExp);
			array[17] = new SqlParameter("@TrainTricKillTexpExp", info.TrainTricKillTexpExp);
			array[18] = new SqlParameter("@TrainDmgTexpExp", info.TrainDmgTexpExp);
			array[19] = new SqlParameter("@TrainArmorTexpExp", info.TrainArmorTexpExp);
			array[20] = new SqlParameter("@SecondTexpCount", info.SecondTexpCount);
            array[21] = new SqlParameter("@TrainAvoidDmgTextExp", info.TrainAvoidDmgTextExp);
            array[22] = new SqlParameter("@TrainTricDefTexpExp", info.TrainTricDefTexpExp);
            array[23] = new SqlParameter("@TrainTenacTexpExp", info.TrainTenacTexpExp);
            array[24] = new SqlParameter("@TrainGuardTexpExp", info.TrainGuardTexpExp);
            array[25] = new SqlParameter("@TrainFireDefTexpExp", info.TrainFireDefTexpExp);
            array[26] = new SqlParameter("@TrainWaterDefTexpExp", info.TrainWaterDefTexpExp);
            array[27] = new SqlParameter("@TrainWindDefTexpExp", info.TrainWindDefTexpExp);
            array[28] = new SqlParameter("@TrainLandDefTexpExp", info.TrainLandDefTexpExp);
            array[29] = new SqlParameter("@ThirdTexpCount", info.ThirdTexpCount);
            array[30] = new SqlParameter("@LastReset", info.lastReset);
            array[31] = new SqlParameter("@ResetCount", info.resetCount);

			db.RunProcedure("SP_UserTexp_Add", array);
			result = (int)array[9].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("InsertTexpInfo", exception);
			}
		}
		return result;
	}

	public bool AddeqPet(PetEquipDataInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[9]
			{
				new SqlParameter("@ID", info.ID),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@UserID", info.UserID);
			array[2] = new SqlParameter("@PetID", info.PetID);
			array[3] = new SqlParameter("@eqType", info.eqType);
			array[4] = new SqlParameter("@eqTemplateID", info.eqTemplateID);
			array[5] = new SqlParameter("@startTime", info.startTime);
			array[6] = new SqlParameter("@ValidDate", info.ValidDate);
			array[7] = new SqlParameter("@IsExit", info.IsExit);
			array[8] = new SqlParameter("@Result", SqlDbType.Int);
			array[8].Direction = ParameterDirection.ReturnValue;
			result = db.RunProcedure("SP_User_Add_eqPet", array);
			int num = (int)array[8].Value;
			result = num == 0;
			info.ID = (int)array[0].Value;
			info.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool UpdateqPet(PetEquipDataInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[9]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@PetID", info.PetID),
				new SqlParameter("@eqType", info.eqType),
				new SqlParameter("@eqTemplateID", info.eqTemplateID),
				new SqlParameter("@startTime", info.startTime),
				new SqlParameter("@ValidDate", info.ValidDate),
				new SqlParameter("@IsExit", info.IsExit),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[8].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_eqPet_Update", array);
			int num = (int)array[8].Value;
			result = num == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool AddUserPet(UsersPetInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[40]
			{
				new SqlParameter("@TemplateID", item.TemplateID),
				new SqlParameter("@Name", (item.Name == null) ? "Error!" : item.Name),
				new SqlParameter("@UserID", item.UserID),
				new SqlParameter("@Attack", item.Attack),
				new SqlParameter("@Defence", item.Defence),
				new SqlParameter("@Luck", item.Luck),
				new SqlParameter("@Agility", item.Agility),
				new SqlParameter("@Blood", item.Blood),
				new SqlParameter("@Damage", item.Damage),
				new SqlParameter("@Guard", item.Guard),
				new SqlParameter("@AttackGrow", item.AttackGrow),
				new SqlParameter("@DefenceGrow", item.DefenceGrow),
				new SqlParameter("@LuckGrow", item.LuckGrow),
				new SqlParameter("@AgilityGrow", item.AgilityGrow),
				new SqlParameter("@BloodGrow", item.BloodGrow),
				new SqlParameter("@DamageGrow", item.DamageGrow),
				new SqlParameter("@GuardGrow", item.GuardGrow),
				new SqlParameter("@Level", item.Level),
				new SqlParameter("@GP", item.GP),
				new SqlParameter("@MaxGP", item.MaxGP),
				new SqlParameter("@Hunger", item.Hunger),
				new SqlParameter("@PetHappyStar", item.PetHappyStar),
				new SqlParameter("@MP", item.MP),
				new SqlParameter("@IsEquip", item.IsEquip),
				new SqlParameter("@Skill", item.Skill),
				new SqlParameter("@SkillEquip", item.SkillEquip),
				new SqlParameter("@Place", item.Place),
				new SqlParameter("@IsExit", item.IsExit),
				new SqlParameter("@ID", item.ID),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[28].Direction = ParameterDirection.Output;
			array[29] = new SqlParameter("@currentStarExp", item.currentStarExp);
			array[30] = new SqlParameter("@Result", SqlDbType.Int);
			array[30].Direction = ParameterDirection.ReturnValue;
			array[31] = new SqlParameter("@breakGrade", item.breakGrade);
			array[32] = new SqlParameter("@breakAttack", item.breakAttack);
			array[33] = new SqlParameter("@breakDefence", item.breakDefence);
			array[34] = new SqlParameter("@breakAgility", item.breakAgility);
			array[35] = new SqlParameter("@breakLuck", item.breakLuck);
			array[36] = new SqlParameter("@breakBlood", item.breakBlood);
			array[37] = new SqlParameter("@eQPets", item.eQPets);
			array[38] = new SqlParameter("@BaseProp", item.BaseProp);
			array[39] = new SqlParameter("@IsSeal", item.IsSeal);
			result = db.RunProcedure("SP_User_Add_Pet_Lacrar", array);
			int num = (int)array[30].Value;
			result = num == 0;
			item.ID = (int)array[28].Value;
			item.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool UpdateUserPet(UsersPetInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[40]
			{
				new SqlParameter("@TemplateID", item.TemplateID),
				new SqlParameter("@Name", (item.Name == null) ? "Error!" : item.Name),
				new SqlParameter("@UserID", item.UserID),
				new SqlParameter("@Attack", item.Attack),
				new SqlParameter("@Defence", item.Defence),
				new SqlParameter("@Luck", item.Luck),
				new SqlParameter("@Agility", item.Agility),
				new SqlParameter("@Blood", item.Blood),
				new SqlParameter("@Damage", item.Damage),
				new SqlParameter("@Guard", item.Guard),
				new SqlParameter("@AttackGrow", item.AttackGrow),
				new SqlParameter("@DefenceGrow", item.DefenceGrow),
				new SqlParameter("@LuckGrow", item.LuckGrow),
				new SqlParameter("@AgilityGrow", item.AgilityGrow),
				new SqlParameter("@BloodGrow", item.BloodGrow),
				new SqlParameter("@DamageGrow", item.DamageGrow),
				new SqlParameter("@GuardGrow", item.GuardGrow),
				new SqlParameter("@Level", item.Level),
				new SqlParameter("@GP", item.GP),
				new SqlParameter("@MaxGP", item.MaxGP),
				new SqlParameter("@Hunger", item.Hunger),
				new SqlParameter("@PetHappyStar", item.PetHappyStar),
				new SqlParameter("@MP", item.MP),
				new SqlParameter("@IsEquip", item.IsEquip),
				new SqlParameter("@Place", item.Place),
				new SqlParameter("@IsExit", item.IsExit),
				new SqlParameter("@ID", item.ID),
				new SqlParameter("@Skill", item.Skill),
				new SqlParameter("@SkillEquip", item.SkillEquip),
				new SqlParameter("@currentStarExp", item.currentStarExp),
				new SqlParameter("@Result", SqlDbType.Int),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[30].Direction = ParameterDirection.ReturnValue;
			array[31] = new SqlParameter("@breakGrade", item.breakGrade);
			array[32] = new SqlParameter("@breakAttack", item.breakAttack);
			array[33] = new SqlParameter("@breakDefence", item.breakDefence);
			array[34] = new SqlParameter("@breakAgility", item.breakAgility);
			array[35] = new SqlParameter("@breakLuck", item.breakLuck);
			array[36] = new SqlParameter("@breakBlood", item.breakBlood);
			array[37] = new SqlParameter("@eQPets", item.eQPets);
			array[38] = new SqlParameter("@BaseProp", item.BaseProp);
			array[39] = new SqlParameter("@IsSeal", item.IsSeal);
			db.RunProcedure("SP_UserPet_Update_Lacrar", array);
			int num = (int)array[30].Value;
			result = num == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public UsersPetInfo GetAdoptPetSingle(int PetID)
	{
		UsersPetInfo usersPetInfo = new UsersPetInfo();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@ID", SqlDbType.Int, 4)
			};
			array[0].Value = PetID;
			db.GetReader(ref ResultDataReader, "SP_AdoptPet_By_Id", array);
			if (ResultDataReader.Read())
			{
				UsersPetInfo usersPetInfo2 = new UsersPetInfo();
				usersPetInfo2.ID = (int)ResultDataReader["ID"];
				usersPetInfo2.TemplateID = (int)ResultDataReader["TemplateID"];
				usersPetInfo2.Name = ResultDataReader["Name"].ToString();
				usersPetInfo2.UserID = (int)ResultDataReader["UserID"];
				usersPetInfo2.Attack = (int)ResultDataReader["Attack"];
				usersPetInfo2.AttackGrow = (int)ResultDataReader["AttackGrow"];
				usersPetInfo2.Agility = (int)ResultDataReader["Agility"];
				usersPetInfo2.AgilityGrow = (int)ResultDataReader["AgilityGrow"];
				usersPetInfo2.Defence = (int)ResultDataReader["Defence"];
				usersPetInfo2.DefenceGrow = (int)ResultDataReader["DefenceGrow"];
				usersPetInfo2.Luck = (int)ResultDataReader["Luck"];
				usersPetInfo2.LuckGrow = (int)ResultDataReader["LuckGrow"];
				usersPetInfo2.Blood = (int)ResultDataReader["Blood"];
				usersPetInfo2.BloodGrow = (int)ResultDataReader["BloodGrow"];
				usersPetInfo2.Damage = (int)ResultDataReader["Damage"];
				usersPetInfo2.DamageGrow = (int)ResultDataReader["DamageGrow"];
				usersPetInfo2.Guard = (int)ResultDataReader["Guard"];
				usersPetInfo2.GuardGrow = (int)ResultDataReader["GuardGrow"];
				usersPetInfo2.Level = (int)ResultDataReader["Level"];
				usersPetInfo2.GP = (int)ResultDataReader["GP"];
				usersPetInfo2.MaxGP = (int)ResultDataReader["MaxGP"];
				usersPetInfo2.Hunger = (int)ResultDataReader["Hunger"];
				usersPetInfo2.MP = (int)ResultDataReader["MP"];
				usersPetInfo2.Place = (int)ResultDataReader["Place"];
				usersPetInfo2.IsEquip = (bool)ResultDataReader["IsEquip"];
				usersPetInfo2.IsExit = (bool)ResultDataReader["IsExit"];
				usersPetInfo2.Skill = ResultDataReader["Skill"].ToString();
				usersPetInfo2.SkillEquip = ResultDataReader["SkillEquip"].ToString();
				return usersPetInfo2;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
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

	public UsersPetInfo[] GetUserAdoptPetSingles(int UserID)
	{
		List<UsersPetInfo> list = new List<UsersPetInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_Get_User_AdoptPetList", array);
			while (ResultDataReader.Read())
			{
				UsersPetInfo usersPetInfo = new UsersPetInfo();
				usersPetInfo.ID = (int)ResultDataReader["ID"];
				usersPetInfo.TemplateID = (int)ResultDataReader["TemplateID"];
				usersPetInfo.Name = ResultDataReader["Name"].ToString();
				usersPetInfo.UserID = (int)ResultDataReader["UserID"];
				usersPetInfo.Attack = (int)ResultDataReader["Attack"];
				usersPetInfo.AttackGrow = (int)ResultDataReader["AttackGrow"];
				usersPetInfo.Agility = (int)ResultDataReader["Agility"];
				usersPetInfo.AgilityGrow = (int)ResultDataReader["AgilityGrow"];
				usersPetInfo.Defence = (int)ResultDataReader["Defence"];
				usersPetInfo.DefenceGrow = (int)ResultDataReader["DefenceGrow"];
				usersPetInfo.Luck = (int)ResultDataReader["Luck"];
				usersPetInfo.LuckGrow = (int)ResultDataReader["LuckGrow"];
				usersPetInfo.Blood = (int)ResultDataReader["Blood"];
				usersPetInfo.BloodGrow = (int)ResultDataReader["BloodGrow"];
				usersPetInfo.Damage = (int)ResultDataReader["Damage"];
				usersPetInfo.DamageGrow = (int)ResultDataReader["DamageGrow"];
				usersPetInfo.Guard = (int)ResultDataReader["Guard"];
				usersPetInfo.GuardGrow = (int)ResultDataReader["GuardGrow"];
				usersPetInfo.Level = (int)ResultDataReader["Level"];
				usersPetInfo.GP = (int)ResultDataReader["GP"];
				usersPetInfo.MaxGP = (int)ResultDataReader["MaxGP"];
				usersPetInfo.Hunger = (int)ResultDataReader["Hunger"];
				usersPetInfo.MP = (int)ResultDataReader["MP"];
				usersPetInfo.Place = (int)ResultDataReader["Place"];
				usersPetInfo.IsEquip = (bool)ResultDataReader["IsEquip"];
				usersPetInfo.IsExit = (bool)ResultDataReader["IsExit"];
				usersPetInfo.Skill = ResultDataReader["Skill"].ToString();
				usersPetInfo.SkillEquip = ResultDataReader["SkillEquip"].ToString();
				list.Add(usersPetInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
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

	public bool RemoveUserAdoptPet(int ID)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[2]
			{
				new SqlParameter("@ID", ID),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[1].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Remove_User_AdoptPet", array);
			int num = (int)array[1].Value;
			result = num == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool ClearAdoptPet(int ID)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[2]
			{
				new SqlParameter("@ID", ID),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[1].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Clear_AdoptPet", array);
			int num = (int)array[1].Value;
			result = num == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool ClearDatabase()
	{
		bool result = false;
		try
		{
			result = db.RunProcedure("SP_Sys_Clear_All");
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool UpdateUserAdoptPet(int ID)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[2]
			{
				new SqlParameter("@ID", ID),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[1].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Update_User_AdoptPet", array);
			int num = (int)array[1].Value;
			result = num == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool AddUserAdoptPet(UsersPetInfo info, bool isUse)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[23]
			{
				new SqlParameter("@TemplateID", info.TemplateID),
				new SqlParameter("@Name", (info.Name == null) ? "Error!" : info.Name),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@Attack", info.Attack),
				new SqlParameter("@Defence", info.Defence),
				new SqlParameter("@Luck", info.Luck),
				new SqlParameter("@Agility", info.Agility),
				new SqlParameter("@Blood", info.Blood),
				new SqlParameter("@Damage", info.Damage),
				new SqlParameter("@Guard", info.Guard),
				new SqlParameter("@AttackGrow", info.AttackGrow),
				new SqlParameter("@DefenceGrow", info.DefenceGrow),
				new SqlParameter("@LuckGrow", info.LuckGrow),
				new SqlParameter("@AgilityGrow", info.AgilityGrow),
				new SqlParameter("@BloodGrow", info.BloodGrow),
				new SqlParameter("@DamageGrow", info.DamageGrow),
				new SqlParameter("@GuardGrow", info.GuardGrow),
				new SqlParameter("@Skill", info.Skill),
				new SqlParameter("@SkillEquip", info.SkillEquip),
				new SqlParameter("@Place", info.Place),
				new SqlParameter("@IsExit", info.IsExit),
				new SqlParameter("@IsUse", isUse),
				new SqlParameter("@ID", info.ID)
			};
			array[22].Direction = ParameterDirection.Output;
			result = db.RunProcedure("SP_User_AdoptPet", array);
			info.ID = (int)array[22].Value;
			info.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public UsersPetInfo[] GetUserPetSingles(int UserID, int vipLv)
	{
		List<UsersPetInfo> list = new List<UsersPetInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_Get_UserPet_By_ID", array);
			while (ResultDataReader.Read())
			{
				UsersPetInfo usersPetInfo = InitPet(ResultDataReader);
				usersPetInfo.VIPLevel = vipLv;
				list.Add(usersPetInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
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

	public List<UsersPetInfo> GetUserPetIsExitSingles(int UserID, int vipLv)
	{
		List<UsersPetInfo> list = new List<UsersPetInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_Get_UserPet_By_IsExit", array);
			while (ResultDataReader.Read())
			{
				UsersPetInfo usersPetInfo = InitPet(ResultDataReader);
				usersPetInfo.VIPLevel = vipLv;
				list.Add(usersPetInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
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

	public UsersPetInfo GetUserPetSingle(int ID, int vipLv)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = ID;
			db.GetReader(ref ResultDataReader, "SP_Get_UserPet_By_ID", array);
			if (ResultDataReader.Read())
			{
				UsersPetInfo usersPetInfo = InitPet(ResultDataReader);
				usersPetInfo.VIPLevel = vipLv;
				return usersPetInfo;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetPetInfoSingle", exception);
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

	public bool AddMountDatas(MountDataInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[7]
			{
				new SqlParameter("@ID", info.ID),
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@UserID", info.UserID);
			array[2] = new SqlParameter("@curUseHorse", info.curUseHorse);
			array[3] = new SqlParameter("@curLevel", info.curLevel);
			array[4] = new SqlParameter("@curExp", info.curExp);
			array[5] = new SqlParameter("@curHasSkill", info.curHasSkill);
			array[6] = new SqlParameter("@horsePicCherish", info.horsePicCherish);
			result = db.RunProcedure("SP_MountData_Add", array);
			info.ID = (int)array[0].Value;
			info.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool UpdateMountData(MountDataInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[8]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@curUseHorse", info.curUseHorse),
				new SqlParameter("@curLevel", info.curLevel),
				new SqlParameter("@curExp", info.curExp),
				new SqlParameter("@curHasSkill", info.curHasSkill),
				new SqlParameter("@horsePicCherish", info.horsePicCherish),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[7].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_MountData_Update", array);
			int num = (int)array[7].Value;
			result = num == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public MountDataInfo GetSingleMountData(int UserID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_MountData_Single", array);
			if (ResultDataReader.Read())
			{
				MountDataInfo mountDataInfo = new MountDataInfo();
				mountDataInfo.ID = (int)ResultDataReader["ID"];
				mountDataInfo.UserID = (int)ResultDataReader["UserID"];
				mountDataInfo.curUseHorse = (int)ResultDataReader["curUseHorse"];
				mountDataInfo.curLevel = (int)ResultDataReader["curLevel"];
				mountDataInfo.curExp = (int)ResultDataReader["curExp"];
				mountDataInfo.curHasSkill = ResultDataReader["curHasSkill"].ToString();
				mountDataInfo.horsePicCherish = ResultDataReader["horsePicCherish"].ToString();
				return mountDataInfo;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
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

	public bool AddCards(UsersCardInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[16]
			{
				new SqlParameter("@CardID", item.CardID),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@CardType", item.CardType);
			array[2] = new SqlParameter("@UserID", item.UserID);
			array[3] = new SqlParameter("@Place", item.Place);
			array[4] = new SqlParameter("@TemplateID", item.TemplateID);
			array[5] = new SqlParameter("@isFirstGet", false);
			array[6] = new SqlParameter("@Attack", item.Attack);
			array[7] = new SqlParameter("@Defence", item.Defence);
			array[8] = new SqlParameter("@Luck", item.Luck);
			array[9] = new SqlParameter("@Agility", item.Agility);
			array[10] = new SqlParameter("@Damage", item.Damage);
			array[11] = new SqlParameter("@Guard", item.Guard);
			array[12] = new SqlParameter("@IsExit", item.IsExit);
			array[13] = new SqlParameter("@Level", item.Level);
			array[14] = new SqlParameter("@CardGP", item.CardGP);
			array[15] = new SqlParameter("@Type", item.Type);
			result = db.RunProcedure("SP_Users_Cards_Add", array);
			item.CardID = (int)array[0].Value;
			item.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
		}
		return result;
	}

	public bool UpdateCards(UsersCardInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[17]
			{
				new SqlParameter("@CardType", info.CardType),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@Place", info.Place),
				new SqlParameter("@TemplateID", info.TemplateID),
				new SqlParameter("@isFirstGet", info.isFirstGet),
				new SqlParameter("@Attack", info.Attack),
				new SqlParameter("@Defence", info.Defence),
				new SqlParameter("@Luck", info.Luck),
				new SqlParameter("@Agility", info.Agility),
				new SqlParameter("@Damage", info.Damage),
				new SqlParameter("@Guard", info.Guard),
				new SqlParameter("@IsExit", info.IsExit),
				new SqlParameter("@Level", info.Level),
				new SqlParameter("@CardGP", info.CardGP),
				new SqlParameter("@Type", info.Type),
				new SqlParameter("@CardID", info.CardID),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[16].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_UserCardProp_Update", array);
			int num = (int)array[16].Value;
			result = num == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public List<UsersCardInfo> GetUserCardEuqip(int UserID)
	{
		List<UsersCardInfo> list = new List<UsersCardInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_Users_Items_Card_Equip", array);
			while (ResultDataReader.Read())
			{
				list.Add(InitCard(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
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

	public UsersCardInfo GetUserCardByPlace(int Place)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@Place", SqlDbType.Int, 4)
			};
			array[0].Value = Place;
			db.GetReader(ref ResultDataReader, "SP_Get_UserCard_By_Place", array);
			if (ResultDataReader.Read())
			{
				return InitCard(ResultDataReader);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
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

	public UsersCardInfo[] GetUserCardSingles(int UserID)
	{
		List<UsersCardInfo> list = new List<UsersCardInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_Get_UserCard_By_ID", array);
			while (ResultDataReader.Read())
			{
				list.Add(InitCard(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
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

	public UserFarmInfo GetSingleFarm(int Id)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@ID", SqlDbType.Int, 4)
			};
			array[0].Value = Id;
			db.GetReader(ref ResultDataReader, "SP_Get_SingleFarm", array);
			if (ResultDataReader.Read())
			{
				UserFarmInfo userFarmInfo = new UserFarmInfo();
				userFarmInfo.ID = (int)ResultDataReader["ID"];
				userFarmInfo.FarmID = (int)ResultDataReader["FarmID"];
				userFarmInfo.PayFieldMoney = (string)ResultDataReader["PayFieldMoney"];
				userFarmInfo.PayAutoMoney = (string)ResultDataReader["PayAutoMoney"];
				userFarmInfo.AutoPayTime = (DateTime)ResultDataReader["AutoPayTime"];
				userFarmInfo.AutoValidDate = (int)ResultDataReader["AutoValidDate"];
				userFarmInfo.VipLimitLevel = (int)ResultDataReader["VipLimitLevel"];
				userFarmInfo.FarmerName = (string)ResultDataReader["FarmerName"];
				userFarmInfo.GainFieldId = (int)ResultDataReader["GainFieldId"];
				userFarmInfo.MatureId = (int)ResultDataReader["MatureId"];
				userFarmInfo.KillCropId = (int)ResultDataReader["KillCropId"];
				userFarmInfo.isAutoId = (int)ResultDataReader["isAutoId"];
				userFarmInfo.isFarmHelper = (bool)ResultDataReader["isFarmHelper"];
				userFarmInfo.buyExpRemainNum = (int)ResultDataReader["buyExpRemainNum"];
				userFarmInfo.isArrange = (bool)ResultDataReader["isArrange"];
				userFarmInfo.TreeLevel = (int)ResultDataReader["TreeLevel"];
				userFarmInfo.TreeExp = (int)ResultDataReader["TreeExp"];
				userFarmInfo.LoveScore = (int)ResultDataReader["LoveScore"];
				userFarmInfo.MonsterExp = (int)ResultDataReader["MonsterExp"];
				userFarmInfo.PoultryState = (int)ResultDataReader["PoultryState"];
				userFarmInfo.CountDownTime = (DateTime)ResultDataReader["CountDownTime"];
				userFarmInfo.TreeCostExp = (int)ResultDataReader["TreeCostExp"];
				return userFarmInfo;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetSingleFarm", exception);
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

	public bool AddFarm(UserFarmInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[22]
			{
				new SqlParameter("@FarmID", info.FarmID),
				new SqlParameter("@PayFieldMoney", info.PayFieldMoney),
				new SqlParameter("@PayAutoMoney", info.PayAutoMoney),
				new SqlParameter("@AutoPayTime", info.AutoPayTime.ToString()),
				new SqlParameter("@AutoValidDate", info.AutoValidDate),
				new SqlParameter("@VipLimitLevel", info.VipLimitLevel),
				new SqlParameter("@FarmerName", info.FarmerName),
				new SqlParameter("@GainFieldId", info.GainFieldId),
				new SqlParameter("@MatureId", info.MatureId),
				new SqlParameter("@KillCropId", info.KillCropId),
				new SqlParameter("@isAutoId", info.isAutoId),
				new SqlParameter("@isFarmHelper", info.isFarmHelper),
				new SqlParameter("@ID", info.ID),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[12].Direction = ParameterDirection.Output;
			array[13] = new SqlParameter("@buyExpRemainNum", info.buyExpRemainNum);
			array[14] = new SqlParameter("@isArrange", info.isArrange);
			array[15] = new SqlParameter("@TreeLevel", info.TreeLevel);
			array[16] = new SqlParameter("@TreeExp", info.TreeExp);
			array[17] = new SqlParameter("@LoveScore", info.LoveScore);
			array[18] = new SqlParameter("@MonsterExp", info.MonsterExp);
			array[19] = new SqlParameter("@PoultryState", info.PoultryState);
			array[20] = new SqlParameter("@CountDownTime", info.CountDownTime);
			array[21] = new SqlParameter("@TreeCostExp", info.TreeCostExp);
			result = db.RunProcedure("SP_Users_Farm_Add", array);
			info.ID = (int)array[12].Value;
			info.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
		}
		return result;
	}

	public bool UpdateFarm(UserFarmInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[22]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@FarmID", info.FarmID),
				new SqlParameter("@PayFieldMoney", info.PayFieldMoney),
				new SqlParameter("@PayAutoMoney", info.PayAutoMoney),
				new SqlParameter("@AutoPayTime", info.AutoPayTime.ToString()),
				new SqlParameter("@AutoValidDate", info.AutoValidDate),
				new SqlParameter("@VipLimitLevel", info.VipLimitLevel),
				new SqlParameter("@FarmerName", info.FarmerName),
				new SqlParameter("@GainFieldId", info.GainFieldId),
				new SqlParameter("@MatureId", info.MatureId),
				new SqlParameter("@KillCropId", info.KillCropId),
				new SqlParameter("@isAutoId", info.isAutoId),
				new SqlParameter("@isFarmHelper", info.isFarmHelper),
				new SqlParameter("@buyExpRemainNum", info.buyExpRemainNum),
				new SqlParameter("@isArrange", info.isArrange),
				new SqlParameter("@TreeLevel", info.TreeLevel),
				new SqlParameter("@TreeExp", info.TreeExp),
				new SqlParameter("@LoveScore", info.LoveScore),
				new SqlParameter("@MonsterExp", info.MonsterExp),
				new SqlParameter("@PoultryState", info.PoultryState),
				new SqlParameter("@CountDownTime", info.CountDownTime),
				new SqlParameter("@TreeCostExp", info.TreeCostExp)
			};
			result = db.RunProcedure("SP_Users_Farm_Update", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool AddFields(UserFieldInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[17]
			{
				new SqlParameter("@FarmID", item.FarmID),
				new SqlParameter("@FieldID", item.FieldID),
				new SqlParameter("@SeedID", item.SeedID),
				new SqlParameter("@PlantTime", item.PlantTime.ToString()),
				new SqlParameter("@AccelerateTime", item.AccelerateTime),
				new SqlParameter("@FieldValidDate", item.FieldValidDate),
				new SqlParameter("@PayTime", item.PayTime.ToString()),
				new SqlParameter("@GainCount", item.GainCount),
				new SqlParameter("@AutoSeedID", item.AutoSeedID),
				new SqlParameter("@AutoFertilizerID", item.AutoFertilizerID),
				new SqlParameter("@AutoSeedIDCount", item.AutoSeedIDCount),
				new SqlParameter("@AutoFertilizerCount", item.AutoFertilizerCount),
				new SqlParameter("@isAutomatic", item.isAutomatic),
				new SqlParameter("@AutomaticTime", item.AutomaticTime.ToString()),
				new SqlParameter("@IsExit", item.IsExit),
				new SqlParameter("@payFieldTime", item.payFieldTime),
				new SqlParameter("@ID", item.ID)
			};
			array[16].Direction = ParameterDirection.Output;
			result = db.RunProcedure("SP_Users_Fields_Add", array);
			item.ID = (int)array[16].Value;
			item.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
		}
		return result;
	}

	public bool UpdateFields(UserFieldInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[17]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@FarmID", info.FarmID),
				new SqlParameter("@FieldID", info.FieldID),
				new SqlParameter("@SeedID", info.SeedID),
				new SqlParameter("@PlantTime", info.PlantTime.ToString()),
				new SqlParameter("@AccelerateTime", info.AccelerateTime),
				new SqlParameter("@FieldValidDate", info.FieldValidDate),
				new SqlParameter("@PayTime", info.PayTime.ToString()),
				new SqlParameter("@GainCount", info.GainCount),
				new SqlParameter("@AutoSeedID", info.AutoSeedID),
				new SqlParameter("@AutoFertilizerID", info.AutoFertilizerID),
				new SqlParameter("@AutoSeedIDCount", info.AutoSeedIDCount),
				new SqlParameter("@AutoFertilizerCount", info.AutoFertilizerCount),
				new SqlParameter("@isAutomatic", info.isAutomatic),
				new SqlParameter("@AutomaticTime", info.AutomaticTime.ToString()),
				new SqlParameter("@IsExit", info.IsExit),
				new SqlParameter("@payFieldTime", info.payFieldTime)
			};
			result = db.RunProcedure("SP_Users_Fields_Update", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public UserFieldInfo[] GetSingleFields(int ID)
	{
		List<UserFieldInfo> list = new List<UserFieldInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@ID", SqlDbType.Int, 4)
			};
			array[0].Value = ID;
			db.GetReader(ref ResultDataReader, "SP_Get_SingleFields", array);
			while (ResultDataReader.Read())
			{
				UserFieldInfo userFieldInfo = new UserFieldInfo();
				userFieldInfo.ID = (int)ResultDataReader["ID"];
				userFieldInfo.FarmID = (int)ResultDataReader["FarmID"];
				userFieldInfo.FieldID = (int)ResultDataReader["FieldID"];
				userFieldInfo.SeedID = (int)ResultDataReader["SeedID"];
				userFieldInfo.PlantTime = (DateTime)ResultDataReader["PlantTime"];
				userFieldInfo.AccelerateTime = (int)ResultDataReader["AccelerateTime"];
				userFieldInfo.FieldValidDate = (int)ResultDataReader["FieldValidDate"];
				userFieldInfo.PayTime = (DateTime)ResultDataReader["PayTime"];
				userFieldInfo.GainCount = (int)ResultDataReader["GainCount"];
				userFieldInfo.AutoSeedID = (int)ResultDataReader["AutoSeedID"];
				userFieldInfo.AutoFertilizerID = (int)ResultDataReader["AutoFertilizerID"];
				userFieldInfo.AutoSeedIDCount = (int)ResultDataReader["AutoSeedIDCount"];
				userFieldInfo.AutoFertilizerCount = (int)ResultDataReader["AutoFertilizerCount"];
				userFieldInfo.isAutomatic = (bool)ResultDataReader["isAutomatic"];
				userFieldInfo.AutomaticTime = (DateTime)ResultDataReader["AutomaticTime"];
				userFieldInfo.IsExit = (bool)ResultDataReader["IsExit"];
				userFieldInfo.payFieldTime = (int)ResultDataReader["payFieldTime"];
				list.Add(userFieldInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_GetSingleFields", exception);
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

	public bool DeleteAllFields(int ID)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[2]
			{
				new SqlParameter("@UserID", ID),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[1].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_RemoveAllFields", array);
			result = (int)array[1].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_RemoveAllFields", exception);
			}
		}
		return result;
	}

	public List<UserGemStone> GetSingleGemStones(int ID)
	{
		List<UserGemStone> list = new List<UserGemStone>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@ID", SqlDbType.Int, 4)
			};
			array[0].Value = ID;
			db.GetReader(ref ResultDataReader, "SP_GetSingleGemStone", array);
			while (ResultDataReader.Read())
			{
				list.Add(InitGemStones(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_GetSingleUserGemStones", exception);
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

	public bool AddUserGemStone(UserGemStone item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[6]
			{
				new SqlParameter("@ID", item.ID),
				null,
				null,
				null,
				null,
				null
			};
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@UserID", item.UserID);
			array[2] = new SqlParameter("@FigSpiritId", item.FigSpiritId);
			array[3] = new SqlParameter("@FigSpiritIdValue", item.FigSpiritIdValue);
			array[4] = new SqlParameter("@EquipPlace", item.EquipPlace);
			array[5] = new SqlParameter("@Result", SqlDbType.Int);
			array[5].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Users_GemStones_Add", array);
			int num = (int)array[5].Value;
			result = num == 0;
			item.ID = (int)array[0].Value;
			item.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
		}
		return result;
	}

	public bool UpdateGemStoneInfo(UserGemStone g)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[6]
			{
				new SqlParameter("@ID", g.ID),
				new SqlParameter("@UserID", g.UserID),
				new SqlParameter("@FigSpiritId", g.FigSpiritId),
				new SqlParameter("@FigSpiritIdValue", g.FigSpiritIdValue),
				new SqlParameter("@EquipPlace", g.EquipPlace),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[5].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_UpdateGemStoneInfo", array);
			result = true;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_UpdateGemStoneInfo", exception);
			}
		}
		return result;
	}

	public UserLabyrinthInfo GetSingleLabyrinth(int ID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@ID", SqlDbType.Int, 4)
			};
			array[0].Value = ID;
			db.GetReader(ref ResultDataReader, "SP_GetSingleLabyrinth", array);
			if (ResultDataReader.Read())
			{
				UserLabyrinthInfo userLabyrinthInfo = new UserLabyrinthInfo();
				userLabyrinthInfo.UserID = (int)ResultDataReader["UserID"];
				userLabyrinthInfo.myProgress = (int)ResultDataReader["myProgress"];
				userLabyrinthInfo.myRanking = (int)ResultDataReader["myRanking"];
				userLabyrinthInfo.completeChallenge = (bool)ResultDataReader["completeChallenge"];
				userLabyrinthInfo.isDoubleAward = (bool)ResultDataReader["isDoubleAward"];
				userLabyrinthInfo.currentFloor = (int)ResultDataReader["currentFloor"];
				userLabyrinthInfo.accumulateExp = (int)ResultDataReader["accumulateExp"];
				userLabyrinthInfo.remainTime = (int)ResultDataReader["remainTime"];
				userLabyrinthInfo.currentRemainTime = (int)ResultDataReader["currentRemainTime"];
				userLabyrinthInfo.cleanOutAllTime = (int)ResultDataReader["cleanOutAllTime"];
				userLabyrinthInfo.cleanOutGold = (int)ResultDataReader["cleanOutGold"];
				userLabyrinthInfo.tryAgainComplete = (bool)ResultDataReader["tryAgainComplete"];
				userLabyrinthInfo.isInGame = (bool)ResultDataReader["isInGame"];
				userLabyrinthInfo.isCleanOut = (bool)ResultDataReader["isCleanOut"];
				userLabyrinthInfo.serverMultiplyingPower = (bool)ResultDataReader["serverMultiplyingPower"];
				userLabyrinthInfo.LastDate = (DateTime)ResultDataReader["LastDate"];
				userLabyrinthInfo.ProcessAward = (string)ResultDataReader["ProcessAward"];
				return userLabyrinthInfo;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_GetSingleUserLabyrinth", exception);
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

	public bool AddUserLabyrinth(UserLabyrinthInfo laby)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[18]
			{
				new SqlParameter("@UserID", laby.UserID),
				new SqlParameter("@myProgress", laby.myProgress),
				new SqlParameter("@myRanking", laby.myRanking),
				new SqlParameter("@completeChallenge", laby.completeChallenge),
				new SqlParameter("@isDoubleAward", laby.isDoubleAward),
				new SqlParameter("@currentFloor", laby.currentFloor),
				new SqlParameter("@accumulateExp", laby.accumulateExp),
				new SqlParameter("@remainTime", laby.remainTime),
				new SqlParameter("@currentRemainTime", laby.currentRemainTime),
				new SqlParameter("@cleanOutAllTime", laby.cleanOutAllTime),
				new SqlParameter("@cleanOutGold", laby.cleanOutGold),
				new SqlParameter("@tryAgainComplete", laby.tryAgainComplete),
				new SqlParameter("@isInGame", laby.isInGame),
				new SqlParameter("@isCleanOut", laby.isCleanOut),
				new SqlParameter("@serverMultiplyingPower", laby.serverMultiplyingPower),
				new SqlParameter("@LastDate", laby.LastDate),
				new SqlParameter("@ProcessAward", laby.ProcessAward),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[17].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Users_Labyrinth_Add", array);
			int num = (int)array[17].Value;
			result = num == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
		}
		return result;
	}

	public bool UpdateLabyrinthInfo(UserLabyrinthInfo laby)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[18]
			{
				new SqlParameter("@UserID", laby.UserID),
				new SqlParameter("@myProgress", laby.myProgress),
				new SqlParameter("@myRanking", laby.myRanking),
				new SqlParameter("@completeChallenge", laby.completeChallenge),
				new SqlParameter("@isDoubleAward", laby.isDoubleAward),
				new SqlParameter("@currentFloor", laby.currentFloor),
				new SqlParameter("@accumulateExp", laby.accumulateExp),
				new SqlParameter("@remainTime", laby.remainTime),
				new SqlParameter("@currentRemainTime", laby.currentRemainTime),
				new SqlParameter("@cleanOutAllTime", laby.cleanOutAllTime),
				new SqlParameter("@cleanOutGold", laby.cleanOutGold),
				new SqlParameter("@tryAgainComplete", laby.tryAgainComplete),
				new SqlParameter("@isInGame", laby.isInGame),
				new SqlParameter("@isCleanOut", laby.isCleanOut),
				new SqlParameter("@serverMultiplyingPower", laby.serverMultiplyingPower),
				new SqlParameter("@LastDate", laby.LastDate),
				new SqlParameter("@ProcessAward", laby.ProcessAward),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[17].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_UpdateLabyrinthInfo", array);
			result = true;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_UpdateLabyrinthInfo", exception);
			}
		}
		return result;
	}

	public Dictionary<int, UserDrillInfo> GetPlayerDrillByID(int UserID)
	{
		Dictionary<int, UserDrillInfo> dictionary = new Dictionary<int, UserDrillInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_Users_Drill_All", array);
			while (ResultDataReader.Read())
			{
				UserDrillInfo userDrillInfo = new UserDrillInfo();
				userDrillInfo.UserID = (int)ResultDataReader["UserID"];
				userDrillInfo.BeadPlace = (int)ResultDataReader["BeadPlace"];
				userDrillInfo.HoleLv = (int)ResultDataReader["HoleLv"];
				userDrillInfo.HoleExp = (int)ResultDataReader["HoleExp"];
				userDrillInfo.DrillPlace = (int)ResultDataReader["DrillPlace"];
				if (!dictionary.ContainsKey(userDrillInfo.DrillPlace))
				{
					dictionary.Add(userDrillInfo.DrillPlace, userDrillInfo);
				}
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
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

	public bool AddUserUserDrill(UserDrillInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[6]
			{
				new SqlParameter("@UserID", item.UserID),
				new SqlParameter("@BeadPlace", item.BeadPlace),
				new SqlParameter("@HoleExp", item.HoleExp),
				new SqlParameter("@HoleLv", item.HoleLv),
				new SqlParameter("@DrillPlace", item.DrillPlace),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[5].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Users_UserDrill_Add", array);
			int num = (int)array[5].Value;
			result = num == 0;
			item.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
		}
		return result;
	}

	public bool UpdateUserDrillInfo(UserDrillInfo g)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[6]
			{
				new SqlParameter("@UserID", g.UserID),
				new SqlParameter("@BeadPlace", g.BeadPlace),
				new SqlParameter("@HoleExp", g.HoleExp),
				new SqlParameter("@HoleLv", g.HoleLv),
				new SqlParameter("@DrillPlace", g.DrillPlace),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[5].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_UpdateUserDrillInfo", array);
			result = true;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_UpdateUserDrillInfo", exception);
			}
		}
		return result;
	}

	public UserTreasureInfo GetSingleTreasure(int UserID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_GetSingleTreasure", array);
			if (ResultDataReader.Read())
			{
				UserTreasureInfo userTreasureInfo = new UserTreasureInfo();
				userTreasureInfo.ID = (int)ResultDataReader["ID"];
				userTreasureInfo.UserID = (int)ResultDataReader["UserID"];
				userTreasureInfo.NickName = (string)ResultDataReader["NickName"];
				userTreasureInfo.logoinDays = (int)ResultDataReader["logoinDays"];
				userTreasureInfo.treasure = (int)ResultDataReader["treasure"];
				userTreasureInfo.treasureAdd = (int)ResultDataReader["treasureAdd"];
				userTreasureInfo.friendHelpTimes = (int)ResultDataReader["friendHelpTimes"];
				userTreasureInfo.isEndTreasure = (bool)ResultDataReader["isEndTreasure"];
				userTreasureInfo.isBeginTreasure = (bool)ResultDataReader["isBeginTreasure"];
				userTreasureInfo.LastLoginDay = (DateTime)ResultDataReader["LastLoginDay"];
				return userTreasureInfo;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_GetSingleTreasure", exception);
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

	public List<TreasureDataInfo> GetSingleTreasureData(int UserID)
	{
		SqlDataReader ResultDataReader = null;
		List<TreasureDataInfo> list = new List<TreasureDataInfo>();
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_GetSingleTreasureData", array);
			while (ResultDataReader.Read())
			{
				TreasureDataInfo treasureDataInfo = new TreasureDataInfo();
				treasureDataInfo.ID = (int)ResultDataReader["ID"];
				treasureDataInfo.UserID = (int)ResultDataReader["UserID"];
				treasureDataInfo.TemplateID = (int)ResultDataReader["TemplateID"];
				treasureDataInfo.Count = (int)ResultDataReader["Count"];
				treasureDataInfo.ValidDate = (int)ResultDataReader["Validate"];
				treasureDataInfo.pos = (int)ResultDataReader["pos"];
				treasureDataInfo.BeginDate = (DateTime)ResultDataReader["BeginDate"];
				treasureDataInfo.IsExit = (bool)ResultDataReader["IsExit"];
				list.Add(treasureDataInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_GetSingleTreasureData", exception);
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

	public bool AddUserTreasureInfo(UserTreasureInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[11]
			{
				new SqlParameter("@ID", item.ID),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@UserID", item.UserID);
			array[2] = new SqlParameter("@NickName", item.NickName);
			array[3] = new SqlParameter("@logoinDays", item.logoinDays);
			array[4] = new SqlParameter("@treasure", item.treasure);
			array[5] = new SqlParameter("@treasureAdd", item.treasureAdd);
			array[6] = new SqlParameter("@friendHelpTimes", item.friendHelpTimes);
			array[7] = new SqlParameter("@isEndTreasure", item.isEndTreasure);
			array[8] = new SqlParameter("@isBeginTreasure", item.isBeginTreasure);
			array[9] = new SqlParameter("@LastLoginDay", item.LastLoginDay);
			array[10] = new SqlParameter("@Result", SqlDbType.Int);
			array[10].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Users_Treasure_Add", array);
			result = (int)array[10].Value == 0;
			item.ID = (int)array[0].Value;
			item.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
		}
		return result;
	}

	public bool UpdateUserTreasureInfo(UserTreasureInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[10]
			{
				new SqlParameter("@UserID", item.UserID),
				new SqlParameter("@NickName", item.NickName),
				new SqlParameter("@logoinDays", item.logoinDays),
				new SqlParameter("@treasure", item.treasure),
				new SqlParameter("@treasureAdd", item.treasureAdd),
				new SqlParameter("@friendHelpTimes", item.friendHelpTimes),
				new SqlParameter("@isEndTreasure", item.isEndTreasure),
				new SqlParameter("@isBeginTreasure", item.isBeginTreasure),
				new SqlParameter("@LastLoginDay", item.LastLoginDay),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[9].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_UpdateUserTreasure", array);
			result = (int)array[9].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_UpdateUserTreasure", exception);
			}
		}
		return result;
	}

	public bool AddTreasureData(TreasureDataInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[9]
			{
				new SqlParameter("@ID", item.ID),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@UserID", item.UserID);
			array[2] = new SqlParameter("@TemplateID", item.TemplateID);
			array[3] = new SqlParameter("@Count", item.Count);
			array[4] = new SqlParameter("@Validate", item.ValidDate);
			array[5] = new SqlParameter("@Pos", item.pos);
			array[6] = new SqlParameter("@BeginDate", item.BeginDate);
			array[7] = new SqlParameter("@IsExit", item.IsExit);
			array[8] = new SqlParameter("@Result", SqlDbType.Int);
			array[8].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_TreasureData_Add", array);
			result = (int)array[8].Value == 0;
			item.ID = (int)array[0].Value;
			item.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
		}
		return result;
	}

	public bool UpdateTreasureData(TreasureDataInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[9]
			{
				new SqlParameter("@ID", item.ID),
				new SqlParameter("@UserID", item.UserID),
				new SqlParameter("@TemplateID", item.TemplateID),
				new SqlParameter("@Count", item.Count),
				new SqlParameter("@Validate", item.ValidDate),
				new SqlParameter("@Pos", item.pos),
				new SqlParameter("@BeginDate", item.BeginDate),
				new SqlParameter("@IsExit", item.IsExit),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[8].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_UpdateTreasureData", array);
			result = (int)array[8].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_UpdateTreasureData", exception);
			}
		}
		return result;
	}

	public bool RemoveTreasureDataByUser(int ID)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[2]
			{
				new SqlParameter("@UserID", ID),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[1].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_RemoveTreasureDataByUser", array);
			result = (int)array[1].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_RemoveTreasureDataByUser", exception);
			}
		}
		return result;
	}

	public bool RemoveIsArrange(int ID)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[2]
			{
				new SqlParameter("@UserID", ID),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[1].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_RemoveIsArrange", array);
			result = (int)array[1].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_RemoveIsArrange", exception);
			}
		}
		return result;
	}

	public bool UpdateFriendHelpTimes(int ID)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[2]
			{
				new SqlParameter("@UserID", ID),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[1].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_UpdateFriendHelpTimes", array);
			result = (int)array[1].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_UpdateFriendHelpTimes", exception);
			}
		}
		return result;
	}

	public SysUsersDetailInfo GetSingleSysUsersDetail(int userID)
	{
		List<SysUsersDetailInfo> list = new List<SysUsersDetailInfo>();
		SqlDataReader Sdr = null;
		try
		{
			db.FillSqlDataReader(ref Sdr, "SELECT * FROM [dbo].[Sys_Users_Detail_2] WHERE UserID = " + userID);
			while (Sdr.Read())
			{
				list.Add(InitSysUsersDetailInfo(Sdr));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_GetSingleUsersExtra", exception);
			}
		}
		finally
		{
			if (Sdr != null && !Sdr.IsClosed)
			{
				Sdr.Close();
			}
		}
		return (list.Count > 0) ? list[0] : null;
	}

	public SysUsersDetailInfo InitSysUsersDetailInfo(SqlDataReader reader)
	{
		SysUsersDetailInfo sysUsersDetailInfo = new SysUsersDetailInfo();
		sysUsersDetailInfo.UserID = (int)reader["UserID"];
		sysUsersDetailInfo.UserAllGP = (int)reader["UserAllGP"];
		sysUsersDetailInfo.UserMoneyPay = (int)reader["UserMoneyPay"];
		sysUsersDetailInfo.UserMoneyWar = (int)reader["UserMoneyWar"];
		sysUsersDetailInfo.UserMoneyShop = (int)reader["UserMoneyShop"];
		return sysUsersDetailInfo;
	}

	public bool AddSysUsersDetail(SysUsersDetailInfo item, int UserId)
	{
		bool result = false;
		try
		{
			string sqlcomm = "INSERT INTO [dbo].[Sys_Users_Detail_2]\r\n                                   ([UserID]\r\n                                   ,[UserAllGP]\r\n                                   ,[UserMoneyPay]\r\n                                   ,[UserMoneyWar]\r\n                                   ,[UserMoneyShop])\r\n                               VALUES\r\n                                   (@UserID\r\n                                   ,@UserAllGP\r\n                                   ,@UserMoneyPay\r\n                                   ,@UserMoneyWar\r\n                                   ,@UserMoneyShop)";
			SqlParameter[] sqlParameters = new SqlParameter[5]
			{
				new SqlParameter("@UserID", UserId),
				new SqlParameter("@UserAllGP", item.UserAllGP),
				new SqlParameter("@UserMoneyPay", item.UserMoneyPay),
				new SqlParameter("@UserMoneyWar", item.UserMoneyWar),
				new SqlParameter("@UserMoneyShop", item.UserMoneyShop)
			};
			result = db.Exesqlcomm(sqlcomm, sqlParameters);
			item.IsDirty = false;
		}
		catch (Exception exception)
		{
			BaseBussiness.log.Error("SP_GetSingleUsersExtra", exception);
		}
		return result;
	}

	public bool UpdateSysUsersDetail(SysUsersDetailInfo item)
	{
		bool result = false;
		try
		{
			string sqlcomm = "UPDATE [dbo].[Sys_Users_Detail_2]\r\n                               SET [UserID] = @UserID\r\n                                  ,[UserAllGP] = @UserAllGP\r\n                                  ,[UserMoneyPay] = @UserMoneyPay\r\n                                  ,[UserMoneyWar] = @UserMoneyWar\r\n                                  ,[UserMoneyShop] = @UserMoneyShop\r\n                            WHERE [UserID] = @UserID";
			SqlParameter[] sqlParameters = new SqlParameter[5]
			{
				new SqlParameter("@UserID", item.UserID),
				new SqlParameter("@UserAllGP", item.UserAllGP),
				new SqlParameter("@UserMoneyPay", item.UserMoneyPay),
				new SqlParameter("@UserMoneyWar", item.UserMoneyWar),
				new SqlParameter("@UserMoneyShop", item.UserMoneyShop)
			};
			result = db.Exesqlcomm(sqlcomm, sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_GetSingleUsersExtra", exception);
			}
		}
		return result;
	}

	public UsersExtraInfo GetSingleUsersExtra(int UserID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_GetSingleUsersExtra", array);
			if (ResultDataReader.Read())
			{
				UsersExtraInfo usersExtraInfo = new UsersExtraInfo();
				usersExtraInfo.ID = (int)ResultDataReader["ID"];
				usersExtraInfo.UserID = (int)ResultDataReader["UserID"];
				usersExtraInfo.starlevel = (int)ResultDataReader["starlevel"];
				usersExtraInfo.nowPosition = (int)ResultDataReader["nowPosition"];
				usersExtraInfo.FreeCount = (int)ResultDataReader["FreeCount"];
				usersExtraInfo.SearchGoodItems = ((ResultDataReader["SearchGoodItems"] == null) ? "" : ResultDataReader["SearchGoodItems"].ToString());
				usersExtraInfo.FreeAddAutionCount = (int)ResultDataReader["FreeAddAutionCount"];
				usersExtraInfo.FreeSendMailCount = (int)ResultDataReader["FreeSendMailCount"];
				usersExtraInfo.KingBlessInfo = ((ResultDataReader["KingBlessInfo"] == null) ? "" : ResultDataReader["KingBlessInfo"].ToString());
				usersExtraInfo.MissionEnergy = (int)ResultDataReader["MissionEnergy"];
				usersExtraInfo.buyEnergyCount = (int)ResultDataReader["buyEnergyCount"];
				usersExtraInfo.KingBlessEnddate = (DateTime)ResultDataReader["KingBlessEnddate"];
				usersExtraInfo.KingBlessIndex = (int)ResultDataReader["KingBlessIndex"];
				usersExtraInfo.DressModelArr = ((ResultDataReader["DressModelArr"] == null) ? ",," : ResultDataReader["DressModelArr"].ToString());
				usersExtraInfo.CurentDressModel = (int)ResultDataReader["CurentDressModel"];
				usersExtraInfo.ScoreMagicstone = (int)ResultDataReader["ScoreMagicstone"];
				usersExtraInfo.DeedInfo = ((ResultDataReader["DeedInfo"] == null) ? "" : ResultDataReader["DeedInfo"].ToString());
				usersExtraInfo.DeedEnddate = (DateTime)ResultDataReader["DeedEnddate"];
				usersExtraInfo.DeedIndex = (int)ResultDataReader["DeedIndex"];
				usersExtraInfo.Score = (int)ResultDataReader["Score"];
				usersExtraInfo.SummerScore = (int)ResultDataReader["SummerScore"];
				usersExtraInfo.CatchInsectGetPrize = ((ResultDataReader["CatchInsectGetPrize"] == null) ? "" : ResultDataReader["CatchInsectGetPrize"].ToString());
				usersExtraInfo.PrizeStatus = (int)ResultDataReader["PrizeStatus"];
				usersExtraInfo.CakeStatus = (bool)ResultDataReader["CakeStatus"];
				usersExtraInfo.NormalFightNum = (int)ResultDataReader["NormalFightNum"];
				usersExtraInfo.HardFightNum = (int)ResultDataReader["HardFightNum"];
				usersExtraInfo.IsDoubleScore = (bool)ResultDataReader["IsDoubleScore"];
				usersExtraInfo.MagpieBridgeItems = ((ResultDataReader["MagpieBridgeItems"] == null) ? "" : ResultDataReader["MagpieBridgeItems"].ToString());
				usersExtraInfo.NowPositionMB = (int)ResultDataReader["NowPositionMB"];
				usersExtraInfo.LastNum = (int)ResultDataReader["LastNum"];
				usersExtraInfo.MagpieNum = (int)ResultDataReader["MagpieNum"];
				usersExtraInfo.FreePresentGoodsCount = (int)ResultDataReader["FreePresentGoodsCount"];
				usersExtraInfo.coupleNum = (int)ResultDataReader["coupleNum"];
				usersExtraInfo.dungeonNum = (int)ResultDataReader["dungeonNum"];
				usersExtraInfo.propsNum = (int)ResultDataReader["propsNum"];
				usersExtraInfo.silverTime = (DateTime)ResultDataReader["silverTime"];
				usersExtraInfo.silverCount = (int)ResultDataReader["silverCount"];
				usersExtraInfo.goldTime = (DateTime)ResultDataReader["goldTime"];
				usersExtraInfo.EnergyTotal = (int)ResultDataReader["EnergyTotal"];
				usersExtraInfo.dungeonCount = ((ResultDataReader["dungeonCount"] == DBNull.Value) ? "" : ResultDataReader["dungeonCount"].ToString());
				usersExtraInfo.loginDeviceDownAward = ResultDataReader["loginDeviceDownAward"] != DBNull.Value && (bool)ResultDataReader["loginDeviceDownAward"];
				usersExtraInfo.loginDeviceDailyAward = ResultDataReader["loginDeviceDailyAward"] != DBNull.Value && (bool)ResultDataReader["loginDeviceDailyAward"];
				usersExtraInfo.curcentRoomBordenTemplateId = ((ResultDataReader["curcentRoomBordenTemplateId"] != DBNull.Value) ? ((int)ResultDataReader["curcentRoomBordenTemplateId"]) : 0);
				usersExtraInfo.pvePowerBuffData = ((ResultDataReader["pvePowerBuffData"] == DBNull.Value) ? "" : ((string)ResultDataReader["pvePowerBuffData"]));
				usersExtraInfo.FreeCountMark = ((ResultDataReader["FreeCountMark"] == DBNull.Value) ? GameProperties.EngraveVaultsFreeTimes : ((int)ResultDataReader["FreeCountMark"]));
				usersExtraInfo.FreeTimeMark = ((ResultDataReader["FreeTimeMark"] == DBNull.Value) ? DateTime.Now : ((DateTime)ResultDataReader["FreeTimeMark"]));
				usersExtraInfo.SoulCrystal = ((ResultDataReader["SoulCrystal"] != DBNull.Value) ? ((int)ResultDataReader["SoulCrystal"]) : 0);
				usersExtraInfo.Coin = (int)ResultDataReader["Coin"];
				usersExtraInfo.SummonCount1 = (int)ResultDataReader["SummonCount1"];
				usersExtraInfo.SummonCount2 = (int)ResultDataReader["SummonCount2"];
				return usersExtraInfo;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_GetSingleUsersExtra", exception);
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

	public bool AddUsersExtra(UsersExtraInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[51];
			array[0] = new SqlParameter("@ID", item.ID);
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@UserID", item.UserID);
			array[2] = new SqlParameter("@starlevel", item.starlevel);
			array[3] = new SqlParameter("@nowPosition", item.nowPosition);
			array[4] = new SqlParameter("@FreeCount", item.FreeCount);
			array[5] = new SqlParameter("@SearchGoodItems", item.SearchGoodItems);
			array[6] = new SqlParameter("@FreeAddAutionCount", item.FreeAddAutionCount);
			array[7] = new SqlParameter("@FreeSendMailCount", item.FreeSendMailCount);
			array[8] = new SqlParameter("@KingBlessInfo", item.KingBlessInfo);
			array[9] = new SqlParameter("@MissionEnergy", item.MissionEnergy);
			array[10] = new SqlParameter("@buyEnergyCount", item.buyEnergyCount);
			array[11] = new SqlParameter("@KingBlessEnddate", item.KingBlessEnddate);
			array[12] = new SqlParameter("@KingBlessIndex", item.KingBlessIndex);
			array[13] = new SqlParameter("@DressModelArr", item.DressModelArr);
			array[14] = new SqlParameter("@CurentDressModel", item.CurentDressModel);
			array[15] = new SqlParameter("@ScoreMagicstone", item.ScoreMagicstone);
			array[16] = new SqlParameter("@DeedInfo", item.DeedInfo);
			array[17] = new SqlParameter("@DeedEnddate", item.DeedEnddate);
			array[18] = new SqlParameter("@DeedIndex", item.DeedIndex);
			array[19] = new SqlParameter("@Result", SqlDbType.Int);
			array[19].Direction = ParameterDirection.ReturnValue;
			array[20] = new SqlParameter("@Score", item.Score);
			array[21] = new SqlParameter("@SummerScore", item.SummerScore);
			array[22] = new SqlParameter("@CatchInsectGetPrize", item.CatchInsectGetPrize);
			array[23] = new SqlParameter("@PrizeStatus", item.PrizeStatus);
			array[24] = new SqlParameter("@CakeStatus", item.CakeStatus);
			array[25] = new SqlParameter("@NormalFightNum", item.NormalFightNum);
			array[26] = new SqlParameter("@HardFightNum", item.HardFightNum);
			array[27] = new SqlParameter("@IsDoubleScore", item.IsDoubleScore);
			array[28] = new SqlParameter("@MagpieBridgeItems", item.MagpieBridgeItems);
			array[29] = new SqlParameter("@NowPositionMB", item.NowPositionMB);
			array[30] = new SqlParameter("@LastNum", item.LastNum);
			array[31] = new SqlParameter("@MagpieNum", item.MagpieNum);
			array[32] = new SqlParameter("@FreePresentGoodsCount", item.FreePresentGoodsCount);
			array[33] = new SqlParameter("@coupleNum", item.coupleNum);
			array[34] = new SqlParameter("@dungeonNum", item.dungeonNum);
			array[35] = new SqlParameter("@propsNum", item.propsNum);
			array[36] = new SqlParameter("@silverTime", item.silverTime);
			array[37] = new SqlParameter("@silverCount", item.silverCount);
			array[38] = new SqlParameter("@goldTime", item.goldTime);
			array[39] = new SqlParameter("@EnergyTotal", item.EnergyTotal);
			array[40] = new SqlParameter("@dungeonCount", item.dungeonCount);
			array[41] = new SqlParameter("@loginDeviceDownAward", item.loginDeviceDownAward);
			array[42] = new SqlParameter("@loginDeviceDailyAward", item.loginDeviceDailyAward);
			array[43] = new SqlParameter("@curcentRoomBordenTemplateId", item.curcentRoomBordenTemplateId);
			array[44] = new SqlParameter("@pvePowerBuffData", item.pvePowerBuffData);
			array[45] = new SqlParameter("@FreeCountMark", item.FreeCountMark);
			array[46] = new SqlParameter("@FreeTimeMark", item.FreeTimeMark);
			array[47] = new SqlParameter("@SoulCrystal", item.SoulCrystal);
			array[48] = new SqlParameter("@Coin", item.Coin);
			array[49] = new SqlParameter("@SummonCount1", item.SummonCount1);
			array[50] = new SqlParameter("@SummonCount2", item.SummonCount2);
			db.RunProcedure("SP_UsersExtra_Add", array);
			result = (int)array[19].Value == 0;
			item.ID = (int)array[0].Value;
			item.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_UsersExtra_Add", exception);
			}
		}
		finally
		{
		}
		return result;
	}

	public bool UpdateUsersExtra(UsersExtraInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[51]
			{
				new SqlParameter("@ID", item.ID),
				new SqlParameter("@UserID", item.UserID),
				new SqlParameter("@starlevel", item.starlevel),
				new SqlParameter("@nowPosition", item.nowPosition),
				new SqlParameter("@FreeCount", item.FreeCount),
				new SqlParameter("@SearchGoodItems", item.SearchGoodItems),
				new SqlParameter("@FreeAddAutionCount", item.FreeAddAutionCount),
				new SqlParameter("@FreeSendMailCount", item.FreeSendMailCount),
				new SqlParameter("@KingBlessInfo", item.KingBlessInfo),
				new SqlParameter("@MissionEnergy", item.MissionEnergy),
				new SqlParameter("@buyEnergyCount", item.buyEnergyCount),
				new SqlParameter("@KingBlessEnddate", item.KingBlessEnddate),
				new SqlParameter("@KingBlessIndex", item.KingBlessIndex),
				new SqlParameter("@DressModelArr", item.DressModelArr),
				new SqlParameter("@CurentDressModel", item.CurentDressModel),
				new SqlParameter("@ScoreMagicstone", item.ScoreMagicstone),
				new SqlParameter("@DeedInfo", item.DeedInfo),
				new SqlParameter("@DeedEnddate", item.DeedEnddate),
				new SqlParameter("@DeedIndex", item.DeedIndex),
				new SqlParameter("@Result", SqlDbType.Int),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[19].Direction = ParameterDirection.ReturnValue;
			array[20] = new SqlParameter("@Score", item.Score);
			array[21] = new SqlParameter("@SummerScore", item.SummerScore);
			array[22] = new SqlParameter("@CatchInsectGetPrize", item.CatchInsectGetPrize);
			array[23] = new SqlParameter("@PrizeStatus", item.PrizeStatus);
			array[24] = new SqlParameter("@CakeStatus", item.CakeStatus);
			array[25] = new SqlParameter("@NormalFightNum", item.NormalFightNum);
			array[26] = new SqlParameter("@HardFightNum", item.HardFightNum);
			array[27] = new SqlParameter("@IsDoubleScore", item.IsDoubleScore);
			array[28] = new SqlParameter("@MagpieBridgeItems", item.MagpieBridgeItems);
			array[29] = new SqlParameter("@NowPositionMB", item.NowPositionMB);
			array[30] = new SqlParameter("@LastNum", item.LastNum);
			array[31] = new SqlParameter("@MagpieNum", item.MagpieNum);
			array[32] = new SqlParameter("@FreePresentGoodsCount", item.FreePresentGoodsCount);
			array[33] = new SqlParameter("@coupleNum", item.coupleNum);
			array[34] = new SqlParameter("@dungeonNum", item.dungeonNum);
			array[35] = new SqlParameter("@propsNum", item.propsNum);
			array[36] = new SqlParameter("@silverTime", item.silverTime);
			array[37] = new SqlParameter("@silverCount", item.silverCount);
			array[38] = new SqlParameter("@goldTime", item.goldTime);
			array[39] = new SqlParameter("@EnergyTotal", item.EnergyTotal);
			array[40] = new SqlParameter("@dungeonCount", item.dungeonCount);
			array[41] = new SqlParameter("@loginDeviceDownAward", item.loginDeviceDownAward);
			array[42] = new SqlParameter("@loginDeviceDailyAward", item.loginDeviceDailyAward);
			array[43] = new SqlParameter("@curcentRoomBordenTemplateId", item.curcentRoomBordenTemplateId);
			array[44] = new SqlParameter("@pvePowerBuffData", item.pvePowerBuffData);
			array[45] = new SqlParameter("@FreeCountMark", item.FreeCountMark);
			array[46] = new SqlParameter("@FreeTimeMark", item.FreeTimeMark);
			array[47] = new SqlParameter("@SoulCrystal", item.SoulCrystal);
			array[48] = new SqlParameter("@Coin", item.Coin);
			array[49] = new SqlParameter("@SummonCount1", item.SummonCount1);
			array[50] = new SqlParameter("@SummonCount2", item.SummonCount2);
			db.RunProcedure("SP_UpdateUsersExtra", array);
			result = (int)array[19].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_UpdateUsersExtra", exception);
			}
		}
		return result;
	}

	public PyramidInfo GetSinglePyramid(int UserID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_GetSinglePyramid", array);
			if (ResultDataReader.Read())
			{
				PyramidInfo pyramidInfo = new PyramidInfo();
				pyramidInfo.ID = (int)ResultDataReader["ID"];
				pyramidInfo.UserID = (int)ResultDataReader["UserID"];
				pyramidInfo.currentLayer = (int)ResultDataReader["currentLayer"];
				pyramidInfo.maxLayer = (int)ResultDataReader["maxLayer"];
				pyramidInfo.totalPoint = (int)ResultDataReader["totalPoint"];
				pyramidInfo.turnPoint = (int)ResultDataReader["turnPoint"];
				pyramidInfo.pointRatio = (int)ResultDataReader["pointRatio"];
				pyramidInfo.currentFreeCount = (int)ResultDataReader["currentFreeCount"];
				pyramidInfo.currentReviveCount = (int)ResultDataReader["currentReviveCount"];
				pyramidInfo.isPyramidStart = (bool)ResultDataReader["isPyramidStart"];
				pyramidInfo.LayerItems = (string)ResultDataReader["LayerItems"];
				return pyramidInfo;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_GetSinglePyramid", exception);
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

	public bool AddPyramid(PyramidInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[12]
			{
				new SqlParameter("@ID", info.ID),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@UserID", info.UserID);
			array[2] = new SqlParameter("@currentLayer", info.currentLayer);
			array[3] = new SqlParameter("@maxLayer", info.maxLayer);
			array[4] = new SqlParameter("@totalPoint", info.totalPoint);
			array[5] = new SqlParameter("@turnPoint", info.turnPoint);
			array[6] = new SqlParameter("@pointRatio", info.pointRatio);
			array[7] = new SqlParameter("@currentFreeCount", info.currentFreeCount);
			array[8] = new SqlParameter("@currentReviveCount", info.currentReviveCount);
			array[9] = new SqlParameter("@isPyramidStart", info.isPyramidStart);
			array[10] = new SqlParameter("@LayerItems", info.LayerItems);
			array[11] = new SqlParameter("@Result", SqlDbType.Int);
			array[11].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Pyramid_Add", array);
			result = (int)array[11].Value == 0;
			info.ID = (int)array[0].Value;
			info.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_Pyramid_Add", exception);
			}
		}
		finally
		{
		}
		return result;
	}

	public bool UpdatePyramid(PyramidInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[12]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@currentLayer", info.currentLayer),
				new SqlParameter("@maxLayer", info.maxLayer),
				new SqlParameter("@totalPoint", info.totalPoint),
				new SqlParameter("@turnPoint", info.turnPoint),
				new SqlParameter("@pointRatio", info.pointRatio),
				new SqlParameter("@currentFreeCount", info.currentFreeCount),
				new SqlParameter("@currentReviveCount", info.currentReviveCount),
				new SqlParameter("@isPyramidStart", info.isPyramidStart),
				new SqlParameter("@LayerItems", info.LayerItems),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[11].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_UpdatePyramid", array);
			result = (int)array[11].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_UpdatePyramid", exception);
			}
		}
		return result;
	}

	public UserAvatarColectionInfo[] GetSingleUserAvatarColectionInfo(int UserID)
	{
		List<UserAvatarColectionInfo> list = new List<UserAvatarColectionInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_Get_Single_User_Avatar_Colection", array);
			while (ResultDataReader.Read())
			{
				UserAvatarColectionInfo item = new UserAvatarColectionInfo
				{
					ID = (int)ResultDataReader["ID"],
					UserID = (int)ResultDataReader["UserID"],
					endTime = (DateTime)ResultDataReader["endTime"],
					dataId = (int)ResultDataReader["dataId"],
					ActiveCount = (int)ResultDataReader["ActiveCount"],
					Sex = (int)ResultDataReader["Sex"],
					ActiveDress = ((ResultDataReader["ActiveDress"] == null) ? "" : ResultDataReader["ActiveDress"].ToString())
				};
				list.Add(item);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_Get_Single_User_Avatar_Colection", exception);
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

	public bool AddUserAvatarColectionInfo(UserAvatarColectionInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[8]
			{
				new SqlParameter("@ID", info.ID),
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@UserID", info.UserID);
			array[2] = new SqlParameter("@endTime", info.endTime);
			array[3] = new SqlParameter("@dataId", info.dataId);
			array[4] = new SqlParameter("@ActiveCount", info.ActiveCount);
			array[5] = new SqlParameter("@Sex", info.Sex);
			array[6] = new SqlParameter("@ActiveDress", info.ActiveDress);
			array[7] = new SqlParameter("@Result", SqlDbType.Int);
			array[7].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_User_Avatar_Colection_Add", array);
			result = (int)array[7].Value == 0;
			info.ID = (int)array[0].Value;
			info.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_User_Avatar_Colection_Add", exception);
			}
		}
		finally
		{
		}
		return result;
	}

	public bool UpdateUserAvatarColectionInfo(UserAvatarColectionInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[8]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@endTime", info.endTime),
				new SqlParameter("@dataId", info.dataId),
				new SqlParameter("@ActiveCount", info.ActiveCount),
				new SqlParameter("@Sex", info.Sex),
				new SqlParameter("@ActiveDress", info.ActiveDress),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[7].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_User_Avatar_Colection_Update", array);
			result = (int)array[7].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_User_Avatar_Colection_Update", exception);
			}
		}
		return result;
	}

	public NewChickenBoxItemInfo[] GetSingleNewChickenBox(int UserID)
	{
		List<NewChickenBoxItemInfo> list = new List<NewChickenBoxItemInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_GetSingleNewChickenBox", array);
			while (ResultDataReader.Read())
			{
				NewChickenBoxItemInfo newChickenBoxItemInfo = new NewChickenBoxItemInfo();
				newChickenBoxItemInfo.ID = (int)ResultDataReader["ID"];
				newChickenBoxItemInfo.UserID = (int)ResultDataReader["UserID"];
				newChickenBoxItemInfo.TemplateID = (int)ResultDataReader["TemplateID"];
				newChickenBoxItemInfo.Count = (int)ResultDataReader["Count"];
				newChickenBoxItemInfo.ValidDate = (int)ResultDataReader["ValidDate"];
				newChickenBoxItemInfo.StrengthenLevel = (int)ResultDataReader["StrengthenLevel"];
				newChickenBoxItemInfo.AttackCompose = (int)ResultDataReader["AttackCompose"];
				newChickenBoxItemInfo.DefendCompose = (int)ResultDataReader["DefendCompose"];
				newChickenBoxItemInfo.AgilityCompose = (int)ResultDataReader["AgilityCompose"];
				newChickenBoxItemInfo.LuckCompose = (int)ResultDataReader["LuckCompose"];
				newChickenBoxItemInfo.Position = (int)ResultDataReader["Position"];
				newChickenBoxItemInfo.IsSelected = (bool)ResultDataReader["IsSelected"];
				newChickenBoxItemInfo.IsSeeded = (bool)ResultDataReader["IsSeeded"];
				newChickenBoxItemInfo.IsBinds = (bool)ResultDataReader["IsBinds"];
				list.Add(newChickenBoxItemInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_GetSingleNewChickenBox", exception);
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

	public bool AddNewChickenBox(NewChickenBoxItemInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[15]
			{
				new SqlParameter("@ID", info.ID),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@UserID", info.UserID);
			array[2] = new SqlParameter("@TemplateID", info.TemplateID);
			array[3] = new SqlParameter("@Count", info.Count);
			array[4] = new SqlParameter("@ValidDate", info.ValidDate);
			array[5] = new SqlParameter("@StrengthenLevel", info.StrengthenLevel);
			array[6] = new SqlParameter("@AttackCompose", info.AttackCompose);
			array[7] = new SqlParameter("@DefendCompose", info.DefendCompose);
			array[8] = new SqlParameter("@AgilityCompose", info.AgilityCompose);
			array[9] = new SqlParameter("@LuckCompose", info.LuckCompose);
			array[10] = new SqlParameter("@Position", info.Position);
			array[11] = new SqlParameter("@IsSelected", info.IsSelected);
			array[12] = new SqlParameter("@IsSeeded", info.IsSeeded);
			array[13] = new SqlParameter("@IsBinds", info.IsBinds);
			array[14] = new SqlParameter("@Result", SqlDbType.Int);
			array[14].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_NewChickenBox_Add", array);
			result = (int)array[14].Value == 0;
			info.ID = (int)array[0].Value;
			info.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_NewChickenBox_Add", exception);
			}
		}
		finally
		{
		}
		return result;
	}

	public bool UpdateNewChickenBox(NewChickenBoxItemInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[15]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@TemplateID", info.TemplateID),
				new SqlParameter("@Count", info.Count),
				new SqlParameter("@ValidDate", info.ValidDate),
				new SqlParameter("@StrengthenLevel", info.StrengthenLevel),
				new SqlParameter("@AttackCompose", info.AttackCompose),
				new SqlParameter("@DefendCompose", info.DefendCompose),
				new SqlParameter("@AgilityCompose", info.AgilityCompose),
				new SqlParameter("@LuckCompose", info.LuckCompose),
				new SqlParameter("@Position", info.Position),
				new SqlParameter("@IsSelected", info.IsSelected),
				new SqlParameter("@IsSeeded", info.IsSeeded),
				new SqlParameter("@IsBinds", info.IsBinds),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[14].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_UpdateNewChickenBox", array);
			result = (int)array[14].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_UpdateNewChickenBox", exception);
			}
		}
		return result;
	}

	public UserChristmasInfo GetSingleUserChristmas(int UserID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_GetSingleUserChristmas", array);
			if (ResultDataReader.Read())
			{
				UserChristmasInfo userChristmasInfo = new UserChristmasInfo();
				userChristmasInfo.ID = (int)ResultDataReader["ID"];
				userChristmasInfo.UserID = (int)ResultDataReader["UserID"];
				userChristmasInfo.exp = (int)ResultDataReader["exp"];
				userChristmasInfo.awardState = (int)ResultDataReader["awardState"];
				userChristmasInfo.count = (int)ResultDataReader["count"];
				userChristmasInfo.packsNumber = (int)ResultDataReader["packsNumber"];
				userChristmasInfo.lastPacks = (int)ResultDataReader["lastPacks"];
				userChristmasInfo.gameBeginTime = (DateTime)ResultDataReader["gameBeginTime"];
				userChristmasInfo.gameEndTime = (DateTime)ResultDataReader["gameEndTime"];
				userChristmasInfo.isEnter = (bool)ResultDataReader["isEnter"];
				userChristmasInfo.dayPacks = (int)ResultDataReader["dayPacks"];
				userChristmasInfo.AvailTime = (int)ResultDataReader["AvailTime"];
				return userChristmasInfo;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_GetSingleUserChristmas", exception);
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

	public bool AddUserChristmas(UserChristmasInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[13]
			{
				new SqlParameter("@ID", info.ID),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@UserID", info.UserID);
			array[2] = new SqlParameter("@exp", info.exp);
			array[3] = new SqlParameter("@awardState", info.awardState);
			array[4] = new SqlParameter("@count", info.count);
			array[5] = new SqlParameter("@packsNumber", info.packsNumber);
			array[6] = new SqlParameter("@lastPacks", info.lastPacks);
			array[7] = new SqlParameter("@gameBeginTime", info.gameBeginTime);
			array[8] = new SqlParameter("@gameEndTime", info.gameEndTime);
			array[9] = new SqlParameter("@isEnter", info.isEnter);
			array[10] = new SqlParameter("@dayPacks", info.dayPacks);
			array[11] = new SqlParameter("@AvailTime", info.AvailTime);
			array[12] = new SqlParameter("@Result", SqlDbType.Int);
			array[12].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_UserChristmas_Add", array);
			result = (int)array[12].Value == 0;
			info.ID = (int)array[0].Value;
			info.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
		}
		return result;
	}

	public bool UpdateUserChristmas(UserChristmasInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[13]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@exp", info.exp),
				new SqlParameter("@awardState", info.awardState),
				new SqlParameter("@count", info.count),
				new SqlParameter("@packsNumber", info.packsNumber),
				new SqlParameter("@lastPacks", info.lastPacks),
				new SqlParameter("@gameBeginTime", info.gameBeginTime),
				new SqlParameter("@gameEndTime", info.gameEndTime),
				new SqlParameter("@isEnter", info.isEnter),
				new SqlParameter("@dayPacks", info.dayPacks),
				new SqlParameter("@AvailTime", info.AvailTime),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[12].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_UpdateUserChristmas", array);
			result = (int)array[12].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_UpdateUserChristmas", exception);
			}
		}
		return result;
	}

	public ActiveSystemInfo[] GetAllActiveSystemData()
	{
		List<ActiveSystemInfo> list = new List<ActiveSystemInfo>();
		if (VersionFlash == "3.4" || VersionFlash == "3.6")
		{
			return list.ToArray();
		}
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_ActiveSystem_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitActiveSystem(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllActiveSystem", exception);
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

	public TreasurePuzzleDataInfo[] GetAllTreasurePuzzleDataByID(int UserID)
	{
		List<TreasurePuzzleDataInfo> list = new List<TreasurePuzzleDataInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_Treasure_Puzzle_Data_All_ByID", array);
			while (ResultDataReader.Read())
			{
				list.Add(InitTreasurePuzzleDataInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("InitTreasurePuzzleDataInfo", exception);
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

	public bool UpdateTreasurePuzzleData(TreasurePuzzleDataInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[16]
			{
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@PuzzleID", info.PuzzleID),
				new SqlParameter("@hole1Need", info.hole1Need),
				new SqlParameter("@hole1Have", info.hole1Have),
				new SqlParameter("@hole2Need", info.hole2Need),
				new SqlParameter("@hole2Have", info.hole2Have),
				new SqlParameter("@hole3Need", info.hole3Need),
				new SqlParameter("@hole3Have", info.hole3Have),
				new SqlParameter("@hole4Need", info.hole4Need),
				new SqlParameter("@hole4Have", info.hole4Have),
				new SqlParameter("@hole5Need", info.hole5Need),
				new SqlParameter("@hole5Have", info.hole5Have),
				new SqlParameter("@hole6Need", info.hole6Need),
				new SqlParameter("@hole6Have", info.hole6Have),
				new SqlParameter("@IsGetAward", info.IsGetAward),
				new SqlParameter("@ID", info.ID)
			};
			result = db.RunProcedure("SP_Treasure_Puzzle_Data_Update", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_Treasure_Puzzle_Data_Update", exception);
			}
		}
		return result;
	}

	public bool AddTreasurePuzzleData(TreasurePuzzleDataInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[16]
			{
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@PuzzleID", info.PuzzleID),
				new SqlParameter("@hole1Need", info.hole1Need),
				new SqlParameter("@hole1Have", info.hole1Have),
				new SqlParameter("@hole2Need", info.hole2Need),
				new SqlParameter("@hole2Have", info.hole2Have),
				new SqlParameter("@hole3Need", info.hole3Need),
				new SqlParameter("@hole3Have", info.hole3Have),
				new SqlParameter("@hole4Need", info.hole4Need),
				new SqlParameter("@hole4Have", info.hole4Have),
				new SqlParameter("@hole5Need", info.hole5Need),
				new SqlParameter("@hole5Have", info.hole5Have),
				new SqlParameter("@hole6Need", info.hole6Need),
				new SqlParameter("@hole6Have", info.hole6Have),
				new SqlParameter("@IsGetAward", info.IsGetAward),
				new SqlParameter("@ID", info.ID)
			};
			array[15].Direction = ParameterDirection.Output;
			result = db.RunProcedure("SP_Treasure_Puzzle_Data_Add", array);
			info.ID = (int)array[15].Value;
			info.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_Treasure_Puzzle_Data_Add", exception);
			}
		}
		return result;
	}

	public TreasurePuzzleDataInfo InitTreasurePuzzleDataInfo(SqlDataReader dr)
	{
		TreasurePuzzleDataInfo treasurePuzzleDataInfo = new TreasurePuzzleDataInfo();
		treasurePuzzleDataInfo.ID = (int)dr["ID"];
		treasurePuzzleDataInfo.UserID = (int)dr["UserID"];
		treasurePuzzleDataInfo.PuzzleID = (int)dr["PuzzleID"];
		treasurePuzzleDataInfo.hole1Need = (int)dr["hole1Need"];
		treasurePuzzleDataInfo.hole1Have = (int)dr["hole1Have"];
		treasurePuzzleDataInfo.hole2Need = (int)dr["hole2Need"];
		treasurePuzzleDataInfo.hole2Have = (int)dr["hole2Have"];
		treasurePuzzleDataInfo.hole3Need = (int)dr["hole3Need"];
		treasurePuzzleDataInfo.hole3Have = (int)dr["hole3Have"];
		treasurePuzzleDataInfo.hole4Need = (int)dr["hole4Need"];
		treasurePuzzleDataInfo.hole4Have = (int)dr["hole4Have"];
		treasurePuzzleDataInfo.hole5Need = (int)dr["hole5Need"];
		treasurePuzzleDataInfo.hole5Have = (int)dr["hole5Have"];
		treasurePuzzleDataInfo.hole6Need = (int)dr["hole6Need"];
		treasurePuzzleDataInfo.hole6Have = (int)dr["hole6Have"];
		return treasurePuzzleDataInfo;
	}

	public ActiveSystemInfo GetSingleActiveSystem(int userId)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@UserID", userId)
			};
			db.GetReader(ref ResultDataReader, "SP_Sys_Active_System_Data_Single", sqlParameters);
			if (ResultDataReader.Read())
			{
				return InitActiveSystem(ResultDataReader);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetSigleActiveSystem", exception);
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

	public ActiveSystemInfo InitActiveSystem(SqlDataReader reader)
	{
		ActiveSystemInfo activeSystemInfo = new ActiveSystemInfo();
		activeSystemInfo.ID = (int)reader["ID"];
		activeSystemInfo.UserID = (int)reader["UserID"];
		activeSystemInfo.useableScore = (int)reader["useableScore"];
		activeSystemInfo.totalScore = (int)reader["totalScore"];
		activeSystemInfo.AvailTime = (int)reader["AvailTime"];
		activeSystemInfo.NickName = ((reader["NickName"] == null) ? "" : reader["NickName"].ToString());
		activeSystemInfo.dayScore = (int)reader["dayScore"];
		activeSystemInfo.CanGetGift = (bool)reader["CanGetGift"];
		activeSystemInfo.canOpenCounts = (int)reader["canOpenCounts"];
		activeSystemInfo.canEagleEyeCounts = (int)reader["canEagleEyeCounts"];
		activeSystemInfo.lastFlushTime = (DateTime)reader["lastFlushTime"];
		activeSystemInfo.isShowAll = (bool)reader["isShowAll"];
		activeSystemInfo.ActiveMoney = (int)reader["AvtiveMoney"];
		activeSystemInfo.activityTanabataNum = (int)reader["activityTanabataNum"];
		activeSystemInfo.ChallengeNum = (int)reader["ChallengeNum"];
		activeSystemInfo.BuyBuffNum = (int)reader["BuyBuffNum"];
		activeSystemInfo.lastEnterYearMonter = (DateTime)reader["lastEnterYearMonter"];
		activeSystemInfo.DamageNum = (int)reader["DamageNum"];
		activeSystemInfo.BoxState = ((reader["BoxState"] == DBNull.Value) ? "" : reader["BoxState"].ToString());
		activeSystemInfo.LuckystarCoins = (int)reader["LuckystarCoins"];
		activeSystemInfo.CryptBoss = ((reader["CryptBoss"] == DBNull.Value) ? "" : reader["CryptBoss"].ToString());
		activeSystemInfo.DDPlayPoint = (int)reader["DDPlayPoint"];
		activeSystemInfo.updateFreeCounts = (int)reader["updateFreeCounts"];
		activeSystemInfo.updateWorshipedCounts = (int)reader["updateWorshipedCounts"];
		activeSystemInfo.update200State = (int)reader["update200State"];
		activeSystemInfo.lastEnterWorshiped = (DateTime)reader["lastEnterWorshiped"];
		activeSystemInfo.luckCount = (int)reader["luckCount"];
		activeSystemInfo.remainTimes = (int)reader["remainTimes"];
		activeSystemInfo.LastRefresh = (DateTime)reader["LastRefresh"];
		activeSystemInfo.CurRefreshedTimes = (int)reader["CurRefreshedTimes"];
		activeSystemInfo.EntertamentPoint = (int)reader["EntertamentPoint"];
		activeSystemInfo.ChickActiveData = ((reader["ChickActiveData"] == DBNull.Value) ? "" : reader["ChickActiveData"].ToString());
		activeSystemInfo.CurrentSelectIndex = (int)reader["CurrentSelectIndex"];
		activeSystemInfo.CurrentLevel = (int)reader["CurrentLevel"];
		activeSystemInfo.HomeTempleExp = (int)reader["HomeTempleExp"];
		activeSystemInfo.loginDays = (int)reader["loginDays"];
		activeSystemInfo.lastLoginDays = ((reader["lastLoginDays"] == DBNull.Value) ? DateTime.Now.AddDays(-1.0) : ((DateTime)reader["lastLoginDays"]));
		activeSystemInfo.wellAddRate = (int)reader["wellAddRate"];
		activeSystemInfo.fightPowerSignData = ((reader["fightPowerSignData"] == DBNull.Value) ? "" : ((string)reader["fightPowerSignData"]));
		activeSystemInfo.SXMapInfoData = ((reader["SXMapInfoData"] == DBNull.Value) ? "" : ((string)reader["SXMapInfoData"]));
		activeSystemInfo.SXCrystal = (int)reader["SXCrystal"];
		activeSystemInfo.SXScore = (int)reader["SXScore"];
		activeSystemInfo.SXStepRemain = (int)reader["SXStepRemain"];
		activeSystemInfo.SXRewardsGet = ((reader["SXRewardsGet"] == DBNull.Value) ? "" : ((string)reader["SXRewardsGet"]));
		activeSystemInfo.MiniShopBuyCount = ((reader["MiniShopBuyCount"] == DBNull.Value) ? "" : ((string)reader["MiniShopBuyCount"]));
		activeSystemInfo.MemoryGameCount = (int)reader["MemoryGameCount"];
		activeSystemInfo.MemoryGameScore = (int)reader["MemoryGameScore"];
		activeSystemInfo.MemoryGameData = ((reader["MemoryGameData"] == DBNull.Value) ? "" : ((string)reader["MemoryGameData"]));
		activeSystemInfo.MemoryGameAwardGetData = ((reader["MemoryGameAwardGetData"] == DBNull.Value) ? "" : ((string)reader["MemoryGameAwardGetData"]));
		activeSystemInfo.cardScore = (int)reader["cardScore"];
		activeSystemInfo.cardFreeCount = (int)reader["cardFreeCount"];
		activeSystemInfo.cardChipCount = (int)reader["cardChipCount"];
		activeSystemInfo.cardListCard = ((reader["cardListCard"] == DBNull.Value) ? "" : reader["cardListCard"].ToString());
		activeSystemInfo.cardListExchange = ((reader["cardListExchange"] == DBNull.Value) ? "" : reader["cardListExchange"].ToString());
		activeSystemInfo.cardListAward = ((reader["cardListAward"] == DBNull.Value) ? "" : reader["cardListAward"].ToString());
		activeSystemInfo.rewardQuestData = ((reader["rewardQuestData"] == DBNull.Value) ? "" : reader["rewardQuestData"].ToString());
		activeSystemInfo.promoteLevel = (int)reader["promoteLevel"];
		activeSystemInfo.godLevel = (int)reader["godLevel"];
		return activeSystemInfo;
	}

	public bool AddActiveSystem(ActiveSystemInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[59];
			array[0] = new SqlParameter("@ID", item.ID);
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@UserID", item.UserID);
			array[2] = new SqlParameter("@useableScore", item.useableScore);
			array[3] = new SqlParameter("@totalScore", item.totalScore);
			array[4] = new SqlParameter("@AvailTime", item.AvailTime);
			array[5] = new SqlParameter("@NickName", item.NickName);
			array[6] = new SqlParameter("@dayScore", item.dayScore);
			array[7] = new SqlParameter("@CanGetGift", item.CanGetGift);
			array[8] = new SqlParameter("@canOpenCounts", item.canOpenCounts);
			array[9] = new SqlParameter("@canEagleEyeCounts", item.canEagleEyeCounts);
			array[10] = new SqlParameter("@lastFlushTime", item.lastFlushTime);
			array[11] = new SqlParameter("@isShowAll", item.isShowAll);
			array[12] = new SqlParameter("@AvtiveMoney", item.ActiveMoney);
			array[13] = new SqlParameter("@activityTanabataNum", item.activityTanabataNum);
			array[14] = new SqlParameter("@ChallengeNum", item.ChallengeNum);
			array[15] = new SqlParameter("@BuyBuffNum", item.BuyBuffNum);
			array[16] = new SqlParameter("@lastEnterYearMonter", item.lastEnterYearMonter);
			array[17] = new SqlParameter("@DamageNum", item.DamageNum);
			array[18] = new SqlParameter("@BoxState", item.BoxState);
			array[19] = new SqlParameter("@LuckystarCoins", item.LuckystarCoins);
			array[20] = new SqlParameter("@CryptBoss", item.CryptBoss);
			array[21] = new SqlParameter("@DDPlayPoint", item.DDPlayPoint);
			array[22] = new SqlParameter("@updateFreeCounts", item.updateFreeCounts);
			array[23] = new SqlParameter("@updateWorshipedCounts", item.updateWorshipedCounts);
			array[24] = new SqlParameter("@update200State", item.update200State);
			array[25] = new SqlParameter("@lastEnterWorshiped", item.lastEnterWorshiped);
			array[26] = new SqlParameter("@luckCount", item.luckCount);
			array[27] = new SqlParameter("@remainTimes", item.remainTimes);
			array[28] = new SqlParameter("@LastRefresh", item.LastRefresh);
			array[29] = new SqlParameter("@CurRefreshedTimes", item.CurRefreshedTimes);
			array[30] = new SqlParameter("@EntertamentPoint", item.EntertamentPoint);
			array[31] = new SqlParameter("@ChickActiveData", item.ChickActiveData);
			array[32] = new SqlParameter("@CurrentSelectIndex", item.CurrentSelectIndex);
			array[33] = new SqlParameter("@CurrentLevel", item.CurrentLevel);
			array[34] = new SqlParameter("@HomeTempleExp", item.HomeTempleExp);
			array[36] = new SqlParameter("@loginDays", item.loginDays);
			array[37] = new SqlParameter("@wellAddRate", item.wellAddRate);
			array[38] = new SqlParameter("@lastLoginDays", item.lastLoginDays);
			array[39] = new SqlParameter("@fightPowerSignData", item.fightPowerSignData);
			array[40] = new SqlParameter("@SXMapInfoData", item.SXMapInfoData);
			array[41] = new SqlParameter("@SXScore", item.SXScore);
			array[42] = new SqlParameter("@SXStepRemain", item.SXStepRemain);
			array[43] = new SqlParameter("@SXCrystal", item.SXCrystal);
			array[44] = new SqlParameter("@SXRewardsGet", item.SXRewardsGet);
			array[45] = new SqlParameter("@MiniShopBuyCount", item.MiniShopBuyCount);
			array[46] = new SqlParameter("@MemoryGameCount", item.MemoryGameCount);
			array[47] = new SqlParameter("@MemoryGameData", (item.MemoryGameData == null) ? "" : item.MemoryGameData);
			array[48] = new SqlParameter("@MemoryGameScore", item.MemoryGameScore);
			array[49] = new SqlParameter("@MemoryGameAwardGetData", (item.MemoryGameAwardGetData == null) ? "" : item.MemoryGameAwardGetData);
			array[50] = new SqlParameter("@cardScore", item.cardScore);
			array[51] = new SqlParameter("@cardFreeCount", item.cardFreeCount);
			array[52] = new SqlParameter("@cardChipCount", item.cardChipCount);
			array[53] = new SqlParameter("@cardListCard", (item.cardListCard == null) ? "" : item.cardListExchange);
			array[54] = new SqlParameter("@cardListExchange", (item.cardListExchange == null) ? "" : item.cardListExchange);
			array[55] = new SqlParameter("@cardListAward", (item.cardListAward == null) ? "" : item.cardListAward);
			array[56] = new SqlParameter("@rewardQuestData", (item.rewardQuestData == null) ? "" : item.rewardQuestData);
			array[57] = new SqlParameter("@promoteLevel", item.promoteLevel);
			array[58] = new SqlParameter("@godLevel", item.godLevel);
			array[35] = new SqlParameter("@Result", SqlDbType.Int);
			array[35].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Sys_Active_System_Data_Add", array);
			result = (int)array[35].Value == 0;
			item.ID = (int)array[0].Value;
			item.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("AddActiveSystem", exception);
			}
		}
		return result;
	}

	public bool UpdateActiveSystem(ActiveSystemInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[59];
			array[0] = new SqlParameter("@ID", item.ID);
			array[1] = new SqlParameter("@UserID", item.UserID);
			array[2] = new SqlParameter("@useableScore", item.useableScore);
			array[3] = new SqlParameter("@totalScore", item.totalScore);
			array[4] = new SqlParameter("@AvailTime", item.AvailTime);
			array[5] = new SqlParameter("@NickName", item.NickName);
			array[6] = new SqlParameter("@dayScore", item.dayScore);
			array[7] = new SqlParameter("@CanGetGift", item.CanGetGift);
			array[8] = new SqlParameter("@canOpenCounts", item.canOpenCounts);
			array[9] = new SqlParameter("@canEagleEyeCounts", item.canEagleEyeCounts);
			array[10] = new SqlParameter("@lastFlushTime", item.lastFlushTime);
			array[11] = new SqlParameter("@isShowAll", item.isShowAll);
			array[12] = new SqlParameter("@AvtiveMoney", item.ActiveMoney);
			array[13] = new SqlParameter("@activityTanabataNum", item.activityTanabataNum);
			array[14] = new SqlParameter("@ChallengeNum", item.ChallengeNum);
			array[15] = new SqlParameter("@BuyBuffNum", item.BuyBuffNum);
			array[16] = new SqlParameter("@lastEnterYearMonter", item.lastEnterYearMonter);
			array[17] = new SqlParameter("@DamageNum", item.DamageNum);
			array[18] = new SqlParameter("@BoxState", item.BoxState);
			array[19] = new SqlParameter("@LuckystarCoins", item.LuckystarCoins);
			array[20] = new SqlParameter("@CryptBoss", item.CryptBoss);
			array[21] = new SqlParameter("@DDPlayPoint", item.DDPlayPoint);
			array[22] = new SqlParameter("@updateFreeCounts", item.updateFreeCounts);
			array[23] = new SqlParameter("@updateWorshipedCounts", item.updateWorshipedCounts);
			array[24] = new SqlParameter("@update200State", item.update200State);
			array[25] = new SqlParameter("@lastEnterWorshiped", item.lastEnterWorshiped);
			array[26] = new SqlParameter("@luckCount", item.luckCount);
			array[27] = new SqlParameter("@remainTimes", item.remainTimes);
			array[28] = new SqlParameter("@LastRefresh", item.LastRefresh);
			array[29] = new SqlParameter("@CurRefreshedTimes", item.CurRefreshedTimes);
			array[30] = new SqlParameter("@EntertamentPoint", item.EntertamentPoint);
			array[31] = new SqlParameter("@ChickActiveData", item.ChickActiveData);
			array[32] = new SqlParameter("@CurrentSelectIndex", item.CurrentSelectIndex);
			array[33] = new SqlParameter("@CurrentLevel", item.CurrentLevel);
			array[34] = new SqlParameter("@HomeTempleExp", item.HomeTempleExp);
			array[36] = new SqlParameter("@loginDays", item.loginDays);
			array[37] = new SqlParameter("@wellAddRate", item.wellAddRate);
			array[38] = new SqlParameter("@lastLoginDays", item.lastLoginDays);
			array[39] = new SqlParameter("@fightPowerSignData", item.fightPowerSignData);
			array[40] = new SqlParameter("@SXMapInfoData", item.SXMapInfoData);
			array[41] = new SqlParameter("@SXScore", item.SXScore);
			array[42] = new SqlParameter("@SXStepRemain", item.SXStepRemain);
			array[43] = new SqlParameter("@SXCrystal", item.SXCrystal);
			array[44] = new SqlParameter("@SXRewardsGet", item.SXRewardsGet);
			array[45] = new SqlParameter("@MiniShopBuyCount", item.MiniShopBuyCount);
			array[46] = new SqlParameter("@MemoryGameCount", item.MemoryGameCount);
			array[47] = new SqlParameter("@MemoryGameData", item.MemoryGameData);
			array[48] = new SqlParameter("@MemoryGameScore", item.MemoryGameScore);
			array[49] = new SqlParameter("@MemoryGameAwardGetData", item.MemoryGameAwardGetData);
			array[50] = new SqlParameter("@cardScore", item.cardScore);
			array[51] = new SqlParameter("@cardFreeCount", item.cardFreeCount);
			array[52] = new SqlParameter("@cardChipCount", item.cardChipCount);
			array[53] = new SqlParameter("@cardListCard", item.cardListCard);
			array[54] = new SqlParameter("@cardListExchange", item.cardListExchange);
			array[55] = new SqlParameter("@cardListAward", item.cardListAward);
			array[56] = new SqlParameter("@rewardQuestData", item.rewardQuestData);
			array[57] = new SqlParameter("@promoteLevel", item.promoteLevel);
			array[58] = new SqlParameter("@godLevel", item.godLevel);
			array[35] = new SqlParameter("@Result", SqlDbType.Int);
			array[35].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Sys_Active_System_Data_Update", array);
			result = (int)array[35].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("UpdateActiveSystem", exception);
			}
		}
		return result;
	}

	public UserMatchInfo[] GetAllUserMatchInfo()
	{
		List<UserMatchInfo> list = new List<UserMatchInfo>();
		SqlDataReader ResultDataReader = null;
		int num = 1;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_UserMatch_All_DESC");
			while (ResultDataReader.Read())
			{
				UserMatchInfo userMatchInfo = new UserMatchInfo();
				userMatchInfo.UserID = (int)ResultDataReader["UserID"];
				userMatchInfo.totalPrestige = (int)ResultDataReader["totalPrestige"];
				userMatchInfo.rank = num;
				list.Add(userMatchInfo);
				num++;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllUserMatchDESC", exception);
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

	public UserMatchInfo GetSingleUserMatchInfo(int UserID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_GetSingleUserMatchInfo", array);
			if (ResultDataReader.Read())
			{
				UserMatchInfo userMatchInfo = new UserMatchInfo();
				userMatchInfo.ID = (int)ResultDataReader["ID"];
				userMatchInfo.UserID = (int)ResultDataReader["UserID"];
				userMatchInfo.dailyScore = (int)ResultDataReader["dailyScore"];
				userMatchInfo.dailyWinCount = (int)ResultDataReader["dailyWinCount"];
				userMatchInfo.dailyGameCount = (int)ResultDataReader["dailyGameCount"];
				userMatchInfo.DailyLeagueFirst = (bool)ResultDataReader["DailyLeagueFirst"];
				userMatchInfo.DailyLeagueLastScore = (int)ResultDataReader["DailyLeagueLastScore"];
				userMatchInfo.weeklyScore = (int)ResultDataReader["weeklyScore"];
				userMatchInfo.weeklyGameCount = (int)ResultDataReader["weeklyGameCount"];
				userMatchInfo.weeklyRanking = (int)ResultDataReader["weeklyRanking"];
				userMatchInfo.addDayPrestge = (int)ResultDataReader["addDayPrestge"];
				userMatchInfo.totalPrestige = (int)ResultDataReader["totalPrestige"];
				userMatchInfo.restCount = (int)ResultDataReader["restCount"];
				return userMatchInfo;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_GetSingleUserMatchInfo", exception);
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

	public bool AddUserMatchInfo(UserMatchInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[14]
			{
				new SqlParameter("@ID", info.ID),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@UserID", info.UserID);
			array[2] = new SqlParameter("@dailyScore", info.dailyScore);
			array[3] = new SqlParameter("@dailyWinCount", info.dailyWinCount);
			array[4] = new SqlParameter("@dailyGameCount", info.dailyGameCount);
			array[5] = new SqlParameter("@DailyLeagueFirst", info.DailyLeagueFirst);
			array[6] = new SqlParameter("@DailyLeagueLastScore", info.DailyLeagueLastScore);
			array[7] = new SqlParameter("@weeklyScore", info.weeklyScore);
			array[8] = new SqlParameter("@weeklyGameCount", info.weeklyGameCount);
			array[9] = new SqlParameter("@weeklyRanking", info.weeklyRanking);
			array[10] = new SqlParameter("@addDayPrestge", info.addDayPrestge);
			array[11] = new SqlParameter("@totalPrestige", info.totalPrestige);
			array[12] = new SqlParameter("@restCount", info.restCount);
			array[13] = new SqlParameter("@Result", SqlDbType.Int);
			array[13].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_UserMatch_Add", array);
			result = (int)array[13].Value == 0;
			info.ID = (int)array[0].Value;
			info.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
		}
		return result;
	}

	public bool UpdateUserMatchInfo(UserMatchInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[14]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@dailyScore", info.dailyScore),
				new SqlParameter("@dailyWinCount", info.dailyWinCount),
				new SqlParameter("@dailyGameCount", info.dailyGameCount),
				new SqlParameter("@DailyLeagueFirst", info.DailyLeagueFirst),
				new SqlParameter("@DailyLeagueLastScore", info.DailyLeagueLastScore),
				new SqlParameter("@weeklyScore", info.weeklyScore),
				new SqlParameter("@weeklyGameCount", info.weeklyGameCount),
				new SqlParameter("@weeklyRanking", info.weeklyRanking),
				new SqlParameter("@addDayPrestge", info.addDayPrestge),
				new SqlParameter("@totalPrestige", info.totalPrestige),
				new SqlParameter("@restCount", info.restCount),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[13].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_UpdateUserMatch", array);
			result = (int)array[13].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_UpdateUserMatch", exception);
			}
		}
		return result;
	}

	public List<UserRankInfo> GetSingleUserRank(int UserID)
	{
		SqlDataReader ResultDataReader = null;
		List<UserRankInfo> list = new List<UserRankInfo>();
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_GetSingleUserRank", array);
			while (ResultDataReader.Read())
			{
				UserRankInfo userRankInfo = new UserRankInfo();
				userRankInfo.ID = (int)ResultDataReader["ID"];
				userRankInfo.UserID = (int)ResultDataReader["UserID"];
				userRankInfo.Name = (string)ResultDataReader["Name"];
				userRankInfo.Attack = (int)ResultDataReader["Attack"];
				userRankInfo.Defence = (int)ResultDataReader["Defence"];
				userRankInfo.Luck = (int)ResultDataReader["Luck"];
				userRankInfo.Agility = (int)ResultDataReader["Agility"];
				userRankInfo.HP = (int)ResultDataReader["HP"];
				userRankInfo.Damage = (int)ResultDataReader["Damage"];
				userRankInfo.Guard = (int)ResultDataReader["Guard"];
				userRankInfo.BeginDate = (DateTime)ResultDataReader["BeginDate"];
				userRankInfo.Validate = (int)ResultDataReader["Validate"];
				userRankInfo.IsExit = (bool)ResultDataReader["IsExit"];
				userRankInfo.NewTitleID = (int)ResultDataReader["NewTitleID"];
				userRankInfo.EndDate = (DateTime)ResultDataReader["EndDate"];
				list.Add(userRankInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_GetSingleUserRankInfo", exception);
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

	public bool AddUserRank(UserRankInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[16]
			{
				new SqlParameter("@ID", item.ID),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@UserID", item.UserID);
			array[2] = new SqlParameter("@Name", item.Name);
			array[3] = new SqlParameter("@Attack", item.Attack);
			array[4] = new SqlParameter("@Defence", item.Defence);
			array[5] = new SqlParameter("@Luck", item.Luck);
			array[6] = new SqlParameter("@Agility", item.Agility);
			array[7] = new SqlParameter("@HP", item.HP);
			array[8] = new SqlParameter("@Damage", item.Damage);
			array[9] = new SqlParameter("@Guard", item.Guard);
			array[10] = new SqlParameter("@BeginDate", item.BeginDate);
			array[11] = new SqlParameter("@Validate", item.Validate);
			array[12] = new SqlParameter("@IsExit", item.IsExit);
			array[13] = new SqlParameter("@Result", SqlDbType.Int);
			array[13].Direction = ParameterDirection.ReturnValue;
			array[14] = new SqlParameter("@NewTitleID", item.NewTitleID);
			array[15] = new SqlParameter("@EndDate", item.EndDate);
			db.RunProcedure("SP_UserRank_Add", array);
			result = (int)array[13].Value == 0;
			item.ID = (int)array[0].Value;
			item.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool UpdateUserRank(UserRankInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[16]
			{
				new SqlParameter("@ID", item.ID),
				new SqlParameter("@UserID", item.UserID),
				new SqlParameter("@Name", item.Name),
				new SqlParameter("@Attack", item.Attack),
				new SqlParameter("@Defence", item.Defence),
				new SqlParameter("@Luck", item.Luck),
				new SqlParameter("@Agility", item.Agility),
				new SqlParameter("@HP", item.HP),
				new SqlParameter("@Damage", item.Damage),
				new SqlParameter("@Guard", item.Guard),
				new SqlParameter("@BeginDate", item.BeginDate),
				new SqlParameter("@Validate", item.Validate),
				new SqlParameter("@IsExit", item.IsExit),
				new SqlParameter("@Result", SqlDbType.Int),
				null,
				null
			};
			array[13].Direction = ParameterDirection.ReturnValue;
			array[14] = new SqlParameter("@NewTitleID", item.NewTitleID);
			array[15] = new SqlParameter("@EndDate", item.EndDate);
			db.RunProcedure("SP_UpdateUserRank", array);
			result = (int)array[13].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_UpdateUserRank", exception);
			}
		}
		return result;
	}

	public bool AddBeautyVote(UserBeautyVoteInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[10];
			array[0] = new SqlParameter("@UserID", item.UserID);
			array[1] = new SqlParameter("@UserName", item.UserName);
			array[2] = new SqlParameter("@NickName", item.NickName);
			array[3] = new SqlParameter("@Rank", item.Rank);
			array[4] = new SqlParameter("@Count", item.Count);
			array[5] = new SqlParameter("@Style", item.Style);
			array[6] = new SqlParameter("@Color", item.Color);
			array[7] = new SqlParameter("@TimeCreate", item.TimeCreate);
			array[9] = new SqlParameter("@IsSendAward", item.IsSendAward);
			array[8] = new SqlParameter("@Result", SqlDbType.Int);
			array[8].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Sys_BeautyVote_Add", array);
			result = (int)array[8].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_Sys_BeautyVote_Add", exception);
			}
		}
		return result;
	}

	public List<UserBeautyVoteInfo> GetAllUserBeautyVote()
	{
		SqlDataReader ResultDataReader = null;
		List<UserBeautyVoteInfo> list = new List<UserBeautyVoteInfo>();
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Sys_BeautyVote_All");
			while (ResultDataReader.Read())
			{
				UserBeautyVoteInfo userBeautyVoteInfo = new UserBeautyVoteInfo();
				userBeautyVoteInfo.UserID = (int)ResultDataReader["UserID"];
				userBeautyVoteInfo.UserName = (string)ResultDataReader["UserName"];
				userBeautyVoteInfo.NickName = (string)ResultDataReader["NickName"];
				userBeautyVoteInfo.Rank = (int)ResultDataReader["Rank"];
				userBeautyVoteInfo.Count = (int)ResultDataReader["Count"];
				userBeautyVoteInfo.Style = ((ResultDataReader["Style"] == DBNull.Value) ? "" : ((string)ResultDataReader["Style"]));
				userBeautyVoteInfo.Color = ((ResultDataReader["Color"] == DBNull.Value) ? "" : ((string)ResultDataReader["Color"]));
				userBeautyVoteInfo.TimeCreate = (DateTime)ResultDataReader["TimeCreate"];
				userBeautyVoteInfo.IsSendAward = (bool)ResultDataReader["IsSendAward"];
				list.Add(userBeautyVoteInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllUserBeautyVote", exception);
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

	public UserMinesInfo GetUsersMines(int userid)
	{
		SqlDataReader ResultDataReader = null;
		UserMinesInfo userMinesInfo = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int)
			};
			array[0].Value = userid;
			db.GetReader(ref ResultDataReader, "SP_User_Mines_All", array);
			while (ResultDataReader.Read())
			{
				userMinesInfo = new UserMinesInfo();
				userMinesInfo.UserID = (int)ResultDataReader["UserID"];
				userMinesInfo.ToolExp = (int)ResultDataReader["ToolExp"];
				userMinesInfo.HeadExp = (int)ResultDataReader["HeadExp"];
				userMinesInfo.ClothExp = (int)ResultDataReader["ClothExp"];
				userMinesInfo.WeaponExp = (int)ResultDataReader["WeaponExp"];
				userMinesInfo.ShieldExp = (int)ResultDataReader["ShieldExp"];
				userMinesInfo.LastTimeDig = (DateTime)ResultDataReader["LastTimeDig"];
				userMinesInfo.TotalTimesDig = (int)ResultDataReader["TotalTimesDig"];
				userMinesInfo.IsDirty = false;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_User_Bank_All", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return userMinesInfo;
	}

	public bool AddOrUpdateUserMines(UserMinesInfo item, bool isUpdate)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[9]
			{
				new SqlParameter("@UserID", item.UserID),
				new SqlParameter("@ToolExp", item.ToolExp),
				new SqlParameter("@HeadExp", item.HeadExp),
				new SqlParameter("@ClothExp", item.ClothExp),
				new SqlParameter("@WeaponExp", item.WeaponExp),
				new SqlParameter("@ShieldExp", item.ShieldExp),
				new SqlParameter("@LastTimeDig", item.LastTimeDig),
				new SqlParameter("@TotalTimesDig", item.TotalTimesDig),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[8].Direction = ParameterDirection.ReturnValue;
			if (!isUpdate)
			{
				db.RunProcedure("SP_User_Mines_Add", array);
			}
			else
			{
				db.RunProcedure("SP_User_Mines_Update", array);
			}
			result = (int)array[8].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("AddOrUpdateUserMines", exception);
			}
		}
		return result;
	}

	public List<UserBankInfo> GetAllUsersBank(int userid)
	{
		SqlDataReader ResultDataReader = null;
		List<UserBankInfo> list = new List<UserBankInfo>();
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int)
			};
			array[0].Value = userid;
			db.GetReader(ref ResultDataReader, "SP_User_Bank_All", array);
			while (ResultDataReader.Read())
			{
				UserBankInfo userBankInfo = new UserBankInfo();
				userBankInfo.BankID = (int)ResultDataReader["BankID"];
				userBankInfo.UserID = (int)ResultDataReader["UserID"];
				userBankInfo.TempID = (int)ResultDataReader["TempID"];
				userBankInfo.Amount = (int)ResultDataReader["Amount"];
				userBankInfo.BeginTime = (DateTime)ResultDataReader["BeginTime"];
				userBankInfo.IsExits = (bool)ResultDataReader["IsExits"];
				userBankInfo.IsDirty = false;
				list.Add(userBankInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_User_Bank_All", exception);
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

	public bool AddOrUpdateUserBank(UserBankInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[7]
			{
				new SqlParameter("@BankID", item.BankID),
				null,
				null,
				null,
				null,
				null,
				null
			};
			if (item.BankID == 0)
			{
				array[0].Direction = ParameterDirection.Output;
			}
			array[1] = new SqlParameter("@TempID", item.TempID);
			array[2] = new SqlParameter("@UserID", item.UserID);
			array[3] = new SqlParameter("@Amount", item.Amount);
			array[4] = new SqlParameter("@BeginTime", item.BeginTime);
			array[5] = new SqlParameter("@IsExits", item.IsExits);
			array[6] = new SqlParameter("@Result", SqlDbType.Int);
			array[6].Direction = ParameterDirection.ReturnValue;
			if (item.BankID == 0)
			{
				db.RunProcedure("SP_User_Bank_Add", array);
			}
			else
			{
				db.RunProcedure("SP_User_Bank_Update", array);
			}
			result = (int)array[6].Value == 0;
			if (item.BankID == 0)
			{
				item.BankID = (int)array[0].Value;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_User_Bank", exception);
			}
		}
		return result;
	}

	public UserBeautyVoteInfo GetSingleUserBeautyVote(int UserID)
	{
		SqlDataReader ResultDataReader = null;
		UserBeautyVoteInfo userBeautyVoteInfo = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_Sys_BeautyVote_Single", array);
			while (ResultDataReader.Read())
			{
				userBeautyVoteInfo = new UserBeautyVoteInfo();
				userBeautyVoteInfo.UserID = (int)ResultDataReader["UserID"];
				userBeautyVoteInfo.UserName = (string)ResultDataReader["UserName"];
				userBeautyVoteInfo.NickName = (string)ResultDataReader["NickName"];
				userBeautyVoteInfo.Rank = (int)ResultDataReader["Rank"];
				userBeautyVoteInfo.Count = (int)ResultDataReader["Count"];
				userBeautyVoteInfo.Style = ((ResultDataReader["Style"] == DBNull.Value) ? "" : ((string)ResultDataReader["Style"]));
				userBeautyVoteInfo.Color = ((ResultDataReader["Color"] == DBNull.Value) ? "" : ((string)ResultDataReader["Color"]));
				userBeautyVoteInfo.TimeCreate = (DateTime)ResultDataReader["TimeCreate"];
				userBeautyVoteInfo.IsSendAward = (bool)ResultDataReader["IsSendAward"];
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllUserBeautyVote", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return userBeautyVoteInfo;
	}

	public bool RemoveAllUserBeautyVote()
	{
		bool result = false;
		try
		{
			db.RunProcedure("SP_Sys_BeautyVote_RemoveAll");
			return true;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool RemoveAllTrailEliteSession()
	{
		bool result = false;
		try
		{
			db.RunProcedure("SP_Sys_TrailElite_RemoveAll");
			return true;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool AddOrUpdateUserMark(MarkChipItemInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[13]
			{
				new SqlParameter("@ItemID", item.ItemID),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			if (item.ItemID == 0)
			{
				array[0].Direction = ParameterDirection.Output;
			}
			array[1] = new SqlParameter("@UserID", item.UserID);
			array[2] = new SqlParameter("@Type", item.Type);
			array[3] = new SqlParameter("@Place", item.Place);
			array[4] = new SqlParameter("@TemplateID", item.TemplateID);
			array[5] = new SqlParameter("@bornLv", item.bornLv);
			array[6] = new SqlParameter("@hammerLv", item.hammerLv);
			array[7] = new SqlParameter("@hLv", item.hLv);
			array[8] = new SqlParameter("@mainPro", item.mainPro);
			array[9] = new SqlParameter("@props", item.props);
			array[10] = new SqlParameter("@IsBind", item.IsBind);
			array[11] = new SqlParameter("@IsExist", item.IsExist);
			array[12] = new SqlParameter("@Result", SqlDbType.Int);
			array[12].Direction = ParameterDirection.ReturnValue;
			if (item.ItemID == 0)
			{
				db.RunProcedure("SP_User_Mark_Add", array);
			}
			else
			{
				db.RunProcedure("SP_User_Mark_Update", array);
			}
			result = (int)array[6].Value == 0;
			if (item.ItemID == 0)
			{
				item.ItemID = (int)array[0].Value;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_User_Mark", exception);
			}
		}
		return result;
	}

	public List<MarkChipItemInfo> GetAllUsersMark(int userid)
	{
		SqlDataReader ResultDataReader = null;
		List<MarkChipItemInfo> list = new List<MarkChipItemInfo>();
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int)
			};
			array[0].Value = userid;
			db.GetReader(ref ResultDataReader, "SP_User_Mark_All", array);
			while (ResultDataReader.Read())
			{
				MarkChipItemInfo markChipItemInfo = new MarkChipItemInfo();
				markChipItemInfo.ItemID = (int)ResultDataReader["ItemID"];
				markChipItemInfo.UserID = (int)ResultDataReader["UserID"];
				markChipItemInfo.Type = (int)ResultDataReader["Type"];
				markChipItemInfo.Place = (int)ResultDataReader["Place"];
				markChipItemInfo.TemplateID = (int)ResultDataReader["TemplateID"];
				markChipItemInfo.bornLv = (int)ResultDataReader["bornLv"];
				markChipItemInfo.hammerLv = (int)ResultDataReader["hammerLv"];
				markChipItemInfo.hLv = (int)ResultDataReader["hLv"];
				markChipItemInfo.mainPro = (string)ResultDataReader["mainPro"];
				markChipItemInfo.props = (string)ResultDataReader["props"];
				markChipItemInfo.IsBind = (bool)ResultDataReader["IsBind"];
				markChipItemInfo.IsExist = (bool)ResultDataReader["IsExist"];
				markChipItemInfo.IsDirty = false;
				list.Add(markChipItemInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_User_Mark_All", exception);
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

	public UserExplorerManualInfo GetSingleUserExplorerManual(int UserID)
	{
		SqlDataReader ResultDataReader = null;
		UserExplorerManualInfo userExplorerManualInfo = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_Users_SingleExplorerManual", array);
			while (ResultDataReader.Read())
			{
				userExplorerManualInfo = new UserExplorerManualInfo();
				userExplorerManualInfo.UserID = (int)ResultDataReader["UserID"];
				userExplorerManualInfo.Agility = (int)ResultDataReader["Agility"];
				userExplorerManualInfo.Armor = (int)ResultDataReader["Armor"];
				userExplorerManualInfo.Attack = (int)ResultDataReader["Attack"];
				userExplorerManualInfo.ConditionCount = (int)ResultDataReader["ConditionCount"];
				userExplorerManualInfo.Damage = (int)ResultDataReader["Damage"];
				userExplorerManualInfo.Defense = (int)ResultDataReader["Defense"];
				userExplorerManualInfo.HavePage = (int)ResultDataReader["HavePage"];
				userExplorerManualInfo.HP = (int)ResultDataReader["HP"];
				userExplorerManualInfo.Lucky = (int)ResultDataReader["Lucky"];
				userExplorerManualInfo.MagicAttack = (int)ResultDataReader["MagicAttack"];
				userExplorerManualInfo.MagicResistance = (int)ResultDataReader["MagicResistance"];
				userExplorerManualInfo.ManualLevel = (int)ResultDataReader["ManualLevel"];
				userExplorerManualInfo.MaxProgress = (int)ResultDataReader["MaxProgress"];
				userExplorerManualInfo.Progress = (int)ResultDataReader["Progress"];
				userExplorerManualInfo.Stamina = (int)ResultDataReader["Stamina"];
				userExplorerManualInfo.ActivesPage = ((ResultDataReader["ActivesPage"] == DBNull.Value) ? "" : ((string)ResultDataReader["ActivesPage"]));
				userExplorerManualInfo.Debris = ((ResultDataReader["Debris"] == DBNull.Value) ? "" : ((string)ResultDataReader["Debris"]));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SingleExplorerManual", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return userExplorerManualInfo;
	}

	public bool UpdateUserExplorerManual(UserExplorerManualInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[19]
			{
				new SqlParameter("@UserID", item.UserID),
				new SqlParameter("@Agility", item.Agility),
				new SqlParameter("@Armor", item.Armor),
				new SqlParameter("@Attack", item.Attack),
				new SqlParameter("@ConditionCount", item.ConditionCount),
				new SqlParameter("@Damage", item.Damage),
				new SqlParameter("@Defense", item.Defense),
				new SqlParameter("@HavePage", item.HavePage),
				new SqlParameter("@HP", item.HP),
				new SqlParameter("@Lucky", item.Lucky),
				new SqlParameter("@MagicAttack", item.MagicAttack),
				new SqlParameter("@MagicResistance", item.MagicResistance),
				new SqlParameter("@ManualLevel", item.ManualLevel),
				new SqlParameter("@MaxProgress", item.MaxProgress),
				new SqlParameter("@Progress", item.Progress),
				new SqlParameter("@Stamina", item.Stamina),
				new SqlParameter("@ActivesPage", item.ActivesPage),
				new SqlParameter("@Debris", item.Debris),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[18].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Users_ExplorerManual_Update", array);
			result = (int)array[18].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("UpdateExplorerManual", exception);
			}
		}
		return result;
	}

	public ExplorerManualInfo InitExplorerManualInfo(int iD)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@UserID", iD)
			};
			db.GetReader(ref ResultDataReader, "SP_Single_ExplorerManualInfo", sqlParameters);
			if (ResultDataReader.Read())
			{
				ExplorerManualInfo explorerManualInfo = new ExplorerManualInfo();
				for (int i = 0; i < ResultDataReader.FieldCount; i++)
				{
					if (explorerManualInfo.GetType().GetProperty(ResultDataReader.GetName(i)) == null)
					{
						BaseBussiness.log.Info("Campo " + ResultDataReader.GetName(i) + " Faltando em " + explorerManualInfo.GetType());
					}
					else
					{
						explorerManualInfo.GetType().GetProperty(ResultDataReader.GetName(i)).SetValue(explorerManualInfo, ResultDataReader[i]);
					}
				}
				explorerManualInfo.IsDirty = false;
				return explorerManualInfo;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
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

	public List<PagesInfo> InitExplorerManualInfoPages(int iD)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			List<PagesInfo> list = new List<PagesInfo>();
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@UserID", iD)
			};
			db.GetReader(ref ResultDataReader, "SP_Single_ExplorerManualInfoPages", sqlParameters);
			while (ResultDataReader.Read())
			{
				PagesInfo pagesInfo = new PagesInfo();
				for (int i = 0; i < ResultDataReader.FieldCount; i++)
				{
					if (pagesInfo.GetType().GetProperty(ResultDataReader.GetName(i)) == null)
					{
						BaseBussiness.log.Info("Campo " + ResultDataReader.GetName(i) + " Faltando em " + pagesInfo.GetType());
					}
					else
					{
						pagesInfo.GetType().GetProperty(ResultDataReader.GetName(i)).SetValue(pagesInfo, ResultDataReader[i]);
					}
				}
				pagesInfo.IsDirty = false;
				list.Add(pagesInfo);
			}
			return list;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
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

	public Dictionary<int, DebrisInfo> InitExplorerManualDebris(int iD)
	{
		SqlDataReader ResultDataReader = null;
		Dictionary<int, DebrisInfo> dictionary = new Dictionary<int, DebrisInfo>();
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@UserID", iD)
			};
			db.GetReader(ref ResultDataReader, "SP_Single_ExplorerManualInfoDebris", sqlParameters);
			while (ResultDataReader.Read())
			{
				DebrisInfo debrisInfo = new DebrisInfo();
				for (int i = 0; i < ResultDataReader.FieldCount; i++)
				{
					if (debrisInfo.GetType().GetProperty(ResultDataReader.GetName(i)) == null)
					{
						BaseBussiness.log.Info("Campo " + ResultDataReader.GetName(i) + " Faltando em " + debrisInfo.GetType());
					}
					else
					{
						debrisInfo.GetType().GetProperty(ResultDataReader.GetName(i)).SetValue(debrisInfo, ResultDataReader[i]);
					}
				}
				debrisInfo.IsDirty = false;
				if (!dictionary.ContainsKey(debrisInfo.ID))
				{
					dictionary.Add(debrisInfo.ID, debrisInfo);
				}
			}
			return dictionary;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
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

	public bool updateExplorerManualInfo(PlayerInfo m_character)
	{
		bool result = false;
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@UserID", m_character.ID));
			ExplorerManualInfo explorerManualInfo = m_character.explorerManualInfo;
			if (!explorerManualInfo.IsDirty)
			{
				return true;
			}
			PropertyInfo[] properties = explorerManualInfo.GetType().GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (propertyInfo.CustomAttributes.Count() > 0)
				{
					list.Add(new SqlParameter(propertyInfo.Name, propertyInfo.GetValue(explorerManualInfo)));
				}
			}
			db.RunProcedure("SP_Users_Update_ExplorerManualInfo", list.ToArray());
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("updateExplorerManualInfo", exception);
			}
		}
		return result;
	}

	public bool updateExplorerManualInfoPages(PlayerInfo m_character)
	{
		bool result = false;
		try
		{
			foreach (PagesInfo item in m_character.explorerManualInfo.activesPage)
			{
				if (!item.IsDirty)
				{
					continue;
				}
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@UserID", m_character.ID));
				PropertyInfo[] properties = item.GetType().GetProperties();
				foreach (PropertyInfo propertyInfo in properties)
				{
					if (propertyInfo.CustomAttributes.Count() > 0)
					{
						list.Add(new SqlParameter(propertyInfo.Name, propertyInfo.GetValue(item)));
					}
				}
				db.RunProcedure("SP_Users_Update_ExplorerManualPages", list.ToArray());
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("updateExplorerManualPages", exception);
			}
		}
		return result;
	}

	public bool updateExplorerManualInfoDebris(PlayerInfo m_character)
	{
		bool result = false;
		try
		{
			foreach (DebrisInfo value in m_character.explorerManualInfo.debris.Values)
			{
				if (!value.IsDirty)
				{
					continue;
				}
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@UserID", m_character.ID));
				PropertyInfo[] properties = value.GetType().GetProperties();
				foreach (PropertyInfo propertyInfo in properties)
				{
					if (propertyInfo.CustomAttributes.Count() > 0)
					{
						list.Add(new SqlParameter(propertyInfo.Name, propertyInfo.GetValue(value)));
					}
				}
				db.RunProcedure("SP_Users_Update_ExplorerManualDebris", list.ToArray());
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("updateExplorerManualPages", exception);
			}
		}
		return result;
	}

	public ItemAwakenPet[] GetEqPetWaken(int UserID)
	{
		List<ItemAwakenPet> list = new List<ItemAwakenPet>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@ID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_eqAwakenPet_Single", array);
			while (ResultDataReader.Read())
			{
				ItemAwakenPet item = new ItemAwakenPet
				{
					isSave = true,
					itemId = (int)ResultDataReader["itemId"],
					skillId1 = (int)ResultDataReader["skillId1"],
					skillId2 = (int)ResultDataReader["skillId2"],
					petId = (int)ResultDataReader["petId"]
				};
				list.Add(item);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
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

	public void AddEqAwakenPet(ItemAwakenPet info, int userId)
	{
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[5]
			{
				new SqlParameter("@UserID", userId),
				new SqlParameter("@itemId", info.itemId),
				new SqlParameter("@petId", info.petId),
				new SqlParameter("@skillId1", info.skillId1),
				new SqlParameter("@skillId2", info.skillId2)
			};
			db.RunProcedure("SP_User_Add_eqAwakenPet", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
	}

	public void UpdateEqAwakenPet(ItemAwakenPet info, int userId)
	{
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[5]
			{
				new SqlParameter("@UserID", userId),
				new SqlParameter("@itemId", info.itemId),
				new SqlParameter("@petId", info.petId),
				new SqlParameter("@skillId1", info.skillId1),
				new SqlParameter("@skillId2", info.skillId2)
			};
			db.RunProcedure("SP_User_Update_eqAwakenPet", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
	}

	public Dictionary<int, List<string>> loadComandos()
	{
		SqlDataReader ResultDataReader = null;
		Dictionary<int, List<string>> dictionary = new Dictionary<int, List<string>>();
		db.GetReader(ref ResultDataReader, "SP_GetAllComandos");
		while (ResultDataReader.Read())
		{
			string[] array = Convert.ToString(ResultDataReader["comandos"] ?? "").Split(';');
			List<string> list = new List<string>();
			string[] array2 = array;
			foreach (string item in array2)
			{
				list.Add(item);
			}
			if (!dictionary.ContainsKey(Convert.ToInt32(ResultDataReader["userid"] ?? ((object)0))))
			{
				dictionary.Add(Convert.ToInt32(ResultDataReader["userid"] ?? ((object)0)), list);
			}
		}
		return dictionary;
	}

	public List<int> DeleteMailAll(int UserID, bool flag)
	{
		SqlDataReader ResultDataReader = null;
		List<int> list = new List<int>();
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[2]
			{
				new SqlParameter("@UserID", UserID),
				new SqlParameter("@flag", flag)
			};
			db.GetReader(ref ResultDataReader, "SP_Mail_DeleteAll", sqlParameters);
			while (ResultDataReader.Read())
			{
				list.Add((int)ResultDataReader["ID"]);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return list;
	}

	public List<PlayerInfo> GetUserAllByUserName(string userName)
	{
		SqlDataReader ResultDataReader = null;
		List<PlayerInfo> list = new List<PlayerInfo>();
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserName", SqlDbType.NVarChar, 200)
			};
			array[0].Value = userName;
			db.GetReader(ref ResultDataReader, "SP_Users_SingleByUserName", array);
			while (ResultDataReader.Read())
			{
				PlayerInfo item = InitPlayerInfo(ResultDataReader);
				list.Add(item);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
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

	public List<UsersPetLacrarInfo> GetUserLacresPet(int id)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@UserID", id)
			};
			db.GetReader(ref ResultDataReader, "SP_USERS_PET_LACRES", sqlParameters);
			if (ResultDataReader.Read())
			{
				if (ResultDataReader["lacresPet"] == null || string.IsNullOrEmpty((string)ResultDataReader["lacresPet"]))
				{
					return null;
				}
				return JsonConvert.DeserializeObject<List<UsersPetLacrarInfo>>((string)ResultDataReader["lacresPet"]).ToList();
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetUserLacresPet", exception);
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

	public bool UpdateUserLacresPet(int id, List<UsersPetLacrarInfo> lacres)
	{
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[2]
			{
				new SqlParameter("@UserID", id),
				new SqlParameter("@Lacres", JsonConvert.SerializeObject(lacres))
			};
			db.RunProcedure("SP_USERS_PET_LACRES_UPDATE", sqlParameters);
			return true;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("UpdateUserLacresPet", exception);
			}
		}
		return false;
	}

	public void DeleteAllUsers()
	{
		try
		{
			db.Exesqlcomm("delete from dbo.sys_users_detail");
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("DeleteAllUsers", exception);
			}
		}
	}

	public void DeleteAllItens()
	{
		try
		{
			db.Exesqlcomm("delete from dbo.sys_users_goods");
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("DeleteAllUsers", exception);
			}
		}
	}

	public HappyRechargeInfo GetHappyRechargeData(int ID)
	{
		HappyRechargeInfo result = null;
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = ID;
			db.GetReader(ref ResultDataReader, "SP_GetUserHappyRechargeData", array);
			while (ResultDataReader.Read())
			{
				result = InitHappyRechargeInfo(ResultDataReader);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_GetUserHappyRechargeData", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return result;
	}

	public HappyRechargeInfo InitHappyRechargeInfo(SqlDataReader reader)
	{
		return new HappyRechargeInfo
		{
			ID = (int)reader["ID"],
			UserID = (int)reader["UserID"],
			LotteryCount = (int)reader["LotteryCount"],
			LotteryTicket = (int)reader["LotteryTicket"]
		};
	}

	public bool UpdateHappyRechargeData(HappyRechargeInfo hristos)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[3]
			{
				new SqlParameter("@UserID", hristos.UserID),
				new SqlParameter("@LotteryCount", hristos.LotteryCount),
				new SqlParameter("@LotteryTicket", hristos.LotteryTicket)
			};
			result = db.RunProcedure("SP_Insert_Update_HappyRechargeData", sqlParameters);
		}
		catch (Exception ex)
		{
			Exception exception = ex;
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_Insert_Update_HappyRechargeData", exception);
			}
		}
		return result;
	}
}
