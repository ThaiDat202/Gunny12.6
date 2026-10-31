using Game.Logic.Phy.Object;

namespace Game.Logic.Effects;

public class NoHole_Active : AbstractEffect
{
	private int m_count;

	public NoHole_Active(int count)
		: base(eEffectType.NoHoleEffect)
	{
		m_count = count;
	}

	public override bool Start(Living living)
	{
		if (living.EffectList.GetOfType(eEffectType.NoHoleEffect) is NoHole_Active noHole_Active)
		{
			noHole_Active.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	public override void OnAttached(Living living)
	{
		living.BeginSelfTurn += player_BeginFitting;
	}

	private void player_BeginFitting(Living player)
	{
		m_count--;
		if (m_count <= 0)
		{
			player.IsNoHole = false;
			player.Game.SendPlayerPicture(player, 5, state: false);
			player.BeginSelfTurn -= player_BeginFitting;
			Stop();
		}
	}
}
