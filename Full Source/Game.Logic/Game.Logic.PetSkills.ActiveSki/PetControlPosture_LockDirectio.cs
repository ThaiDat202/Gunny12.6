using Game.Logic.Effects;
using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetControlPosture_LockDirection : BasePetEffect
{
	private int m_probability;

	private int m_currentId;

	private int m_count;

	public PetControlPosture_LockDirection(int count, int probability, int skillId, string elementID)
		: base(ePetEffectType.PetControlPosture_LockDirection, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
		m_count = count;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetControlPosture_LockDirection) is PetControlPosture_LockDirection petControlPosture_LockDirection)
		{
			petControlPosture_LockDirection.m_probability = ((m_probability > petControlPosture_LockDirection.m_probability) ? m_probability : petControlPosture_LockDirection.m_probability);
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
		player.AfterKillingLiving += player_AfterKillingLiving;
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
		if (IsTrigger)
		{
			IsTrigger = false;
			target.Game.sendShowPicSkil(target, base.ElementInfo, isActive: true);
			new LockDirectionEffect(m_count).Start(target);
			target.AddPetEffect(new PetControlPosture_LockDirectionEquip(m_count, base.ElementInfo.ID.ToString()), 0);
		}
	}
}
