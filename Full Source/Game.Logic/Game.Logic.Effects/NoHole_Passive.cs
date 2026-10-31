using Game.Logic.Phy.Object;

namespace Game.Logic.Effects;

public class NoHole_Passive : AbstractEffect
{
	private int m_count;

	public NoHole_Passive(int count)
		: base(eEffectType.NoHole_Passive)
	{
		m_count = count;
	}

	public override bool Start(Living living)
	{
		if (living.EffectList.GetOfType(eEffectType.NoHole_Passive) is NoHole_Passive noHole_Passive)
		{
			noHole_Passive.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	public override void OnAttached(Living living)
	{
		living.IsNoHole = true;
		living.BeginSelfTurn += player_BeginFitting;
		living.Game.SendPlayerPicture(living, 5, state: true);
	}

	public override void OnRemoved(Living living)
	{
		living.BeginSelfTurn -= player_BeginFitting;
		living.IsNoHole = false;
		living.Game.SendPlayerPicture(living, 5, state: false);
	}

	private void player_BeginFitting(Living player)
	{
		m_count--;
		if (m_count <= 0)
		{
			Stop();
		}
	}
}
