using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SqlDataProvider.Data;

namespace Bussiness;

public class PveBussiness : BaseCrossBussiness
{
	public PveInfo InitPveInfo(SqlDataReader dr)
	{
		PveInfo pveInfo = new PveInfo();
		pveInfo.ID = (int)dr["ID"];
		pveInfo.Name = ((dr["Name"] == null) ? "" : dr["Name"].ToString());
		pveInfo.Type = (int)dr["Type"];
		pveInfo.LevelLimits = (int)dr["LevelLimits"];
		pveInfo.SimpleTemplateIds = ((dr["SimpleTemplateIds"] == null) ? "" : dr["SimpleTemplateIds"].ToString());
		pveInfo.NormalTemplateIds = ((dr["NormalTemplateIds"] == null) ? "" : dr["NormalTemplateIds"].ToString());
		pveInfo.HardTemplateIds = ((dr["HardTemplateIds"] == null) ? "" : dr["HardTemplateIds"].ToString());
		pveInfo.TerrorTemplateIds = ((dr["TerrorTemplateIds"] == null) ? "" : dr["TerrorTemplateIds"].ToString());
		pveInfo.EpicTemplateIds = ((dr["EpicTemplateIds"] == null) ? "" : dr["EpicTemplateIds"].ToString());
		pveInfo.NightmareTemplateIds = ((dr["NightmareTemplateIds"] == null) ? "" : dr["NightmareTemplateIds"].ToString());
		pveInfo.Pic = ((dr["Pic"] == null) ? "" : dr["Pic"].ToString());
		pveInfo.Description = ((dr["Description"] == null) ? "" : dr["Description"].ToString());
		pveInfo.Ordering = (int)dr["Ordering"];
		pveInfo.AdviceTips = ((dr["AdviceTips"] == null) ? "" : dr["AdviceTips"].ToString());
		pveInfo.BossFightNeedMoney = ((dr["BossFightNeedMoney"] == null) ? "" : dr["BossFightNeedMoney"].ToString());
		pveInfo.MinLv = (int)dr["MinLv"];
		pveInfo.MaxLv = (int)dr["MaxLv"];
		pveInfo.SimpleGameScript = ((dr["SimpleGameScript"] == null) ? "" : dr["SimpleGameScript"].ToString());
		pveInfo.NormalGameScript = ((dr["NormalGameScript"] == null) ? "" : dr["NormalGameScript"].ToString());
		pveInfo.HardGameScript = ((dr["HardGameScript"] == null) ? "" : dr["HardGameScript"].ToString());
		pveInfo.TerrorGameScript = ((dr["TerrorGameScript"] == null) ? "" : dr["TerrorGameScript"].ToString());
		pveInfo.EpicGameScript = ((dr["EpicGameScript"] == null) ? "" : dr["EpicGameScript"].ToString());
		pveInfo.NightmareGameScript = ((dr["NightmareGameScript"] == null) ? "" : dr["NightmareGameScript"].ToString());
		return pveInfo;
	}

	public PveInfo[] GetAllPveInfos()
	{
		List<PveInfo> list = new List<PveInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_PveInfos_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitPveInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("GetAllPveInfos", exception);
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
