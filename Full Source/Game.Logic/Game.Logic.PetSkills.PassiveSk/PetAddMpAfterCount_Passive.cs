using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetAddMpAfterCount_Passive : BasePetEffect
{
	private int m_added;

	private int m_value;

	private int m_countTotal;

	public PetAddMpAfterCount_Passive(string elementID)
		: base(ePetEffectType.PetAddMpAfterCount_Passive, elementID)
	{
		switch (elementID)
		{
		case "1748":
			m_added = 20;
			m_value = 20;
			break;
		case "1781":
			m_added = 30;
			m_value = 14;
			break;
		case "1815":
			m_added = 20;
			m_value = 25;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddMpAfterCount_Passive) is PetAddMpAfterCount_Passive)
		{
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeforeTakeDamage += player_BeforeTakeDamage;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.BeforeTakeDamage -= player_BeforeTakeDamage;
	}

	private void player_BeforeTakeDamage(Living living, Living source, ref int damageamount, ref int criticalamount)
	{
		if (living != source)
		{
			m_countTotal++;
			if (m_countTotal >= m_value)
			{
				m_countTotal = 0;
				(living as Player).AddPetMP(m_added);
				living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: true);
				living.AddPetEffect(new PetShowFireOfHatredEquip(99, "1751"), 0);
			}
		}
	}
}
