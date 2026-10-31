using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetReduceTakeDamage : BasePetEffect
{
	private int m_probability = 0;

	private int m_currentId;

	private int m_count = 0;

	public PetReduceTakeDamage(int count, int probability, int skillId, string elementID)
		: base(ePetEffectType.PetReduceTakeDamage, elementID)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetReduceTakeDamage) is PetReduceTakeDamage petReduceTakeDamage)
		{
			petReduceTakeDamage.m_probability = ((m_probability > petReduceTakeDamage.m_probability) ? m_probability : petReduceTakeDamage.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		if (m_count == -1)
		{
			player.AddPetEffect(new PetReduceTakeDamageEquip(999999, base.Info.ID.ToString()), 0);
			player.BeginNextTurn += enviarImg;
		}
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
			player.Game.sendShowPicSkil(player, base.ElementInfo, isActive: true);
			player.AddPetEffect(new PetReduceTakeDamageEquip(m_count, base.ElementInfo.ID.ToString()), 0);
		}
	}

	private void enviarImg(Living living)
	{
		living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: true);
		living.BeginNextTurn -= enviarImg;
	}
}
