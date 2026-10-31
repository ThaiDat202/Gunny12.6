using Game.Logic.PetSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.MarkSkills;

public class AddBaseGuardAfterKilled : AbstractPetEffect
{
	private int m_value;

	private int m_added = 0;

	private int m_added1 = 0;

	private int m_count = 0;

	public AddBaseGuardAfterKilled(string elementID)
		: base(ePetEffectType.AddBaseGuardAfterKilled, elementID)
	{
		m_value = 10;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.AddBaseGuardAfterKilled) is AddBaseGuardAfterKilled)
		{
			return true;
		}
		return base.Start(living);
	}

	public override void OnAttached(Living living)
	{
		living.AfterKilledByLiving += player_AfterKilledByLiving;
		living.BeginSelfTurn += player_BeginSelfTurn;
	}

	public override void OnRemoved(Living living)
	{
		living.AfterKilledByLiving -= player_AfterKilledByLiving;
		living.BeginSelfTurn += player_BeginSelfTurn;
	}

	private void player_AfterKilledByLiving(Living living, Living target, int damageAmount, int criticalAmount)
	{
		if (m_added == 0 && m_added1 == 0 && living != target && rand.Next(100) <= 10)
		{
			m_added = (int)(living.BaseGuard * (double)m_value / 100.0);
			living.BaseGuard += m_added;
			m_added1 = (int)(living.Defence * (double)m_value / 100.0);
			living.Defence += m_added1;
			living.Game.sendShowPicSkil(living, base.Info, isActive: true);
			m_count = 2;
		}
	}

	private void player_BeginSelfTurn(Living living)
	{
		m_count--;
		if (m_count < 0)
		{
			living.BaseGuard -= m_added;
			living.Defence -= m_added1;
			living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: false);
			m_added = 0;
			m_added1 = 0;
			m_count = 0;
		}
	}
}
