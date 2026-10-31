using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetAddMPAfterKilledByLiving_Passive : BasePetEffect
{
	private int m_probability = 0;

	private string m_elementID;

	private int m_value;

	public PetAddMPAfterKilledByLiving_Passive(int probability, string elementID)
		: base(ePetEffectType.PetAddMPAfterKilledByLiving_Passive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_elementID = elementID;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddMPAfterKilledByLiving_Passive) is PetAddMPAfterKilledByLiving_Passive petAddMPAfterKilledByLiving_Passive)
		{
			petAddMPAfterKilledByLiving_Passive.m_probability = ((m_probability > petAddMPAfterKilledByLiving_Passive.m_probability) ? m_probability : petAddMPAfterKilledByLiving_Passive.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.AfterKilledByLiving += player_AfterKilledByLiving;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.AfterKilledByLiving -= player_AfterKilledByLiving;
	}

	private void player_AfterKilledByLiving(Living living, Living target, int damageAmount, int criticalAmount)
	{
		switch (m_elementID)
		{
		case "1223":
		case "1263":
			m_value = 2;
			break;
		case "1457":
		case "3822":
			m_value = 1;
			break;
		}
		(living as Player).AddPetMP(m_value);
	}
}
