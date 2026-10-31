using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetChangeDamageWind : AbstractPetEffect
{
	private int m_added;

	private int m_skillID;

	private int m_removed = 10;

	public PetChangeDamageWind(int skillId, string elementID)
		: base(ePetEffectType.PetChangeDamageWind, elementID)
	{
		m_skillID = skillId;
		switch (skillId)
		{
		case 572:
			m_added = 15;
			break;
		case 573:
			m_added = 30;
			break;
		case 623:
			m_added = 5;
			break;
		case 624:
			m_added = 10;
			break;
		case 599:
			m_added = 30;
			m_removed = 0;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetChangeDamageWind) is PetChangeDamageWind)
		{
			return true;
		}
		return base.Start(living);
	}

	public override void OnAttached(Living living)
	{
		living.BeginNextTurn += player_BeginNextTurn;
		living.OnMakeDamageEvent += player_OnMakeDamageEvent;
	}

	private void player_BeginNextTurn(Living living)
	{
		living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: true);
		living.BeginNextTurn -= player_BeginNextTurn;
	}

	private void player_OnMakeDamageEvent(Living living, Living target, ref int damageAmount, ref int criticalAmount)
	{
		if (living != target)
		{
			float wind = living.Game.Wind;
			int direction = target.Direction;
			float num = 0f;
			num = ((direction != -1) ? (wind * (float)m_removed / 100f * (float)direction) : (wind * (float)m_added / 100f * (float)direction));
			if (m_skillID == 599 && num <= 0f)
			{
				num = 0f;
			}
			damageAmount = (int)((float)damageAmount * (1f + num));
			criticalAmount = (int)((float)criticalAmount * (1f + num));
		}
	}

	public override void OnRemoved(Living living)
	{
		living.OnMakeDamageEvent -= player_OnMakeDamageEvent;
	}
}
