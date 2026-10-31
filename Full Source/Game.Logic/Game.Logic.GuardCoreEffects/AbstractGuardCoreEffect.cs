using System;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic.GuardCoreEffects;

public abstract class AbstractGuardCoreEffect
{
	private eGuardCoreEffectType m_type;

	protected Living m_living;

	public Random rand;

	public bool IsTrigger;

	private GuardCoreTemplateInfo m_info;

	public GuardCoreTemplateInfo Info => m_info;

	public eGuardCoreEffectType Type => m_type;

	public int TypeValue => (int)m_type;

	public AbstractGuardCoreEffect(eGuardCoreEffectType type, GuardCoreTemplateInfo info)
	{
		rand = new Random();
		m_type = type;
		m_info = info;
	}

	public virtual bool Start(Living living)
	{
		m_living = living;
		if (m_living.GuardCoreEffectList.Add(this))
		{
			return true;
		}
		return false;
	}

	public virtual bool Stop()
	{
		if (m_living != null)
		{
			return m_living.GuardCoreEffectList.Remove(this);
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
