using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using SqlDataProvider.BaseClass;
using SqlDataProvider.Data;

namespace Bussiness;

public class ConsortiaBussiness : BaseBussiness
{
	private string AreaName = "Area 1";

	private int ZoneID = 1;

	public ConsortiaBussiness()
	{
	}

	public ConsortiaBussiness(AreaConfigInfo config)
	{
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

	public bool RenameConsortiaName(string userName, string nickName, string consortiaName, ref string msg)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[4]
			{
				new SqlParameter("@UserName", userName),
				new SqlParameter("@NickName", nickName),
				new SqlParameter("@ConsortiaName", consortiaName),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[3].Direction = ParameterDirection.ReturnValue;
			result = db.RunProcedure("SP_Users_RenameConsortiaName", array);
			int num = (int)array[3].Value;
			result = num == 0;
			int num2 = num;
			int num3 = num2;
			if ((uint)(num3 - 4) <= 1u)
			{
				msg = LanguageMgr.GetTranslation("PlayerBussiness.SP_Users_RenameConsortiaName.Msg4");
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

	public bool RenameConsortia(int ConsortiaID, string nickName, string newNickName)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[4]
			{
				new SqlParameter("@ConsortiaID", ConsortiaID),
				new SqlParameter("@NickName", nickName),
				new SqlParameter("@NewNickName", newNickName),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[3].Direction = ParameterDirection.ReturnValue;
			result = db.RunProcedure("SP_Users_RenameConsortia", array);
			int num = (int)array[3].Value;
			result = num == 0;
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

	public bool AddConsortia(ConsortiaInfo info, ref string msg, ref ConsortiaDutyInfo dutyInfo)
	{
		bool flag = false;
		try
		{
			SqlParameter[] array = new SqlParameter[23];
			array[0] = new SqlParameter("@ConsortiaID", info.ConsortiaID);
			array[0].Direction = ParameterDirection.InputOutput;
			array[1] = new SqlParameter("@BuildDate", info.BuildDate);
			array[2] = new SqlParameter("@CelebCount", info.CelebCount);
			array[3] = new SqlParameter("@ChairmanID", info.ChairmanID);
			array[4] = new SqlParameter("@ChairmanName", (info.ChairmanName == null) ? "" : info.ChairmanName);
			array[5] = new SqlParameter("@ConsortiaName", (info.ConsortiaName == null) ? "" : info.ConsortiaName);
			array[6] = new SqlParameter("@CreatorID", info.CreatorID);
			array[7] = new SqlParameter("@CreatorName", (info.CreatorName == null) ? "" : info.CreatorName);
			array[8] = new SqlParameter("@Description", info.Description);
			array[9] = new SqlParameter("@Honor", info.Honor);
			array[10] = new SqlParameter("@IP", info.IP);
			array[11] = new SqlParameter("@IsExist", info.IsExist);
			array[12] = new SqlParameter("@Level", info.Level);
			array[13] = new SqlParameter("@MaxCount", info.MaxCount);
			array[14] = new SqlParameter("@Placard", info.Placard);
			array[15] = new SqlParameter("@Port", info.Port);
			array[16] = new SqlParameter("@Repute", info.Repute);
			array[17] = new SqlParameter("@Count", info.Count);
			array[18] = new SqlParameter("@Riches", info.Riches);
			array[19] = new SqlParameter("@Result", SqlDbType.Int);
			array[19].Direction = ParameterDirection.ReturnValue;
			array[20] = new SqlParameter("@tempDutyLevel", SqlDbType.Int);
			array[20].Direction = ParameterDirection.InputOutput;
			array[20].Value = dutyInfo.Level;
			array[21] = new SqlParameter("@tempDutyName", SqlDbType.NVarChar, 100);
			array[21].Direction = ParameterDirection.InputOutput;
			array[21].Value = "";
			array[22] = new SqlParameter("@tempRight", SqlDbType.Int);
			array[22].Direction = ParameterDirection.InputOutput;
			array[22].Value = dutyInfo.Right;
			flag = db.RunProcedure("SP_Consortia_Add", array);
			int num = (int)array[19].Value;
			flag = num == 0;
			if (flag)
			{
				info.ConsortiaID = (int)array[0].Value;
				dutyInfo.Level = (int)array[20].Value;
				dutyInfo.DutyName = array[21].Value.ToString();
				dutyInfo.Right = (int)array[22].Value;
			}
			int num2 = num;
			int num3 = num2;
			if (num3 == 2)
			{
				msg = "ConsortiaBussiness.AddConsortia.Msg2";
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

	public bool DeleteConsortia(int consortiaID, int userID, ref string msg)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[3]
			{
				new SqlParameter("@ConsortiaID", consortiaID),
				new SqlParameter("@UserID", userID),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[2].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Consortia_Delete", array);
			int num = (int)array[2].Value;
			result = num == 0;
			switch (num)
			{
			case 2:
				msg = "ConsortiaBussiness.DeleteConsortia.Msg2";
				break;
			case 3:
				msg = "ConsortiaBussiness.DeleteConsortia.Msg3";
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

	public ConsortiaInfo GetSingleConsoritaFirst()
	{
		int total = 0;
		DataTable page = GetPage("V_Consortia", " IsExist=1 ", 1, 1, "*", "", "ConsortiaID", ref total);
		IEnumerator enumerator = page.Rows.GetEnumerator();
		try
		{
			if (enumerator.MoveNext())
			{
				DataRow dr = (DataRow)enumerator.Current;
				return InitConsortiaInfo(dr);
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
		return null;
	}

	public ConsortiaInfo[] GetConsortiaPage(int page, int size, ref int total, int order, string name, int consortiaID, int level, int openApply)
	{
		List<ConsortiaInfo> list = new List<ConsortiaInfo>();
		try
		{
			string text = " IsExist=1 ";
			if (!string.IsNullOrEmpty(name))
			{
				text = text + " and ConsortiaName like '%" + name + "%' ";
			}
			if (consortiaID != -1)
			{
				text = text + " and ConsortiaID =" + consortiaID + " ";
			}
			if (level != -1)
			{
				text = text + " and Level =" + level + " ";
			}
			if (openApply != -1)
			{
				text = text + " and OpenApply =" + openApply + " ";
			}
			string text2 = "ConsortiaName";
			switch (order)
			{
			case 1:
				text2 = "ReputeSort";
				break;
			case 2:
				text2 = "ChairmanName";
				break;
			case 3:
				text2 = "Count desc";
				break;
			case 4:
				text2 = "Level desc";
				break;
			case 5:
				text2 = "Honor desc";
				break;
			case 6:
				text2 = "Riches desc";
				break;
			case 10:
				text2 = "LastDayRiches desc";
				break;
			case 11:
				text2 = "AddDayRiches desc";
				break;
			case 12:
				text2 = "AddWeekRiches desc";
				break;
			case 13:
				text2 = "LastDayHonor desc";
				break;
			case 14:
				text2 = "AddDayHonor desc";
				break;
			case 15:
				text2 = "AddWeekHonor desc";
				break;
			case 16:
				text2 = "Level desc,LastDayRiches desc";
				break;
			case 17:
				text2 = "FightPower desc";
				break;
			}
			text2 += ",ConsortiaID ";
			DataTable page2 = GetPage("V_Consortia", text, page, size, "*", text2, "ConsortiaID", ref total);
			foreach (DataRow row in page2.Rows)
			{
				ConsortiaInfo item = InitConsortiaInfo(row);
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
		}
		return list.ToArray();
	}

	public ConsortiaInfo InitConsortiaInfo(DataRow dr)
	{
		ConsortiaInfo consortiaInfo = new ConsortiaInfo();
		consortiaInfo.ZoneID = ZoneID;
		consortiaInfo.ZoneName = AreaName;
		consortiaInfo.ConsortiaID = (int)dr["ConsortiaID"];
		consortiaInfo.BuildDate = (DateTime)dr["BuildDate"];
		consortiaInfo.CelebCount = (int)dr["CelebCount"];
		consortiaInfo.ChairmanID = (int)dr["ChairmanID"];
		consortiaInfo.ChairmanName = dr["ChairmanName"].ToString();
		consortiaInfo.ChairmanTypeVIP = Convert.ToByte(dr["typeVIP"]);
		consortiaInfo.ChairmanVIPLevel = (int)dr["VIPLevel"];
		consortiaInfo.ConsortiaName = dr["ConsortiaName"].ToString();
		consortiaInfo.CreatorID = (int)dr["CreatorID"];
		consortiaInfo.CreatorName = dr["CreatorName"].ToString();
		consortiaInfo.Description = dr["Description"].ToString();
		consortiaInfo.Honor = (int)dr["Honor"];
		consortiaInfo.IsExist = (bool)dr["IsExist"];
		consortiaInfo.Level = (int)dr["Level"];
		consortiaInfo.MaxCount = (int)dr["MaxCount"];
		consortiaInfo.Placard = dr["Placard"].ToString();
		consortiaInfo.IP = dr["IP"].ToString();
		consortiaInfo.Port = (int)dr["Port"];
		consortiaInfo.FightPower = (long)dr["FightPower"];
		consortiaInfo.Repute = (int)dr["Repute"];
		consortiaInfo.Count = (int)dr["Count"];
		consortiaInfo.Riches = (int)dr["Riches"];
		consortiaInfo.DeductDate = (DateTime)dr["DeductDate"];
		consortiaInfo.AddDayHonor = (int)dr["AddDayHonor"];
		consortiaInfo.AddDayRiches = (int)dr["AddDayRiches"];
		consortiaInfo.AddWeekHonor = (int)dr["AddWeekHonor"];
		consortiaInfo.AddWeekRiches = (int)dr["AddWeekRiches"];
		consortiaInfo.LastDayRiches = (int)dr["LastDayRiches"];
		consortiaInfo.OpenApply = (bool)dr["OpenApply"];
		consortiaInfo.StoreLevel = (int)dr["StoreLevel"];
		consortiaInfo.SmithLevel = (int)dr["SmithLevel"];
		consortiaInfo.ShopLevel = (int)dr["ShopLevel"];
		consortiaInfo.SkillLevel = (int)dr["SkillLevel"];
		consortiaInfo.BadgeID = (int)dr["BadgeID"];
		consortiaInfo.DateDayRiches = (DateTime)dr["DateDayRiches"];
		consortiaInfo.DateWeekRiches = (DateTime)dr["DateWeekRiches"];
		consortiaInfo.LastDayHonor = (int)dr["LastDayHonor"];
		consortiaInfo.LastWeekHonor = (int)dr["LastWeekHonor"];
		consortiaInfo.LastWeekRiches = (int)dr["LastWeekRiches"];
		return consortiaInfo;
	}

	public bool BuyBadge(int consortiaID, int userID, ConsortiaInfo info, ref string msg)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[6]
			{
				new SqlParameter("@ConsortiaID", consortiaID),
				new SqlParameter("@UserID", userID),
				new SqlParameter("@BadgeID", info.BadgeID),
				new SqlParameter("@ValidDate", info.ValidDate),
				new SqlParameter("@BadgeBuyTime", info.BadgeBuyTime),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[5].Direction = ParameterDirection.ReturnValue;
			result = db.RunProcedure("SP_ConsortiaBadge_Update", array);
			int num = (int)array[5].Value;
			result = num == 0;
			int num2 = num;
			int num3 = num2;
			if (num3 == 2)
			{
				msg = "ConsortiaBussiness.BuyBadge.Msg2";
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

	public bool UpdateConsortiaRiches(int consortiaID, int userID, int Riches, ref string msg)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[4]
			{
				new SqlParameter("@ConsortiaID", consortiaID),
				new SqlParameter("@UserID", userID),
				new SqlParameter("@Riches", Riches),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[3].Direction = ParameterDirection.ReturnValue;
			result = db.RunProcedure("SP_ConsortiaRiches_Update", array);
			int num = (int)array[3].Value;
			result = num == 0;
			int num2 = num;
			int num3 = num2;
			if (num3 == 2)
			{
				msg = "ConsortiaBussiness.UpdateConsortiaRiches.Msg2";
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

	public bool UpdateConsortiaDescription(int consortiaID, int userID, string description, ref string msg)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[4]
			{
				new SqlParameter("@ConsortiaID", consortiaID),
				new SqlParameter("@UserID", userID),
				new SqlParameter("@Description", description),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[3].Direction = ParameterDirection.ReturnValue;
			result = db.RunProcedure("SP_ConsortiaDescription_Update", array);
			int num = (int)array[3].Value;
			result = num == 0;
			int num2 = num;
			int num3 = num2;
			if (num3 == 2)
			{
				msg = "ConsortiaBussiness.UpdateConsortiaDescription.Msg2";
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

	public bool UpdateConsortiaPlacard(int consortiaID, int userID, string placard, ref string msg)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[4]
			{
				new SqlParameter("@ConsortiaID", consortiaID),
				new SqlParameter("@UserID", userID),
				new SqlParameter("@Placard", placard),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[3].Direction = ParameterDirection.ReturnValue;
			result = db.RunProcedure("SP_ConsortiaPlacard_Update", array);
			int num = (int)array[3].Value;
			result = num == 0;
			int num2 = num;
			int num3 = num2;
			if (num3 == 2)
			{
				msg = "ConsortiaBussiness.UpdateConsortiaPlacard.Msg2";
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

	public bool UpdateConsortiaChairman(string nickName, int consortiaID, int userID, ref string msg, ref ConsortiaDutyInfo info, ref int tempUserID, ref string tempUserName)
	{
		bool flag = false;
		try
		{
			SqlParameter[] array = new SqlParameter[9]
			{
				new SqlParameter("@NickName", nickName),
				new SqlParameter("@ConsortiaID", consortiaID),
				new SqlParameter("@UserID", userID),
				new SqlParameter("@Result", SqlDbType.Int),
				null,
				null,
				null,
				null,
				null
			};
			array[3].Direction = ParameterDirection.ReturnValue;
			array[4] = new SqlParameter("@tempUserID", SqlDbType.Int);
			array[4].Direction = ParameterDirection.InputOutput;
			array[4].Value = tempUserID;
			array[5] = new SqlParameter("@tempUserName", SqlDbType.NVarChar, 100);
			array[5].Direction = ParameterDirection.InputOutput;
			array[5].Value = tempUserName;
			array[6] = new SqlParameter("@tempDutyLevel", SqlDbType.Int);
			array[6].Direction = ParameterDirection.InputOutput;
			array[6].Value = info.Level;
			array[7] = new SqlParameter("@tempDutyName", SqlDbType.NVarChar, 100);
			array[7].Direction = ParameterDirection.InputOutput;
			array[7].Value = "";
			array[8] = new SqlParameter("@tempRight", SqlDbType.Int);
			array[8].Direction = ParameterDirection.InputOutput;
			array[8].Value = info.Right;
			flag = db.RunProcedure("SP_ConsortiaChangeChairman", array);
			int num = (int)array[3].Value;
			flag = num == 0;
			if (flag)
			{
				tempUserID = (int)array[4].Value;
				tempUserName = array[5].Value.ToString();
				info.Level = (int)array[6].Value;
				info.DutyName = array[7].Value.ToString();
				info.Right = (int)array[8].Value;
			}
			switch (num)
			{
			case 1:
				msg = "ConsortiaBussiness.UpdateConsortiaChairman.Msg3";
				break;
			case 2:
				msg = "ConsortiaBussiness.UpdateConsortiaChairman.Msg2";
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
		finally
		{
		}
		return flag;
	}

	public ConsortiaInfo GetConsortiaSingleByName(string ConsortiaName)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@ConsortiaName", SqlDbType.NVarChar, 200)
			};
			array[0].Value = ConsortiaName;
			db.GetReader(ref ResultDataReader, "SP_Consortia_CheckByName", array);
			if (ResultDataReader.Read())
			{
				ConsortiaInfo consortiaInfo = new ConsortiaInfo();
				consortiaInfo.ConsortiaID = (int)ResultDataReader["ConsortiaID"];
				consortiaInfo.BuildDate = (DateTime)ResultDataReader["BuildDate"];
				consortiaInfo.CelebCount = (int)ResultDataReader["CelebCount"];
				consortiaInfo.ChairmanID = (int)ResultDataReader["ChairmanID"];
				consortiaInfo.ChairmanName = ResultDataReader["ChairmanName"].ToString();
				consortiaInfo.ConsortiaName = ResultDataReader["ConsortiaName"].ToString();
				consortiaInfo.CreatorID = (int)ResultDataReader["CreatorID"];
				consortiaInfo.CreatorName = ResultDataReader["CreatorName"].ToString();
				consortiaInfo.Description = ResultDataReader["Description"].ToString();
				consortiaInfo.Honor = (int)ResultDataReader["Honor"];
				consortiaInfo.IsExist = (bool)ResultDataReader["IsExist"];
				consortiaInfo.Level = (int)ResultDataReader["Level"];
				consortiaInfo.MaxCount = (int)ResultDataReader["MaxCount"];
				consortiaInfo.Placard = ResultDataReader["Placard"].ToString();
				consortiaInfo.IP = ResultDataReader["IP"].ToString();
				consortiaInfo.Port = (int)ResultDataReader["Port"];
				consortiaInfo.Repute = (int)ResultDataReader["Repute"];
				consortiaInfo.Count = (int)ResultDataReader["Count"];
				consortiaInfo.Riches = (int)ResultDataReader["Riches"];
				consortiaInfo.DeductDate = (DateTime)ResultDataReader["DeductDate"];
				consortiaInfo.StoreLevel = (int)ResultDataReader["StoreLevel"];
				consortiaInfo.SmithLevel = (int)ResultDataReader["SmithLevel"];
				consortiaInfo.ShopLevel = (int)ResultDataReader["ShopLevel"];
				consortiaInfo.SkillLevel = (int)ResultDataReader["SkillLevel"];
				return consortiaInfo;
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

	public bool UpGradeConsortia(int consortiaID, int userID, ref string msg)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[3]
			{
				new SqlParameter("@ConsortiaID", consortiaID),
				new SqlParameter("@UserID", userID),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[2].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Consortia_UpGrade", array);
			int num = (int)array[2].Value;
			result = num == 0;
			switch (num)
			{
			case 2:
				msg = "ConsortiaBussiness.UpGradeConsortia.Msg2";
				break;
			case 3:
				msg = "ConsortiaBussiness.UpGradeConsortia.Msg3";
				break;
			case 4:
				msg = "ConsortiaBussiness.UpGradeConsortia.Msg4";
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

	public bool UpGradeSkillConsortia(int consortiaID, int userID, ref string msg)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[3]
			{
				new SqlParameter("@ConsortiaID", consortiaID),
				new SqlParameter("@UserID", userID),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[2].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Consortia_Skill_UpGrade", array);
			int num = (int)array[2].Value;
			result = num == 0;
			switch (num)
			{
			case 2:
				msg = "ConsortiaBussiness.UpGradeSkillConsortia.Msg2";
				break;
			case 3:
				msg = "ConsortiaBussiness.UpGradeSkillConsortia.Msg3";
				break;
			case 4:
				msg = "ConsortiaBussiness.UpGradeSkillConsortia.Msg4";
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

	public bool UpGradeShopConsortia(int consortiaID, int userID, ref string msg)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[3]
			{
				new SqlParameter("@ConsortiaID", consortiaID),
				new SqlParameter("@UserID", userID),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[2].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Consortia_Shop_UpGrade", array);
			int num = (int)array[2].Value;
			result = num == 0;
			switch (num)
			{
			case 2:
				msg = "ConsortiaBussiness.UpGradeShopConsortia.Msg2";
				break;
			case 3:
				msg = "ConsortiaBussiness.UpGradeShopConsortia.Msg3";
				break;
			case 4:
				msg = "ConsortiaBussiness.UpGradeShopConsortia.Msg4";
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

	public bool UpGradeStoreConsortia(int consortiaID, int userID, ref string msg)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[3]
			{
				new SqlParameter("@ConsortiaID", consortiaID),
				new SqlParameter("@UserID", userID),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[2].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Consortia_Store_UpGrade", array);
			int num = (int)array[2].Value;
			result = num == 0;
			switch (num)
			{
			case 2:
				msg = "ConsortiaBussiness.UpGradeStoreConsortia.Msg2";
				break;
			case 3:
				msg = "ConsortiaBussiness.UpGradeStoreConsortia.Msg3";
				break;
			case 4:
				msg = "ConsortiaBussiness.UpGradeStoreConsortia.Msg4";
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

	public bool UpGradeSmithConsortia(int consortiaID, int userID, ref string msg)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[3]
			{
				new SqlParameter("@ConsortiaID", consortiaID),
				new SqlParameter("@UserID", userID),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[2].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Consortia_Smith_UpGrade", array);
			int num = (int)array[2].Value;
			result = num == 0;
			switch (num)
			{
			case 2:
				msg = "ConsortiaBussiness.UpGradeSmithConsortia.Msg2";
				break;
			case 3:
				msg = "ConsortiaBussiness.UpGradeSmithConsortia.Msg3";
				break;
			case 4:
				msg = "ConsortiaBussiness.UpGradeSmithConsortia.Msg4";
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

	public ConsortiaInfo[] GetRankWeekRichesConsortias()
	{
		List<ConsortiaInfo> list = new List<ConsortiaInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Consortia_Rank_Week_Riches");
			while (ResultDataReader.Read())
			{
				ConsortiaInfo consortiaInfo = new ConsortiaInfo();
				consortiaInfo.ConsortiaID = (int)ResultDataReader["ConsortiaID"];
				consortiaInfo.Honor = (int)ResultDataReader["Honor"];
				consortiaInfo.Level = (int)ResultDataReader["Level"];
				consortiaInfo.Riches = (int)ResultDataReader["Riches"];
				consortiaInfo.AddDayRiches = (int)ResultDataReader["AddDayRiches"];
				consortiaInfo.AddWeekRiches = (int)ResultDataReader["AddWeekRiches"];
				consortiaInfo.MaxCount = (int)ResultDataReader["MaxCount"];
				consortiaInfo.BuildDate = (DateTime)ResultDataReader["BuildDate"];
				consortiaInfo.IsExist = (bool)ResultDataReader["IsExist"];
				consortiaInfo.DeductDate = (DateTime)ResultDataReader["DeductDate"];
				consortiaInfo.StoreLevel = (int)ResultDataReader["StoreLevel"];
				consortiaInfo.SmithLevel = (int)ResultDataReader["SmithLevel"];
				consortiaInfo.ShopLevel = (int)ResultDataReader["ShopLevel"];
				consortiaInfo.SkillLevel = (int)ResultDataReader["SkillLevel"];
				consortiaInfo.ConsortiaName = ((ResultDataReader["ConsortiaName"] == null) ? "" : ResultDataReader["ConsortiaName"].ToString());
				consortiaInfo.IsDirty = false;
				list.Add(consortiaInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetRankWeekRichesConsortias", exception);
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

	public ConsortiaInfo[] GetRankDayRichesConsortias()
	{
		List<ConsortiaInfo> list = new List<ConsortiaInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Consortia_Rank_Day_Riches");
			while (ResultDataReader.Read())
			{
				ConsortiaInfo consortiaInfo = new ConsortiaInfo();
				consortiaInfo.ConsortiaID = (int)ResultDataReader["ConsortiaID"];
				consortiaInfo.Honor = (int)ResultDataReader["Honor"];
				consortiaInfo.Level = (int)ResultDataReader["Level"];
				consortiaInfo.Riches = (int)ResultDataReader["Riches"];
				consortiaInfo.AddDayRiches = (int)ResultDataReader["AddDayRiches"];
				consortiaInfo.AddWeekRiches = (int)ResultDataReader["AddWeekRiches"];
				consortiaInfo.MaxCount = (int)ResultDataReader["MaxCount"];
				consortiaInfo.BuildDate = (DateTime)ResultDataReader["BuildDate"];
				consortiaInfo.IsExist = (bool)ResultDataReader["IsExist"];
				consortiaInfo.DeductDate = (DateTime)ResultDataReader["DeductDate"];
				consortiaInfo.StoreLevel = (int)ResultDataReader["StoreLevel"];
				consortiaInfo.SmithLevel = (int)ResultDataReader["SmithLevel"];
				consortiaInfo.ShopLevel = (int)ResultDataReader["ShopLevel"];
				consortiaInfo.SkillLevel = (int)ResultDataReader["SkillLevel"];
				consortiaInfo.ConsortiaName = ((ResultDataReader["ConsortiaName"] == null) ? "" : ResultDataReader["ConsortiaName"].ToString());
				consortiaInfo.IsDirty = false;
				list.Add(consortiaInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetRankDayRichesConsortias", exception);
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

	public ConsortiaInfo[] GetRankDayOnlineConsortias()
	{
		List<ConsortiaInfo> list = new List<ConsortiaInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Consortia_Rank_Online");
			while (ResultDataReader.Read())
			{
				ConsortiaInfo consortiaInfo = new ConsortiaInfo();
				consortiaInfo.ConsortiaID = (int)ResultDataReader["ConsortiaID"];
				consortiaInfo.ConsortiaName = ((ResultDataReader["ConsortiaName"] == null) ? "" : ResultDataReader["ConsortiaName"].ToString());
				consortiaInfo.IsDirty = false;
				list.Add(consortiaInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetRankDayRichesConsortias", exception);
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

	public ConsortiaInfo[] GetConsortiaAll()
	{
		List<ConsortiaInfo> list = new List<ConsortiaInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Consortia_All");
			while (ResultDataReader.Read())
			{
				ConsortiaInfo consortiaInfo = new ConsortiaInfo();
				consortiaInfo.ConsortiaID = (int)ResultDataReader["ConsortiaID"];
				consortiaInfo.Honor = (int)ResultDataReader["Honor"];
				consortiaInfo.Level = (int)ResultDataReader["Level"];
				consortiaInfo.Riches = (int)ResultDataReader["Riches"];
				consortiaInfo.MaxCount = (int)ResultDataReader["MaxCount"];
				consortiaInfo.BuildDate = (DateTime)ResultDataReader["BuildDate"];
				consortiaInfo.IsExist = (bool)ResultDataReader["IsExist"];
				consortiaInfo.DeductDate = (DateTime)ResultDataReader["DeductDate"];
				consortiaInfo.StoreLevel = (int)ResultDataReader["StoreLevel"];
				consortiaInfo.SmithLevel = (int)ResultDataReader["SmithLevel"];
				consortiaInfo.ShopLevel = (int)ResultDataReader["ShopLevel"];
				consortiaInfo.SkillLevel = (int)ResultDataReader["SkillLevel"];
				consortiaInfo.ConsortiaName = ((ResultDataReader["ConsortiaName"] == null) ? "" : ResultDataReader["ConsortiaName"].ToString());
				consortiaInfo.IsDirty = false;
				list.Add(consortiaInfo);
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

	public ConsortiaInfo GetConsortiaSingle(int id)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@ID", id)
			};
			db.GetReader(ref ResultDataReader, "SP_Consortia_Single", sqlParameters);
			if (ResultDataReader.Read())
			{
				ConsortiaInfo consortiaInfo = new ConsortiaInfo();
				consortiaInfo.ConsortiaID = (int)ResultDataReader["ConsortiaID"];
				consortiaInfo.BuildDate = (DateTime)ResultDataReader["BuildDate"];
				consortiaInfo.CelebCount = (int)ResultDataReader["CelebCount"];
				consortiaInfo.ChairmanID = (int)ResultDataReader["ChairmanID"];
				consortiaInfo.ChairmanName = ResultDataReader["ChairmanName"].ToString();
				consortiaInfo.ChairmanTypeVIP = Convert.ToByte(ResultDataReader["typeVIP"]);
				consortiaInfo.ChairmanVIPLevel = (int)ResultDataReader["VIPLevel"];
				consortiaInfo.ConsortiaName = ResultDataReader["ConsortiaName"].ToString();
				consortiaInfo.CreatorID = (int)ResultDataReader["CreatorID"];
				consortiaInfo.CreatorName = ResultDataReader["CreatorName"].ToString();
				consortiaInfo.Description = ResultDataReader["Description"].ToString();
				consortiaInfo.Honor = (int)ResultDataReader["Honor"];
				consortiaInfo.IsExist = (bool)ResultDataReader["IsExist"];
				consortiaInfo.Level = (int)ResultDataReader["Level"];
				consortiaInfo.MaxCount = (int)ResultDataReader["MaxCount"];
				consortiaInfo.Placard = ResultDataReader["Placard"].ToString();
				consortiaInfo.IP = ResultDataReader["IP"].ToString();
				consortiaInfo.Port = (int)ResultDataReader["Port"];
				consortiaInfo.Repute = (int)ResultDataReader["Repute"];
				consortiaInfo.Count = (int)ResultDataReader["Count"];
				consortiaInfo.Riches = (int)ResultDataReader["Riches"];
				consortiaInfo.DeductDate = (DateTime)ResultDataReader["DeductDate"];
				consortiaInfo.StoreLevel = (int)ResultDataReader["StoreLevel"];
				consortiaInfo.SmithLevel = (int)ResultDataReader["SmithLevel"];
				consortiaInfo.ShopLevel = (int)ResultDataReader["ShopLevel"];
				consortiaInfo.SkillLevel = (int)ResultDataReader["SkillLevel"];
				consortiaInfo.LastOpenBoss = (DateTime)ResultDataReader["LastOpenBoss"];
				consortiaInfo.extendAvailableNum = (int)ResultDataReader["extendAvailableNum"];
				return consortiaInfo;
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

	public bool ConsortiaFight(int consortiWin, int consortiaLose, int playerCount, out int riches, int state, int totalKillHealth, float richesRate)
	{
		bool result = false;
		riches = 0;
		try
		{
			SqlParameter[] array = new SqlParameter[8]
			{
				new SqlParameter("@ConsortiaWin", consortiWin),
				new SqlParameter("@ConsortiaLose", consortiaLose),
				new SqlParameter("@PlayerCount", playerCount),
				new SqlParameter("@Riches", SqlDbType.Int),
				null,
				null,
				null,
				null
			};
			array[3].Direction = ParameterDirection.InputOutput;
			array[3].Value = riches;
			array[4] = new SqlParameter("@Result", SqlDbType.Int);
			array[4].Direction = ParameterDirection.ReturnValue;
			array[5] = new SqlParameter("@State", state);
			array[6] = new SqlParameter("@TotalKillHealth", totalKillHealth);
			array[7] = new SqlParameter("@RichesRate", richesRate);
			result = db.RunProcedure("SP_Consortia_Fight", array);
			riches = (int)array[3].Value;
			int num = (int)array[4].Value;
			result = num == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("ConsortiaFight", exception);
			}
		}
		finally
		{
		}
		return result;
	}

	public bool ConsortiaRichAdd(int consortiID, ref int riches)
	{
		return ConsortiaRichAdd(consortiID, ref riches, 0, "");
	}

	public bool ConsortiaRichAdd(int consortiID, ref int riches, int type, string username)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[5]
			{
				new SqlParameter("@ConsortiaID", consortiID),
				new SqlParameter("@Riches", SqlDbType.Int),
				null,
				null,
				null
			};
			array[1].Direction = ParameterDirection.InputOutput;
			array[1].Value = riches;
			array[2] = new SqlParameter("@Result", SqlDbType.Int);
			array[2].Direction = ParameterDirection.ReturnValue;
			array[3] = new SqlParameter("@Type", type);
			array[4] = new SqlParameter("@UserName", username);
			result = db.RunProcedure("SP_Consortia_Riches_Add", array);
			riches = (int)array[1].Value;
			int num = (int)array[2].Value;
			result = num == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("ConsortiaRichAdd", exception);
			}
		}
		finally
		{
		}
		return result;
	}

	public bool ConsortiaRichRemove(int consortiID, ref int riches)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[3]
			{
				new SqlParameter("@ConsortiaID", consortiID),
				new SqlParameter("@Riches", SqlDbType.Int),
				null
			};
			array[1].Direction = ParameterDirection.InputOutput;
			array[1].Value = riches;
			array[2] = new SqlParameter("@Result", SqlDbType.Int);
			array[2].Direction = ParameterDirection.ReturnValue;
			result = db.RunProcedure("SP_Consortia_Riches_Remove", array);
			riches = (int)array[1].Value;
			int num = (int)array[2].Value;
			result = num == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Consortia_Riches_Remove", exception);
			}
		}
		finally
		{
		}
		return result;
	}

	public bool ScanConsortia(ref string noticeID)
	{
		bool flag = false;
		try
		{
			SqlParameter[] array = new SqlParameter[2]
			{
				new SqlParameter("@NoticeID", SqlDbType.NVarChar, 4000),
				null
			};
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@Result", SqlDbType.Int);
			array[1].Direction = ParameterDirection.ReturnValue;
			flag = db.RunProcedure("SP_Consortia_Scan", array);
			int num = (int)array[1].Value;
			flag = num == 0;
			if (flag)
			{
				noticeID = array[0].Value.ToString();
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

	public bool RenamesBatch(string data)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@Db_A", SqlDbType.NVarChar, 100)
			};
			array[0].Value = data;
			result = db.RunProcedure("Sp_Renames_Batch", array);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init Sp_Renames_Batch", exception);
			}
		}
		return result;
	}

	public bool UpdateConsortiaRankDayAndWeek(DateTime curTime, bool updateDay, bool updateWeek)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[4];
			array[0] = new SqlParameter("@CurrentDate", curTime);
			array[1] = new SqlParameter("@UpdateDay", updateDay);
			array[3] = new SqlParameter("@UpdateWeek", updateWeek);
			array[2] = new SqlParameter("@Result", SqlDbType.Int);
			array[2].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Consortia_RankDayWeek_Update", array);
			int num = (int)array[2].Value;
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

	public bool UpdateConsotiaApplyState(int consortiaID, int userID, bool state, ref string msg)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[4]
			{
				new SqlParameter("@ConsortiaID", consortiaID),
				new SqlParameter("@UserID", userID),
				new SqlParameter("@State", state),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[3].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Consortia_Apply_State", array);
			int num = (int)array[3].Value;
			result = num == 0;
			int num2 = num;
			int num3 = num2;
			if (num3 == 2)
			{
				msg = "ConsortiaBussiness.UpdateConsotiaApplyState.Msg2";
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

	public bool AddConsortiaApplyUsers(ConsortiaApplyUserInfo info, ref string msg)
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
			array[0].Direction = ParameterDirection.InputOutput;
			array[1] = new SqlParameter("@ApplyDate", info.ApplyDate);
			array[2] = new SqlParameter("@ConsortiaID", info.ConsortiaID);
			array[3] = new SqlParameter("@ConsortiaName", (info.ConsortiaName == null) ? "" : info.ConsortiaName);
			array[4] = new SqlParameter("@IsExist", info.IsExist);
			array[5] = new SqlParameter("@Remark", (info.Remark == null) ? "" : info.Remark);
			array[6] = new SqlParameter("@UserID", info.UserID);
			array[7] = new SqlParameter("@UserName", (info.UserName == null) ? "" : info.UserName);
			array[8] = new SqlParameter("@Result", SqlDbType.Int);
			array[8].Direction = ParameterDirection.ReturnValue;
			result = db.RunProcedure("SP_ConsortiaApplyUser_Add", array);
			info.ID = (int)array[0].Value;
			int num = (int)array[8].Value;
			result = num == 0;
			switch (num)
			{
			case 2:
				msg = "ConsortiaBussiness.AddConsortiaApplyUsers.Msg2";
				break;
			case 6:
				msg = "ConsortiaBussiness.AddConsortiaApplyUsers.Msg6";
				break;
			case 7:
				msg = "ConsortiaBussiness.AddConsortiaApplyUsers.Msg7";
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
		finally
		{
		}
		return result;
	}

	public bool DeleteConsortiaApplyUsers(int applyID, int userID, int consortiaID, ref string msg)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[4]
			{
				new SqlParameter("@ID", applyID),
				new SqlParameter("@UserID", userID),
				new SqlParameter("@ConsortiaID", consortiaID),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[3].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_ConsortiaApplyUser_Delete", array);
			int num = (int)array[3].Value;
			result = num switch
			{
				3 => true, 
				0 => true, 
				_ => false, 
			};
			switch (num)
			{
			case 2:
				msg = "ConsortiaBussiness.DeleteConsortiaApplyUsers.Msg2";
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

	public bool PassConsortiaApplyUsers(int applyID, int userID, string userName, int consortiaID, ref string msg, ConsortiaUserInfo info, ref int consortiaRepute)
	{
		bool flag = false;
		try
		{
			SqlParameter[] array = new SqlParameter[24]
			{
				new SqlParameter("@ID", applyID),
				new SqlParameter("@UserID", userID),
				new SqlParameter("@UserName", userName),
				new SqlParameter("@ConsortiaID", consortiaID),
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
				null
			};
			array[4].Direction = ParameterDirection.ReturnValue;
			array[5] = new SqlParameter("@tempID", SqlDbType.Int);
			array[5].Direction = ParameterDirection.InputOutput;
			array[5].Value = info.UserID;
			array[6] = new SqlParameter("@tempName", SqlDbType.NVarChar, 100);
			array[6].Direction = ParameterDirection.InputOutput;
			array[6].Value = "";
			array[7] = new SqlParameter("@tempDutyID", SqlDbType.Int);
			array[7].Direction = ParameterDirection.InputOutput;
			array[7].Value = info.DutyID;
			array[8] = new SqlParameter("@tempDutyName", SqlDbType.NVarChar, 100);
			array[8].Direction = ParameterDirection.InputOutput;
			array[8].Value = "";
			array[9] = new SqlParameter("@tempOffer", SqlDbType.Int);
			array[9].Direction = ParameterDirection.InputOutput;
			array[9].Value = info.Offer;
			array[10] = new SqlParameter("@tempRichesOffer", SqlDbType.Int);
			array[10].Direction = ParameterDirection.InputOutput;
			array[10].Value = info.RichesOffer;
			array[11] = new SqlParameter("@tempRichesRob", SqlDbType.Int);
			array[11].Direction = ParameterDirection.InputOutput;
			array[11].Value = info.RichesRob;
			array[12] = new SqlParameter("@tempLastDate", SqlDbType.DateTime);
			array[12].Direction = ParameterDirection.InputOutput;
			array[12].Value = DateTime.Now;
			array[13] = new SqlParameter("@tempWin", SqlDbType.Int);
			array[13].Direction = ParameterDirection.InputOutput;
			array[13].Value = info.Win;
			array[14] = new SqlParameter("@tempTotal", SqlDbType.Int);
			array[14].Direction = ParameterDirection.InputOutput;
			array[14].Value = info.Total;
			array[15] = new SqlParameter("@tempEscape", SqlDbType.Int);
			array[15].Direction = ParameterDirection.InputOutput;
			array[15].Value = info.Escape;
			array[16] = new SqlParameter("@tempGrade", SqlDbType.Int);
			array[16].Direction = ParameterDirection.InputOutput;
			array[16].Value = info.Grade;
			array[17] = new SqlParameter("@tempLevel", SqlDbType.Int);
			array[17].Direction = ParameterDirection.InputOutput;
			array[17].Value = info.Level;
			array[18] = new SqlParameter("@tempCUID", SqlDbType.Int);
			array[18].Direction = ParameterDirection.InputOutput;
			array[18].Value = info.ID;
			array[19] = new SqlParameter("@tempState", SqlDbType.Int);
			array[19].Direction = ParameterDirection.InputOutput;
			array[19].Value = info.State;
			array[20] = new SqlParameter("@tempSex", SqlDbType.Bit);
			array[20].Direction = ParameterDirection.InputOutput;
			array[20].Value = info.Sex;
			array[21] = new SqlParameter("@tempDutyRight", SqlDbType.Int);
			array[21].Direction = ParameterDirection.InputOutput;
			array[21].Value = info.Right;
			array[22] = new SqlParameter("@tempConsortiaRepute", SqlDbType.Int);
			array[22].Direction = ParameterDirection.InputOutput;
			array[22].Value = consortiaRepute;
			array[23] = new SqlParameter("@tempLoginName", SqlDbType.NVarChar, 200);
			array[23].Direction = ParameterDirection.InputOutput;
			array[23].Value = consortiaRepute;
			db.RunProcedure("SP_ConsortiaApplyUser_Pass", array);
			int num = (int)array[4].Value;
			flag = num == 0;
			if (flag)
			{
				info.UserID = (int)array[5].Value;
				info.UserName = array[6].Value.ToString();
				info.DutyID = (int)array[7].Value;
				info.DutyName = array[8].Value.ToString();
				info.Offer = (int)array[9].Value;
				info.RichesOffer = (int)array[10].Value;
				info.RichesRob = (int)array[11].Value;
				info.LastDate = (DateTime)array[12].Value;
				info.Win = (int)array[13].Value;
				info.Total = (int)array[14].Value;
				info.Escape = (int)array[15].Value;
				info.Grade = (int)array[16].Value;
				info.Level = (int)array[17].Value;
				info.ID = (int)array[18].Value;
				info.State = (int)array[19].Value;
				info.Sex = (bool)array[20].Value;
				info.Right = (int)array[21].Value;
				consortiaRepute = (int)array[22].Value;
				info.LoginName = array[23].Value.ToString();
			}
			switch (num)
			{
			case 2:
				msg = "ConsortiaBussiness.PassConsortiaApplyUsers.Msg2";
				break;
			case 3:
				msg = "ConsortiaBussiness.PassConsortiaApplyUsers.Msg3";
				break;
			case 6:
				msg = "ConsortiaBussiness.PassConsortiaApplyUsers.Msg6";
				break;
			case 4:
			case 5:
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
		return flag;
	}

	public ConsortiaApplyUserInfo[] GetConsortiaApplyUserPage(int page, int size, ref int total, int order, int consortiaID, int applyID, int userID)
	{
		List<ConsortiaApplyUserInfo> list = new List<ConsortiaApplyUserInfo>();
		try
		{
			string text = " IsExist=1 ";
			if (consortiaID != -1)
			{
				text = text + " and ConsortiaID =" + consortiaID + " ";
			}
			if (applyID != -1)
			{
				text = text + " and ID =" + applyID + " ";
			}
			if (userID != -1)
			{
				text = text + " and UserID ='" + userID + "' ";
			}
			string fdOreder = "ID";
			switch (order)
			{
			case 1:
				fdOreder = "UserName,ID";
				break;
			case 2:
				fdOreder = "ApplyDate,ID";
				break;
			}
			DataTable page2 = GetPage("V_Consortia_Apply_Users", text, page, size, "*", fdOreder, "ID", ref total);
			foreach (DataRow row in page2.Rows)
			{
				ConsortiaApplyUserInfo consortiaApplyUserInfo = new ConsortiaApplyUserInfo();
				consortiaApplyUserInfo.ID = (int)row["ID"];
				consortiaApplyUserInfo.ApplyDate = (DateTime)row["ApplyDate"];
				consortiaApplyUserInfo.ConsortiaID = (int)row["ConsortiaID"];
				consortiaApplyUserInfo.ConsortiaName = row["ConsortiaName"].ToString();
				consortiaApplyUserInfo.ChairmanID = (int)row["ChairmanID"];
				consortiaApplyUserInfo.ChairmanName = row["ChairmanName"].ToString();
				consortiaApplyUserInfo.ID = (int)row["ID"];
				consortiaApplyUserInfo.IsExist = (bool)row["IsExist"];
				consortiaApplyUserInfo.Remark = row["Remark"].ToString();
				consortiaApplyUserInfo.UserID = (int)row["UserID"];
				consortiaApplyUserInfo.UserName = row["UserName"].ToString();
				consortiaApplyUserInfo.UserLevel = (int)row["Grade"];
				consortiaApplyUserInfo.typeVIP = (int)row["typeVIP"];
				consortiaApplyUserInfo.Win = (int)row["Win"];
				consortiaApplyUserInfo.Total = (int)row["Total"];
				consortiaApplyUserInfo.Repute = (int)row["Repute"];
				consortiaApplyUserInfo.FightPower = (int)row["FightPower"];
				consortiaApplyUserInfo.IsOld = (bool)row["IsOldPlayer"];
				consortiaApplyUserInfo.Offer = (int)row["Offer"];
				list.Add(consortiaApplyUserInfo);
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

	public bool AddConsortiaInviteUsers(ConsortiaInviteUserInfo info, ref string msg)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[11]
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
				null
			};
			array[0].Direction = ParameterDirection.InputOutput;
			array[1] = new SqlParameter("@ConsortiaID", info.ConsortiaID);
			array[2] = new SqlParameter("@ConsortiaName", (info.ConsortiaName == null) ? "" : info.ConsortiaName);
			array[3] = new SqlParameter("@InviteDate", info.InviteDate);
			array[4] = new SqlParameter("@InviteID", info.InviteID);
			array[5] = new SqlParameter("@InviteName", (info.InviteName == null) ? "" : info.InviteName);
			array[6] = new SqlParameter("@IsExist", info.IsExist);
			array[7] = new SqlParameter("@Remark", (info.Remark == null) ? "" : info.Remark);
			array[8] = new SqlParameter("@UserID", info.UserID);
			array[8].Direction = ParameterDirection.InputOutput;
			array[9] = new SqlParameter("@UserName", (info.UserName == null) ? "" : info.UserName);
			array[10] = new SqlParameter("@Result", SqlDbType.Int);
			array[10].Direction = ParameterDirection.ReturnValue;
			result = db.RunProcedure("SP_ConsortiaInviteUser_Add", array);
			info.ID = (int)array[0].Value;
			info.UserID = (int)array[8].Value;
			int num = (int)array[10].Value;
			result = num == 0;
			switch (num)
			{
			case 2:
				msg = "ConsortiaBussiness.AddConsortiaInviteUsers.Msg2";
				break;
			case 4:
				msg = "ConsortiaBussiness.AddConsortiaInviteUsers.Msg4";
				break;
			case 5:
				msg = "ConsortiaBussiness.AddConsortiaInviteUsers.Msg5";
				break;
			case 6:
				msg = "ConsortiaBussiness.AddConsortiaInviteUsers.Msg6";
				break;
			case 3:
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
		finally
		{
		}
		return result;
	}

	public bool DeleteConsortiaInviteUsers(int intiveID, int userID)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[3]
			{
				new SqlParameter("@ID", intiveID),
				new SqlParameter("@UserID", userID),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[2].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_ConsortiaInviteUser_Delete", array);
			int num = (int)array[2].Value;
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

	public bool PassConsortiaInviteUsers(int inviteID, int userID, string userName, ref int consortiaID, ref string consortiaName, ref string msg, ConsortiaUserInfo info, ref int tempID, ref string tempName, ref int consortiaRepute)
	{
		bool flag = false;
		try
		{
			SqlParameter[] array = new SqlParameter[24]
			{
				new SqlParameter("@ID", inviteID),
				new SqlParameter("@UserID", userID),
				new SqlParameter("@UserName", userName),
				new SqlParameter("@ConsortiaID", consortiaID),
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
			array[3].Direction = ParameterDirection.InputOutput;
			array[4] = new SqlParameter("@ConsortiaName", SqlDbType.NVarChar, 100);
			array[4].Value = consortiaName;
			array[4].Direction = ParameterDirection.InputOutput;
			array[5] = new SqlParameter("@Result", SqlDbType.Int);
			array[5].Direction = ParameterDirection.ReturnValue;
			array[6] = new SqlParameter("@tempName", SqlDbType.NVarChar, 100);
			array[6].Direction = ParameterDirection.InputOutput;
			array[6].Value = tempName;
			array[7] = new SqlParameter("@tempDutyID", SqlDbType.Int);
			array[7].Direction = ParameterDirection.InputOutput;
			array[7].Value = info.DutyID;
			array[8] = new SqlParameter("@tempDutyName", SqlDbType.NVarChar, 100);
			array[8].Direction = ParameterDirection.InputOutput;
			array[8].Value = "";
			array[9] = new SqlParameter("@tempOffer", SqlDbType.Int);
			array[9].Direction = ParameterDirection.InputOutput;
			array[9].Value = info.Offer;
			array[10] = new SqlParameter("@tempRichesOffer", SqlDbType.Int);
			array[10].Direction = ParameterDirection.InputOutput;
			array[10].Value = info.RichesOffer;
			array[11] = new SqlParameter("@tempRichesRob", SqlDbType.Int);
			array[11].Direction = ParameterDirection.InputOutput;
			array[11].Value = info.RichesRob;
			array[12] = new SqlParameter("@tempLastDate", SqlDbType.DateTime);
			array[12].Direction = ParameterDirection.InputOutput;
			array[12].Value = DateTime.Now;
			array[13] = new SqlParameter("@tempWin", SqlDbType.Int);
			array[13].Direction = ParameterDirection.InputOutput;
			array[13].Value = info.Win;
			array[14] = new SqlParameter("@tempTotal", SqlDbType.Int);
			array[14].Direction = ParameterDirection.InputOutput;
			array[14].Value = info.Total;
			array[15] = new SqlParameter("@tempEscape", SqlDbType.Int);
			array[15].Direction = ParameterDirection.InputOutput;
			array[15].Value = info.Escape;
			array[16] = new SqlParameter("@tempID", SqlDbType.Int);
			array[16].Direction = ParameterDirection.InputOutput;
			array[16].Value = tempID;
			array[17] = new SqlParameter("@tempGrade", SqlDbType.Int);
			array[17].Direction = ParameterDirection.InputOutput;
			array[17].Value = info.Level;
			array[18] = new SqlParameter("@tempLevel", SqlDbType.Int);
			array[18].Direction = ParameterDirection.InputOutput;
			array[18].Value = info.Level;
			array[19] = new SqlParameter("@tempCUID", SqlDbType.Int);
			array[19].Direction = ParameterDirection.InputOutput;
			array[19].Value = info.ID;
			array[20] = new SqlParameter("@tempState", SqlDbType.Int);
			array[20].Direction = ParameterDirection.InputOutput;
			array[20].Value = info.State;
			array[21] = new SqlParameter("@tempSex", SqlDbType.Bit);
			array[21].Direction = ParameterDirection.InputOutput;
			array[21].Value = info.Sex;
			array[22] = new SqlParameter("@tempRight", SqlDbType.Int);
			array[22].Direction = ParameterDirection.InputOutput;
			array[22].Value = info.Right;
			array[23] = new SqlParameter("@tempConsortiaRepute", SqlDbType.Int);
			array[23].Direction = ParameterDirection.InputOutput;
			array[23].Value = consortiaRepute;
			db.RunProcedure("SP_ConsortiaInviteUser_Pass", array);
			int num = (int)array[5].Value;
			flag = num == 0;
			if (flag)
			{
				consortiaID = (int)array[3].Value;
				consortiaName = array[4].Value.ToString();
				tempName = array[6].Value.ToString();
				info.DutyID = (int)array[7].Value;
				info.DutyName = array[8].Value.ToString();
				info.Offer = (int)array[9].Value;
				info.RichesOffer = (int)array[10].Value;
				info.RichesRob = (int)array[11].Value;
				info.LastDate = (DateTime)array[12].Value;
				info.Win = (int)array[13].Value;
				info.Total = (int)array[14].Value;
				info.Escape = (int)array[15].Value;
				tempID = (int)array[16].Value;
				info.Grade = (int)array[17].Value;
				info.Level = (int)array[18].Value;
				info.ID = (int)array[19].Value;
				info.State = (int)array[20].Value;
				info.Sex = (bool)array[21].Value;
				info.Right = (int)array[22].Value;
				consortiaRepute = (int)array[23].Value;
			}
			switch (num)
			{
			case 3:
				msg = "ConsortiaBussiness.PassConsortiaInviteUsers.Msg3";
				break;
			case 6:
				msg = "ConsortiaBussiness.PassConsortiaInviteUsers.Msg6";
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
		return flag;
	}

	public ConsortiaInviteUserInfo[] GetConsortiaInviteUserPage(int page, int size, ref int total, int order, int userID, int inviteID)
	{
		List<ConsortiaInviteUserInfo> list = new List<ConsortiaInviteUserInfo>();
		try
		{
			string text = " IsExist=1 ";
			if (userID != -1)
			{
				text = text + " and UserID =" + userID + " ";
			}
			if (inviteID != -1)
			{
				text = text + " and UserID =" + inviteID + " ";
			}
			string text2 = "ConsortiaName";
			switch (order)
			{
			case 1:
				text2 = "Repute";
				break;
			case 2:
				text2 = "ChairmanName";
				break;
			case 3:
				text2 = "Count";
				break;
			case 4:
				text2 = "CelebCount";
				break;
			case 5:
				text2 = "Honor";
				break;
			}
			text2 += ",ID ";
			DataTable page2 = GetPage("V_Consortia_Invite", text, page, size, "*", text2, "ID", ref total);
			foreach (DataRow row in page2.Rows)
			{
				ConsortiaInviteUserInfo consortiaInviteUserInfo = new ConsortiaInviteUserInfo();
				consortiaInviteUserInfo.ID = (int)row["ID"];
				consortiaInviteUserInfo.CelebCount = (int)row["CelebCount"];
				consortiaInviteUserInfo.ChairmanName = row["ChairmanName"].ToString();
				consortiaInviteUserInfo.ConsortiaID = (int)row["ConsortiaID"];
				consortiaInviteUserInfo.ConsortiaName = row["ConsortiaName"].ToString();
				consortiaInviteUserInfo.Count = (int)row["Count"];
				consortiaInviteUserInfo.Honor = (int)row["Honor"];
				consortiaInviteUserInfo.InviteDate = (DateTime)row["InviteDate"];
				consortiaInviteUserInfo.InviteID = (int)row["InviteID"];
				consortiaInviteUserInfo.InviteName = row["InviteName"].ToString();
				consortiaInviteUserInfo.IsExist = (bool)row["IsExist"];
				consortiaInviteUserInfo.Remark = row["Remark"].ToString();
				consortiaInviteUserInfo.Repute = (int)row["Repute"];
				consortiaInviteUserInfo.UserID = (int)row["UserID"];
				consortiaInviteUserInfo.UserName = row["UserName"].ToString();
				list.Add(consortiaInviteUserInfo);
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

	public bool DeleteConsortiaUser(int userID, int kickUserID, int consortiaID, ref string msg, ref string nickName)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[5]
			{
				new SqlParameter("@UserID", userID),
				new SqlParameter("@KickUserID", kickUserID),
				new SqlParameter("@ConsortiaID", consortiaID),
				new SqlParameter("@Result", SqlDbType.Int),
				null
			};
			array[3].Direction = ParameterDirection.ReturnValue;
			array[4] = new SqlParameter("@NickName", SqlDbType.NVarChar, 200);
			array[4].Direction = ParameterDirection.InputOutput;
			array[4].Value = nickName;
			db.RunProcedure("SP_ConsortiaUser_Delete", array);
			int num = (int)array[3].Value;
			if (num == 0)
			{
				result = true;
				nickName = array[4].Value.ToString();
			}
			switch (num)
			{
			case 2:
				msg = "ConsortiaBussiness.DeleteConsortiaUser.Msg2";
				break;
			case 3:
				msg = "ConsortiaBussiness.DeleteConsortiaUser.Msg3";
				break;
			case 4:
				msg = "ConsortiaBussiness.DeleteConsortiaUser.Msg4";
				break;
			case 5:
				msg = "ConsortiaBussiness.DeleteConsortiaUser.Msg5";
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

	public bool UpdateConsortiaIsBanChat(int banUserID, int consortiaID, int userID, bool isBanChat, ref int tempID, ref string tempName, ref string msg)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[7]
			{
				new SqlParameter("@ID", banUserID),
				new SqlParameter("@ConsortiaID", consortiaID),
				new SqlParameter("@UserID", userID),
				new SqlParameter("@IsBanChat", isBanChat),
				new SqlParameter("@TempID", tempID),
				null,
				null
			};
			array[4].Direction = ParameterDirection.InputOutput;
			array[5] = new SqlParameter("@TempName", SqlDbType.NVarChar, 100);
			array[5].Value = tempName;
			array[5].Direction = ParameterDirection.InputOutput;
			array[6] = new SqlParameter("@Result", SqlDbType.Int);
			array[6].Direction = ParameterDirection.ReturnValue;
			result = db.RunProcedure("SP_ConsortiaIsBanChat_Update", array);
			int num = (int)array[6].Value;
			tempID = (int)array[4].Value;
			tempName = array[5].Value.ToString();
			result = num == 0;
			switch (num)
			{
			case 2:
				msg = "ConsortiaBussiness.UpdateConsortiaIsBanChat.Msg2";
				break;
			case 3:
				msg = "ConsortiaBussiness.UpdateConsortiaIsBanChat.Msg3";
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
		finally
		{
		}
		return result;
	}

	public bool UpdateConsortiaUserRemark(int id, int consortiaID, int userID, string remark, ref string msg)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[5]
			{
				new SqlParameter("@ID", id),
				new SqlParameter("@ConsortiaID", consortiaID),
				new SqlParameter("@UserID", userID),
				new SqlParameter("@Remark", remark),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[4].Direction = ParameterDirection.ReturnValue;
			result = db.RunProcedure("SP_ConsortiaUserRemark_Update", array);
			int num = (int)array[4].Value;
			result = num == 0;
			int num2 = num;
			int num3 = num2;
			if (num3 == 2)
			{
				msg = "ConsortiaBussiness.UpdateConsortiaUserRemark.Msg2";
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

	public bool UpdateConsortiaUserGrade(int id, int consortiaID, int userID, bool upGrade, ref string msg, ref ConsortiaDutyInfo info, ref string tempUserName)
	{
		bool flag = false;
		try
		{
			SqlParameter[] array = new SqlParameter[9]
			{
				new SqlParameter("@ID", id),
				new SqlParameter("@ConsortiaID", consortiaID),
				new SqlParameter("@UserID", userID),
				new SqlParameter("@UpGrade", upGrade),
				new SqlParameter("@Result", SqlDbType.Int),
				null,
				null,
				null,
				null
			};
			array[4].Direction = ParameterDirection.ReturnValue;
			array[5] = new SqlParameter("@tempUserName", SqlDbType.NVarChar, 100);
			array[5].Direction = ParameterDirection.InputOutput;
			array[5].Value = tempUserName;
			array[6] = new SqlParameter("@tempDutyLevel", SqlDbType.Int);
			array[6].Direction = ParameterDirection.InputOutput;
			array[6].Value = info.Level;
			array[7] = new SqlParameter("@tempDutyName", SqlDbType.NVarChar, 100);
			array[7].Direction = ParameterDirection.InputOutput;
			array[7].Value = "";
			array[8] = new SqlParameter("@tempRight", SqlDbType.Int);
			array[8].Direction = ParameterDirection.InputOutput;
			array[8].Value = info.Right;
			flag = db.RunProcedure("SP_ConsortiaUserGrade_Update", array);
			int num = (int)array[4].Value;
			flag = num == 0;
			if (flag)
			{
				tempUserName = array[5].Value.ToString();
				info.Level = (int)array[6].Value;
				info.DutyName = array[7].Value.ToString();
				info.Right = (int)array[8].Value;
			}
			switch (num)
			{
			case 2:
				msg = "ConsortiaBussiness.UpdateConsortiaUserGrade.Msg2";
				break;
			case 3:
				msg = (upGrade ? "ConsortiaBussiness.UpdateConsortiaUserGrade.Msg3" : "ConsortiaBussiness.UpdateConsortiaUserGrade.Msg10");
				break;
			case 4:
				msg = "ConsortiaBussiness.UpdateConsortiaUserGrade.Msg4";
				break;
			case 5:
				msg = "ConsortiaBussiness.UpdateConsortiaUserGrade.Msg5";
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
		finally
		{
		}
		return flag;
	}

	public ConsortiaWarPlayerRankInfo[] GetConsortiaWarPlayerRank(int consortia)
	{
		List<ConsortiaWarPlayerRankInfo> list = new List<ConsortiaWarPlayerRankInfo>();
		try
		{
			string queryWhere = " ConsortiaID=" + consortia + " ";
			string fdOreder = "Score";
			int total = 0;
			DataTable page = GetPage("ConsortiaWarPlayerRank", queryWhere, 1, 20, "*", fdOreder, "UserID", ref total);
			foreach (DataRow row in page.Rows)
			{
				ConsortiaWarPlayerRankInfo consortiaWarPlayerRankInfo = new ConsortiaWarPlayerRankInfo();
				consortiaWarPlayerRankInfo.UserID = (int)row["UserID"];
				consortiaWarPlayerRankInfo.NickName = (string)row["NickName"];
				consortiaWarPlayerRankInfo.ConsortiaID = (int)row["ConsortiaID"];
				consortiaWarPlayerRankInfo.Score = (int)row["Score"];
				consortiaWarPlayerRankInfo.ZoneName = (string)row["ZoneName"];
				consortiaWarPlayerRankInfo.ZoneID = (int)row["ZoneID"];
				list.Add(consortiaWarPlayerRankInfo);
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

	public ConsortiaWarRankInfo[] GetConsortiaWarRank()
	{
		List<ConsortiaWarRankInfo> list = new List<ConsortiaWarRankInfo>();
		try
		{
			string queryWhere = " ";
			string fdOreder = "Rank";
			int total = 0;
			DataTable page = GetPage("ConsortiaWarRank", queryWhere, 1, 10, "*", fdOreder, "ConsortiaID", ref total);
			foreach (DataRow row in page.Rows)
			{
				ConsortiaWarRankInfo consortiaWarRankInfo = new ConsortiaWarRankInfo();
				consortiaWarRankInfo.ConsortiaID = (int)row["ConsortiaID"];
				consortiaWarRankInfo.ConsortiaName = (string)row["ConsortiaName"];
				consortiaWarRankInfo.Rank = (int)row["Rank"];
				consortiaWarRankInfo.Score = (int)row["Score"];
				list.Add(consortiaWarRankInfo);
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

	public ConsortiaUserInfo[] GetConsortiaUsersPage(int page, int size, ref int total, int order, int consortiaID, int userID, int state)
	{
		List<ConsortiaUserInfo> list = new List<ConsortiaUserInfo>();
		try
		{
			string text = " IsExist=1 ";
			if (consortiaID != -1)
			{
				text = text + " and ConsortiaID =" + consortiaID + " ";
			}
			if (userID != -1)
			{
				text = text + " and UserID =" + userID + " ";
			}
			if (state != -1)
			{
				text = text + " and State =" + state + " ";
			}
			string text2 = "UserName";
			switch (order)
			{
			case 1:
				text2 = "DutyID";
				break;
			case 2:
				text2 = "Grade";
				break;
			case 3:
				text2 = "Repute";
				break;
			case 4:
				text2 = "GP";
				break;
			case 5:
				text2 = "State";
				break;
			case 6:
				text2 = "Offer";
				break;
			case 7:
				text2 = "LastDate";
				break;
			}
			text2 += ",ID ";
			DataTable page2 = GetPage("V_Consortia_Users", text, page, size, "*", text2, "ID", ref total);
			foreach (DataRow row in page2.Rows)
			{
				ConsortiaUserInfo consortiaUserInfo = new ConsortiaUserInfo();
				consortiaUserInfo.ID = (int)row["ID"];
				consortiaUserInfo.ConsortiaID = (int)row["ConsortiaID"];
				consortiaUserInfo.DutyID = (int)row["DutyID"];
				consortiaUserInfo.DutyName = row["DutyName"].ToString();
				consortiaUserInfo.IsExist = (bool)row["IsExist"];
				consortiaUserInfo.RatifierID = (int)row["RatifierID"];
				consortiaUserInfo.RatifierName = row["RatifierName"].ToString();
				consortiaUserInfo.Remark = row["Remark"].ToString();
				consortiaUserInfo.UserID = (int)row["UserID"];
				consortiaUserInfo.UserName = row["UserName"].ToString();
				consortiaUserInfo.Grade = (int)row["Grade"];
				consortiaUserInfo.GP = (int)row["GP"];
				consortiaUserInfo.Repute = (int)row["Repute"];
				consortiaUserInfo.State = (int)row["State"];
				consortiaUserInfo.Right = (int)row["Right"];
				consortiaUserInfo.Offer = (int)row["Offer"];
				consortiaUserInfo.Colors = row["Colors"].ToString();
				consortiaUserInfo.Style = row["Style"].ToString();
				consortiaUserInfo.Hide = (int)row["Hide"];
				consortiaUserInfo.Skin = ((row["Skin"] == null) ? "" : consortiaUserInfo.Skin);
				consortiaUserInfo.Level = (int)row["Level"];
				consortiaUserInfo.LastDate = (DateTime)row["LastDate"];
				consortiaUserInfo.Sex = (bool)row["Sex"];
				consortiaUserInfo.IsBanChat = (bool)row["IsBanChat"];
				consortiaUserInfo.Win = (int)row["Win"];
				consortiaUserInfo.Total = (int)row["Total"];
				consortiaUserInfo.Escape = (int)row["Escape"];
				consortiaUserInfo.RichesOffer = (int)row["RichesOffer"];
				consortiaUserInfo.RichesRob = (int)row["RichesRob"];
				consortiaUserInfo.LoginName = ((row["LoginName"] == null) ? "" : row["LoginName"].ToString());
				consortiaUserInfo.Nimbus = (int)row["Nimbus"];
				consortiaUserInfo.FightPower = (int)row["FightPower"];
				consortiaUserInfo.typeVIP = Convert.ToByte(row["typeVIP"]);
				consortiaUserInfo.VIPLevel = (int)row["VIPLevel"];
				list.Add(consortiaUserInfo);
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

	public ConsortiaUserInfo GetConsortiaUsersByUserID(int userID)
	{
		int total = 0;
		ConsortiaUserInfo[] consortiaUsersPage = GetConsortiaUsersPage(1, 1, ref total, -1, -1, userID, -1);
		if (consortiaUsersPage.Length == 1)
		{
			return consortiaUsersPage[0];
		}
		return null;
	}

	public bool ConsortiaEventAdd(int consortiID, int type, string username, string managername)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[4]
			{
				new SqlParameter("@ConsortiaID", consortiID),
				new SqlParameter("@type", type),
				new SqlParameter("@username", username),
				new SqlParameter("@managername", managername)
			};
			result = db.RunProcedure("SP_Consortia_Event_Add", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("ConsortiaEventAdd", exception);
			}
		}
		return result;
	}

	public bool AddConsortiaDuty(ConsortiaDutyInfo info, int userID, ref string msg)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[7]
			{
				new SqlParameter("@DutyID", info.DutyID),
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[0].Direction = ParameterDirection.InputOutput;
			array[1] = new SqlParameter("@ConsortiaID", info.ConsortiaID);
			array[2] = new SqlParameter("@DutyName", info.DutyName);
			array[3] = new SqlParameter("@Level", info.Level);
			array[4] = new SqlParameter("@UserID", userID);
			array[5] = new SqlParameter("@Right", info.Right);
			array[6] = new SqlParameter("@Result", SqlDbType.Int);
			array[6].Direction = ParameterDirection.ReturnValue;
			result = db.RunProcedure("SP_ConsortiaDuty_Add", array);
			info.DutyID = (int)array[0].Value;
			int num = (int)array[6].Value;
			result = num == 0;
			switch (num)
			{
			case 2:
				msg = "ConsortiaBussiness.AddConsortiaDuty.Msg2";
				break;
			case 3:
				msg = "ConsortiaBussiness.AddConsortiaDuty.Msg3";
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
		finally
		{
		}
		return result;
	}

	public bool DeleteConsortiaDuty(int dutyID, int userID, int consortiaID, ref string msg)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[4]
			{
				new SqlParameter("@UserID", userID),
				new SqlParameter("@ConsortiaID", consortiaID),
				new SqlParameter("@DutyID", dutyID),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[3].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_ConsortiaDuty_Delete", array);
			int num = (int)array[3].Value;
			result = num == 0;
			switch (num)
			{
			case 2:
				msg = "ConsortiaBussiness.DeleteConsortiaDuty.Msg2";
				break;
			case 3:
				msg = "ConsortiaBussiness.DeleteConsortiaDuty.Msg3";
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

	public bool UpdateConsortiaDuty(ConsortiaDutyInfo info, int userID, int updateType, ref string msg)
	{
		bool flag = false;
		try
		{
			SqlParameter[] array = new SqlParameter[8]
			{
				new SqlParameter("@DutyID", info.DutyID),
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[0].Direction = ParameterDirection.InputOutput;
			array[1] = new SqlParameter("@ConsortiaID", info.ConsortiaID);
			array[2] = new SqlParameter("@DutyName", SqlDbType.NVarChar, 100);
			array[2].Direction = ParameterDirection.InputOutput;
			array[2].Value = info.DutyName;
			array[3] = new SqlParameter("@Right", SqlDbType.Int);
			array[3].Direction = ParameterDirection.InputOutput;
			array[3].Value = info.Right;
			array[4] = new SqlParameter("@Level", SqlDbType.Int);
			array[4].Direction = ParameterDirection.InputOutput;
			array[4].Value = info.Level;
			array[5] = new SqlParameter("@UserID", userID);
			array[6] = new SqlParameter("@UpdateType", updateType);
			array[7] = new SqlParameter("@Result", SqlDbType.Int);
			array[7].Direction = ParameterDirection.ReturnValue;
			flag = db.RunProcedure("SP_ConsortiaDuty_Update", array);
			int num = (int)array[7].Value;
			flag = num == 0;
			if (flag)
			{
				info.DutyID = (int)array[0].Value;
				info.DutyName = ((array[2].Value == null) ? "" : array[2].Value.ToString());
				info.Right = (int)array[3].Value;
				info.Level = (int)array[4].Value;
			}
			switch (num)
			{
			case 2:
				msg = "ConsortiaBussiness.UpdateConsortiaDuty.Msg2";
				break;
			case 3:
			case 4:
				msg = "ConsortiaBussiness.UpdateConsortiaDuty.Msg3";
				break;
			case 5:
				msg = "ConsortiaBussiness.DeleteConsortiaDuty.Msg5";
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
		finally
		{
		}
		return flag;
	}

	public ConsortiaDutyInfo[] GetConsortiaDutyPage(int page, int size, ref int total, int order, int consortiaID, int dutyID)
	{
		List<ConsortiaDutyInfo> list = new List<ConsortiaDutyInfo>();
		try
		{
			string text = " IsExist=1 ";
			if (consortiaID != -1)
			{
				text = text + " and ConsortiaID =" + consortiaID + " ";
			}
			if (dutyID != -1)
			{
				text = text + " and DutyID =" + dutyID + " ";
			}
			string text2 = "Level";
			if (order == 1)
			{
				text2 = "DutyName";
			}
			text2 += ",DutyID ";
			DataTable page2 = GetPage("Consortia_Duty", text, page, size, "*", text2, "DutyID", ref total);
			foreach (DataRow row in page2.Rows)
			{
				ConsortiaDutyInfo consortiaDutyInfo = new ConsortiaDutyInfo();
				consortiaDutyInfo.DutyID = (int)row["DutyID"];
				consortiaDutyInfo.ConsortiaID = (int)row["ConsortiaID"];
				consortiaDutyInfo.DutyID = (int)row["DutyID"];
				consortiaDutyInfo.DutyName = row["DutyName"].ToString();
				consortiaDutyInfo.IsExist = (bool)row["IsExist"];
				consortiaDutyInfo.Right = (int)row["Right"];
				consortiaDutyInfo.Level = (int)row["Level"];
				list.Add(consortiaDutyInfo);
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

	public int[] GetConsortiaByAllyByState(int consortiaID, int state)
	{
		List<int> list = new List<int>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[2]
			{
				new SqlParameter("@ConsortiaID", consortiaID),
				new SqlParameter("@State", state)
			};
			db.GetReader(ref ResultDataReader, "SP_Consortia_AllyByState", sqlParameters);
			while (ResultDataReader.Read())
			{
				list.Add((int)ResultDataReader["Consortia2ID"]);
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

	public bool AddConsortiaApplyAlly(ConsortiaApplyAllyInfo info, int userID, ref string msg)
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
			array[0].Direction = ParameterDirection.InputOutput;
			array[1] = new SqlParameter("@Consortia1ID", info.Consortia1ID);
			array[2] = new SqlParameter("@Consortia2ID", info.Consortia2ID);
			array[3] = new SqlParameter("@Date", info.Date);
			array[4] = new SqlParameter("@Remark", info.Remark);
			array[5] = new SqlParameter("@IsExist", info.IsExist);
			array[6] = new SqlParameter("@UserID", userID);
			array[7] = new SqlParameter("@State", info.State);
			array[8] = new SqlParameter("@Result", SqlDbType.Int);
			array[8].Direction = ParameterDirection.ReturnValue;
			result = db.RunProcedure("SP_ConsortiaApplyAlly_Add", array);
			info.ID = (int)array[0].Value;
			int num = (int)array[8].Value;
			result = num == 0;
			switch (num)
			{
			case 2:
				msg = "ConsortiaBussiness.AddConsortiaApplyAlly.Msg2";
				break;
			case 3:
				msg = "ConsortiaBussiness.AddConsortiaApplyAlly.Msg3";
				break;
			case 4:
				msg = "ConsortiaBussiness.AddConsortiaApplyAlly.Msg4";
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
		finally
		{
		}
		return result;
	}

	public bool DeleteConsortiaApplyAlly(int applyID, int userID, int consortiaID, ref string msg)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[4]
			{
				new SqlParameter("@ID", applyID),
				new SqlParameter("@UserID", userID),
				new SqlParameter("@ConsortiaID", consortiaID),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[3].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_ConsortiaApplyAlly_Delete", array);
			int num = (int)array[3].Value;
			result = num == 0;
			int num2 = num;
			int num3 = num2;
			if (num3 == 2)
			{
				msg = "ConsortiaBussiness.DeleteConsortiaApplyAlly.Msg2";
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

	public bool PassConsortiaApplyAlly(int applyID, int userID, int consortiaID, ref int tempID, ref int state, ref string msg)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[6]
			{
				new SqlParameter("@ID", applyID),
				new SqlParameter("@UserID", userID),
				new SqlParameter("@ConsortiaID", consortiaID),
				new SqlParameter("@tempID", tempID),
				null,
				null
			};
			array[3].Direction = ParameterDirection.InputOutput;
			array[4] = new SqlParameter("@State", state);
			array[4].Direction = ParameterDirection.InputOutput;
			array[5] = new SqlParameter("@Result", SqlDbType.Int);
			array[5].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_ConsortiaApplyAlly_Pass", array);
			int num = (int)array[5].Value;
			if (num == 0)
			{
				result = true;
				tempID = (int)array[3].Value;
				state = (int)array[4].Value;
			}
			switch (num)
			{
			case 2:
				msg = "ConsortiaBussiness.PassConsortiaApplyAlly.Msg2";
				break;
			case 3:
				msg = "ConsortiaBussiness.PassConsortiaApplyAlly.Msg3";
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

	public ConsortiaApplyAllyInfo[] GetConsortiaApplyAllyPage(int page, int size, ref int total, int order, int consortiaID, int applyID, int state)
	{
		List<ConsortiaApplyAllyInfo> list = new List<ConsortiaApplyAllyInfo>();
		try
		{
			string text = " IsExist=1 ";
			if (consortiaID != -1)
			{
				text = text + " and Consortia2ID =" + consortiaID + " ";
			}
			if (applyID != -1)
			{
				text = text + " and ID =" + applyID + " ";
			}
			if (state != -1)
			{
				text = text + " and State =" + state + " ";
			}
			string text2 = "ConsortiaName";
			switch (order)
			{
			case 1:
				text2 = "Repute";
				break;
			case 2:
				text2 = "ChairmanName";
				break;
			case 3:
				text2 = "Count";
				break;
			case 4:
				text2 = "Level";
				break;
			case 5:
				text2 = "Honor";
				break;
			}
			text2 += ",ID ";
			DataTable page2 = GetPage("V_Consortia_Apply_Ally", text, page, size, "*", text2, "ID", ref total);
			foreach (DataRow row in page2.Rows)
			{
				ConsortiaApplyAllyInfo consortiaApplyAllyInfo = new ConsortiaApplyAllyInfo();
				consortiaApplyAllyInfo.ID = (int)row["ID"];
				consortiaApplyAllyInfo.CelebCount = (int)row["CelebCount"];
				consortiaApplyAllyInfo.ChairmanName = row["ChairmanName"].ToString();
				consortiaApplyAllyInfo.Consortia1ID = (int)row["Consortia1ID"];
				consortiaApplyAllyInfo.Consortia2ID = (int)row["Consortia2ID"];
				consortiaApplyAllyInfo.ConsortiaName = row["ConsortiaName"].ToString();
				consortiaApplyAllyInfo.Count = (int)row["Count"];
				consortiaApplyAllyInfo.Date = (DateTime)row["Date"];
				consortiaApplyAllyInfo.Honor = (int)row["Honor"];
				consortiaApplyAllyInfo.IsExist = (bool)row["IsExist"];
				consortiaApplyAllyInfo.Remark = row["Remark"].ToString();
				consortiaApplyAllyInfo.Repute = (int)row["Repute"];
				consortiaApplyAllyInfo.State = (int)row["State"];
				consortiaApplyAllyInfo.Level = (int)row["Level"];
				consortiaApplyAllyInfo.Description = ((row["Description"] == null) ? "" : row["Description"].ToString());
				list.Add(consortiaApplyAllyInfo);
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

	public Dictionary<int, int> GetConsortiaByAlly(int consortiaID)
	{
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@ConsortiaID", consortiaID)
			};
			db.GetReader(ref ResultDataReader, "SP_Consortia_Ally_Neutral", sqlParameters);
			while (ResultDataReader.Read())
			{
				if ((int)ResultDataReader["Consortia1ID"] != consortiaID)
				{
					dictionary.Add((int)ResultDataReader["Consortia1ID"], (int)ResultDataReader["State"]);
				}
				else
				{
					dictionary.Add((int)ResultDataReader["Consortia2ID"], (int)ResultDataReader["State"]);
				}
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetConsortiaByAlly", exception);
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

	public bool AddConsortiaAlly(ConsortiaAllyInfo info, int userID, ref string msg)
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
			array[0].Direction = ParameterDirection.InputOutput;
			array[1] = new SqlParameter("@Consortia1ID", info.Consortia1ID);
			array[2] = new SqlParameter("@Consortia2ID", info.Consortia2ID);
			array[3] = new SqlParameter("@State", info.State);
			array[4] = new SqlParameter("@Date", info.Date);
			array[5] = new SqlParameter("@ValidDate", info.ValidDate);
			array[6] = new SqlParameter("@IsExist", info.IsExist);
			array[7] = new SqlParameter("@UserID", userID);
			array[8] = new SqlParameter("@Result", SqlDbType.Int);
			array[8].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_ConsortiaAlly_Add", array);
			int num = (int)array[8].Value;
			result = num == 0;
			switch (num)
			{
			case 2:
				msg = "ConsortiaBussiness.AddConsortiaAlly.Msg2";
				break;
			case 3:
				msg = "ConsortiaBussiness.AddConsortiaAlly.Msg3";
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

	public ConsortiaAllyInfo[] GetConsortiaAllyPage(int page, int size, ref int total, int order, int consortiaID, int state, string name)
	{
		List<ConsortiaAllyInfo> list = new List<ConsortiaAllyInfo>();
		string text = " IsExist=1 and ConsortiaID<>" + consortiaID;
		Dictionary<int, int> consortiaByAlly = GetConsortiaByAlly(consortiaID);
		try
		{
			if (state != -1)
			{
				string text2 = string.Empty;
				foreach (int key in consortiaByAlly.Keys)
				{
					text2 = text2 + key + ",";
				}
				text2 += 0;
				text = ((state != 0) ? (text + " and ConsortiaID in (" + text2 + ") ") : (text + " and ConsortiaID not in (" + text2 + ") "));
			}
			if (!string.IsNullOrEmpty(name))
			{
				text = text + " and ConsortiaName like '%" + name + "%' ";
			}
			DataTable page2 = GetPage("Consortia", text, page, size, "*", "ConsortiaID", "ConsortiaID", ref total);
			foreach (DataRow row in page2.Rows)
			{
				ConsortiaAllyInfo consortiaAllyInfo = new ConsortiaAllyInfo();
				consortiaAllyInfo.Consortia1ID = (int)row["ConsortiaID"];
				consortiaAllyInfo.ConsortiaName1 = ((row["ConsortiaName"] == null) ? "" : row["ConsortiaName"].ToString());
				consortiaAllyInfo.ConsortiaName2 = "";
				consortiaAllyInfo.Count1 = (int)row["Count"];
				consortiaAllyInfo.Repute1 = (int)row["Repute"];
				consortiaAllyInfo.ChairmanName1 = ((row["ChairmanName"] == null) ? "" : row["ChairmanName"].ToString());
				consortiaAllyInfo.ChairmanName2 = "";
				consortiaAllyInfo.Level1 = (int)row["Level"];
				consortiaAllyInfo.Honor1 = (int)row["Honor"];
				consortiaAllyInfo.Description1 = ((row["Description"] == null) ? "" : row["Description"].ToString());
				consortiaAllyInfo.Description2 = "";
				consortiaAllyInfo.Riches1 = (int)row["Riches"];
				consortiaAllyInfo.Date = DateTime.Now;
				consortiaAllyInfo.IsExist = true;
				if (consortiaByAlly.ContainsKey(consortiaAllyInfo.Consortia1ID))
				{
					consortiaAllyInfo.State = consortiaByAlly[consortiaAllyInfo.Consortia1ID];
				}
				consortiaAllyInfo.ValidDate = 0;
				list.Add(consortiaAllyInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetConsortiaAllyPage", exception);
			}
		}
		finally
		{
		}
		return list.ToArray();
	}

	public ConsortiaAllyInfo[] GetConsortiaAllyAll()
	{
		List<ConsortiaAllyInfo> list = new List<ConsortiaAllyInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_ConsortiaAlly_All");
			while (ResultDataReader.Read())
			{
				ConsortiaAllyInfo consortiaAllyInfo = new ConsortiaAllyInfo();
				consortiaAllyInfo.Consortia1ID = (int)ResultDataReader["Consortia1ID"];
				consortiaAllyInfo.Consortia2ID = (int)ResultDataReader["Consortia2ID"];
				consortiaAllyInfo.Date = (DateTime)ResultDataReader["Date"];
				consortiaAllyInfo.ID = (int)ResultDataReader["ID"];
				consortiaAllyInfo.State = (int)ResultDataReader["State"];
				consortiaAllyInfo.ValidDate = (int)ResultDataReader["ValidDate"];
				consortiaAllyInfo.IsExist = (bool)ResultDataReader["IsExist"];
				list.Add(consortiaAllyInfo);
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

	public bool AddConsortiaBattleRank(ConsortiaWarRankInfo guild)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[6]
			{
				new SqlParameter("@ConsortiaID", guild.ConsortiaID),
				new SqlParameter("@ConsortiaName", guild.ConsortiaName),
				new SqlParameter("@Rank", guild.Rank),
				new SqlParameter("@Score", guild.Score),
				new SqlParameter("@TimeCreate", guild.TimeCreate),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[5].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_ConsortiaWarRank_Add", array);
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

	public bool AddConsortiaBattlePlayerRank(ConsortiaWarPlayerRankInfo player)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[8]
			{
				new SqlParameter("@UserID", player.UserID),
				new SqlParameter("@NickName", player.NickName),
				new SqlParameter("@ConsortiaID", player.ConsortiaID),
				new SqlParameter("@Score", player.Score),
				new SqlParameter("@ZoneName", player.ZoneName),
				new SqlParameter("@ZoneID", player.ZoneID),
				new SqlParameter("@TimeCreate", player.TimeCreate),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[7].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_ConsortiaWarPlayerRank_Add", array);
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

	public void RemoveAllConsortiaBattleRank()
	{
		try
		{
			db.RunProcedure("SP_ConsortiaWarRank_RemoveAll");
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
	}

	public void RemoveAllConsortiaBattlePlayerRank()
	{
		try
		{
			db.RunProcedure("SP_ConsortiaWarPlayerRank_RemoveAll");
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
	}

	public ConsortiaEventInfo[] GetConsortiaEventPage(int page, int size, ref int total, int order, int consortiaID)
	{
		List<ConsortiaEventInfo> list = new List<ConsortiaEventInfo>();
		try
		{
			string text = " IsExist=1 ";
			if (consortiaID != -1)
			{
				text = text + " and ConsortiaID =" + consortiaID + " ";
			}
			string fdOreder = "Date desc,ID ";
			DataTable page2 = GetPage("Consortia_Event", text, page, size, "*", fdOreder, "ID", ref total);
			foreach (DataRow row in page2.Rows)
			{
				ConsortiaEventInfo consortiaEventInfo = new ConsortiaEventInfo();
				consortiaEventInfo.ID = (int)row["ID"];
				consortiaEventInfo.ConsortiaID = (int)row["ConsortiaID"];
				consortiaEventInfo.Date = (DateTime)row["Date"];
				consortiaEventInfo.IsExist = (bool)row["IsExist"];
				consortiaEventInfo.Remark = row["Remark"].ToString();
				consortiaEventInfo.Type = (int)row["Type"];
				consortiaEventInfo.NickName = row["NickName"].ToString();
				consortiaEventInfo.EventValue = (int)row["EventValue"];
				consortiaEventInfo.ManagerName = row["ManagerName"].ToString();
				list.Add(consortiaEventInfo);
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

	public bool AddAndUpdateConsortiaEuqipControl(ConsortiaEquipControlInfo info, int userID, ref string msg)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[6]
			{
				new SqlParameter("@ConsortiaID", info.ConsortiaID),
				new SqlParameter("@Level", info.Level),
				new SqlParameter("@Type", info.Type),
				new SqlParameter("@Riches", info.Riches),
				new SqlParameter("@UserID", userID),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[5].Direction = ParameterDirection.ReturnValue;
			result = db.RunProcedure("SP_Consortia_Equip_Control_Add", array);
			int num = (int)array[2].Value;
			result = num == 0;
			int num2 = num;
			int num3 = num2;
			if (num3 == 2)
			{
				msg = "ConsortiaBussiness.AddAndUpdateConsortiaEuqipControl.Msg2";
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

	public ConsortiaEquipControlInfo GetConsortiaEuqipRiches(int consortiaID, int Level, int type)
	{
		ConsortiaEquipControlInfo consortiaEquipControlInfo = null;
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[3]
			{
				new SqlParameter("@ConsortiaID", consortiaID),
				new SqlParameter("@Level", Level),
				new SqlParameter("@Type", type)
			};
			db.GetReader(ref ResultDataReader, "SP_Consortia_Equip_Control_Single", sqlParameters);
			if (ResultDataReader.Read())
			{
				consortiaEquipControlInfo = new ConsortiaEquipControlInfo();
				consortiaEquipControlInfo.ConsortiaID = (int)ResultDataReader["ConsortiaID"];
				consortiaEquipControlInfo.Level = (int)ResultDataReader["Level"];
				consortiaEquipControlInfo.Riches = (int)ResultDataReader["Riches"];
				consortiaEquipControlInfo.Type = (int)ResultDataReader["Type"];
				return consortiaEquipControlInfo;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllConsortiaLevel", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		if (consortiaEquipControlInfo == null)
		{
			consortiaEquipControlInfo = new ConsortiaEquipControlInfo();
			consortiaEquipControlInfo.ConsortiaID = consortiaID;
			consortiaEquipControlInfo.Level = Level;
			consortiaEquipControlInfo.Riches = 100;
			consortiaEquipControlInfo.Type = type;
		}
		return consortiaEquipControlInfo;
	}

	public ConsortiaEquipControlInfo[] GetConsortiaEquipControlPage(int page, int size, ref int total, int order, int consortiaID, int level, int type)
	{
		List<ConsortiaEquipControlInfo> list = new List<ConsortiaEquipControlInfo>();
		try
		{
			string text = " IsExist=1 ";
			if (consortiaID != -1)
			{
				text = text + " and ConsortiaID =" + consortiaID + " ";
			}
			if (level != -1)
			{
				text = text + " and Level =" + level + " ";
			}
			if (type != -1)
			{
				text = text + " and Type =" + type + " ";
			}
			string fdOreder = "ConsortiaID ";
			DataTable page2 = GetPage("Consortia_Equip_Control", text, page, size, "*", fdOreder, "ConsortiaID", ref total);
			foreach (DataRow row in page2.Rows)
			{
				ConsortiaEquipControlInfo consortiaEquipControlInfo = new ConsortiaEquipControlInfo();
				consortiaEquipControlInfo.ConsortiaID = (int)row["ConsortiaID"];
				consortiaEquipControlInfo.Level = (int)row["Level"];
				consortiaEquipControlInfo.Riches = (int)row["Riches"];
				consortiaEquipControlInfo.Type = (int)row["Type"];
				list.Add(consortiaEquipControlInfo);
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

	public ConsortiaRedPackageInfo[] GetConsortiaRedPackageAll()
	{
		List<ConsortiaRedPackageInfo> list = new List<ConsortiaRedPackageInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Consortia_RedPackage_All");
			while (ResultDataReader.Read())
			{
				ConsortiaRedPackageInfo consortiaRedPackageInfo = new ConsortiaRedPackageInfo();
				consortiaRedPackageInfo.ID = (int)ResultDataReader["ID"];
				consortiaRedPackageInfo.ConsortiaID = (int)ResultDataReader["ConsortiaID"];
				consortiaRedPackageInfo.UserID = (int)ResultDataReader["UserID"];
				consortiaRedPackageInfo.NickName = (string)ResultDataReader["NickName"];
				consortiaRedPackageInfo.Money = (int)ResultDataReader["Money"];
				consortiaRedPackageInfo.Package = (int)ResultDataReader["Package"];
				consortiaRedPackageInfo.WishWord = ((ResultDataReader["WishWord"] == DBNull.Value) ? "" : ((string)ResultDataReader["WishWord"]));
				consortiaRedPackageInfo.ReceiveData = ((ResultDataReader["ReceiveData"] == DBNull.Value) ? "" : ((string)ResultDataReader["ReceiveData"]));
				consortiaRedPackageInfo.TimeCreate = (DateTime)ResultDataReader["TimeCreate"];
				consortiaRedPackageInfo.IsDirty = false;
				list.Add(consortiaRedPackageInfo);
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

	public bool AddConsortiaRedPackage(ConsortiaRedPackageInfo info)
	{
		bool flag = false;
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
			array[0].Direction = ParameterDirection.InputOutput;
			array[1] = new SqlParameter("@ConsortiaID", info.ConsortiaID);
			array[2] = new SqlParameter("@UserID", info.UserID);
			array[3] = new SqlParameter("@NickName", info.NickName);
			array[4] = new SqlParameter("@Money", info.Money);
			array[5] = new SqlParameter("@Package", info.Package);
			array[6] = new SqlParameter("@WishWord", info.WishWord);
			array[7] = new SqlParameter("@ReceiveData", info.ReceiveData);
			array[8] = new SqlParameter("@Result", SqlDbType.Int);
			array[8].Direction = ParameterDirection.ReturnValue;
			flag = db.RunProcedure("SP_Consortia_RedPackage_Add", array);
			int num = (int)array[8].Value;
			flag = num == 0;
			if (flag)
			{
				info.ID = (int)array[0].Value;
				info.IsDirty = false;
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

	public bool UpdateConsortiaRedPackage(ConsortiaRedPackageInfo info)
	{
		bool flag = false;
		try
		{
			SqlParameter[] array = new SqlParameter[9]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@ConsortiaID", info.ConsortiaID),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@NickName", info.NickName),
				new SqlParameter("@Money", info.Money),
				new SqlParameter("@Package", info.Package),
				new SqlParameter("@WishWord", info.WishWord),
				new SqlParameter("@ReceiveData", info.ReceiveData),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[8].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Consortia_RedPackage_Update", array);
			int num = (int)array[8].Value;
			flag = num == 0;
			if (flag)
			{
				info.IsDirty = false;
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

	public bool AddConsortiaDomain(ConsortiaDomainInfo guild)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[5]
			{
				new SqlParameter("@ConsortiaID", guild.ConsortiaID),
				new SqlParameter("@UnFightBuildList", guild.UnFightBuildList),
				new SqlParameter("@BeginTime", guild.BeginTime),
				new SqlParameter("@EndTime", guild.EndTime),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[4].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_ConsortiaDomain_Add", array);
			int num = (int)array[4].Value;
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

	public bool UpdateConsortiaDomain(ConsortiaDomainInfo guild)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[5]
			{
				new SqlParameter("@ConsortiaID", guild.ConsortiaID),
				new SqlParameter("@UnFightBuildList", guild.UnFightBuildList),
				new SqlParameter("@BeginTime", guild.BeginTime),
				new SqlParameter("@EndTime", guild.EndTime),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[4].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_ConsortiaDomain_Update", array);
			int num = (int)array[4].Value;
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

	public ConsortiaDomainInfo[] GetConsortiaDomainAll()
	{
		List<ConsortiaDomainInfo> list = new List<ConsortiaDomainInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Consortia_Domain_All");
			while (ResultDataReader.Read())
			{
				ConsortiaDomainInfo consortiaDomainInfo = new ConsortiaDomainInfo();
				consortiaDomainInfo.ConsortiaID = (int)ResultDataReader["ConsortiaID"];
				consortiaDomainInfo.UnFightBuildList = (string)ResultDataReader["UnFightBuildList"];
				consortiaDomainInfo.BeginTime = (DateTime)ResultDataReader["BeginTime"];
				consortiaDomainInfo.EndTime = (DateTime)ResultDataReader["EndTime"];
				list.Add(consortiaDomainInfo);
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
}
