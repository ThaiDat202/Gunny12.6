using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetAddMpAfterUseSkillCount_Passive : BasePetEffect
{
	private int m_added;

	private int m_value;

	private int m_countTotal;

	public PetAddMpAfterUseSkillCount_Passive(string elementID)
		: base(ePetEffectType.PetAddMpAfterUseSkillCount_Passive, elementID)
	{
		switch (elementID)
		{
		case "1765":
			m_added = 20;
			m_value = 3;
			break;
		case "1784":
			m_added = 30;
			m_value = 2;
			break;
		case "1815":
			m_added = 20;
			m_value = 4;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddMpAfterUseSkillCount_Passive) is PetAddMpAfterUseSkillCount_Passive)
		{
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerBuffSkill += player_PlayerBuffSkill;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerBuffSkill -= player_PlayerBuffSkill;
	}

	private void player_PlayerBuffSkill(Living living)
	{
		m_countTotal++;
		if (m_countTotal >= m_value)
		{
			m_countTotal = 0;
			(living as Player).AddPetMP(m_added);
			living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: true);
			living.AddPetEffect(new PetShowFireofResentmentEquip(99, "1750"), 0);
		}
	}
}
