using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetBurningTargetBlood : BasePetEffect
{
	private int m_count;

	private int m_probability;

	private int m_currentId;

	public PetBurningTargetBlood(int count, int probability, int skillId, string elementID)
		: base(ePetEffectType.PetBurningTargetBlood, elementID)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetBurningTargetBlood) is PetBurningTargetBlood petBurningTargetBlood)
		{
			petBurningTargetBlood.m_probability = ((m_probability > petBurningTargetBlood.m_probability) ? m_probability : petBurningTargetBlood.m_probability);
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
		if (IsTrigger)
		{
			IsTrigger = false;
			target.Game.sendShowPicSkil(target, base.ElementInfo, isActive: true);
			target.AddPetEffect(new PetReduceBloodEquip(m_count, base.Info.ID.ToString(), living), 0);
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
