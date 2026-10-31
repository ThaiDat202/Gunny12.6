using Game.Logic.PetSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.ElementSkills.ContinueSkills;

public class ElementFlamingGrillingEquip : AbstractPetEffect
{
	private int m_value = 0;

	private int m_skillid = 0;

	private int m_count = 3;

	private Living m_liv;

	public ElementFlamingGrillingEquip(int skillid, int value, Living liv)
		: base(ePetEffectType.ElementFlamingGrillingEquip, "")
	{
		m_skillid = skillid;
		m_value = value;
		m_liv = liv;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.ElementFlamingGrillingEquip) is ElementFlamingGrillingEquip elementFlamingGrillingEquip)
		{
			elementFlamingGrillingEquip.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	public override void OnAttached(Living living)
	{
		living.BeginSelfTurn += player_BeginSelfTurn;
	}

	public override void OnRemoved(Living living)
	{
		living.BeginSelfTurn -= player_BeginSelfTurn;
	}

	private void player_BeginSelfTurn(Living living)
	{
		if (m_count > 0)
		{
			int num = m_value;
			if ((living as Player).PlayerDetail.PlayerCharacter.EmblemInfoDefendSKill == 821)
			{
				num -= (int)(living.FireDefence * 0.8);
			}
			living.SyncAtTime = true;
			living.AddBlood(-num, 1);
			living.SyncAtTime = false;
			if (living.Blood <= 0)
			{
				living.Die();
				if (m_liv != null && m_liv is Player)
				{
					int type = 2;
					if (living is Player)
					{
						type = 1;
					}
					(m_liv as Player).PlayerDetail.OnKillingLiving(m_liv.Game, type, living.Id, living.IsLiving, num);
				}
			}
			m_count--;
		}
		else
		{
			living.EffectBadges = false;
			Stop();
		}
	}
}
