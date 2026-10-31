using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using SqlDataProvider;

namespace Game.Server.Managers;

public class VipIntegrationMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static readonly ServerData ServerData = new ServerData();

	public static List<VipStoreList> VipStoreLists { get; private set; }

	public static bool Init()
	{
		try
		{
			VipStoreLists = ServerData.VipStoreList.ToList();
			return true;
		}
		catch (Exception arg)
		{
			log.ErrorFormat("VipIntegrationMgr error: {0}", arg);
			return false;
		}
	}
}
