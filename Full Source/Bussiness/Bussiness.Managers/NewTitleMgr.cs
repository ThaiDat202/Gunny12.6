using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class NewTitleMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, NewTitleInfo> m_NewTitles = new Dictionary<int, NewTitleInfo>();

	private static Random random = new Random();

	private static ReaderWriterLock m_clientLocker = new ReaderWriterLock();

	public static bool ReLoad()
	{
		try
		{
			NewTitleInfo[] array = LoadNewTitleDb();
			Dictionary<int, NewTitleInfo> value = LoadNewTitles(array);
			if (array.Length != 0)
			{
				Interlocked.Exchange(ref m_NewTitles, value);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ReLoad NewTitle", exception);
			}
			return false;
		}
		return true;
	}

	public static bool Init()
	{
		return ReLoad();
	}

	public static NewTitleInfo[] LoadNewTitleDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetAllNewTitle();
	}

	public static Dictionary<int, NewTitleInfo> LoadNewTitles(NewTitleInfo[] NewTitle)
	{
		Dictionary<int, NewTitleInfo> dictionary = new Dictionary<int, NewTitleInfo>();
		foreach (NewTitleInfo newTitleInfo in NewTitle)
		{
			if (!dictionary.Keys.Contains(newTitleInfo.ID))
			{
				dictionary.Add(newTitleInfo.ID, newTitleInfo);
			}
		}
		return dictionary;
	}

	public static NewTitleInfo FindNewTitle(int ID)
	{
		m_clientLocker.AcquireWriterLock(-1);
		try
		{
			if (m_NewTitles.ContainsKey(ID))
			{
				return m_NewTitles[ID];
			}
		}
		finally
		{
			m_clientLocker.ReleaseWriterLock();
		}
		return null;
	}

	public static NewTitleInfo FindNewTitleByName(string Name)
	{
		m_clientLocker.AcquireWriterLock(-1);
		try
		{
			foreach (NewTitleInfo value in m_NewTitles.Values)
			{
				if (value.Name.ToLower() == Name.ToLower())
				{
					return value;
				}
			}
		}
		finally
		{
			m_clientLocker.ReleaseWriterLock();
		}
		return null;
	}
}
