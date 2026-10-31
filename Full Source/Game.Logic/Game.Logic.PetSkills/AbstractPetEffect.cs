using System;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic.PetSkills;

public abstract class AbstractPetEffect
{
	private ePetEffectType m_type;

	protected Living m_living;

	protected Random rand;

	public bool IsTrigger;

	private PetSkillElementInfo m_info;

	public PetSkillElementInfo Info => m_info;

	public PetSkillElementInfo ElementInfo => m_info;

	public ePetEffectType Type => m_type;

	public int TypeValue => (int)m_type;

	public AbstractPetEffect(ePetEffectType type, string ElementID)
	{
		rand = new Random();
		m_type = type;
		if (ElementID == string.Empty)
		{
			m_info = new PetSkillElementInfo();
			m_info.EffectPic = "";
			m_info.Pic = -1;
			return;
		}
		m_info = PetMgr.FindPetSkillElement(int.Parse(ElementID));
		if (m_info == null)
		{
			m_info = new PetSkillElementInfo();
			m_info.EffectPic = "";
			m_info.Pic = -1;
		}
	}

	public bool CheckPVP(Living living, bool IsPVP)
	{
		if (living.Game is PVEGame && IsPVP)
		{
			return true;
		}
		return true;
	}

	public virtual bool Start(Living living)
	{
		m_living = living;
		return m_living.PetEffectList.Add(this);
	}

	public virtual bool Stop()
	{
		return m_living != null && m_living.PetEffectList.Remove(this);
	}

	public virtual bool Pause()
	{
		if (m_living != null)
		{
			return m_living.PetEffectList.Pause(this);
		}
		return false;
	}

	public virtual void OnAttached(Living living)
	{
	}

	public virtual void OnRemoved(Living living)
	{
	}

	public virtual void OnPaused(Living living)
	{
	}
}
