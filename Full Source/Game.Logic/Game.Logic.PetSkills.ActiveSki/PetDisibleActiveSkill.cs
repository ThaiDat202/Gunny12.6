using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetDisibleActiveSkill : BasePetEffect
{
	private int m_probability = 0;

	private int m_currentId;

	private int m_count;

	public PetDisibleActiveSkill(int count, int probability, int skillid, string elementID)
		: base(ePetEffectType.PetDisibleActiveSkill, elementID)
	{
		m_count = count;
		m_currentId = skillid;
		m_probability = ((probability == -1) ? 10000 : probability);
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetDisibleActiveSkill) is PetDisibleActiveSkill petDisibleActiveSkill)
		{
			petDisibleActiveSkill.m_probability = ((m_probability > petDisibleActiveSkill.m_probability) ? m_probability : petDisibleActiveSkill.m_probability);
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

	private void player_PlayerBuffSkillPet(Player player)
	{
		if (player.PetEffects.CurrentUseSkill == m_currentId)
		{
			IsTrigger = true;
		}
	}

	private void player_AfterKillingLiving(Living living, Living target, int damageAmount, int criticalAmount)
	{
		if (IsTrigger && target is Player)
		{
			target.Game.sendShowPicSkil(target, base.ElementInfo, isActive: true);
			target.AddPetEffect(new PetDisibleActiveSkillEquip(m_count, base.ElementInfo.ID.ToString()), 0);
			IsTrigger = false;
		}
	}
}
