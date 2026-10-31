using Game.Logic.PetSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.ElementSkills.ContinueSkills;

public class ElementPowerOfTheEarthEquip : AbstractPetEffect
{
	private int m_value = 0;

	private int m_countY;

	public ElementPowerOfTheEarthEquip(int skillid, int value, int countY)
		: base(ePetEffectType.ElementPowerOfTheEarthEquip, "")
	{
		m_value = value;
		m_countY = countY;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.ElementPowerOfTheEarthEquip) is ElementPowerOfTheEarthEquip)
		{
			return true;
		}
		return base.Start(living);
	}

	public override void OnAttached(Living living)
	{
		living.BeginSelfTurn += player_BeginSelfTurn;
	}

	public override void OnRemoved(Living living)
	{
		living.BeginSelfTurn -= player_BeginSelfTurn;
	}

	private void player_BeginSelfTurn(Living living)
	{
		int num = m_value * m_countY;
		if ((living as Player).PlayerDetail.PlayerCharacter.EmblemInfoDefendSKill == 824)
		{
			num -= (living as Player).PlayerDetail.PlayerCharacter.EmblemInfoDefendSKill;
		}
		living.ReduceBlood(num, 4);
		living.EffectBadges = false;
		Stop();
	}
}
