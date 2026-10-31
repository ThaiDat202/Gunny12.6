using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class TotemMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, TotemInfo> _totem;

	private static Random rand;

	private static ReaderWriterLock m_clientLocker = new ReaderWriterLock();

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, TotemInfo> totem = new Dictionary<int, TotemInfo>();
			if (Load(totem))
			{
				try
				{
					_totem = totem;
					return true;
				}
				catch
				{
				}
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("TotemMgr", exception);
			}
		}
		return false;
	}

	public static bool Init()
	{
		try
		{
			_totem = new Dictionary<int, TotemInfo>();
			rand = new Random();
			return Load(_totem);
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("TotemMgr", exception);
			}
			return false;
		}
	}

	private static bool Load(Dictionary<int, TotemInfo> totem)
	{
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			TotemInfo[] allTotem = produceBussiness.GetAllTotem();
			TotemInfo[] array = allTotem;
			foreach (TotemInfo totemInfo in array)
			{
				if (!totem.ContainsKey(totemInfo.ID))
				{
					totem.Add(totemInfo.ID, totemInfo);
				}
			}
		}
		return true;
	}

	public static int MaxTotem()
	{
		return 10350;
	}

	public static TotemInfo FindTotemInfo(int id)
	{
		m_clientLocker.AcquireWriterLock(-1);
		try
		{
			if (_totem.ContainsKey(id))
			{
				return _totem[id];
			}
		}
		finally
		{
			m_clientLocker.ReleaseWriterLock();
		}
		return null;
	}

	public static int GetTotemProp(int id, string typeOf, double ratePlus = 0.0)
	{
		int num = 0;
		for (int i = 10001; i <= id; i++)
		{
			TotemInfo totemInfo = FindTotemInfo(i);
			switch (typeOf)
			{
			case "att":
				num += totemInfo.AddAttack;
				break;
			case "agi":
				num += totemInfo.AddAgility;
				break;
			case "def":
				num += totemInfo.AddDefence;
				break;
			case "luc":
				num += totemInfo.AddLuck;
				break;
			case "blo":
				num += totemInfo.AddBlood;
				break;
			case "dam":
				num += totemInfo.AddDamage;
				break;
			case "gua":
				num += totemInfo.AddGuard;
				break;
			}
		}
		return num + (int)((double)num * (ratePlus / 100.0));
	}
}
