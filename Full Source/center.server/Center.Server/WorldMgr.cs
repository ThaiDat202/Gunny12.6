using System;
using System.Collections.Generic;
using System.Reflection;
using Bussiness;
using Center.Server.Managers;
using log4net;
using SqlDataProvider.Data;

namespace Center.Server
{
	public class WorldMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private static object _syncStop = new object();

		private static Dictionary<int, AreaConfigInfo> m_areaConfigs = new Dictionary<int, AreaConfigInfo>();

		private static CityBattleMgr m_cityBattle;

		private static QiYuanControl m_qiYuan;

		public static CityBattleMgr CityBattle => m_cityBattle;

		public static QiYuanControl QiYuan => m_qiYuan;

		public static bool Setup()
		{
			try
			{
				m_areaConfigs.Clear();
				using (AreaBussiness areaBussiness = new AreaBussiness())
				{
					AreaConfigInfo[] allAreaConfig = areaBussiness.GetAllAreaConfig();
					foreach (AreaConfigInfo areaConfigInfo in allAreaConfig)
					{
						if (!m_areaConfigs.ContainsKey(areaConfigInfo.AreaID))
						{
							m_areaConfigs.Add(areaConfigInfo.AreaID, areaConfigInfo);
						}
					}
				}
				m_cityBattle = new CityBattleMgr();
				m_qiYuan = new QiYuanControl();
				return true;
			}
			catch (Exception arg)
			{
				log.ErrorFormat("Load server list from db failed:{0}", arg);
				return false;
			}
		}

		public static AreaConfigInfo FindAreaConfig(int zoneId)
		{
			return m_areaConfigs.ContainsKey(zoneId) ? m_areaConfigs[zoneId] : null;
		}

		public static AreaConfigInfo[] GetAllAreaConfig()
		{
			List<AreaConfigInfo> list = new List<AreaConfigInfo>();
			foreach (AreaConfigInfo value in m_areaConfigs.Values)
			{
				list.Add(value);
			}
			return list.ToArray();
		}
	}
}
