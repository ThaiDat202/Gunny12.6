using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetGetShieldEquip : AbstractPetEffect
{
	private int m_count;

	private int m_value;

	public PetGetShieldEquip(int count, int value, string elementID)
		: base(ePetEffectType.PetGetShieldEquip, elementID)
	{
		m_count = count;
		m_value = value;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetGetShieldEquip) is PetGetShieldEquip petGetShieldEquip)
		{
			petGetShieldEquip.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	public override void OnAttached(Living player)
	{
		player.Game.sendShowPicSkil(player, base.Info, isActive: false);
		player.Game.sendShowPicSkil(player, base.Info, isActive: true);
		player.BeforeTakeDamage += player_BeforeTakeDamage;
		player.BeginSelfTurn += player_SelfTurn;
	}

	public override void OnRemoved(Living player)
	{
		player.BeforeTakeDamage -= player_BeforeTakeDamage;
		player.BeginSelfTurn += player_SelfTurn;
	}

	private void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount)
	{
		if (m_value > 0)
		{
			living.Game.sendShowPicSkil(living, base.Info, isActive: true);
			int num = damageAmount;
			int num2 = criticalAmount;
			damageAmount -= m_value;
			criticalAmount -= m_value;
			if (damageAmount <= 0)
			{
				damageAmount = 0;
			}
			if (criticalAmount <= 0)
			{
				criticalAmount = 0;
			}
			m_value -= num;
			m_value -= num2;
			living.Game.SendEquipEffect(living, $"触发<冰霜屏障>,抵挡{num}伤害,{num2}暴击伤害.");
			if (m_value <= 0)
			{
				living.Game.SendEquipEffect(living, "屏障消除");
				m_value = 0;
				m_count = -1;
			}
		}
	}

	private void player_SelfTurn(Living living)
	{
		living.Game.sendShowPicSkil(living, base.Info, isActive: false);
		living.Game.sendShowPicSkil(living, base.Info, isActive: true);
		m_count--;
		if (m_count < 0)
		{
			living.Game.sendShowPicSkil(living, base.Info, isActive: false);
			Stop();
		}
	}
}
