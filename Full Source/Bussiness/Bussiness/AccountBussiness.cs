using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SqlDataProvider.Data;

namespace Bussiness;

public class AccountBussiness : BaseBussiness
{
	public AccountBussiness()
		: base("conString2")
	{
	}

	public List<PlayerInfo> LoginMobile(string username, string password, ref bool flag)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[2]
			{
				new SqlParameter("@UserName", username),
				new SqlParameter("@PassWord", password)
			};
			db.GetReader(ref ResultDataReader, "Mem_Users_Accede_New", sqlParameters);
			if (ResultDataReader != null && ResultDataReader.Read())
			{
				flag = true;
				using PlayerBussiness playerBussiness = new PlayerBussiness();
				return playerBussiness.GetUserAllByUserName(username);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("LoginMobile", exception);
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
}
