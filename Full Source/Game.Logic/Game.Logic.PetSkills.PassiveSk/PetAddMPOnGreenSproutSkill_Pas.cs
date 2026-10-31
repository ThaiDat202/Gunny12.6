using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetAddMPOnGreenSproutSkill_Passive : BasePetEffect
{
	private int m_probability = 0;

	public PetAddMPOnGreenSproutSkill_Passive(int probability, string elementID)
		: base(ePetEffectType.PetAddMPOnGreenSproutSkill_Passive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddMPOnGreenSproutSkill_Passive) is PetAddMPOnGreenSproutSkill_Passive petAddMPOnGreenSproutSkill_Passive)
		{
			petAddMPOnGreenSproutSkill_Passive.m_probability = ((m_probability > petAddMPOnGreenSproutSkill_Passive.m_probability) ? m_probability : petAddMPOnGreenSproutSkill_Passive.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeginNextTurn += player_beginNextTurn;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.BeginNextTurn -= player_beginNextTurn;
	}

	public void player_beginNextTurn(Living living)
	{
		if (living.Game.CurrentLiving is Player && living.Game.CurrentLiving != living && living.Game.CurrentLiving.PetEffectList.GetOfType(ePetEffectType.PetAddBloodEquip) is PetAddBloodEquip)
		{
			(living as Player).AddPetMP(1);
		}
	}
}
