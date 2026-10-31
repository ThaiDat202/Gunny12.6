using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetNoFly : BasePetEffect
{
	private int m_count;

	private int m_probability;

	private int m_currentId;

	public PetNoFly(int count, int probability, int skillId, string elementID)
		: base(ePetEffectType.PetNoFly, elementID)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetNoFly) is PetNoFly petNoFly)
		{
			petNoFly.m_probability = ((m_probability > petNoFly.m_probability) ? m_probability : petNoFly.m_probability);
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
			target.AddPetEffect(new PetNoFlyEquip(m_count, base.Info.ID.ToString()), 0);
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
