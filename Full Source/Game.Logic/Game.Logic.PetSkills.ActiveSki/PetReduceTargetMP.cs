using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetReduceTargetMP : BasePetEffect
{
	private int m_probability = 0;

	private int m_currentId;

	private int m_value = 0;

	public PetReduceTargetMP(int probability, int skillId, string elementID)
		: base(ePetEffectType.PetReduceTargetMP, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
		if (!(elementID == "1266"))
		{
			if (elementID == "1267")
			{
				m_value = 20;
			}
		}
		else
		{
			m_value = 10;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetReduceTargetMP) is PetReduceTargetMP petReduceTargetMP)
		{
			petReduceTargetMP.m_probability = ((m_probability > petReduceTargetMP.m_probability) ? m_probability : petReduceTargetMP.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.AfterKillingLiving += player_AfterKillingLiving;
		player.PlayerBuffSkillPet += Player_PlayerBuffSkillPet;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.AfterKillingLiving -= player_AfterKillingLiving;
		player.PlayerBuffSkillPet -= Player_PlayerBuffSkillPet;
	}

	private void Player_PlayerBuffSkillPet(Player player)
	{
		if (player.PetEffects.CurrentUseSkill == m_currentId && player.Game is PVPGame)
		{
			IsTrigger = true;
		}
	}

	private void player_AfterKillingLiving(Living living, Living target, int damageAmount, int criticalAmount)
	{
		if (IsTrigger)
		{
			target.Game.sendShowPicSkil(target, base.ElementInfo, isActive: true);
			if ((target as Player).PetMP < m_value)
			{
				m_value = (target as Player).PetMP;
			}
			(target as Player).PetMP -= m_value;
			IsTrigger = false;
			target.AddPetEffect(new PetReduceTargetMPEquip(m_probability, base.ElementInfo.ID.ToString()), 0);
		}
	}
}
