using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetDisibleActiveSkillEquip : BasePetEffect
{
	private int m_count = 0;

	public PetDisibleActiveSkillEquip(int count, string elementID)
		: base(ePetEffectType.PetDisibleActiveSkillEquip, elementID)
	{
		m_count = count;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetDisibleActiveSkillEquip) is PetDisibleActiveSkillEquip petDisibleActiveSkillEquip)
		{
			petDisibleActiveSkillEquip.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeginSelfTurn += player_beginSeftTurn;
		player.PlayerClearBuffSkillPet += Player_PlayerClearBuffSkillPet;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.BeginSelfTurn -= player_beginSeftTurn;
	}

	public void player_beginSeftTurn(Living player)
	{
		m_count--;
		if (m_count < 0)
		{
			player.Game.sendShowPicSkil(player, base.ElementInfo, isActive: true);
			Stop();
		}
		else
		{
			m_living.PetEffects.DisibleActiveSkill = true;
		}
	}

	private void Player_PlayerClearBuffSkillPet(Player player)
	{
		Stop();
	}
}
