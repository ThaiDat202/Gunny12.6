using Game.Logic.ElementSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetIceFronzeTarget : BasePetEffect
{
	private int m_probability = 0;

	private int m_currentId;

	public PetIceFronzeTarget(int probability, int skillId, string elementID)
		: base(ePetEffectType.PetIceFronzeTarget, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetIceFronzeTarget) is PetIceFronzeTarget petIceFronzeTarget)
		{
			petIceFronzeTarget.m_probability = ((m_probability > petIceFronzeTarget.m_probability) ? m_probability : petIceFronzeTarget.m_probability);
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
			new ElementForceFreezeEquip(1, 1).Start(target);
		}
	}

	private void player_PlayerBuffSkillPet(Player player)
	{
		IsTrigger = false;
		if (player.PetEffects.CurrentUseSkill == m_currentId && player.Game is PVPGame)
		{
			IsTrigger = true;
			player.PetEffectTrigger = true;
		}
	}
}
