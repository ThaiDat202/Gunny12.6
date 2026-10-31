using System;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic.HorseEffects;

public class AbstractHorseEffect
{
	private eHorseEffectType m_type;

	protected Living m_living;

	public Random rand;

	public bool IsTrigger;

	private MountSkillElementTemplateInfo m_info;

	public MountSkillElementTemplateInfo Info => m_info;

	public eHorseEffectType Type => m_type;

	public int TypeValue => (int)m_type;

	public AbstractHorseEffect(eHorseEffectType type, string ElementID)
	{
		rand = new Random();
		m_type = type;
		m_info = MountMgr.FindMountSkillElement(int.Parse(ElementID));
		if (m_info == null)
		{
			m_info = new MountSkillElementTemplateInfo();
			m_info.EffectPic = "";
			m_info.Pic = -1;
		}
	}

	public virtual bool Start(Living living)
	{
		m_living = living;
		if (m_living.HorseEffectList.Add(this))
		{
			return true;
		}
		return false;
	}

	public virtual bool Stop()
	{
		if (m_living != null)
		{
			return m_living.HorseEffectList.Remove(this);
		}
		return false;
	}

	public virtual void OnAttached(Living living)
	{
	}

	public virtual void OnRemoved(Living living)
	{
	}
}
