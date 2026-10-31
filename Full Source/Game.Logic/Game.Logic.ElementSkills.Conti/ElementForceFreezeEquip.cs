using Game.Logic.PetSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.ElementSkills.ContinueSkills;

public class ElementForceFreezeEquip : AbstractPetEffect
{
	private int m_value = 0;

	private int m_skillid = 0;

	private int m_count = 2;

	public ElementForceFreezeEquip(int skillid, int value)
		: base(ePetEffectType.ElementForceFreezeEquip, "")
	{
		m_skillid = skillid;
		m_value = value;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.ElementForceFreezeEquip) is ElementForceFreezeEquip elementForceFreezeEquip)
		{
			elementForceFreezeEquip.m_count += m_count;
			return true;
		}
		return base.Start(living);
	}

	public override void OnAttached(Living living)
	{
		living.BeginNextTurn += player_BeginSelfTurn;
	}

	public override void OnRemoved(Living living)
	{
		living.BeginNextTurn -= player_BeginSelfTurn;
	}

	private void player_BeginSelfTurn(Living living)
	{
		if (m_count > 0)
		{
			living.SyncAtTime = true;
			if (living.CanFrozen)
			{
				living.IsFrost = true;
				(living as Player).AddDelay(500);
			}
			living.SyncAtTime = false;
			m_count--;
		}
		else
		{
			living.EffectBadges = false;
			living.SyncAtTime = true;
			living.IsFrost = false;
			living.SyncAtTime = false;
			Stop();
		}
	}
}
