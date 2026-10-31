using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class AddMagicAttackOnMakeDamage : BasePetEffect
{
	private int m_value;

	private int m_added = 0;

	private int m_count = 0;

	public AddMagicAttackOnMakeDamage(string elementID)
		: base(ePetEffectType.AddMagicAttackOnMakeDamage, elementID)
	{
		m_value = 30;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.AddMagicAttackOnMakeDamage) is AddMagicAttackOnMakeDamage)
		{
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.OnMakeDamageEvent += player_OnMakeDamageEvent;
		player.BeginSelfTurn += player_BeginSelfTurn;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.OnMakeDamageEvent -= player_OnMakeDamageEvent;
		player.BeginSelfTurn += player_BeginSelfTurn;
	}

	private void player_BeginSelfTurn(Living living)
	{
		m_count--;
		if (m_count <= 0)
		{
			living.MagicAttack -= m_added;
			living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: false);
			m_added = 0;
			m_count = 0;
		}
	}

	private void player_OnMakeDamageEvent(Living living, Living source, ref int damageamount, ref int criticalamount)
	{
		if (m_added == 0 && source.Game is PVPGame && living != source && rand.Next(100) <= 10)
		{
			m_added = (int)(source.MagicAttack * (double)m_value / 100.0);
			source.MagicAttack += m_added;
			source.Game.sendShowPicSkil(source, base.Info, isActive: true);
			m_count = 2;
		}
	}
}
