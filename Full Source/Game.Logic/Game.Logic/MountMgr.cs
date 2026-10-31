using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using EntityDatabase.ServerModels;
using log4net;
using SqlDataProvider.Data;

namespace Game.Logic;

public class MountMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, MountSkillElementTemplateInfo> _mountSkillElementTemplates;

	private static Dictionary<int, MountSkillGetTemplateInfo> _mountSkillGetTemplates;

	private static Dictionary<int, MountSkillTemplateInfo> _mountSkillTemplates;

	private static Dictionary<int, MountTemplateInfo> _mountTemplates;

	private static Dictionary<int, MountDrawTemplateInfo> m_mountDrawTemplates;

	private static Dictionary<int, MountTalismansInfo> m_mountTalismanss = new Dictionary<int, MountTalismansInfo>();

	private static Random rand;

	private static ReaderWriterLock m_clientLocker = new ReaderWriterLock();

	public static bool Init()
	{
		try
		{
			rand = new Random();
			return ReLoad();
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("PetInfoMgr", exception);
			}
			return false;
		}
	}

	public static int[] calculateRate(MountDrawDataInfo mountDrawDataInfo)
	{
		int[] array = new int[5];
		MountDrawTemplateInfo mountDrawTemplateInfo = FindMountDrawInfo(mountDrawDataInfo.ID);
		if (mountDrawDataInfo.qiLingGrade > 0)
		{
			TS_UpgradeTemplate upgradeSublimarImagemMontaria = ItemMgr.getUpgradeSublimarImagemMontaria(mountDrawDataInfo.qiLingGrade);
			array[0] = (int)((double)Convert.ToSingle(mountDrawTemplateInfo.AddHurt) + ((double)(Convert.ToSingle(mountDrawTemplateInfo.AddHurt) * Convert.ToSingle(upgradeSublimarImagemMontaria.Param2)) / 1000.0 + 0.5));
			array[1] = (int)((double)Convert.ToSingle(mountDrawTemplateInfo.AddGuard) + ((double)(Convert.ToSingle(mountDrawTemplateInfo.AddGuard) * Convert.ToSingle(upgradeSublimarImagemMontaria.Param2)) / 1000.0 + 0.5));
			array[2] = (int)((double)Convert.ToSingle(mountDrawTemplateInfo.AddBlood) + ((double)(Convert.ToSingle(mountDrawTemplateInfo.AddBlood) * Convert.ToSingle(upgradeSublimarImagemMontaria.Param2)) / 1000.0 + 0.5));
			array[3] = (int)((double)Convert.ToSingle(mountDrawTemplateInfo.MagicAttack) + ((double)(Convert.ToSingle(mountDrawTemplateInfo.MagicAttack) * Convert.ToSingle(upgradeSublimarImagemMontaria.Param2)) / 1000.0 + 0.5));
			array[4] = (int)((double)Convert.ToSingle(mountDrawTemplateInfo.MagicDefence) + ((double)(Convert.ToSingle(mountDrawTemplateInfo.MagicDefence) * Convert.ToSingle(upgradeSublimarImagemMontaria.Param2)) / 1000.0 + 0.5));
		}
		else
		{
			array[0] = mountDrawTemplateInfo.AddHurt;
			array[1] = mountDrawTemplateInfo.AddGuard;
			array[2] = mountDrawTemplateInfo.AddBlood;
			array[3] = mountDrawTemplateInfo.MagicAttack;
			array[4] = mountDrawTemplateInfo.MagicDefence;
		}
		return array;
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, MountSkillElementTemplateInfo> dictionary = new Dictionary<int, MountSkillElementTemplateInfo>();
			Dictionary<int, MountSkillGetTemplateInfo> dictionary2 = new Dictionary<int, MountSkillGetTemplateInfo>();
			Dictionary<int, MountSkillTemplateInfo> dictionary3 = new Dictionary<int, MountSkillTemplateInfo>();
			Dictionary<int, MountTemplateInfo> dictionary4 = new Dictionary<int, MountTemplateInfo>();
			MountDrawTemplateInfo[] array = LoadMountDrawTemplateDb();
			Dictionary<int, MountDrawTemplateInfo> value = LoadMountDrawTemplates(array);
			if (array.Length != 0)
			{
				Interlocked.Exchange(ref m_mountDrawTemplates, value);
			}
			Dictionary<int, MountTalismansInfo> dictionary5 = LoadMountTalismansFromDatabase();
			if (dictionary5.Values.Count > 0)
			{
				Interlocked.Exchange(ref m_mountTalismanss, dictionary5);
			}
			if (LoadPetMgr(dictionary, dictionary2, dictionary3, dictionary4))
			{
				try
				{
					_mountSkillElementTemplates = dictionary;
					_mountSkillGetTemplates = dictionary2;
					_mountSkillTemplates = dictionary3;
					_mountTemplates = dictionary4;
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
				log.Error("PetMgr", exception);
			}
		}
		return false;
	}

	private static Dictionary<int, MountTalismansInfo> LoadMountTalismansFromDatabase()
	{
		Dictionary<int, MountTalismansInfo> dictionary = new Dictionary<int, MountTalismansInfo>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			MountTalismansInfo[] allMountTalismans = produceBussiness.GetAllMountTalismans();
			MountTalismansInfo[] array = allMountTalismans;
			foreach (MountTalismansInfo mountTalismansInfo in array)
			{
				if (!dictionary.ContainsKey(mountTalismansInfo.ID))
				{
					dictionary.Add(mountTalismansInfo.ID, mountTalismansInfo);
				}
			}
		}
		return dictionary;
	}

	public static MountTalismansInfo FindMountTalismans(int id)
	{
		if (m_mountTalismanss.ContainsKey(id))
		{
			return m_mountTalismanss[id];
		}
		return new MountTalismansInfo();
	}

	public static MountDrawTemplateInfo[] LoadMountDrawTemplateDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetAllMountDrawTemplate();
	}

	public static Dictionary<int, MountDrawTemplateInfo> LoadMountDrawTemplates(MountDrawTemplateInfo[] MountDrawTemplate)
	{
		Dictionary<int, MountDrawTemplateInfo> dictionary = new Dictionary<int, MountDrawTemplateInfo>();
		foreach (MountDrawTemplateInfo mountDrawTemplateInfo in MountDrawTemplate)
		{
			if (!dictionary.Keys.Contains(mountDrawTemplateInfo.ID))
			{
				dictionary.Add(mountDrawTemplateInfo.ID, mountDrawTemplateInfo);
			}
		}
		return dictionary;
	}

	public static MountDrawTemplateInfo FindMountDrawInfo(int ID)
	{
		m_clientLocker.AcquireWriterLock(-1);
		try
		{
			if (m_mountDrawTemplates.ContainsKey(ID))
			{
				return m_mountDrawTemplates[ID];
			}
		}
		finally
		{
			m_clientLocker.ReleaseWriterLock();
		}
		return null;
	}

	public static MountDrawTemplateInfo FindMountDrawByTemplate(int templateID)
	{
		foreach (MountDrawTemplateInfo value in m_mountDrawTemplates.Values)
		{
			if (value.TemplateId == templateID)
			{
				return value;
			}
		}
		return null;
	}

	private static bool LoadPetMgr(Dictionary<int, MountSkillElementTemplateInfo> SkillElement, Dictionary<int, MountSkillGetTemplateInfo> Skill, Dictionary<int, MountSkillTemplateInfo> SkillTemplate, Dictionary<int, MountTemplateInfo> TemplateId)
	{
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			MountSkillElementTemplateInfo[] allMountSkillElementTemplateInfo = produceBussiness.GetAllMountSkillElementTemplateInfo();
			MountSkillGetTemplateInfo[] allMountSkillGetTemplateInfo = produceBussiness.GetAllMountSkillGetTemplateInfo();
			MountSkillTemplateInfo[] allMountSkillTemplateInfo = produceBussiness.GetAllMountSkillTemplateInfo();
			MountTemplateInfo[] allMountTemplateInfo = produceBussiness.GetAllMountTemplateInfo();
			MountSkillElementTemplateInfo[] array = allMountSkillElementTemplateInfo;
			foreach (MountSkillElementTemplateInfo mountSkillElementTemplateInfo in array)
			{
				if (!SkillElement.ContainsKey(mountSkillElementTemplateInfo.ID))
				{
					SkillElement.Add(mountSkillElementTemplateInfo.ID, mountSkillElementTemplateInfo);
				}
			}
			MountSkillGetTemplateInfo[] array2 = allMountSkillGetTemplateInfo;
			foreach (MountSkillGetTemplateInfo mountSkillGetTemplateInfo in array2)
			{
				if (!Skill.ContainsKey(mountSkillGetTemplateInfo.ID))
				{
					Skill.Add(mountSkillGetTemplateInfo.ID, mountSkillGetTemplateInfo);
				}
			}
			MountSkillTemplateInfo[] array3 = allMountSkillTemplateInfo;
			foreach (MountSkillTemplateInfo mountSkillTemplateInfo in array3)
			{
				if (!SkillTemplate.ContainsKey(mountSkillTemplateInfo.ID))
				{
					SkillTemplate.Add(mountSkillTemplateInfo.ID, mountSkillTemplateInfo);
				}
			}
			MountTemplateInfo[] array4 = allMountTemplateInfo;
			foreach (MountTemplateInfo mountTemplateInfo in array4)
			{
				if (!TemplateId.ContainsKey(mountTemplateInfo.Grade))
				{
					TemplateId.Add(mountTemplateInfo.Grade, mountTemplateInfo);
				}
			}
		}
		return true;
	}

	public static MountSkillElementTemplateInfo FindMountSkillElement(int SkillID)
	{
		if (_mountSkillElementTemplates == null)
		{
			Init();
		}
		if (_mountSkillElementTemplates.ContainsKey(SkillID))
		{
			return _mountSkillElementTemplates[SkillID];
		}
		return null;
	}

	public static List<MountSkillElementTemplateInfo> GameNeedMountSkill()
	{
		if (_mountSkillElementTemplates == null)
		{
			Init();
		}
		List<MountSkillElementTemplateInfo> list = new List<MountSkillElementTemplateInfo>();
		Dictionary<string, MountSkillElementTemplateInfo> dictionary = new Dictionary<string, MountSkillElementTemplateInfo>();
		foreach (MountSkillElementTemplateInfo value in _mountSkillElementTemplates.Values)
		{
			if (!dictionary.Keys.Contains(value.EffectPic) && !string.IsNullOrEmpty(value.EffectPic))
			{
				list.Add(value);
				dictionary.Add(value.EffectPic, value);
			}
		}
		dictionary = null;
		return list;
	}

	public static MountSkillGetTemplateInfo FindMountSkillGetTemplate(int SkillID)
	{
		if (_mountSkillGetTemplates == null)
		{
			Init();
		}
		foreach (MountSkillGetTemplateInfo value in _mountSkillGetTemplates.Values)
		{
			if (value.SkillID == SkillID)
			{
				return value;
			}
		}
		return null;
	}

	public static MountSkillGetTemplateInfo[] GetSkillGetTemplates(int SkillID)
	{
		MountSkillGetTemplateInfo info = FindMountSkillGetTemplate(SkillID);
		if (info == null)
		{
			return null;
		}
		IEnumerable<MountSkillGetTemplateInfo> source = _mountSkillGetTemplates.Values.Where((MountSkillGetTemplateInfo s) => s.Type == info.Type);
		return source.ToArray();
	}

	public static MountSkillTemplateInfo GetMountSkillTemplate(int ID)
	{
		if (_mountSkillTemplates == null)
		{
			Init();
		}
		if (_mountSkillTemplates.ContainsKey(ID))
		{
			return _mountSkillTemplates[ID];
		}
		return null;
	}

	public static MountTemplateInfo GetMountTemplate(int Grade)
	{
		m_clientLocker.AcquireWriterLock(-1);
		try
		{
			if (_mountTemplates.ContainsKey(Grade))
			{
				return _mountTemplates[Grade];
			}
		}
		finally
		{
			m_clientLocker.ReleaseWriterLock();
		}
		return null;
	}
}
