using Game.Logic.PetSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.ElementSkills.ContinueSkills;

public class ElementLightningRadiationEquip : AbstractPetEffect
{
	private int m_value = 0;

	private int m_skillid = 0;

	private int m_count = 3;

	public ElementLightningRadiationEquip(int skillid, int value)
		: base(ePetEffectType.ElementLightningRadiationEquip, "")
	{
		m_skillid = skillid;
		m_value = value;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.ElementLightningRadiationEquip) is ElementLightningRadiationEquip elementLightningRadiationEquip)
		{
			elementLightningRadiationEquip.m_count += m_count;
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
			if ((living as Player).PlayerDetail.PlayerCharacter.EmblemInfoDefendSKill == 822)
			{
				living.AddBlood((living as Player).PlayerDetail.PlayerCharacter.EmblemInfoDefendSKill, 0, 2);
			}
			living.SyncAtTime = true;
			if (living.CanFrozen)
			{
				living.IsFrost = true;
			}
			living.SyncAtTime = false;
			m_count--;
		}
		else
		{
			living.EffectBadges = false;
			living.IsFrost = false;
			Stop();
		}
	}
}
