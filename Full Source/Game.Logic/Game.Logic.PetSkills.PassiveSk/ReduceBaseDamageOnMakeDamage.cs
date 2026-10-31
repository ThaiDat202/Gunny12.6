using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class ReduceBaseDamageOnMakeDamage : BasePetEffect
{
	private int m_value;

	private int m_added = 0;

	private int m_count = 0;

	public ReduceBaseDamageOnMakeDamage(string elementID)
		: base(ePetEffectType.ReduceBaseDamageOnMakeDamage, elementID)
	{
		m_value = 50;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.ReduceBaseDamageOnMakeDamage) is ReduceBaseDamageOnMakeDamage)
		{
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.OnMakeDamageEvent += player_OnMakeDamageEvent;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.OnMakeDamageEvent -= player_OnMakeDamageEvent;
	}

	private void player_BeginSelfTurn(Living living)
	{
		m_count--;
		if (m_count <= 0)
		{
			living.BaseDamage += m_added;
			living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: false);
			m_added = 0;
			m_count = 0;
		}
	}

	private void player_OnMakeDamageEvent(Living living, Living source, ref int damageamount, ref int criticalamount)
	{
		if (m_added == 0 && living.Game is PVPGame && living != source && rand.Next(100) <= 10)
		{
			m_added = (int)(living.BaseDamage * (double)m_value / 100.0);
			living.BaseDamage -= m_added;
			living.Game.sendShowPicSkil(living, base.Info, isActive: true);
			living.BeginSelfTurn += player_BeginSelfTurn;
			m_count = 2;
		}
	}
}
