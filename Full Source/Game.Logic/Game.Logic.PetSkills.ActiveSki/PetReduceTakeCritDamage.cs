using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetReduceTakeCritDamage : BasePetEffect
{
	private int m_probability = 0;

	private int m_currentId;

	private int m_count = 0;

	public PetReduceTakeCritDamage(int count, int probability, int skillId, string elementID)
		: base(ePetEffectType.PetReduceTakeCritDamage, elementID)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetReduceTakeCritDamage) is PetReduceTakeCritDamage petReduceTakeCritDamage)
		{
			petReduceTakeCritDamage.m_probability = ((m_probability > petReduceTakeCritDamage.m_probability) ? m_probability : petReduceTakeCritDamage.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		if (m_count == -1)
		{
			player.AddPetEffect(new PetReduceTakeCritDamageEquip(999999, base.Info.ID.ToString()), 0);
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
			player.AddPetEffect(new PetReduceTakeCritDamageEquip(m_count, base.ElementInfo.ID.ToString()), 0);
		}
	}

	private void enviarImg(Living living)
	{
		living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: true);
		living.BeginNextTurn -= enviarImg;
	}
}
