using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class ServerDataMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<string, ServerDataInfo> m_serverData;

	private static object m_lock = new object();

	public static bool ReLoad()
	{
		try
		{
			Dictionary<string, ServerDataInfo> dictionary = LoadServerDataDb();
			if (dictionary != null)
			{
				Interlocked.Exchange(ref m_serverData, dictionary);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ReLoad", exception);
			}
			return false;
		}
		return true;
	}

	public static bool Init()
	{
		return ReLoad();
	}

	public static Dictionary<string, ServerDataInfo> LoadServerDataDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetAllServerData();
	}

	public static ServerDataInfo Find(string name)
	{
		lock (m_lock)
		{
			if (m_serverData.ContainsKey(name))
			{
				return m_serverData[name];
			}
			return null;
		}
	}

	public static void UpdateServerData(ServerDataInfo info)
	{
		lock (m_lock)
		{
			if (!m_serverData.ContainsKey(info.Name))
			{
				m_serverData.Add(info.Name, info);
			}
			else
			{
				m_serverData[info.Name] = info;
			}
		}
	}

	public static void Update(string name, string valString)
	{
		ServerDataInfo serverDataInfo = new ServerDataInfo();
		serverDataInfo.Name = name;
		serverDataInfo.ValString = valString;
		UpdateServerData(serverDataInfo);
	}

	public static void Update(string name, DateTime valdate)
	{
		ServerDataInfo serverDataInfo = new ServerDataInfo();
		serverDataInfo.Name = name;
		serverDataInfo.ValString = "";
		serverDataInfo.ValDateTime = valdate;
		UpdateServerData(serverDataInfo);
	}

	public static void Update(string name, int valInt)
	{
		ServerDataInfo serverDataInfo = new ServerDataInfo();
		serverDataInfo.Name = name;
		serverDataInfo.ValInt = valInt;
		UpdateServerData(serverDataInfo);
	}

	public static void SaveServerData()
	{
		lock (m_lock)
		{
			using ProduceBussiness produceBussiness = new ProduceBussiness();
			foreach (ServerDataInfo value in m_serverData.Values)
			{
				if (value.IsDirty)
				{
					produceBussiness.AddServerData(value);
				}
			}
		}
	}
}
