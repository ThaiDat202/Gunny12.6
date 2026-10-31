using Game.Logic.Phy.Object;

namespace Game.Logic.Effects;

public class ContinueReduceGreenBloodEffect : AbstractEffect
{
	private int m_count;

	private int m_blood;

	private Living m_liv;

	public ContinueReduceGreenBloodEffect(int count, int blood, Living liv)
		: base(eEffectType.ContinueReduceGreenBloodEffect)
	{
		m_count = count;
		m_blood = blood;
		m_liv = liv;
	}

	public override bool Start(Living living)
	{
		if (living.EffectList.GetOfType(eEffectType.ContinueReduceGreenBloodEffect) is ContinueReduceGreenBloodEffect continueReduceGreenBloodEffect)
		{
			continueReduceGreenBloodEffect.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	public override void OnAttached(Living living)
	{
		living.BeginSelfTurn += player_BeginFitting;
		living.Game.SendPlayerPicture(living, 28, state: true);
	}

	public override void OnRemoved(Living living)
	{
		living.BeginSelfTurn -= player_BeginFitting;
		living.Game.SendPlayerPicture(living, 28, state: false);
	}

	private void player_BeginFitting(Living living)
	{
		m_count--;
		if (m_count < 0)
		{
			Stop();
			return;
		}
		living.AddBlood(-m_blood, 1);
		if (living.Blood > 0)
		{
			return;
		}
		living.Die();
		if (m_liv != null && m_liv is Player)
		{
			int type = 2;
			if (living is Player)
			{
				type = 1;
			}
			(m_liv as Player).PlayerDetail.OnKillingLiving(m_liv.Game, type, living.Id, living.IsLiving, m_blood);
		}
	}
}
