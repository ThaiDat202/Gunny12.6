using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class MagicItemTemplateMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, MagicItemTemplateInfo> m_MagicItemTemplates = new Dictionary<int, MagicItemTemplateInfo>();

	private static Random random = new Random();

	private static ReaderWriterLock m_clientLocker = new ReaderWriterLock();

	public static bool ReLoad()
	{
		try
		{
			MagicItemTemplateInfo[] array = LoadMagicItemTemplateDb();
			Dictionary<int, MagicItemTemplateInfo> value = LoadMagicItemTemplates(array);
			if (array.Length != 0)
			{
				Interlocked.Exchange(ref m_MagicItemTemplates, value);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ReLoad MagicItemTemplate", exception);
			}
			return false;
		}
		return true;
	}

	public static bool Init()
	{
		return ReLoad();
	}

	public static MagicItemTemplateInfo[] LoadMagicItemTemplateDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetAllMagicItemTemplate();
	}

	public static Dictionary<int, MagicItemTemplateInfo> LoadMagicItemTemplates(MagicItemTemplateInfo[] MagicItemTemplate)
	{
		Dictionary<int, MagicItemTemplateInfo> dictionary = new Dictionary<int, MagicItemTemplateInfo>();
		foreach (MagicItemTemplateInfo magicItemTemplateInfo in MagicItemTemplate)
		{
			if (!dictionary.Keys.Contains(magicItemTemplateInfo.Lv))
			{
				dictionary.Add(magicItemTemplateInfo.Lv, magicItemTemplateInfo);
			}
		}
		return dictionary;
	}

	public static MagicItemTemplateInfo FindMagicItemTemplate(int ID)
	{
		m_clientLocker.AcquireWriterLock(-1);
		try
		{
			if (m_MagicItemTemplates.ContainsKey(ID))
			{
				return m_MagicItemTemplates[ID];
			}
		}
		finally
		{
			m_clientLocker.ReleaseWriterLock();
		}
		return null;
	}
}
