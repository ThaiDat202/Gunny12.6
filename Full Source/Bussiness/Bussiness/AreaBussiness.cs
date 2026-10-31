using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SqlDataProvider.Data;

namespace Bussiness;

public class AreaBussiness : BaseCrossBussiness
{
	public AreaConfigInfo[] GetAllAreaConfig()
	{
		List<AreaConfigInfo> list = new List<AreaConfigInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_AreaConfig_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitAreaConfigInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseCrossBussiness.log.IsErrorEnabled)
			{
				BaseCrossBussiness.log.Error("InitAreaConfigInfo", exception);
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

	public AreaConfigInfo InitAreaConfigInfo(SqlDataReader dr)
	{
		AreaConfigInfo areaConfigInfo = new AreaConfigInfo();
		areaConfigInfo.AreaID = (int)dr["AreaID"];
		areaConfigInfo.AreaServer = ((dr["AreaServer"] == null) ? "" : dr["AreaServer"].ToString());
		areaConfigInfo.AreaName = ((dr["AreaName"] == null) ? "" : dr["AreaName"].ToString());
		areaConfigInfo.DataSource = ((dr["DataSource"] == null) ? "" : dr["DataSource"].ToString());
		areaConfigInfo.Catalog = ((dr["Catalog"] == null) ? "" : dr["Catalog"].ToString());
		areaConfigInfo.UserID = ((dr["UserID"] == null) ? "" : dr["UserID"].ToString());
		areaConfigInfo.Password = ((dr["Password"] == null) ? "" : dr["Password"].ToString());
		areaConfigInfo.RequestUrl = ((dr["RequestUrl"] == null) ? "" : dr["RequestUrl"].ToString());
		areaConfigInfo.CrossChatAllow = (bool)dr["CrossChatAllow"];
		areaConfigInfo.CrossPrivateChat = (bool)dr["CrossPrivateChat"];
		areaConfigInfo.Version = ((dr["Version"] == null) ? "" : dr["Version"].ToString());
		return areaConfigInfo;
	}
}
