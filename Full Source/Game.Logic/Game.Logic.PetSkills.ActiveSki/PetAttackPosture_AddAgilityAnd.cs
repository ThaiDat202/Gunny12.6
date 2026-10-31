using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetAttackPosture_AddAgilityAndSpeed : BasePetEffect
{
	private int m_probability;

	private int m_currentId;

	public PetAttackPosture_AddAgilityAndSpeed(int probability, int skillId, string elementID)
		: base(ePetEffectType.PetAttackPosture_AddAgilityAndSpeed, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAttackPosture_AddAgilityAndSpeed) is PetAttackPosture_AddAgilityAndSpeed petAttackPosture_AddAgilityAndSpeed)
		{
			petAttackPosture_AddAgilityAndSpeed.m_probability = ((m_probability > petAttackPosture_AddAgilityAndSpeed.m_probability) ? m_probability : petAttackPosture_AddAgilityAndSpeed.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerBuffSkillPet += player_PlayerBuffSkillPet;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerBuffSkillPet -= player_PlayerBuffSkillPet;
	}

	private void player_PlayerBuffSkillPet(Player player)
	{
		if (player.PetEffects.CurrentUseSkill == m_currentId)
		{
			player.PetEffectTrigger = true;
			player.Game.sendShowPicSkil(player, base.Info, isActive: true);
			new PetAddSpeedEquip(1, base.Info.ID.ToString()).Start(player);
		}
	}
}
