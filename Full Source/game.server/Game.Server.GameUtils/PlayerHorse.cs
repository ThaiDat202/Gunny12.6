using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bussiness;
using Game.Logic;
using Game.Server.GameObjects;
using log4net;
using Newtonsoft.Json;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils;

public class PlayerHorse
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	protected object m_lock = new object();

	protected GamePlayer m_player;

	private MountDataInfo m_info;

	private Dictionary<int, MountSkillDataInfo> m_curHasSkill;

	private Dictionary<int, int> m_curUsedSkill;

	private bool m_updateProperties;

	private Dictionary<int, MountDrawDataInfo> m_horsePicCherish;

	private bool m_saveToDb;

	public GamePlayer Player => m_player;

	public MountDataInfo Info
	{
		get
		{
			return m_info;
		}
		set
		{
			m_info = value;
		}
	}

	public Dictionary<int, MountSkillDataInfo> curHasSkill => m_curHasSkill;

	public Dictionary<int, int> curUsedSkill => m_curUsedSkill;

	public bool UpdateProperties
	{
		get
		{
			return m_updateProperties;
		}
		set
		{
			m_updateProperties = value;
		}
	}

	public Dictionary<int, MountDrawDataInfo> horsePicCherish => m_horsePicCherish;

	public MountSkillDataInfo[] GetSkills => curHasSkill.Values.ToArray();

	public int[] GetHorseSkillEquip => m_curUsedSkill.Values.ToArray();

	public MountDrawDataInfo[] GetPicCherishs => horsePicCherish.Values.ToArray();

	public PlayerHorse(GamePlayer player, bool saveTodb)
	{
		m_player = player;
		m_saveToDb = saveTodb;
		m_curHasSkill = new Dictionary<int, MountSkillDataInfo>();
		m_curUsedSkill = new Dictionary<int, int>();
		m_horsePicCherish = new Dictionary<int, MountDrawDataInfo>();
	}

	public virtual void LoadFromDatabase()
	{
		if (!m_saveToDb)
		{
			return;
		}
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		try
		{
			MountDataInfo singleMountData = playerBussiness.GetSingleMountData(m_player.PlayerCharacter.ID);
			if (singleMountData == null)
			{
				CreateHorse();
				return;
			}
			lock (m_lock)
			{
				m_info = singleMountData;
			}
		}
		finally
		{
			DeserializeMount();
		}
	}

	public void UpdateHorsePacket()
	{
		Player.Out.SendHorseInitAllData(Info, GetSkills);
		Player.Out.SendPicCherishInfo(Info.UserID, GetPicCherishs);
	}

	public void TakeUpDownSkill(int place, int skilId)
	{
		if (m_curHasSkill.ContainsKey(skilId))
		{
			m_curHasSkill[skilId].IsUse = place;
			UpdateUsedSkill();
		}
	}

	public void UpSkill(int skilId, MountSkillDataInfo newSkill)
	{
		m_curHasSkill.Remove(skilId);
		if (!m_curHasSkill.ContainsKey(newSkill.SKillID))
		{
			m_curHasSkill.Add(newSkill.SKillID, newSkill);
		}
		m_curUsedSkill.Remove(skilId);
		if (!m_curUsedSkill.ContainsKey(newSkill.IsUse))
		{
			m_curUsedSkill.Add(newSkill.IsUse, newSkill.SKillID);
		}
	}

	public MountSkillDataInfo AddHorseSkill()
	{
		MountTemplateInfo mountTemplate = MountMgr.GetMountTemplate(Info.curLevel);
		if (mountTemplate != null && mountTemplate.SkillID > 0)
		{
			int skillID = mountTemplate.SkillID;
			MountSkillTemplateInfo mountSkillTemplate = MountMgr.GetMountSkillTemplate(skillID);
			if (mountSkillTemplate != null)
			{
				MountSkillDataInfo mountSkillDataInfo = new MountSkillDataInfo();
				mountSkillDataInfo.SKillID = skillID;
				mountSkillDataInfo.Exp = 0;
				mountSkillDataInfo.IsUse = 0;
				if (!m_curHasSkill.ContainsKey(skillID))
				{
					m_curHasSkill.Add(skillID, mountSkillDataInfo);
					return mountSkillDataInfo;
				}
			}
		}
		return null;
	}

	public void CreateHorse()
	{
		MountDataInfo mountDataInfo = new MountDataInfo();
		MountTemplateInfo mountTemplate = MountMgr.GetMountTemplate(0);
		if (mountTemplate != null)
		{
			mountDataInfo.UserID = Player.PlayerCharacter.ID;
			mountDataInfo.curLevel = mountTemplate.Grade;
			mountDataInfo.curExp = mountTemplate.Experience;
			mountDataInfo.curHasSkill = "";
		}
		else
		{
			mountDataInfo = null;
			Console.WriteLine("MountTemplateInfo not found item!");
		}
		lock (m_lock)
		{
			m_info = mountDataInfo;
		}
	}

	public void UpdateUsedSkill()
	{
		m_curUsedSkill.Clear();
		foreach (MountSkillDataInfo value in m_curHasSkill.Values)
		{
			if (value.IsUse > 0 && !m_curUsedSkill.ContainsKey(value.IsUse))
			{
				m_curUsedSkill.Add(value.IsUse, value.SKillID);
			}
		}
	}

	public void ActiveHorsePicCherish(int mountId)
	{
		if (!m_horsePicCherish.ContainsKey(mountId) && mountId > 100)
		{
			MountDrawDataInfo mountDrawDataInfo = new MountDrawDataInfo();
			mountDrawDataInfo.ID = mountId;
			mountDrawDataInfo.Active = 0;
			m_horsePicCherish.Add(mountId, mountDrawDataInfo);
			Player.EquipBag.UpdatePlayerProperties();
		}
		foreach (int key in horsePicCherish.Keys)
		{
			m_horsePicCherish[key].Active = 0;
		}
		if (m_horsePicCherish.ContainsKey(mountId))
		{
			m_horsePicCherish[mountId].Active = 1;
		}
	}

	private void OldDeseriallize()
	{
		string[] array = Info.curHasSkill.Split('|');
		string[] array2 = array;
		foreach (string text in array2)
		{
			int num = int.Parse(text.Split(',')[0]);
			MountSkillTemplateInfo mountSkillTemplate = MountMgr.GetMountSkillTemplate(num);
			if (mountSkillTemplate != null)
			{
				MountSkillDataInfo mountSkillDataInfo = new MountSkillDataInfo();
				mountSkillDataInfo.SKillID = num;
				mountSkillDataInfo.Exp = int.Parse(text.Split(',')[1]);
				int num3 = (mountSkillDataInfo.IsUse = int.Parse(text.Split(',')[2]));
				if (!m_curHasSkill.ContainsKey(num))
				{
					m_curHasSkill.Add(num, mountSkillDataInfo);
				}
				if (num3 > 0 && !m_curUsedSkill.ContainsKey(num3))
				{
					m_curUsedSkill.Add(num3, num);
				}
			}
		}
	}

	public void DeserializeMount()
	{
		try
		{
			if (Info != null && !string.IsNullOrEmpty(Info.horsePicCherish))
			{
				m_horsePicCherish = JsonConvert.DeserializeObject<Dictionary<int, MountDrawDataInfo>>(Info.horsePicCherish);
			}
			if (Info == null || string.IsNullOrEmpty(Info.curHasSkill))
			{
				return;
			}
			if (Info.curHasSkill.Contains("|"))
			{
				OldDeseriallize();
				return;
			}
			try
			{
				m_curHasSkill = JsonConvert.DeserializeObject<Dictionary<int, MountSkillDataInfo>>(Info.curHasSkill);
				foreach (MountSkillDataInfo value in curHasSkill.Values)
				{
					int isUse = value.IsUse;
					if (isUse > 0 && !m_curUsedSkill.ContainsKey(isUse))
					{
						m_curUsedSkill.Add(isUse, value.SKillID);
					}
				}
			}
			catch
			{
				OldDeseriallize();
			}
		}
		catch (Exception exception)
		{
			log.Error("DeserializeMount fail: ", exception);
		}
	}

	public void SerializeMount()
	{
		try
		{
			Info.curHasSkill = JsonConvert.SerializeObject(curHasSkill);
			Info.horsePicCherish = JsonConvert.SerializeObject(horsePicCherish);
		}
		catch (Exception exception)
		{
			log.Error("SerializeMount fail: ", exception);
		}
	}

	public virtual void SaveToDatabase()
	{
		if (!m_saveToDb)
		{
			return;
		}
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		lock (m_lock)
		{
			if (Info != null && Info.IsDirty)
			{
				SerializeMount();
				if (Info.ID > 0)
				{
					playerBussiness.UpdateMountData(Info);
				}
				else
				{
					playerBussiness.AddMountDatas(Info);
				}
			}
		}
	}
}
