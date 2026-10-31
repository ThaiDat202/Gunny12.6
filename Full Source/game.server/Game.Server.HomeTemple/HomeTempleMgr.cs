using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EntityDatabase.ServerModels;
using log4net;

namespace Game.Server.HomeTemple;

public static class HomeTempleMgr
{
	public static List<TS_HomeTempAdvance_Template> TS_HomeTempAdvance_Template;

	private static ServerData db = new ServerData();

	private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public static HomeTemplePacketOut Out = new HomeTemplePacketOut();

	public static bool Init()
	{
		try
		{
			TS_HomeTempAdvance_Template = db.TS_HomeTempAdvance_Template.ToList();
			return true;
		}
		catch (Exception arg)
		{
			if (Log.IsErrorEnabled)
			{
				Log.ErrorFormat("HomeTempleMgr.Init error: {0}", arg);
			}
			return false;
		}
	}
}
