using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SqlDataProvider.Data;

namespace Bussiness;

public class TeamBussiness : BaseBussiness
{
	public TeamInfo InitTeamInfo(SqlDataReader dr)
	{
		TeamInfo teamInfo = new TeamInfo();
		teamInfo.TeamID = (int)dr["TeamID"];
		teamInfo.Name = (string)dr["Name"];
		teamInfo.Tag = (string)dr["Tag"];
		teamInfo.Division = (int)dr["Division"];
		teamInfo.Grade = (int)dr["Grade"];
		teamInfo.Score = (int)dr["Score"];
		teamInfo.Active = (int)dr["Active"];
		teamInfo.TotalActive = (int)dr["TotalActive"];
		teamInfo.CreateDate = (DateTime)dr["CreateDate"];
		teamInfo.WinTime = (int)dr["WinTime"];
		teamInfo.TotalTime = (int)dr["TotalTime"];
		teamInfo.Rank = (int)dr["Rank"];
		teamInfo.Season = (int)dr["Season"];
		teamInfo.SeasonInfo = (string)dr["SeasonInfo"];
		teamInfo.Member = (int)dr["Member"];
		teamInfo.TotalMember = (int)dr["TotalMember"];
		teamInfo.CreatorID = (int)dr["CreatorID"];
		teamInfo.CreatorName = (string)dr["CreatorName"];
		teamInfo.ChairmanID = (int)dr["ChairmanID"];
		teamInfo.ChairmanName = (string)dr["ChairmanName"];
		teamInfo.IsExits = (bool)dr["IsExits"];
		return teamInfo;
	}

	public TeamUserInfo InitTeamUserInfo(SqlDataReader dr)
	{
		TeamUserInfo teamUserInfo = new TeamUserInfo();
		teamUserInfo.UserID = (int)dr["UserID"];
		teamUserInfo.TeamID = (int)dr["TeamID"];
		teamUserInfo.NickName = (string)dr["NickName"];
		teamUserInfo.type = (int)dr["type"];
		teamUserInfo.activeScore = (int)dr["activeScore"];
		teamUserInfo.weekActiveScore = (int)dr["weekActiveScore"];
		teamUserInfo.totalActiveScore = (int)dr["totalActiveScore"];
		teamUserInfo.seasonActiveScore = (int)dr["seasonActiveScore"];
		teamUserInfo.teamSocre = (int)dr["teamSocre"];
		teamUserInfo.totalTiems = (int)dr["totalTiems"];
		teamUserInfo.SignDate = (DateTime)dr["SignDate"];
		return teamUserInfo;
	}

	public bool AddTeam(TeamInfo info, ref string msg)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[22];
			array[0] = new SqlParameter("@TeamID", info.TeamID);
			array[0].Direction = ParameterDirection.InputOutput;
			array[1] = new SqlParameter("@Name", info.Name);
			array[2] = new SqlParameter("@Tag", info.Tag);
			array[3] = new SqlParameter("@Division", info.Division);
			array[4] = new SqlParameter("@Grade", info.Grade);
			array[5] = new SqlParameter("@Score", info.Score);
			array[6] = new SqlParameter("@Active", info.Active);
			array[7] = new SqlParameter("@TotalActive", info.TotalActive);
			array[8] = new SqlParameter("@CreateDate", info.CreateDate);
			array[9] = new SqlParameter("@WinTime", info.WinTime);
			array[10] = new SqlParameter("@TotalTime", info.TotalTime);
			array[11] = new SqlParameter("@Rank", info.Rank);
			array[12] = new SqlParameter("@Season", info.Season);
			array[13] = new SqlParameter("@SeasonInfo", info.SeasonInfo);
			array[14] = new SqlParameter("@Member", info.Member);
			array[15] = new SqlParameter("@TotalMember", info.TotalMember);
			array[16] = new SqlParameter("@CreatorID", info.CreatorID);
			array[17] = new SqlParameter("@CreatorName", info.CreatorName);
			array[18] = new SqlParameter("@ChairmanID", info.ChairmanID);
			array[19] = new SqlParameter("@ChairmanName", info.ChairmanName);
			array[20] = new SqlParameter("@IsExits", info.IsExits);
			array[21] = new SqlParameter("@Result", SqlDbType.Int);
			array[21].Direction = ParameterDirection.ReturnValue;
			result = db.RunProcedure("SP_Team_Add", array);
			int num = (int)array[21].Value;
			result = num == 0;
			switch (num)
			{
			case 2:
				msg = "GameServer.Team.Name.IsExits";
				break;
			case 3:
				msg = "GameServer.Team.Tag.IsExits";
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

	public bool AddTeamUser(TeamUserInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[11]
			{
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@TeamID", info.TeamID),
				new SqlParameter("@type", info.type),
				new SqlParameter("@activeScore", info.activeScore),
				new SqlParameter("@weekActiveScore", info.weekActiveScore),
				new SqlParameter("@totalActiveScore", info.totalActiveScore),
				new SqlParameter("@seasonActiveScore", info.seasonActiveScore),
				new SqlParameter("@teamSocre", info.teamSocre),
				new SqlParameter("@totalTiems", info.totalTiems),
				new SqlParameter("@SignDate", info.SignDate),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[10].Direction = ParameterDirection.ReturnValue;
			result = db.RunProcedure("SP_TeamUsers_Add", array);
			int num = (int)array[10].Value;
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

	public TeamInfo[] GetTeamAll()
	{
		List<TeamInfo> list = new List<TeamInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Team_All");
			while (ResultDataReader.Read())
			{
				TeamInfo item = InitTeamInfo(ResultDataReader);
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

	public TeamUserInfo[] GetTeamUsersAll(int TeamID)
	{
		List<TeamUserInfo> list = new List<TeamUserInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@TeamID", TeamID)
			};
			db.GetReader(ref ResultDataReader, "SP_TeamUsers_All", sqlParameters);
			while (ResultDataReader.Read())
			{
				TeamUserInfo item = InitTeamUserInfo(ResultDataReader);
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
}
