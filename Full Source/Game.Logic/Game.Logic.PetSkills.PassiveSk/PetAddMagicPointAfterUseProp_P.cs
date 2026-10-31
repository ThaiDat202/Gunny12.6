using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetAddMagicPointAfterUseProp_Passive : BasePetEffect
{
	private int m_probability = 0;

	private int m_value = 0;

	private string m_elementID;

	public PetAddMagicPointAfterUseProp_Passive(int probability, string elementID)
		: base(ePetEffectType.PetAddMagicPointAfterUseProp_Passive, elementID)
	{
		m_elementID = elementID;
		m_probability = ((probability == -1) ? 10000 : probability);
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddMagicPointAfterUseProp_Passive) is PetAddMagicPointAfterUseProp_Passive petAddMagicPointAfterUseProp_Passive)
		{
			petAddMagicPointAfterUseProp_Passive.m_probability = ((m_probability > petAddMagicPointAfterUseProp_Passive.m_probability) ? m_probability : petAddMagicPointAfterUseProp_Passive.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeginSelfTurn += player_BeginSelfTurn;
		player.PropUseItem += player_PropUseItem;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.BeginSelfTurn -= player_BeginSelfTurn;
		player.PropUseItem -= player_PropUseItem;
	}

	private void player_BeginSelfTurn(Living living)
	{
		m_value = 0;
	}

	private void player_PropUseItem(Living living)
	{
		if (m_elementID == "1515")
		{
			(living as Player).AddPetMP(1);
			(living as Player).AddDander(4);
		}
		else if (m_value < 8)
		{
			(living as Player).AddPetMP(1);
			m_value++;
		}
	}
}
