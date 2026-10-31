using System.Drawing;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetMoveToRandomMapPoint : BasePetEffect
{
	private int m_probability = 0;

	private int m_currentId;

	public PetMoveToRandomMapPoint(int probability, int skillId, string elementID)
		: base(ePetEffectType.PetMoveToRandomMapPoint, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetMoveToRandomMapPoint) is PetMoveToRandomMapPoint petMoveToRandomMapPoint)
		{
			petMoveToRandomMapPoint.m_probability = ((m_probability > petMoveToRandomMapPoint.m_probability) ? m_probability : petMoveToRandomMapPoint.m_probability);
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
			Point playerPoint = m_living.Game.GetPlayerPoint(m_living.Team);
			m_living.SetXY(playerPoint);
			m_living.StartMoving();
		}
	}
}
