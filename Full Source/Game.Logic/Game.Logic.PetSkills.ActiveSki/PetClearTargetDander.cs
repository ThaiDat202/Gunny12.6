using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetClearTargetDander : BasePetEffect
{
	private int m_probability = 0;

	private int m_currentId;

	public PetClearTargetDander(int probability, int skillId, string elementID)
		: base(ePetEffectType.PetClearTargetDander, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetClearTargetDander) is PetClearTargetDander petClearTargetDander)
		{
			petClearTargetDander.m_probability = ((m_probability > petClearTargetDander.m_probability) ? m_probability : petClearTargetDander.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerBuffSkillPet += player_PlayerBuffSkillPet;
		player.AfterKillingLiving += player_AfterKillingLiving;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerBuffSkillPet -= player_PlayerBuffSkillPet;
		player.AfterKillingLiving -= player_AfterKillingLiving;
	}

	private void player_AfterKillingLiving(Living living, Living target, int damageAmount, int criticalAmount)
	{
		if (IsTrigger && living != target)
		{
			IsTrigger = false;
			target.Game.sendShowPicSkil(living, base.ElementInfo, isActive: true);
			(target as Player).SetDander(0);
		}
	}

	private void player_PlayerBuffSkillPet(Player player)
	{
		IsTrigger = false;
		if (player.PetEffects.CurrentUseSkill == m_currentId)
		{
			IsTrigger = true;
			player.PetEffectTrigger = true;
		}
	}
}
