using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Bussiness;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Managers;

public class JampsManualMgr
{
	private static Dictionary<int, JampsChapterItemList> _capitulos;

	public static Dictionary<int, JampsManualItemList> _jampsManualItemList;

	public static List<JampsUpgradeItemList> _jampsCondictions;

	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static ReaderWriterLock m_lock;

	private static ThreadSafeRandom rand;

	public static int maxLevel = 0;

	public static int randNumber(int min, int max, HashSet<int> exclude)
	{
		IEnumerable<int> source = from i in Enumerable.Range(min, max)
			where !exclude.Contains(i)
			select i;
		int index = rand.Next(0, max - exclude.Count);
		return source.ElementAt(index);
	}

	public static int randNumber(int min, int max)
	{
		return rand.Next(min, max);
	}

	private static List<E> ShuffleList<E>(List<E> inputList)
	{
		List<E> list = new List<E>();
		int num = 0;
		while (inputList.Count > 0)
		{
			num = randNumber(0, inputList.Count);
			list.Add(inputList[num]);
			inputList.RemoveAt(num);
		}
		return list;
	}

	public static JampsChapterItemList getRandomChapter()
	{
		m_lock.AcquireReaderLock(10000);
		try
		{
			List<JampsChapterItemList> list = ShuffleList(_capitulos.Values.ToList());
			return list[randNumber(0, list.Count)];
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("JampsManualMgr: GetAllDebrisByPageID", exception);
			}
		}
		finally
		{
			m_lock.ReleaseReaderLock();
		}
		return null;
	}

	public static JampsUpgradeItemList[] getCondictionsByLevel(int level)
	{
		m_lock.AcquireReaderLock(10000);
		try
		{
			return _jampsCondictions.Where((JampsUpgradeItemList o) => o.ManualLevel == level).ToArray();
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("JampsManualMgr: getCondictionsByLevel", exception);
			}
		}
		finally
		{
			m_lock.ReleaseReaderLock();
		}
		return null;
	}

	public static JampsUpgradeItemList[] getCondictionsByLevelAndType(int level, int type)
	{
		m_lock.AcquireReaderLock(10000);
		try
		{
			return (from o in _jampsCondictions
				where o.ManualLevel == level
				where o.ConditionType == type
				select o).ToArray();
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("JampsManualMgr: getCondictionsByLevel", exception);
			}
		}
		finally
		{
			m_lock.ReleaseReaderLock();
		}
		return null;
	}

	public static JampsChapterItemList getChapter(int id)
	{
		m_lock.AcquireReaderLock(10000);
		try
		{
			if (_capitulos.ContainsKey(id))
			{
				return _capitulos[id];
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("JampsManualMgr: GetAllDebrisByPageID", exception);
			}
		}
		finally
		{
			m_lock.ReleaseReaderLock();
		}
		return null;
	}

	public static JampsPageItemList getRandomPageFromChapter(int ID, int max)
	{
		m_lock.AcquireReaderLock(10000);
		try
		{
			if (_capitulos.ContainsKey(ID))
			{
				List<JampsPageItemList> list = ShuffleList(_capitulos[ID].paginas.Values.ToList());
				int max2 = ((max > list.Count) ? list.Count : max);
				return list[randNumber(0, max2)];
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("JampsManualMgr: GetAllDebrisByPageID", exception);
			}
		}
		finally
		{
			m_lock.ReleaseReaderLock();
		}
		return null;
	}

	public static List<JampsPageItemList> getPageFromChapter(int ID)
	{
		m_lock.AcquireReaderLock(10000);
		try
		{
			List<JampsPageItemList> list = new List<JampsPageItemList>();
			foreach (JampsChapterItemList item in _capitulos.Values.Where((JampsChapterItemList o) => o.ID == ID))
			{
				foreach (JampsPageItemList value in item.paginas.Values)
				{
					list.Add(value);
				}
			}
			return list;
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("JampsManualMgr: GetAllDebrisByPageID", exception);
			}
		}
		finally
		{
			m_lock.ReleaseReaderLock();
		}
		return null;
	}

	public static List<JampsDebrisItemList> getRandomDebrisFromPage(int ID)
	{
		m_lock.AcquireReaderLock(10000);
		List<JampsDebrisItemList> list = new List<JampsDebrisItemList>();
		try
		{
			foreach (JampsChapterItemList value in _capitulos.Values)
			{
				foreach (JampsPageItemList item in value.paginas.Values.Where((JampsPageItemList o) => o.ID == ID))
				{
					foreach (JampsDebrisItemList value2 in item._fragmentos.Values)
					{
						list.Add(value2);
					}
				}
			}
			return list;
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("JampsManualMgr: GetAllDebrisByPageID", exception);
			}
		}
		finally
		{
			m_lock.ReleaseReaderLock();
		}
		return list;
	}

	public static List<JampsDebrisItemList> GetAllDebrisByCapitulo(int capitulo)
	{
		m_lock.AcquireReaderLock(10000);
		List<JampsDebrisItemList> list = new List<JampsDebrisItemList>();
		try
		{
			foreach (JampsPageItemList value in _capitulos[capitulo].paginas.Values)
			{
				foreach (JampsDebrisItemList value2 in value._fragmentos.Values)
				{
					list.Add(value2);
				}
			}
			return list;
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("JampsManualMgr: GetAllDebrisByPageID", exception);
			}
		}
		finally
		{
			m_lock.ReleaseReaderLock();
		}
		return list;
	}

	public static int getChapterIDFromDebrisID(int ID)
	{
		m_lock.AcquireReaderLock(10000);
		List<JampsDebrisItemList> list = new List<JampsDebrisItemList>();
		try
		{
			foreach (JampsChapterItemList value in _capitulos.Values)
			{
				foreach (JampsPageItemList value2 in value.paginas.Values)
				{
					foreach (JampsDebrisItemList value3 in value2._fragmentos.Values)
					{
						if (value3.ID == ID)
						{
							return value.ID;
						}
					}
				}
			}
			return -1;
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("JampsManualMgr: GetAllDebrisByPageID", exception);
			}
		}
		finally
		{
			m_lock.ReleaseReaderLock();
		}
		return -1;
	}

	public static JampsPageItemList getPageFromID(int ID)
	{
		m_lock.AcquireReaderLock(10000);
		List<JampsDebrisItemList> list = new List<JampsDebrisItemList>();
		try
		{
			foreach (JampsChapterItemList value in _capitulos.Values)
			{
				foreach (JampsPageItemList value2 in value.paginas.Values)
				{
					if (value2.ID == ID)
					{
						return value2;
					}
				}
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("JampsManualMgr: GetAllDebrisByPageID", exception);
			}
		}
		finally
		{
			m_lock.ReleaseReaderLock();
		}
		return null;
	}

	public static bool Init()
	{
		try
		{
			m_lock = new ReaderWriterLock();
			_capitulos = new Dictionary<int, JampsChapterItemList>();
			_jampsManualItemList = new Dictionary<int, JampsManualItemList>();
			_jampsCondictions = new List<JampsUpgradeItemList>();
			rand = new ThreadSafeRandom();
			return LoadCapitulos(_capitulos) && LoadJampsManualItemList(_jampsManualItemList) && LoadJampsCondictions(_jampsCondictions);
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("你获得了点数，但这个点数没有用！", exception);
			}
			return false;
		}
	}

	private static bool LoadCapitulos(Dictionary<int, JampsChapterItemList> list)
	{
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			JampsChapterItemList[] jampsChapterItemList = produceBussiness.Get_JampsChapterItemList();
			JampsChapterItemList[] array = jampsChapterItemList;
			foreach (JampsChapterItemList jampsChapterItemList2 in array)
			{
				jampsChapterItemList2.paginas = new Dictionary<int, JampsPageItemList>();
				JampsPageItemList[] array2 = produceBussiness.Get_JampsPageItemList(jampsChapterItemList2.ID);
				JampsPageItemList[] array3 = array2;
				foreach (JampsPageItemList jampsPageItemList in array3)
				{
					jampsPageItemList._fragmentos = new Dictionary<int, JampsDebrisItemList>();
					JampsDebrisItemList[] array4 = produceBussiness.Get_JampsDebrisItemList(jampsPageItemList.ID);
					JampsDebrisItemList[] array5 = array4;
					foreach (JampsDebrisItemList jampsDebrisItemList in array5)
					{
						jampsPageItemList._fragmentos.Add(jampsDebrisItemList.ID, jampsDebrisItemList);
					}
					jampsChapterItemList2.paginas.Add(jampsPageItemList.ID, jampsPageItemList);
				}
				list.Add(jampsChapterItemList2.ID, jampsChapterItemList2);
			}
		}
		return true;
	}

	private static bool LoadJampsManualItemList(Dictionary<int, JampsManualItemList> list)
	{
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			JampsManualItemList[] jampsManualItemList = produceBussiness.Get_JampsManualItemList();
			JampsManualItemList[] array = jampsManualItemList;
			foreach (JampsManualItemList jampsManualItemList2 in array)
			{
				if (jampsManualItemList2.Level > maxLevel)
				{
					maxLevel = jampsManualItemList2.Level;
				}
				if (!list.ContainsKey(jampsManualItemList2.Level))
				{
					list.Add(jampsManualItemList2.Level, jampsManualItemList2);
				}
			}
		}
		return true;
	}

	private static bool LoadJampsCondictions(List<JampsUpgradeItemList> list)
	{
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			JampsUpgradeItemList[] jampsUpgradeItemList = produceBussiness.Get_JampsUpgradeItemList();
			JampsUpgradeItemList[] array = jampsUpgradeItemList;
			foreach (JampsUpgradeItemList item in array)
			{
				list.Add(item);
			}
		}
		return true;
	}

	public static JampsDebrisItemList getRandomDebrisFromPages(List<PagesInfo> activesPage)
	{
		m_lock.AcquireReaderLock(10000);
		List<JampsDebrisItemList> list = new List<JampsDebrisItemList>();
		try
		{
			foreach (JampsChapterItemList value in _capitulos.Values)
			{
				foreach (JampsDebrisItemList item in GetAllDebrisByCapitulo(value.ID))
				{
					if (activesPage.Select((PagesInfo o) => o.pageID).Contains(item.PageID))
					{
						list.Add(item);
					}
				}
			}
			return ShuffleList(list)[randNumber(0, list.Count)];
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("JampsManualMgr: GetAllDebrisByPageID", exception);
			}
		}
		finally
		{
			m_lock.ReleaseReaderLock();
		}
		return null;
	}

	public static JampsDebrisItemList getRandomDebrisFromPages(List<PagesInfo> activesPage, int chapter)
	{
		m_lock.AcquireReaderLock(10000);
		List<JampsDebrisItemList> list = new List<JampsDebrisItemList>();
		try
		{
			foreach (JampsChapterItemList item in _capitulos.Values.Where((JampsChapterItemList o) => o.ID == chapter))
			{
				foreach (JampsDebrisItemList item2 in GetAllDebrisByCapitulo(item.ID))
				{
					if (activesPage.Select((PagesInfo o) => o.pageID).Contains(item2.PageID))
					{
						list.Add(item2);
					}
				}
			}
			int num = randNumber(0, list.Count);
			return ShuffleList(list)[(num >= 0) ? ((num > list.Count) ? list.Count : num) : 0];
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("JampsManualMgr: GetAllDebrisByPageID", exception);
			}
		}
		finally
		{
			m_lock.ReleaseReaderLock();
		}
		return null;
	}
}
