using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class AddDamageOnMakeDamage : AbstractPetEffect
{
	private int m_value;

	private int m_count;

	public AddDamageOnMakeDamage(string elementID)
		: base(ePetEffectType.AddDamageOnMakeDamage, elementID)
	{
		m_value = 25;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.AddDamageOnMakeDamage) is AddDamageOnMakeDamage)
		{
			return true;
		}
		return base.Start(living);
	}

	private void player_BeginSelfTurn(Living living)
	{
		m_count--;
		if (m_count < 0)
		{
			living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: false);
		}
	}

	public override void OnAttached(Living living)
	{
		living.OnMakeDamageEvent += player_OnMakeDamageEvent;
	}

	public override void OnRemoved(Living living)
	{
		living.OnMakeDamageEvent -= player_OnMakeDamageEvent;
	}

	private void player_OnMakeDamageEvent(Living living, Living source, ref int damageamount, ref int criticalamount)
	{
		if (living.Game is PVPGame && living != source && rand.Next(100) <= 20)
		{
			damageamount += damageamount * m_value / 100;
			living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: true);
			living.BeginSelfTurn += player_BeginSelfTurn;
			m_count = 1;
		}
	}
}
